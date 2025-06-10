import requests
from constants import OLLAMA_API_URL, OLLAMA_HOST, OLLAMA_PORT

POST_ENDPOINT = f"http://{OLLAMA_HOST}:{OLLAMA_PORT}/{OLLAMA_API_URL}"

class OllamaService:
    """Service to interact with the Ollama API."""

    def update_conversation(self, agent):
        """Send a chat request to the Ollama API."""
        if messages is None:
            messages = [{"role": "system", "content": "You are a helpful assistant."}]
        
        payload = {
            "model": model,
            "messages": messages,
            "stream": stream
        }
        
        response = requests.post(POST_ENDPOINT, json=payload)
        response.raise_for_status()  # Raise an error for bad responses
        return response.json()

chat_data = {
    "model": "deepseek-r1:1.5b",
    "messages": [
        {"role": "system", "content": "You are a helpful assistant."},
        {"role": "user", "content": "Tell me a joke."}
    ],
    "stream": False
}

response = requests.post('http://localhost:11434/api/chat', json=chat_data)
print(response.json()['message']['content'])