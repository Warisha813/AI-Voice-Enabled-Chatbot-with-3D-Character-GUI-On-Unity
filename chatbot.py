from flask import Flask, request, jsonify
import pyttsx3
from datetime import datetime
import requests
import os
import time
import subprocess
import threading
import comtypes.client

app = Flask(__name__)

# Lock to prevent concurrent calls to engine.runAndWait()
speech_lock = threading.Lock()

def _speak(text):
    """Helper function for the TTS engine."""
    print(f"Speaking: {text}")
    comtypes.CoInitialize()  # Initialize COM library for the current thread
    try:
        engine = pyttsx3.init()  # Re-initialize the TTS engine for each call
        with speech_lock:  # Ensure that only one thread can run engine.say at a time
            engine.say(text)
            engine.runAndWait()
        engine.stop()  # Stop the engine explicitly
    except Exception as e:
        print(f"Error in TTS: {e}")
    finally:
        comtypes.CoUninitialize()  # Uninitialize COM library

def say(text):
    """Function to speak text on a separate thread."""
    speak_thread = threading.Thread(target=_speak, args=(text,))
    speak_thread.start()

def get_weather():
    """Fetches the current weather."""
    api_key = "cc1cf4f3e6c69e2ee7ac041aa1d4f228"
    city_name = "Wah Cantt"
    url = f"http://api.openweathermap.org/data/2.5/weather?q={city_name}&appid={api_key}&units=metric"
    try:
        response = requests.get(url)
        if response.status_code == 200:
            data = response.json()
            temp = data['main']['temp']
            description = data['weather'][0]['description']
            return f"The current temperature in Wah Cantt is {temp}°C with {description}."
        else:
            return "Failed to fetch weather data."
    except Exception as e:
        return f"Weather API error: {e}"

def get_time():
    """Fetches the current system time."""
    return datetime.now().strftime("%H:%M")

# Path to Chrome executable
chrome_path = r"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
# Path to your profile directory
profile_path = r"C:\Users\Laptop\AppData\Local\Google\Chrome\User Data"

@app.route('/chat', methods=['POST'])
def chat():
    """Main chatbot endpoint."""
    data = request.json
    query = data.get("message", "").lower()
    print(f"Received query: {query}")

    response = {"action": "idle", "message": ""}

    if "greeting" in query:
        greeting = "Assalamo alikum, how can I help you?"
        response = {"action": "StartGreeting", "message": greeting}
        time.sleep(2)
        say(greeting)

    elif "song" in query or "music" in query or "gana" in query:
        music_path = r"D:\Music\Talha Anjum - Kaun Talha _ Prod. by Umair (Official Audio).mp3"
        try:
            os.startfile(music_path)
            response = {"action": "StartDancing", "message": "DJ wale Babu mera gana chala do!"}
            say(response["message"])
        except Exception as e:
            response = {"action": "Error", "message": f"Error playing song: {e}"}
            say("Unable to play the song.")

    elif "open" in query:
        sites = {
            "youtube": "https://www.youtube.com/",
            "google": "https://www.google.com/",
            "wikipedia": "https://www.wikipedia.org/",
            "instagram": "https://www.instagram.com/"
        }
        for site, url in sites.items():
            if f"open {site}" in query:
                try:
                    subprocess.Popen([
                    chrome_path,
                    f"--profile-directory=Profile 1",
                    f"--user-data-dir={profile_path}",
                    url
                ])
                    response = {"action": "OpenWebsite", "message": f"Opening {site}"}
                    say(f"Opening {site}")
                except Exception as e:
                    response = {"action": "Error", "message": f"Error opening site: {e}"}
                    say("Unable to open the requested site.")
                break
        else:
            response = {"action": "StartTalking", "message": "I don't know that site."}
            say(response["message"])

    elif "weather" in query:
        weather = get_weather()
        response = {"action": "StartTalking", "message": weather}
        say(weather)

    elif "time" in query:
        current_time = get_time()
        response = {"action": "StartTalking", "message": f"The current time is {current_time}."}
        say(response["message"])

    elif any(x in query for x in ["leave", "exit", "stop", "band"]):
        farewell = "Allah Hafiz! Take care."
        response = {"action": "StartSalute", "message": farewell}
        say(response["message"])
    

    else:
        response = {"action": "StartTalking", "message": f"You said: {query}"}
        say(response["message"])

    return jsonify(response)

if __name__ == "__main__":
    app.run(debug=True, port=5000)
