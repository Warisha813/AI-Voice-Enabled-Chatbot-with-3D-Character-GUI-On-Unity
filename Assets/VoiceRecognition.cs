using UnityEngine;
using UnityEngine.Windows.Speech;
using Newtonsoft.Json; // Remove this if unused
using System.Collections; // Remove this if unused

public class VoiceRecognition : MonoBehaviour
{
    private DictationRecognizer dictationRecognizer;
    private string recognizedText = string.Empty;

    public ChatbotClient chatbotClient; // Reference to ChatbotClient script
    public Animator animator; // Reference to Animator for triggering animations
    public UnityEngine.UI.Text feedbackText; // UI feedback element

    private bool isListening = false;

    void Start()
    {
        // Initialize DictationRecognizer
        dictationRecognizer = new DictationRecognizer();

        // Set up events for dictation recognizer
        dictationRecognizer.DictationResult += OnDictationResult;
        dictationRecognizer.DictationComplete += OnDictationComplete;
        dictationRecognizer.DictationError += OnDictationError;

        // Start listening
        StartListening();
    }

    private void StartListening()
    {
        if (!isListening)
        {
            dictationRecognizer.Start();
            isListening = true;
            Debug.Log("Started Listening...");
            UpdateFeedback("Listening...");
        }
    }

    private void StopListening()
    {
        if (isListening)
        {
            dictationRecognizer.Stop();
            isListening = false;
            Debug.Log("Stopped Listening.");
            UpdateFeedback("Stopped Listening.");
        }
    }

    private void UpdateFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        recognizedText = text;
        Debug.Log("Recognized Text: " + recognizedText);
        UpdateFeedback("Processing: " + recognizedText);

        if (chatbotClient != null)
        {
            chatbotClient.SendMessageToFlask(recognizedText);
        }
        else
        {
            Debug.LogError("ChatbotClient not assigned in VoiceRecognition!");
        }
    }

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        Debug.Log("Dictation Complete: " + cause);
        if (cause != DictationCompletionCause.Complete && cause != DictationCompletionCause.Canceled)
        {
            Debug.LogWarning("Dictation stopped unexpectedly. Restarting...");
            RestartListening();
        }
    }

    private void OnDictationError(string error, int hresult)
    {
        Debug.LogError("Dictation Error: " + error);
        UpdateFeedback("Error occurred. Restarting...");
        RestartListening();
    }

    private void RestartListening()
    {
        StopListening();
        StartListening();
    }

    // Trigger animation and pause dictation for a short duration
    public void TriggerAnimationWithPause(string animationTrigger)
    {
        if (!isListening) return; // Avoid re-triggering if already paused
        
        StopListening(); // Stop dictation during animation
        animator.SetTrigger(animationTrigger); // Trigger animation

        // Pause dictation for 2-3 seconds and then restart
        StartCoroutine(PauseAndRestartListening(2.5f)); // Adjust the delay as needed
    }

    private IEnumerator PauseAndRestartListening(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartListening();
    }

    public void StopDictation()
    {
        StopListening();
        UpdateFeedback("Dictation Stopped.");
    }
}
