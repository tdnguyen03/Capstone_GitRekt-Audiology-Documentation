using UnityEngine;
using System.Collections.Generic;

/// @class CurvedTubeForceFeedback
/// @brief Provides force feedback for a curved tube object.
/// @details This class calculates force feedback based on the cursor's position, velocity, and radius relative to a spline-based curved tube. It also supports visualization of the tube and its closest points.
public class CurvedTubeForceFeedback : MonoBehaviour
{
    [Header("Spline Tube Settings")]
    /// @brief List of control points defining the spline of the tube.
    public List<Transform> controlPoints;

    /// @brief Animation curve defining the radius profile of the tube.
    public AnimationCurve radiusProfile;

    /// @brief Resolution of the spline for sampling points.
    public float splineResolution = 100;

    [Header("Force Feedback")]
    /// @brief Stiffness of the tube for force feedback calculations.
    public float stiffness = 300f;

    /// @brief Damping factor for force feedback calculations.
    public float damping = 1f;

    /// @brief Enables or disables force feedback.
    public bool enableForce = true;

    [Header("Debug")]
    /// @brief Enables or disables visualization of the tube in the editor.
    public bool showGizmos = true;

    /// @brief Enables or disables visualization of the closest point on the tube.
    public bool visualizeClosestPoint = true;

    /// @brief Cached positions of the control points for efficient calculations.
    private Vector3[] cachedPositions;

    /// @brief Closest point on the tube to the cursor.
    private Vector3 _closestPoint;

    /// @brief Penetration depth of the cursor into the tube.
    private float _penetration;

    /**
     * @brief Unity Awake method.
     * @details Caches the control point positions and registers the tube with the HapticManager.
    */
    private void Awake()
    {
        CacheControlPointPositions();
        FindObjectOfType<HapticManager>().RegisterTube(this);
    }

    /**
     * @brief Caches the positions of the control points.
     * @details Stores the world positions of the control points in an array for efficient spline calculations.
    */
    private void CacheControlPointPositions()
    {
        cachedPositions = new Vector3[controlPoints.Count];
        for (int i = 0; i < controlPoints.Count; i++)
        {
            cachedPositions[i] = controlPoints[i].position;
        }
    }

    /**
     * @brief Calculates the force feedback based on cursor interaction.
     * @details Determines the force applied to the cursor based on its position, velocity, and radius relative to the tube.
     * @param cursorPos The position of the cursor in world space.
     * @param cursorVel The velocity of the cursor.
     * @param cursorRadius The radius of the cursor.
     * @return A vector representing the calculated force.
    */
    public Vector3 CalculateForce(Vector3 cursorPos, Vector3 cursorVel, float cursorRadius)
    {
        _penetration = 0f;
        Vector3 force = Vector3.zero;

        float closestDist = float.MaxValue;
        Vector3 bestPoint = Vector3.zero;
        float bestT = 0f;

        for (int i = 0; i <= splineResolution; i++)
        {
            float t = i / (float)splineResolution;
            Vector3 pointOnSpline = GetPointOnSpline(t);
            float dist = Vector3.Distance(cursorPos, pointOnSpline);

            if (dist < closestDist)
            {
                closestDist = dist;
                bestPoint = pointOnSpline;
                bestT = t;
            }
        }

        _closestPoint = bestPoint;
        float wallRadius = radiusProfile.Evaluate(bestT);
        float distanceFromCenterline = Vector3.Distance(cursorPos, bestPoint);

        _penetration = distanceFromCenterline - (wallRadius - cursorRadius);

        if (_penetration > 0 && enableForce)
        {
            Vector3 normal = (bestPoint - cursorPos).normalized;
            force = normal * _penetration * stiffness;
            force -= cursorVel * damping;
        }

        return force;
    }

    /**
     * @brief Gets a point on the spline based on a parameter t.
     * @details Uses Catmull-Rom spline interpolation to calculate a point on the spline.
     * @param t The parameter (0 to 1) representing the position along the spline.
     * @return A vector representing the point on the spline.
    */
    private Vector3 GetPointOnSpline(float t)
    {
        if (cachedPositions == null || cachedPositions.Length < 4)
            return Vector3.zero;

        int numSections = cachedPositions.Length - 3;
        t = Mathf.Clamp01(t) * numSections;
        int currIndex = Mathf.FloorToInt(t);
        t -= currIndex;

        int p0 = Mathf.Clamp(currIndex, 0, cachedPositions.Length - 1);
        int p1 = Mathf.Clamp(currIndex + 1, 0, cachedPositions.Length - 1);
        int p2 = Mathf.Clamp(currIndex + 2, 0, cachedPositions.Length - 1);
        int p3 = Mathf.Clamp(currIndex + 3, 0, cachedPositions.Length - 1);

        return 0.5f * (
            (2f * cachedPositions[p1]) +
            (-cachedPositions[p0] + cachedPositions[p2]) * t +
            (2f * cachedPositions[p0] - 5f * cachedPositions[p1] + 4f * cachedPositions[p2] - cachedPositions[p3]) * t * t +
            (-cachedPositions[p0] + 3f * cachedPositions[p1] - 3f * cachedPositions[p2] + cachedPositions[p3]) * t * t * t
        );
    }

