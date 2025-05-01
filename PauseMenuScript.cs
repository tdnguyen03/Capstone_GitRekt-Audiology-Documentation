using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{

    /**
     * @brief the UI game object for the pause panels
    */
    public GameObject PausePanel;

    /**
     * @brief name of the start game scene
    */
    public string startGameScene;

    /**
     * @brief Start game method
     * @details Loads the start game scene
    */
    public void StartGame()
    {
        SceneManager.LoadScene(startGameScene);
    }

    /**
     * @brief Pause method
     * @details opens the pause panel and sets the time scale to 0
    */
    public void Pause()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    /**
     * @brief Continue method
     * @details disables the pause panel and sets the time scale to 1
    */
    public void Continue()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    /**
     * @brief Reset button method
     * @details restarts the current scene and sets the time scale to 1
    */
    public void ResetButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}
