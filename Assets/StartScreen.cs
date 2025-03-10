using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class StartScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup titleCanvasGroup;
    public TextMeshProUGUI pressAnyKeyText;

    [Header("Audio")]
    public AudioSource introAudio;

    [Header("Settings")]
    public float fadeDuration = 2f;
    public string nextSceneName = "MainScene";

    private bool inputReceived = false;
    private InputAction anyKeyAction; // New Input Action for detecting key presses

    void Awake()
    {
        // Create an input action that listens for any key/button press
        anyKeyAction = new InputAction(binding: "<Keyboard>/anyKey");
        anyKeyAction.performed += ctx => OnAnyKeyPressed();
    }

    void OnEnable()
    {
        anyKeyAction.Enable(); // Enable the input action
    }

    void OnDisable()
    {
        anyKeyAction.Disable(); // Disable the input action when not needed
    }

    void Start()
    {
        if (introAudio != null)
            introAudio.Play();

        if (titleCanvasGroup != null)
            titleCanvasGroup.alpha = 0;
        if (pressAnyKeyText != null)
            pressAnyKeyText.gameObject.SetActive(false);

        StartCoroutine(ShowTitleScreen());
    }

    private void OnAnyKeyPressed()
    {
        inputReceived = true;
    }

    IEnumerator ShowTitleScreen()
    {
        // Fade in the title
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (titleCanvasGroup != null)
                titleCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        if (titleCanvasGroup != null)
            titleCanvasGroup.alpha = 1f;

        if (pressAnyKeyText != null)
            pressAnyKeyText.gameObject.SetActive(true);

        StartCoroutine(BlinkText());

        // Wait for user input
        while (!inputReceived)
        {
            yield return null;
        }

        // Fade out the title screen
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (titleCanvasGroup != null)
                titleCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        if (titleCanvasGroup != null)
            titleCanvasGroup.alpha = 0f;

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator BlinkText()
    {
        while (!inputReceived)
        {
            if (pressAnyKeyText != null)
                pressAnyKeyText.enabled = !pressAnyKeyText.enabled;
            yield return new WaitForSeconds(0.5f);
        }
        if (pressAnyKeyText != null)
            pressAnyKeyText.enabled = true;
    }
}