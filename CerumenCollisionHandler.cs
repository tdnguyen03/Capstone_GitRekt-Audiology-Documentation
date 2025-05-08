using UnityEngine;
using UnityEngine.UI;

/// @class DespawnTrigger
/// @brief Handles the collision of cerumen objects and updates the UI.
/// @details This class detects collisions with cerumen objects, removes them, updates the UI text, and triggers the spawning of new objects.
public class DespawnTrigger : MonoBehaviour
{
    /// @brief Counter for the number of cerumen objects removed.
    private int value = 0;

    /// @brief UI Text element to display the number of cerumen removed.
    [SerializeField] private Text CerumenText;

    /// @brief Reference to the SpawnSphereScript for spawning new objects.
    [SerializeField] private SpawnSphereScript spawnSphereScript;

    /**
     * @brief Unity OnTriggerEnter method.
     * @details Detects collisions with cerumen objects, removes them, updates the UI, and spawns new objects.
     * @param other The collider of the object that triggered the collision.
    */
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovingSphere>() != null)
        {
            value++;
            Destroy(other.gameObject);
            CerumenText.text = $"Cerumen removed: {value}";
            spawnSphereScript.Spawner(1);
        }
    }
}
