# AI Voice-Enabled Chatbot with 3D Character GUI

An interactive voice-enabled chatbot that integrates natural language processing, real-time speech recognition, text-to-speech (TTS), and a 3D animated character interface to create a highly engaging user experience.

## Features

- 🎙️ Voice Recognition: Real-time speech-to-text using Python's SpeechRecognition library.
- 🔊 Text-to-Speech (TTS): Uses pyttsx3 for converting chatbot replies to human-like speech.
- 🧍 3D Animated Character GUI: A Blender-modeled character integrated into Unity for real-time animations like waving, nodding, and dancing.
- 🧠 Natural Language Processing: Understands and responds to user greetings, farewells, and casual conversation.
- ⚙️ Task Automation:
  - Open websites like Google, YouTube, and Instagram
  - Play music from a local directory
  - Fetch weather updates via OpenWeatherMap API
  - Tell the current time

## Technologies Used

- Backend: Python, Flask
- Voice Modules: pyttsx3 (TTS), SpeechRecognition
- 3D Character GUI: Blender (Modeling), Unity (Integration)
- Frontend: HTML5, CSS3, JavaScript (if any)
- APIs: OpenWeatherMap
- Tools: VS Code, Postman, Unity Editor

## Installation & Usage

1. Clone the repository:
   git clone https://github.com/Faiq-Ali-372/AI-Voice-Enabled-Chatbot-with-3D-Character-GUI-On-Unity.git
   cd AI-Voice-Enabled-Chatbot

2. Set up the Python environment:
   pip install -r requirements.txt

3. Run the Flask app:
   python python-backend/app.py

4. Open Unity Project:
   - Launch Unity Hub and open the "unity-3d-gui" folder.
   - Run the scene to see the animated character in action.

5. Interact with the Bot:
   - Speak into your microphone and interact with the bot via voice.
   - The 3D character will respond visually and verbally.

## Screenshots

Add relevant screenshots or GIFs here from your GUI and Unity scene.

## Challenges Faced

- Syncing Flask responses with TTS  
  Solution: Used threading to separate TTS execution from Flask’s response cycle.

- Syncing chatbot actions with 3D animations  
  Solution: Mapped chatbot actions to predefined animation triggers.

- Handling API delays  
  Solution: Implemented caching for frequently requested data.

## Future Improvements

- Add multilingual support
- Enable chatbot memory and contextual awareness
- Deploy as a web app with hosted backend

## References

- OpenWeatherMap API: https://openweathermap.org/api
- Flask Documentation: https://flask.palletsprojects.com/
- pyttsx3 Documentation: https://pyttsx3.readthedocs.io/
- Blender Official: https://www.blender.org/
