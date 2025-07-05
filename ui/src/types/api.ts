export type GridPosition = {
  item1: number;
  item2: number;
};

export type Connection = {
  id: string;
  createdAt: string;
  endpoint: string;
  host: string;
  model: string;
  port: number;
  temperature: number;
};

export interface CharacterResponse {
  emotion?: string;
  thoughts?: string;
  desires?: string[];
  decisions?: string[];
}

export type EnvironmentActionResult = {
  action_name: string;
  action_result: string;
};

export type EnvironmentSound = {
  content: string;
  source: string;
  type: string;
};

export type EnvironmentInput = {
  correlation_id?: string;
  action_results: EnvironmentActionResult[];
  age: number;
  environment: string;
  grid_position: string;
  iteration: number;
  sounds: EnvironmentSound[];
  time: number;
};

export type Question = {
  id: string;
  character_id: string;
  question: string;
  answer?: string;
};

export type Character = {
  id: string;
  created_at: string;
  age: number;
  name: string;
  colour: string;
  grid_position: GridPosition;
  model: string;
  inputs?: EnvironmentInput[];
  responses?: CharacterResponse[];
  questions?: Question[];
};

export type CreateCharacterInput = {
  colour: string;
  grid_position: GridPosition;
  model: string;
};

export interface PlaygroundSetup {
  available_models: string[];
  characters: Character[];
  grid_width: number;
  grid_height: number;
  iteration: number;
} 