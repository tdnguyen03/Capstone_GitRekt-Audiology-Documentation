using UnityEngine;

/// @class DangerZone
/// @brief Represents a danger zone that detects penetration depth.
/// @details This class calculates the penetration depth of a cursor into a danger zone and normalizes the penetration for use in visual or haptic feedback systems.
public class DangerZone : MonoBehaviour
{
    /// @brief Radius of the danger zone for normalized penetration calculations.
    public float dangerRadius = 0.01f;

    /// @brief Warning message to display when the danger zone is penetrated.
    public string warningMessage;

    /// @brief Position of the danger zone in world space.
    private Vector3 _cubePosition;

    /// @brief Size of the danger zone in world space.
    private Vector3 _cubeSize;

    /// @brief Current penetration depth of the cursor into the danger zone.
    private float _penetration = 0f;

    /**
     * @brief Unity Awake method.
     * @details Initializes the danger zone's position and size, and registers it with the HapticManager.
    */
    private void Awake()
    {
        _cubePosition = transform.position;
        _cubeSize = transform.lossyScale;
        FindFirstObjectByType<HapticManager>().RegisterDangerZone(this);
    }

    /**
     * @brief Calculates the penetration depth of the cursor into the danger zone.
     * @details Determines how far the cursor has penetrated into the danger zone based on its position and radius.
     * @param cursorPosition The position of the cursor in world space.
     * @param cursorRadius The radius of the cursor.
     * @return The penetration depth of the cursor into the danger zone.
    */
    public float GetPenetrationDepth(Vector3 cursorPosition, float cursorRadius)
    {
        Vector3 closestPoint = new Vector3(
            Mathf.Clamp(cursorPosition.x, _cubePosition.x - _cubeSize.x / 2f, _cubePosition.x + _cubeSize.x / 2f),
            Mathf.Clamp(cursorPosition.y, _cubePosition.y - _cubeSize.y / 2f, _cubePosition.y + _cubeSize.y / 2f),
            Mathf.Clamp(cursorPosition.z, _cubePosition.z - _cubeSize.z / 2f, _cubePosition.z + _cubeSize.z / 2f)
        );

        Vector3 distanceVector = cursorPosition - closestPoint;
        float distance = distanceVector.magnitude;
        float penetration = cursorRadius - distance;

        _penetration = Mathf.Max(penetration, 0f);
        return _penetration;
    }

    /**
     * @brief Normalizes the penetration depth.
     * @details Converts the penetration depth into a normalized value between 0 and 1 based on the danger radius.
     * @return A float representing the normalized penetration depth.
    */
    public float NormalizedPenetration()
    {
        return Mathf.Clamp01(_penetration / dangerRadius);
    }
}