    /**
     * @brief Unity OnDrawGizmos method.
     * @details Visualizes the spline and radius profile in the editor using gizmos.
    */
    private void OnDrawGizmos()
    {
        if (!showGizmos || controlPoints == null || controlPoints.Count < 4)
            return;

        Gizmos.color = Color.green;

        int samples = 100;
        Vector3 prev = EvaluateSplineFromTransforms(0f);

        for (int i = 1; i <= samples; i++)
        {
            float t = i / (float)samples;
            Vector3 curr = EvaluateSplineFromTransforms(t);
            Gizmos.DrawLine(prev, curr);
            prev = curr;
        }

        Gizmos.color = Color.cyan;
        for (int i = 0; i <= samples; i += 10)
        {
            float t = i / (float)samples;
            Vector3 p = EvaluateSplineFromTransforms(t);
            float r = radiusProfile != null ? radiusProfile.Evaluate(t) : 0.01f;
            Gizmos.DrawWireSphere(p, r);
        }
    }

    /**
     * @brief Evaluates a point on the spline based on control points.
     * @details Uses Catmull-Rom spline interpolation to calculate a point on the spline.
     * @param t The parameter (0 to 1) representing the position along the spline.
     * @return A vector representing the point on the spline.
    */
    private Vector3 EvaluateSplineFromTransforms(float t)
    {
        if (controlPoints == null || controlPoints.Count < 4)
            return Vector3.zero;

        int numSections = controlPoints.Count - 3;
        t = Mathf.Clamp01(t) * numSections;
        int currIndex = Mathf.FloorToInt(t);
        t -= currIndex;

        int p0 = Mathf.Clamp(currIndex, 0, controlPoints.Count - 1);
        int p1 = Mathf.Clamp(currIndex + 1, 0, controlPoints.Count - 1);
        int p2 = Mathf.Clamp(currIndex + 2, 0, controlPoints.Count - 1);
        int p3 = Mathf.Clamp(currIndex + 3, 0, controlPoints.Count - 1);

        Vector3 pos0 = controlPoints[p0].position;
        Vector3 pos1 = controlPoints[p1].position;
        Vector3 pos2 = controlPoints[p2].position;
        Vector3 pos3 = controlPoints[p3].position;

        return 0.5f * (
            (2f * pos1) +
            (-pos0 + pos2) * t +
            (2f * pos0 - 5f * pos1 + 4f * pos2 - pos3) * t * t +
            (-pos0 + 3f * pos1 - 3f * pos2 + pos3) * t * t * t
        );
    }

    /**
     * @brief Calculates the radial wall force for a sphere interacting with the tube.
     * @details Computes the force applied to the sphere based on its position, velocity, and radius relative to the tube's walls.
     * @param spherePosition The position of the sphere in world space.
     * @param sphereVelocity The velocity of the sphere.
     * @param sphereRadius The radius of the sphere.
     * @return A vector representing the calculated radial wall force.
    */
    public Vector3 CalculateRadialWallForce(Vector3 spherePosition, Vector3 sphereVelocity, float sphereRadius)
    {
        float closestDist = float.MaxValue;
        Vector3 bestPoint = Vector3.zero;
        float bestT = 0f;

        for (int i = 0; i <= splineResolution; i++)
        {
            float t = i / (float)splineResolution;
            Vector3 point = GetPointOnSpline(t);
            float dist = Vector3.Distance(spherePosition, point);

            if (dist < closestDist)
            {
                closestDist = dist;
                bestPoint = point;
                bestT = t;
            }
        }

        Vector3 radial = spherePosition - bestPoint;
        float distFromCenter = radial.magnitude;
        float wallRadius = radiusProfile.Evaluate(bestT);
        float targetDist = wallRadius - sphereRadius;
        float deviation = targetDist - distFromCenter;

        Vector3 normal = radial.normalized;

        float springForce = Mathf.Sign(deviation) * Mathf.Pow(Mathf.Abs(deviation), 2f) * stiffness * 1.5f;

        Vector3 force = normal * springForce;
        force -= sphereVelocity * damping;

        return force;
    }
}
