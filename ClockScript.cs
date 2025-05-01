using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class ClockScript : MonoBehaviour
{

    /**
     * @brief text mesh pro element of the timer
    */
    [SerializeField] private TMP_Text timerText;

    /**
     * @brief text mesh pro element of the pauseText
    */
    [SerializeField] private TMP_Text pauseText;

    /**
     * @brief variable for the time elapsed in the simulation while the clock is running
    */
    private float elapsedTime;

    /**
     * @brief boolean to keep track of run state
    */
    private bool isRunning = false;

    /**
     * @brief name of the end game scene for scene transfer
    */
    public string endGameScene;

    //[SerializeField] private GameObject statsManager;
    
    /**
     * @brief name of the start game scene for scene transfer
    */
    public string startGameScene;

    /**
     * @brief Start Game method
     * @details loads the start game scene
    */
    public void StartGame()
    {
        SceneManager.LoadScene(startGameScene);
    }

    void Start() {
        
    }


    /**
     * @brief Button to restart the scene
     * @details Resets current scene and timescale
    */
    public void ResetButton()
    {
/*         StatsManager[] allManagers = FindObjectsByType<StatsManager>(FindObjectsSortMode.None);

        foreach (StatsManager sm in allManagers)
        {
            DestroyImmediate(sm.gameObject);  // instead of Destroy()
        } */
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    /**
     * @brief Unity Update method for the clock
     * @details While the program is running keep track of the time in the stats manager and let the user be able to pause
    */
    void Update()
    {
        if (isRunning)
        {
            GameObject statsManager = GameObject.Find("StatsManager");
            pauseText.text = "Pause";
            elapsedTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);


            statsManager.GetComponent<StatsManager>().setElapsedTime(elapsedTime);


    
        }
    }

    /**
     * @brief Start clock method
     * @details sets the running variable to true so that the game can run
    */
    public void StartClock()
    {
        isRunning = true;
    }

    /**
     * @brief Check method for if the clock is running
     * @details returns the run status
     * @return isRunning run status
    */
    public bool IsClockRunning()
    {
        return isRunning;
    }

    /**
     * @brief get the time elapsed in the simulation
     * @details returns the elapsed time
     * @return elapsedTime elapsed time
    */
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    /**
     * @brief Exit the simulation
     * @details pauses the game, saves the stats, moves on to the end game scene
    */
    public void ExitSimulation()
    {
        GameObject statsManager = GameObject.Find("StatsManager");
        Debug.Log("Exiting simulation");
        isRunning = false;
        statsManager.GetComponent<StatsManager>().SaveCurrentRecord();
        SceneManager.LoadScene(endGameScene);
    }

    /**
     * @brief Pause clock method
     * @details if paused run program and let the user pause, if program is running pause and let the user resume
    */
    public void PauseClock()
    {
        isRunning = !isRunning;

        if (isRunning)
        {
            Debug.Log("Clock resumed");
            pauseText.text = "Pause";
            pauseText.color = Color.green;
        }
        else
        {
            Debug.Log("Clock paused");
            pauseText.text = "Resume";
            pauseText.color = Color.yellow;
        }
    }
}
