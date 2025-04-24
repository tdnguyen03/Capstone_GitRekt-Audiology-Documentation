using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * @brief This class handles the end scene functionality
 * @details EndSceneScript retrieves and displays the stats to the user and allows changes to different scenes through buttons.
*/
public class EndSceneScript : MonoBehaviour
{
    public string mainMenuScene;
    public string statsScene;
    public Text introText;
    public Text timeText;

    /**
     * @brief Unity Start method.
     * @details Finds the stats manager and if it is found display each stat from the session to the user through the saved data in StatsManager
    */
    private void Start()
    {
        GameObject statsManager = GameObject.Find("StatsManager");

        if (statsManager != null )
        {
            string name = statsManager.GetComponent<StatsManager>().getName();
            float elapsedTime = statsManager.GetComponent<StatsManager>().getElapsedTime();

            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            introText.text = $"Congratulations {name}! \r\nHere is how you did:";
            timeText.text = $"Time Taken: {formattedTime}";
        }


    }

    /**
     * @brief Loads the main menu scene.
     * @details This method is called when the user clicks the button in the end scene scene to return to the main menu.
    */
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    /**
     * @brief Loads the stats scene.
     * @details This method is called when the user clicks the button in the end scene scene to go to the stats page.
    */
    public void GoToStatsScene()
    {
        SceneManager.LoadScene(statsScene); 
    }
}
