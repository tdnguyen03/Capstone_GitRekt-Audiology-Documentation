using UnityEngine;
using TMPro;

/// @class CubeForceFeedback
/// @brief Provides force feedback for a cube object.
/// @details This class calculates the force feedback based on the cursor's position, velocity, and radius. It also provides visual feedback by changing the cube's color based on the penetration depth.
public class CubeForceFeedback : MonoBehaviour
{
    /// @brief Stiffness of the cube for force feedback calculations.
    [Header("Force Feedback Settings")]
    [Range(0, 800)] public float stiffness = 300f;

    /// @brief Damping factor for force feedback calculations.
    [Range(0, 3)] public float damping = 1f;

    /// @brief Warning message to display when needed.
    [Header("Warning Settings")]
    public string warningMessage;

    /// @brief Enables or disables color feedback based on penetration depth.
    [Header("Visual Feedback")]
    public bool enableColorFeedback = false;

    /// @brief Color of the cube when no force is applied.
    public Color minForceColor = Color.white;

    /// @brief Color of the cube when maximum force is applied.
    public Color maxForceColor = Color.red;

    /// @brief Position of the cube in world space.
    private Vector3 _cubePosition;

    /// @brief Size of the cube in world space.
    private Vector3 _cubeSize;

    /// @brief Renderer component of the cube for visual feedback.
    private Renderer _renderer;

    /// @brief Penetration depth of the cursor into the cube.
    private float _penetration = 0f;

    /**
     * @brief Unity Awake method.
     * @details Initializes the cube's position, size, and renderer. Registers the cube with the HapticManager.
    */
    private void Awake()
    {
        _cubePosition = transform.position;
        _cubeSize = transform.lossyScale;
        _renderer = GetComponent<Renderer>();

        FindFirstObjectByType<HapticManager>().RegisterCube(this);
    }

    /**
     * @brief Calculates the force feedback based on cursor interaction.
     * @details Determines the force applied to the cursor based on its position, velocity, and radius relative to the cube.
     * @param cursorPosition The position of the cursor in world space.
     * @param cursorVelocity The velocity of the cursor.
     * @param cursorRadius The radius of the cursor.
     * @return A vector representing the calculated force.
    */
    public Vector3 CalculateForce(Vector3 cursorPosition, Vector3 cursorVelocity, float cursorRadius)
    {
        Vector3 force = Vector3.zero;

        Vector3 closestPoint = new Vector3(
            Mathf.Clamp(cursorPosition.x, _cubePosition.x - _cubeSize.x / 2f, _cubePosition.x + _cubeSize.x / 2f),
            Mathf.Clamp(cursorPosition.y, _cubePosition.y - _cubeSize.y / 2f, _cubePosition.y + _cubeSize.y / 2f),
            Mathf.Clamp(cursorPosition.z, _cubePosition.z - _cubeSize.z / 2f, _cubePosition.z + _cubeSize.z / 2f)
        );

        Vector3 distanceVector = cursorPosition - closestPoint;
        float distance = distanceVector.magnitude;
        _penetration = cursorRadius - distance;

        if (_penetration > 0)
        {
            Vector3 normal = distanceVector.normalized;
            force = normal * _penetration * stiffness;
            force -= cursorVelocity * damping;
        }
        else
        {
            _penetration = 0f;
        }

        return force;
    }

    /**
     * @brief Calculates the normalized penetration depth.
     * @details Normalizes the penetration depth to a value between 0 and 1.
     * @return A float representing the normalized penetration depth.
    */
    public float NormalizedPenetration()
    {
        return Mathf.Clamp01(_penetration / 0.01f);
    }

    /**
     * @brief Unity Update method.
     * @details Updates the cube's color based on the penetration depth if color feedback is enabled.
    */
    private void Update()
    {
        if (enableColorFeedback && _renderer != null)
        {
            float normalized = Mathf.Clamp01(_penetration / 0.01f);
            _renderer.material.color = Color.Lerp(minForceColor, maxForceColor, normalized);
        }
    }
}
