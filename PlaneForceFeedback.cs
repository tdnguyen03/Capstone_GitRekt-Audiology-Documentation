using UnityEngine;

/// @class PlaneForceFeedback
/// @brief Provides force feedback for a horizontal plane.
/// @details This class calculates the force feedback based on the cursor's position, velocity, and radius relative to a horizontal plane.
public class PlaneForceFeedback : MonoBehaviour
{
    /// @brief Stiffness of the plane for force feedback calculations.
    [Range(0, 800)]
    public float stiffness = 300f;

    /// @brief Damping factor for force feedback calculations.
    [Range(0, 3)]
    public float damping = 1f;

    /// @brief Y-coordinate of the plane in world space.
    private float planeY;

    /**
     * @brief Unity Awake method.
     * @details Initializes the plane's Y-coordinate and registers it with the HapticManager.
    */
    private void Awake()
    {
        planeY = transform.position.y;
        FindObjectOfType<HapticManager>().RegisterPlane(this);
    }

    /**
     * @brief Calculates the force feedback based on cursor interaction.
     * @details Determines the force applied to the cursor based on its position, velocity, and radius relative to the plane.
     * @param cursorPosition The position of the cursor in world space.
     * @param cursorVelocity The velocity of the cursor.
     * @param cursorRadius The radius of the cursor.
     * @return A vector representing the calculated force.
    */
    public Vector3 CalculateForce(Vector3 cursorPosition, Vector3 cursorVelocity, float cursorRadius)
    {
        Vector3 force = Vector3.zero;

        float penetration = (planeY - (cursorPosition.y - cursorRadius));

        if (penetration > 0)
        {
            Vector3 normal = Vector3.up;
            force = normal * penetration * stiffness;
            force -= cursorVelocity * damping;
        }

        return force;
    }
}
