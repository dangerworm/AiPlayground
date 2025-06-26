import axios from 'axios';
import { Character, CreateCharacterInput, PlaygroundSetup } from '../types/api';

const API_BASE_URL = 'https://localhost:7012';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const getPlaygroundSetup = async (): Promise<PlaygroundSetup> => {
  const response = await api.get<PlaygroundSetup>('/Playground/GetPlaygroundSetup');
  return response.data;
};

export const createCharacter = async (input: CreateCharacterInput): Promise<Character> => {
  const response = await api.post<Character>('/Character/CreateCharacter', input);
  return response.data;
};

export const iteratePlayground = async (): Promise<PlaygroundSetup> => {
  const response = await api.post<PlaygroundSetup>('/Playground/Iterate');
  return response.data;
}; 