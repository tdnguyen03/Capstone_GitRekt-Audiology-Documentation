using UnityEngine;

/// @class MovingSphere
/// @brief Represents a sphere that moves and interacts with haptic feedback systems.
/// @details This class handles the movement, collision resistance, and visual squish effects of a sphere. It interacts with a spline tube and responds to haptic forces.
public class MovingSphere : MonoBehaviour
{
    [Header("Force Settings")]
    /// @brief Force applied to push the sphere.
    [Range(0, 100)] public float pushForce = 0.01f;

    /// @brief Stickiness factor for resisting movement.
    [Range(0, 100)] public float stickiness = 30f;

    /// @brief Force required to break away from the stickiness.
    [Range(0, 10)] public float breakawayForce = 3f;

    [Header("Squish Visuals")]
    /// @brief Amount of squish applied to the sphere.
    [Range(0, 1)] public float squishAmount = 0.2f;

    /// @brief Speed at which the squish effect is applied.
    [Range(0, 10)] public float squishSpeed = 5f;

    /// @brief Transform representing the visual sphere for squish effects.
    public Transform visualSphere;

    [Header("Spline Tube Reference")]
    /// @brief Reference to the curved tube for radial force calculations.
    public CurvedTubeForceFeedback curvedTube;

    /// @brief Rigidbody component of the sphere.
    private Rigidbody rb;

    /// @brief Radius of the sphere.
    private float sphereRadius;

    /// @brief Position of the sphere on the main thread.
    private Vector3 mainThreadPosition;

    /// @brief Initial scale of the visual sphere.
    private Vector3 initialScale;

    /// @brief Pending force to be applied to the sphere.
    private Vector3 pendingForce = Vector3.zero;

    /// @brief Indicates whether the sphere is stuck.
    private bool stuck = true;

    /// @brief Current squish factor of the sphere.
    private float currentSquish = 0f;

    /// @brief Target squish factor of the sphere.
    private float targetSquish = 0f;

    /**
     * @brief Unity Awake method.
     * @details Initializes the sphere's components, registers it with the HapticManager, and sets up references.
    */
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        sphereRadius = GetComponent<SphereCollider>().radius * transform.lossyScale.x;

        if (visualSphere == null && transform.childCount > 0)
            visualSphere = transform.GetChild(0);

        initialScale = visualSphere != null ? visualSphere.localScale : Vector3.one;

        FindObjectOfType<HapticManager>()?.RegisterMovingSphere(this);

        if (curvedTube == null)
            curvedTube = FindObjectOfType<CurvedTubeForceFeedback>();
    }

    /**
     * @brief Unity Update method.
     * @details Updates the sphere's position, applies forces, and handles squish effects.
    */
    private void Update()
    {
        mainThreadPosition = transform.position;

        if (pendingForce != Vector3.zero)
        {
            if (stuck)
            {
                rb.AddForce(-rb.linearVelocity * stickiness, ForceMode.Force);
                if (rb.linearVelocity.magnitude > breakawayForce)
                    stuck = false;
            }

            rb.AddForce(pendingForce, ForceMode.Force);
            pendingForce = Vector3.zero;
        }

        // Apply outward radial force from spline center
        if (curvedTube != null)
        {
            Vector3 radialForce = curvedTube.CalculateRadialWallForce(transform.position, rb.linearVelocity, sphereRadius);
            rb.AddForce(radialForce, ForceMode.Force);
        }

        currentSquish = Mathf.Lerp(currentSquish, targetSquish, squishSpeed * Time.deltaTime);
        ApplySquish(currentSquish);
    }

    /**
     * @brief Calculates the force to resist cursor penetration.
     * @details Computes the collision resistance and builds a pushing force based on the cursor's position and velocity.
     * @param cursorPos The position of the cursor in world space.
     * @param cursorVel The velocity of the cursor.
     * @param cursorRadius The radius of the cursor.
     * @return A vector representing the calculated force.
    */
    public Vector3 CalculateForce(Vector3 cursorPos, Vector3 cursorVel, float cursorRadius)
    {
        Vector3 delta = cursorPos - mainThreadPosition;
        float distance = delta.magnitude;
        float penetration = cursorRadius + sphereRadius - distance;

        if (penetration > 0f)
        {
            Vector3 normal = delta.normalized;

            Vector3 force = normal * penetration * 100f;
            force -= cursorVel * 10f;

            pendingForce += -normal * pushForce;
            targetSquish = 1f;
            return force;
        }

        targetSquish = 0f;
        return Vector3.zero;
    }

    /**
     * @brief Applies the squish effect to the visual sphere.
     * @details Adjusts the scale of the visual sphere based on the squish factor.
     * @param factor The squish factor to apply.
    */
    private void ApplySquish(float factor)
    {
        if (visualSphere == null) return;

        float stretch = 1f + factor * squishAmount;
        float squash = 1f - factor * squishAmount;

        visualSphere.localScale = new Vector3(
            initialScale.x * stretch,
            initialScale.y * squash,
            initialScale.z * stretch
        );
    }
}
