using UnityEngine;
using UnityEngine.UI;

/**
 * @brief This class manages the settings pop up in the main menu
 * @details records and updates changes to the settings such as cerumen and canal type.
*/
public class SettingsScript : MonoBehaviour
{

    /**
     * @brief label for cerumen amount
    */
    [SerializeField] Text CerumenText;

    /**
     * @brief slider element for cerumen amount
    */
    [SerializeField] Slider cerumenSlider;

    /**
     * @brief label for canal type
    */
    [SerializeField] Text CanalText;

    /**
     * @brief Updates the cerumen setting
     * @details From the cerumen slider value, set the setting and display it to the user
    */
    public void updateCerumen()
    {
        PlayerPrefs.SetInt("cerumenAmount", (int) cerumenSlider.value);
        CerumenText.text = $"Amount of Cerumen: {PlayerPrefs.GetInt("cerumenAmount")}";
    }

    /**
     * @brief Updates the head setting
     * @details after pressing the left head button, set the setting and display it to the user
    */
    public void updateHeadLeft()
    {
        PlayerPrefs.SetString("headType", "Left");
        CanalText.text = $"Canal Type: 1";
    }

    /**
     * @brief Updates the head setting
     * @details after pressing the center head button, set the setting and display it to the user
    */
    public void updateHeadCenter()
    {
        PlayerPrefs.SetString("headType", "Center");
        CanalText.text = $"Canal Type: 2";
    }

    /**
     * @brief Updates the head setting
     * @details after pressing the right head button, set the setting and display it to the user
    */
    public void updateHeadRight()
    {
        PlayerPrefs.SetString("headType", "Right");
        CanalText.text = $"Canal Type: 3";
    }
}
