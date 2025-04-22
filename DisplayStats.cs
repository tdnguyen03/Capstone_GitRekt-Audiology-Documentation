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
     * @brief Stats Search Button
     * @details when clicked, load the stats with search input
    */
    public void OnSearchButtonPressed()
    {
        string input = searchInput.text;
        LoadStats(input);
    }


    /**
     * @brief Loads player statistics from the JSON file and displays them in the UI Text element.
     * @details Reads the stats JSON file, parses it, and updates the `statsText` UI element with the player names and times using the search filter. If the file does not exist or is empty, it displays a default message.
     * @param nameFilter the filter for the search bar
     * 
    */
    private void LoadStats(string nameFilter = "")
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        StatsData statsData = JsonUtility.FromJson<StatsData>(json);

        List<(string name, float time)> combined = new List<(string, float)>();

        for (int i = 0; i < statsData.playerNames.Count; i++)
        {
            string player = statsData.playerNames[i];
            float time = statsData.times[i];

            if (!string.IsNullOrEmpty(nameFilter) &&
                !player.ToLower().Contains(nameFilter.ToLower()))
            {
                continue;
            }

            combined.Add((player, time));
        }

        switch (currentSortMode)
        {
            case SortMode.Fastest:
                combined.Sort((a, b) => a.time.CompareTo(b.time));
                break;
            case SortMode.Slowest:
                combined.Sort((a, b) => b.time.CompareTo(a.time));
                break;
            case SortMode.LastCompleted:
                break;
        }

        foreach (var (player, time) in combined)
        {
            GameObject row = Instantiate(statEntryRowPrefab, contentParent);
            row.transform.Find("PlayerNameText").GetComponent<Text>().text = player;
            row.transform.Find("TimeText").GetComponent<Text>().text = FormatTime(time);
            row.SetActive(true);
        }
    }

    /**
     * @brief When the sort options change, sort the stats
     * @param index the index of the sort mode being used
     * 
    */
    public void OnSortOptionChanged(int index)
    {
        Debug.Log("Dropdown changed: " + index);
        currentSortMode = (SortMode)index;
        LoadStats(searchInput.text);
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