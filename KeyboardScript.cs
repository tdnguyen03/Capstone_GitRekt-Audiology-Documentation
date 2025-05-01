using UnityEngine;
using UnityEngine.UI;

/**
 * @brief This class handles keyboard functionality
*/
public class KeyboardScript : MonoBehaviour
{

    /**
     * @brief text box element for keyboard input
    */
    public InputField inputField;

    /**
     * @brief text of the key to append to the input field
    */
    public Text textField;

    /**
     * @brief Button press method
     * @details From the text element specified in the editor, append that text element to a text box.
    */
    public void OnButtonPress()
    {
        inputField.text += textField.text;
    }

    /**
     * @brief Keyboard backspace method
     * @details From the text input specified, delete the last element
    */
    public void OnBackspacePress()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }
}
