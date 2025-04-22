using UnityEngine;
using UnityEngine.UI;

public class KeyboardScript : MonoBehaviour
{
    public InputField inputField;
    public Text textField;

    /**
     * @brief Button press method
     * @details From the text element specified in the editor, append that text element to a text box.
    */
    public void OnButtonPress()
    {
        inputField.text += textField.text;
    }
}
