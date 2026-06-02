using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ChatbotController : MonoBehaviour
{
    private string serverURL = "http://127.0.0.1:5000/chat"; // Flask server URL
    private Animator animator; // Animator for the character animations

    // Flag to ensure a message is sent only once


    void Start()
    {
        // Get the animator component attached to this GameObject
        animator = GetComponent<Animator>();

        // Check the animator component
        if (animator == null)
        {
            Debug.LogError("Animator not assigned!");
        }

    }

    // Example to send a message to the server
    public void SendMessageToServer(string message)
    {
        Debug.Log($"Sending message to server: {message}");
        StartCoroutine(SendRequest(message));
    }

    // Coroutine to send the request and receive the response
    IEnumerator SendRequest(string message)
    {
        // Create the JSON data
        string json = "{\"message\": \"" + message + "\"}";

        // Set up the UnityWebRequest with the correct URL and method
        UnityWebRequest request = new UnityWebRequest(serverURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Successfully received the response
            string responseText = request.downloadHandler.text;
            Debug.Log("Response: " + responseText);

            // Process the response and trigger animation
            ProcessResponse(responseText);
        }
    }

    // Method to trigger animation based on the response
    void ProcessResponse(string jsonResponse)
    {
        // Parse the JSON response (you can use a library like JsonUtility or a third-party library)
        Response response = JsonUtility.FromJson<Response>(jsonResponse);

        // Trigger actions based on the response
        switch (response.action)
        {
            case "StartTalking":
                Debug.Log("Action: Start Talking");
                animator.SetTrigger("Talking"); // Ensure you have this trigger set in the Animator
                break;
            case "StartDancing":
                Debug.Log("Action: Start Dancing");
                animator.SetTrigger("Dancing");
                break;
            case "StartThinking":
                Debug.Log("Action: Start Thinking");
                animator.SetTrigger("Thinking");
                break;
            case "StartGreeting":
                Debug.Log("Action: Start Greeting");
                animator.SetTrigger("Greeting");
                break;
            case "StartSalute":
                Debug.Log("Action: Start Salute");
                animator.SetTrigger("Salute");
                break;
            default:
                Debug.LogWarning("Unknown action received: " + response.action);
                animator.SetTrigger("Idle");
                break;
        }
    }

    // Response class to deserialize the JSON response
    [System.Serializable]
    public class Response
    {
        public string action;
        public string message;
    }
}
