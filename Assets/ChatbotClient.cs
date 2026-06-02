using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ChatbotClient : MonoBehaviour
{
    public Animator animator; // Animator to control animations
    private string flaskServerUrl = "http://127.0.0.1:5000/chat"; // Flask server URL

    // Flag to ensure the greeting message is sent only once
    private bool greetingSent = false;

    void Start()
    {
        // Log the current state of animator assignment
        Debug.Log("Checking Animator at Start:");
        if (animator == null)
        {
            Debug.LogError("Animator not assigned!");

            // Attempt to find the Animator component on the GameObject named "5"
            GameObject mainGameObject = GameObject.Find("5");

            if (mainGameObject != null)
            {
                animator = mainGameObject.GetComponent<Animator>();
                if (animator == null)
                {
                    Debug.LogError("Animator component not found on GameObject '5'.");
                }
                else
                {
                    Debug.Log("Animator successfully assigned dynamically.");
                }
            }
            else
            {
                Debug.LogError("GameObject '5' not found.");
            }
        }
        else
        {
            Debug.Log("Animator is already assigned.");
        }

        // Send a greeting message if it hasn't been sent already
        if (!greetingSent)
        {
            SendMessageToFlask("greeting");
            greetingSent = true; // Set flag to true to prevent future greetings
        }
    }

    // Method to send recognized text to Flask server
    public void SendMessageToFlask(string message)
    {
        Debug.Log($"Sending message to Flask: {message}");
        StartCoroutine(SendRequestToFlask(message));
    }

    // Coroutine to send POST request to Flask
    private IEnumerator SendRequestToFlask(string message)
    {
        // Create the JSON object
        var requestData = new { message = message };
        string json = JsonConvert.SerializeObject(requestData);

        // Create UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest(flaskServerUrl, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request and wait for the response
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string response = request.downloadHandler.text;
                Debug.Log($"Received raw JSON response: {response}");

                // Process the response and trigger actions
                ProcessResponse(response);
            }
        }
    }

    // Process the response and trigger the correct animation or action
    private void ProcessResponse(string jsonResponse)
    {
        try
        {
            Response response = JsonUtility.FromJson<Response>(jsonResponse);

            if (string.IsNullOrEmpty(response.action))
            {
                Debug.LogWarning("Received empty action in response.");
                return;
            }

            switch (response.action)
            {
                case "OpenWebsite":
                    OpenWebsite(response.message); // The URL is passed as `message` here
                    break;
                case "StartTalking":
                    TriggerAnimation("Talking");
                    break;
                case "StartDancing":
                    TriggerAnimation("Dancing");
                    break;
                case "StartThinking":
                    TriggerAnimation("Thinking");
                    break;
                case "StartSalute":
                    TriggerAnimation("Salute");
                    break;
                case "Greeting":
                    TriggerAnimation("Greeting");
                    Debug.Log("Triggering StartGreetings animation");
                    break;
                default:
                    Debug.LogWarning($"Unknown action received: {response.action}");
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error processing response: {e.Message}");
        }
    }

    // Function to trigger animations and reset to idle
    private void TriggerAnimation(string triggerName)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
            return;
        }

        animator.SetTrigger(triggerName); // Trigger the requested animation
        StartCoroutine(ReturnToIdle());
    }

    // Coroutine to return to idle after a delay
    private IEnumerator ReturnToIdle()
    {
        // Wait for the animation to finish (adjust delay as needed)
        yield return new WaitForSeconds(3f);
        animator.SetTrigger("Idle"); // Trigger the Idle state
    }

    // Function to open a website
    private void OpenWebsite(string url)
    {
        try
        {
            Application.OpenURL(url); // Use Unity's method to open URLs
            Debug.Log("Opening website: " + url);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to open website: " + e.Message);
        }
    }

    // Class to deserialize the response from Flask
    [System.Serializable]
    public class Response
    {
        public string action;
        public string message; // For websites, this is the URL
    }
}
