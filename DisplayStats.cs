using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

/**
 * @brief This class handles the display of player statistics in the Stats scene from a JSON file.
 * @details It reads the player names and their corresponding simulation times from the JSON file and displays them in a UI Text element.
 * It also provides a method to return to the main menu scene.
*/
public class StatsSceneScript : MonoBehaviour
{
    /**
     * @brief UI Text element to display player statistics.
    */
    public Text statsText;
    /**
     * @brief Path to the JSON file containing player statistics.
    */
    private string filePath;
    /**
     * @brief Name of the main menu scene to load.
    */
    public string mainMenuScene;

    /**
     * @brief Unity Start method.
     * @details Initializes the file path for the JSON file and calls the LoadStats method to read and display player statistics.
    */
    void Start()
    {
        filePath = Application.persistentDataPath + "/stats.json";
        LoadStats();
    }

    /**
     * @brief Loads player statistics from the JSON file and displays them in the UI Text element.
     * @details Reads the stats JSON file, parses it, and updates the `statsText` UI element with the player names and times.
     * If the file does not exist or is empty, it displays a default message.
    */
    private void LoadStats()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Debug.Log("Loaded JSON: " + json);
            StatsData statsData = JsonUtility.FromJson<StatsData>(json);

            if (statsData != null && statsData.playerNames.Count > 0)
            {
                statsText.text = "Player Stats:\n";
                for (int i = 0; i < statsData.playerNames.Count; i++)
                {
                    string formattedTime = FormatTime(statsData.times[i]);
                    statsText.text += $"{statsData.playerNames[i]} - {formattedTime}\n";
                }
            }
            else
            {
                statsText.text = "No stats recorded yet.";
                Debug.Log("StatsData is empty.");
            }
        }
        else
        {
            statsText.text = "No stats recorded yet.";
            Debug.Log("Stats file not found: " + filePath);
        }
    }

    /**
     * @brief Formats the time in seconds to a string in MM:SS format.
     * @param time The time in seconds to format.
     * @return A string representing the formatted time.
    */
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /**
     * @brief Loads the main menu scene.
     * @details This method is called when the user clicks the button in the Stats scene to return to the main menu.
    */
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}