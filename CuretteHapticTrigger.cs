using UnityEngine;

/// @class CuretteHapticTrigger
/// @brief Handles haptic feedback for the curette tool.
/// @details This class detects collisions between the curette and cubes, calculates haptic feedback forces, and manages vibration settings.
public class CuretteHapticTrigger : MonoBehaviour
{
    /// @brief Indicates whether the curette is currently touching a cube.
    public bool isTouchingCube { get; private set; } = false;

    /// @brief The normal vector of the last collision point.
    public Vector3 lastCollisionNormal { get; private set; } = Vector3.up;

    /// @brief Amplitude of the vibration for haptic feedback.
    [Header("Vibration Settings")]
    [Range(0, 10)]
    public float vibrationAmplitude = 2f;

    /// @brief Frequency of the vibration for haptic feedback.
    [Range(0, 100)]
    public float vibrationFrequency = 50f;

    /// @brief Local timer used for vibration calculations.
    private float localTime = 0f;

    /**
     * @brief Unity Awake method.
     * @details Registers the curette trigger with the HapticManager.
    */
    private void Awake()
    {
        FindFirstObjectByType<HapticManager>().RegisterCuretteTrigger(this);
    }

    /**
     * @brief Unity Update method.
     * @details Updates the local timer used for vibration calculations.
    */
    private void Update()
    {
        localTime += Time.deltaTime;
    }

    /**
     * @brief Unity OnTriggerEnter method.
     * @details Detects when the curette enters a collision with a cube and updates the collision state and normal vector.
     * @param other The collider of the object that triggered the collision.
    */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            isTouchingCube = true;
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            lastCollisionNormal = (transform.position - contactPoint).normalized;
        }
    }

    /**
     * @brief Unity OnTriggerExit method.
     * @details Detects when the curette exits a collision with a cube and resets the collision state and normal vector.
     * @param other The collider of the object that triggered the collision exit.
    */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            isTouchingCube = false;
            lastCollisionNormal = Vector3.up;
        }
    }

    /**
     * @brief Calculates the haptic feedback force.
     * @details Computes the vibration force based on the collision state, vibration settings, and the last collision normal.
     * @param cursorPosition The position of the cursor in world space.
     * @param cursorVelocity The velocity of the cursor.
     * @param cursorRadius The radius of the cursor.
     * @return A vector representing the calculated haptic feedback force.
    */
    public Vector3 CalculateForce(Vector3 cursorPosition, Vector3 cursorVelocity, float cursorRadius)
    {
        if (!isTouchingCube)
            return Vector3.zero;

        float oscillation = Mathf.Sin(2.0f * Mathf.PI * vibrationFrequency * localTime);
        Vector3 vibrationForce = vibrationAmplitude * oscillation * lastCollisionNormal;

        return vibrationForce;
    }
}
