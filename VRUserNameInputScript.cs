using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief A class that manages the scene to start the game after a name is entered
*/
public class VRUserNameInputScript : MonoBehaviour 
{
    /**
     * @brief the text box that contains the user's name
    */
    [SerializeField] InputField inputField;

     /**
     * @brief script for the timer
    */
    [SerializeField] TimerScript timerScript;

    /**
     * @brief stats manager game object
    */
    [SerializeField] GameObject statsManager;

    /**
     * @brief keyboard game object
    */
    [SerializeField] GameObject keyboardUI;

    /**
     * @brief timer gmae object
    */
    [SerializeField] GameObject timerUI;


    /**
     * @brief After entering a name on the keyboard, save it to the stats manager and hide the elements, then start the game
    */
    public void ValidateInput()
    {
        string input = inputField.text;

        if (!string.IsNullOrEmpty(input))
        {
            if (statsManager != null)
            {
                statsManager.GetComponent<StatsManager>().setName(input);
                keyboardUI.gameObject.SetActive(false);
                timerUI.gameObject.SetActive(true);

                timerScript.StartTimer();
                Time.timeScale = 1;
            }
            else
            {
                Debug.LogWarning("StatsManager not found or has been destroyed.");
            }
        }

   
    }
    
}
