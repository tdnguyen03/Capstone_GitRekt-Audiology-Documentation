using UnityEngine;


/**
 * @brief This class manages the spawning of the head object in the simulation scene
*/
public class HeadLoader : MonoBehaviour
{
    [SerializeField] GameObject leftHead;
    [SerializeField] GameObject rightHead;
    [SerializeField] GameObject head;

    /**
     * @brief Unity Start method.
     * @details Retrieves the player head type setting and match the headType by hiding all other head elements and only showing the active head from the setting
    */
    void Start()
    {
        string headType = PlayerPrefs.GetString("headType");
        if (headType == "Right")
        {
            head.SetActive(false);
            rightHead.SetActive(true);
        }
        else if (headType == "Left")
        {
            head.SetActive(false);
            leftHead.SetActive(true);
        }
        else
        {
            head.SetActive(true);
            leftHead.SetActive(false);
            rightHead.SetActive(false);
        }
    }
}
