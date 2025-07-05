import { CreateCharacterInput, PlaygroundSetup, Character, Question } from '../types/api';

const API_BASE_URL = 'https://localhost:7012';

export async function getPlaygroundSetup(): Promise<PlaygroundSetup> {
  const response = await fetch(`${API_BASE_URL}/Playground/GetPlaygroundSetup`);
  if (!response.ok) {
    throw new Error('Failed to get playground setup');
  }
  return response.json();
}

export async function createCharacter(character: CreateCharacterInput): Promise<Character> {
  const response = await fetch(`${API_BASE_URL}/Character/CreateCharacter`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(character),
  });
  if (!response.ok) {
    throw new Error('Failed to create character');
  }
  return response.json();
}

export async function iteratePlayground(questionAnswers?: Question[]): Promise<PlaygroundSetup> {
  const response = await fetch(`${API_BASE_URL}/Playground/Iterate`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      question_answers: questionAnswers?.map(q => ({
        id: q.id,
        character_id: q.character_id,
        question: q.question,
        answer: q.answer
      }))
    }),
  });
  if (!response.ok) {
    throw new Error('Failed to iterate playground');
  }
  return response.json();
}

export async function resetPlayground(): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/Playground/ResetPlayground`, {
    method: 'POST',
  });
  if (!response.ok) {
    throw new Error('Failed to reset playground');
  }
}

export async function answerQuestions(answers: Record<string, string>): Promise<PlaygroundSetup> {
  const response = await fetch(`${API_BASE_URL}/playground/answer`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ answers }),
  });
  if (!response.ok) {
    throw new Error('Failed to submit answers');
  }
  return response.json();
} 