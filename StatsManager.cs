using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;


/**
 * @brief A class to hold the multiple instances of statistics that are saved/will be saved in a JSON file through lists
 * 
 */
[System.Serializable]
public class StatsData
{
    public List<string> playerNames = new List<string>();
    public List<float> times = new List<float>();
}

/**
 * @brief A class that manages user stats taken from data acquired in the simulation scene
 *
 */
public class StatsManager : MonoBehaviour
{

    /**
     * @brief new instance of StatsData class
    */
    private StatsData statsData = new StatsData();

    /**
     * @brief file path
    */
    private string filePath;

    /**
     * @brief name input by the user
    */
    public string currentName;

    /**
     * @brief elapsed time in the simulation scene
    */
	public float currentElapsedTime;


    /**
     * @brief On awake mark the object to persist and keep track of the file path, then load the stats from the file
     * 
     */
    void Awake() {
        DontDestroyOnLoad(gameObject);
        Debug.Log("StatsManager has been initialized.");
        filePath = Application.persistentDataPath + "/stats.json";
        Debug.Log("Stats saved at: " + filePath);
        LoadStats();
    }

    /**
     * @brief Stores an instance of stats from a user's session and then calls the SaveStats() method to save them
     * 
     * @param playerName
     * @param elapsedTime a float value representing the user's time taken from the current session
     */
    public void AddRecord(string playerName, float elapsedTime)
    {
        statsData.playerNames.Add(playerName);
        statsData.times.Add(elapsedTime);
        SaveStats();
    }

    /**
     * @brief Sets the name of the user
     * 
     * @param n a string that represents the name of the user
     * 
     * @usage
     * setName("Bob"); // currentName will be "Bob"
     */
    public void setName(string n) {
		currentName = n;
	}

    /**
     * @brief Gets the name set by the user
     * 
     * @return The name of the user
     * 
     * @usage
     * String name = getName(); // name will be the value of currentName
     */
    public string getName() {
		return currentName;
	}


    /**
     * @brief Sets the elapsed time as a float value
     * 
     * @param time a float that represents the time taken by the user
     * 
     * @usage
     * setElapsedTime(12.0); // currentElapsedTime stat will be 12.0
     */
    public void setElapsedTime(float time)
    {
        currentElapsedTime = time;
    }


    /**
     * @brief Gets the time taken by the user from the simulation scene
     * 
     * @return The time taken in the simulation scene
     * 
     * @usage
     * float timeTaken = getElapsedTime(); // timeTaken will be the value of the currentElapsedTime
     */
    public float getElapsedTime()
    {
        return currentElapsedTime;
    }

    /**
     * @brief The initial method for keeping track of the name and time of the user called when the user inputs their name
     * 
     * 
     */
    public void SaveCurrentRecord()
    {
        if (!string.IsNullOrEmpty(currentName))
        {
            statsData.playerNames.Add(currentName);
            statsData.times.Add(currentElapsedTime);
            SaveStats();
        }
    }

    /**
     * @brief Retrieve the list of player names stored in the statsData class
     * 
     * @return a list of player names
     *
     */
    public List<string> GetPlayerNames()
    {
        return statsData.playerNames;
    }

    /**
     * @brief retrieve the list of time taken in the simulation stored in the statsData class
     * 
     * @return a list of times
     */
    public List<float> GetTimes()
    {
        return statsData.times;
    }

    /**
     * @brief a method that saves an instance of stats saved in the statsData class to the JSON file
     * 
     */
    private void SaveStats()
    {
        string json = JsonUtility.ToJson(statsData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Saved Stats: " + json);
    }

    /**
     * @brief a method that reads the stats file and loads them into the respective variables when called
     * 
     * 
     */
    private void LoadStats()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            statsData = JsonUtility.FromJson<StatsData>(json);

            if (statsData.playerNames.Count > 0)
            {
                currentName = statsData.playerNames[statsData.playerNames.Count - 1];
                currentElapsedTime = statsData.times[statsData.times.Count - 1];
            }
        }
    }

    // For deleteing and reseting stats, saved for later
    //public void ResetStats()
    //{
    //    statsData = new StatsData();
    //    File.Delete(filePath);    
    //}


}
