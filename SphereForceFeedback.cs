using UnityEngine;

/// @class SphereForceFeedback
/// @brief Provides force feedback for a spherical object.
/// @details This class calculates the force feedback based on the cursor's position, velocity, and radius relative to a sphere.
public class SphereForceFeedback : MonoBehaviour
{
    /// @brief Stiffness of the sphere for force feedback calculations.
    [Range(0, 800)]
    public float stiffness = 300f;

    /// @brief Damping factor for force feedback calculations.
    [Range(0, 3)]
    public float damping = 1f;

    /// @brief Position of the sphere in world space.
    private Vector3 _ballPosition;

    /// @brief Radius of the sphere.
    private float _ballRadius;

    /**
     * @brief Unity Awake method.
     * @details Initializes the sphere's position and radius, and registers it with the HapticManager.
    */
    private void Awake()
    {
        _ballPosition = transform.position;
        _ballRadius = transform.lossyScale.x / 2f;

        FindObjectOfType<HapticManager>().RegisterSphere(this);
    }

    /**
     * @brief Calculates the force feedback based on cursor interaction.
     * @details Determines the force applied to the cursor based on its position, velocity, and radius relative to the sphere.
     * @param cursorPosition The position of the cursor in world space.
     * @param cursorVelocity The velocity of the cursor.
     * @param cursorRadius The radius of the cursor.
     * @return A vector representing the calculated force.
    */
    public Vector3 CalculateForce(Vector3 cursorPosition, Vector3 cursorVelocity, float cursorRadius)
    {
        Vector3 force = Vector3.zero;

        Vector3 distanceVector = cursorPosition - _ballPosition;
        float distance = distanceVector.magnitude;
        float penetration = cursorRadius + _ballRadius - distance;

        if (penetration > 0)
        {
            Vector3 normal = distanceVector.normalized;
            force = normal * penetration * stiffness;
            force -= cursorVelocity * damping;
        }

        return force;
    }
}
