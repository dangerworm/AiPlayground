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

export type CharacterResponse = {
  decisions: string[];
  desires: string[];
  emotion: string;
  thoughts: string;
};

export type Character = {
  id: string;
  created_at: string;
  age: number;
  colour: string;
  grid_position: GridPosition;
  model: string;
  questions: string[];
  responses: CharacterResponse[];
};

export type CreateCharacterInput = {
  colour: string;
  grid_position: GridPosition;
  model: string;
};

export type PlaygroundSetup = {
  available_models: string[];
  characters: Character[];
  cell_size: number;
  grid_width: number;
  grid_height: number;
}; 