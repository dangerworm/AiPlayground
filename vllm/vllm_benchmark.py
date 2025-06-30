import time
import requests

API_URL = "http://localhost:8000/v1/chat/completions"

headers = {
    "Content-Type": "application/json",
}

data = {
    "model": "mistralai/Mistral-7B-Instruct-v0.3",  # Or your deployed model name
    "messages": [
        {"role": "system", "content": "You are a helpful assistant."},
        {"role": "user", "content": "Tell me something interesting about jellyfish."}
    ],
    "temperature": 0.7,
    "max_tokens": 200,
    "stream": False
}

start_time = time.time()
response = requests.post(API_URL, headers=headers, json=data)
end_time = time.time()

if response.status_code == 200:
    result = response.json()
    output = result['choices'][0]['message']['content']
    prompt_tokens = result['usage']['prompt_tokens']
    completion_tokens = result['usage']['completion_tokens']
    total_tokens = prompt_tokens + completion_tokens
    duration = end_time - start_time
    tokens_per_sec = total_tokens / duration

    print("\n=== RESPONSE ===\n")
    print(output)
    print("\n=== BENCHMARK ===\n")
    print(f"Prompt tokens:     {prompt_tokens}")
    print(f"Completion tokens: {completion_tokens}")
    print(f"Total tokens:      {total_tokens}")
    print(f"Time taken:        {duration:.2f} seconds")
    print(f"Tokens/sec:        {tokens_per_sec:.2f}")
else:
    print(f"Error {response.status_code}: {response.text}")
