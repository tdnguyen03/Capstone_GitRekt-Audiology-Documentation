using System.Collections.Generic;
using Haply.Inverse.DeviceControllers;
using Haply.Inverse.DeviceData;
using UnityEngine;

/// @class HapticManager
/// @brief Manages haptic feedback for various objects in the scene.
/// @details This class handles the registration of haptic-enabled objects and calculates the total force applied to the cursor. It also manages danger zones and updates the danger overlay UI.
public class HapticManager : MonoBehaviour
{
    /// @brief Reference to the Inverse3Controller for haptic device control.
    public Inverse3Controller inverse3;

    /// @brief List of registered cube force feedback objects.
    private readonly List<CubeForceFeedback> cubes = new();

    /// @brief List of registered sphere force feedback objects.
    private readonly List<SphereForceFeedback> spheres = new();

    /// @brief List of registered danger zones.
    private readonly List<DangerZone> dangerZones = new();

    /// @brief List of registered curved tube force feedback objects.
    private readonly List<CurvedTubeForceFeedback> curvedTubes = new();

    /// @brief List of registered moving spheres.
    private readonly List<MovingSphere> movingSpheres = new();

    /// @brief List of registered plane force feedback objects.
    private readonly List<PlaneForceFeedback> planes = new();

    /// @brief List of registered curette haptic triggers.
    private readonly List<CuretteHapticTrigger> curetteTriggers = new();

    /// @brief List of registered cylinder force feedback objects.
    private readonly List<CylinderForceFeedback> cylinders = new();

    /**
     * @brief Unity OnEnable method.
     * @details Subscribes to the `DeviceStateChanged` event when the component is enabled.
    */
    private void OnEnable()
    {
        if (inverse3 == null)
        {
            inverse3 = FindFirstObjectByType<Inverse3Controller>();
        }

        inverse3.DeviceStateChanged += OnDeviceStateChanged;
    }

    /**
     * @brief Unity OnDisable method.
     * @details Unsubscribes from the `DeviceStateChanged` event when the component is disabled.
    */
    private void OnDisable()
    {
        if (inverse3 != null)
            inverse3.DeviceStateChanged -= OnDeviceStateChanged;
    }

    /**
     * @brief Registers a cube force feedback object.
     * @param cube The cube force feedback object to register.
    */
    public void RegisterCube(CubeForceFeedback cube)
    {
        if (!cubes.Contains(cube))
            cubes.Add(cube);
    }

    /**
     * @brief Registers a sphere force feedback object.
     * @param sphere The sphere force feedback object to register.
    */
    public void RegisterSphere(SphereForceFeedback sphere)
    {
        if (!spheres.Contains(sphere))
            spheres.Add(sphere);
    }

    /**
     * @brief Registers a danger zone.
     * @param zone The danger zone to register.
    */
    public void RegisterDangerZone(DangerZone zone)
    {
        if (!dangerZones.Contains(zone))
            dangerZones.Add(zone);
    }

    /**
     * @brief Registers a curved tube force feedback object.
     * @param tube The curved tube force feedback object to register.
    */
    public void RegisterTube(CurvedTubeForceFeedback tube)
    {
        if (!curvedTubes.Contains(tube))
            curvedTubes.Add(tube);
    }

    /**
     * @brief Registers a moving sphere.
     * @param sphere The moving sphere to register.
    */
    public void RegisterMovingSphere(MovingSphere sphere)
    {
        if (!movingSpheres.Contains(sphere))
            movingSpheres.Add(sphere);
    }

    /**
     * @brief Registers a plane force feedback object.
     * @param plane The plane force feedback object to register.
    */
    public void RegisterPlane(PlaneForceFeedback plane)
    {
        if (!planes.Contains(plane))
            planes.Add(plane);
    }

    /**
     * @brief Registers a curette haptic trigger.
     * @param trigger The curette haptic trigger to register.
    */
    public void RegisterCuretteTrigger(CuretteHapticTrigger trigger)
    {
        if (!curetteTriggers.Contains(trigger))
            curetteTriggers.Add(trigger);
    }

    /**
     * @brief Registers a cylinder force feedback object.
     * @param cylinder The cylinder force feedback object to register.
    */
    public void RegisterCylinder(CylinderForceFeedback cylinder)
    {
        if (!cylinders.Contains(cylinder))
            cylinders.Add(cylinder);
    }

    /**
     * @brief Handles the `DeviceStateChanged` event.
     * @details Calculates the total force applied to the cursor and updates the danger overlay UI.
     * @param sender The object that triggered the event.
     * @param args The event arguments containing the device data.
    */
    private void OnDeviceStateChanged(object sender, Inverse3EventArgs args)
    {
        Vector3 pos = args.DeviceController.CursorLocalPosition;
        Vector3 vel = args.DeviceController.CursorLocalVelocity;
        float radius = args.DeviceController.Cursor.Radius;

        Vector3 totalForce = Vector3.zero;
        float maxDanger = 0f;
        string warningMessage = "";

        foreach (var cube in cubes)
        {
            totalForce += cube.CalculateForce(pos, vel, radius);

            float cubeDanger = cube.NormalizedPenetration();
            if (cubeDanger > maxDanger)
            {
                maxDanger = cubeDanger;
                warningMessage = cube.warningMessage;
            }
        }

        foreach (var sphere in spheres)
        {
            totalForce += sphere.CalculateForce(pos, vel, radius);
        }

        foreach (var zone in dangerZones)
        {
            float penetration = zone.GetPenetrationDepth(pos, radius);
            float zoneDanger = zone.NormalizedPenetration();
            if (zoneDanger > maxDanger)
            {
                maxDanger = zoneDanger;
                warningMessage = zone.warningMessage;
            }
        }

        foreach (var tube in curvedTubes)
        {
            totalForce += tube.CalculateForce(pos, vel, radius);
        }

        foreach (var sphere in movingSpheres)
        {
            totalForce += sphere.CalculateForce(pos, vel, radius);
        }

        foreach (var plane in planes)
        {
            totalForce += plane.CalculateForce(pos, vel, radius);
        }

        foreach (var trigger in curetteTriggers)
        {
            if (trigger.isTouchingCube)
            {
                totalForce += trigger.CalculateForce(pos, vel, radius);
            }
        }

        foreach (var cylinder in cylinders)
        {
            totalForce += cylinder.CalculateForce(pos, vel, radius);
        }

        args.DeviceController.SetCursorLocalForce(totalForce);

        DangerOverlayUI.SetIntensity(maxDanger, warningMessage);
    }
}
