using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

/// @class DangerOverlayUI
/// @brief Manages the danger overlay UI and warning system.
/// @details This class handles the visual and audio feedback for danger situations, including a red overlay and warning text. It also plays a scream sound when entering a danger state.
public class DangerOverlayUI : MonoBehaviour
{
    /// @brief Singleton instance of the DangerOverlayUI.
    public static DangerOverlayUI Instance;

    /// @brief Audio clip to play when entering a danger state.
    public AudioClip scream;

    /// @brief Audio source component for playing the scream sound.
    [SerializeField] private AudioSource audioData;

    /// @brief UI image for the danger overlay.
    [SerializeField] private Image overlayImage;

    /// @brief TextMeshPro element for displaying warning messages.
    [SerializeField] private TextMeshProUGUI warningText;

    /// @brief Color of the danger overlay.
    [SerializeField] private Color dangerColor = new Color(1f, 0f, 0f, 0.4f);

    /// @brief Target alpha value for the overlay's transparency.
    private float _targetAlpha = 0f;

    /// @brief Current alpha value for the overlay's transparency.
    private float _currentAlpha = 0f;

    /// @brief Pending warning message to display.
    private string _pendingMessage = "";

    /// @brief Flag to indicate if the scream sound should play on the next frame.
    private bool _playScreamNextFrame = false;

    /**
     * @brief Unity Awake method.
     * @details Initializes the singleton instance and sets up the initial state of the overlay and warning text.
    */
    private void Awake()
    {
        Instance = this;

        if (warningText != null)
        {
            warningText.alpha = 0f;
            warningText.text = "";  // Completely empty at start
        }

        if (overlayImage != null)
        {
            Color clearColor = dangerColor;
            clearColor.a = 0f;
            overlayImage.color = clearColor;
        }
    }

    /**
     * @brief Sets the intensity of the danger overlay.
     * @details Updates the target alpha and pending warning message. Triggers the scream sound if entering a danger state.
     * @param normalized The normalized intensity value (0 to 1).
     * @param message The warning message to display.
    */
    public static void SetIntensity(float normalized, string message)
    {
        if (Instance == null) return;

        // Request audio playback if entering danger
        if (normalized > 0f && Instance._targetAlpha == 0f)
        {
            Instance._playScreamNextFrame = true;
        }

        Instance._targetAlpha = Mathf.Clamp01(normalized);
        Instance._pendingMessage = message;
    }

    /**
     * @brief Unity Start method.
     * @details Initializes the audio source component.
    */
    private void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    /**
     * @brief Unity Update method.
     * @details Updates the overlay's transparency, plays the scream sound if needed, and updates the warning text.
    */
    private void Update()
    {
        if (overlayImage == null || warningText == null) return;

        // Smoothly interpolate the current alpha to the target alpha
        _currentAlpha = Mathf.Lerp(_currentAlpha, _targetAlpha, Time.deltaTime * 10f);

        // Update the overlay color
        Color newColor = dangerColor;
        newColor.a = Mathf.Lerp(0f, dangerColor.a, _currentAlpha);
        overlayImage.color = newColor;

        if (_currentAlpha > 0.01f)
        {
            // Play the scream sound if entering danger
            if (_playScreamNextFrame)
            {
                if (!audioData.isPlaying)
                {
                    audioData.clip = scream;
                    audioData.Play();
                }
                _playScreamNextFrame = false;
            }

            // Update the warning text
            warningText.text = _pendingMessage;
            warningText.alpha = Mathf.Clamp01(_currentAlpha * 2f);
        }
        else
        {
            // Clear the warning text when not in danger
            warningText.text = "";
            warningText.alpha = 0f;
        }
    }
}
