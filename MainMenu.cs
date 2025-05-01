using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/**
 * @brief This class handles the main menu scene functionality
 * @details MainMenu initializes starter settings and allows changes to different scenes and opening the settings menu through buttons.
*/
public class MainMenu : MonoBehaviour
{
    /**
     * @brief the settings menu UI element
    */
    [SerializeField] Canvas settingsCanvas;

    /**
     * @brief name of the start game scene
    */
    public string startGameScene;

    /**
     * @brief name of the stats scene
    */
    public string statsScene;

    /**
     * @brief Unity Start method.
     * @details Initialized the head settings on startup and hides the settings canvas
    */
    private void Start()
    {
        PlayerPrefs.SetString("headType", "Center");
        PlayerPrefs.SetInt("cerumenAmount", 15);
        settingsCanvas.gameObject.SetActive(false);
    }

    /**
     * @brief Loads the simulation scene.
     * @details This method is called when the user clicks the button in the main menu scene to enter the simulation scene.
    */
    public void StartGame()
    {
        SceneManager.LoadScene(startGameScene);
    }

    /**
     * @brief Loads the stats scene.
     * @details This method is called when the user clicks the button in the main menu scene to view the stats page.
    */
    public void GoToStatsScene()
    {
        SceneManager.LoadScene(statsScene);
    }

    /**
     * @brief Opens the settings menu
    */
    public void  OpenSettings()
    {
        settingsCanvas.gameObject.SetActive(true);
    }

    /**
     * @brief Closes the settings menu
    */
    public void CloseSettings()
    {
        settingsCanvas.gameObject.SetActive(false);
    }
}
