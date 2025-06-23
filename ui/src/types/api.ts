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

export type Character = {
  id: string;
  createdAt: string;
  connection: Connection;
  ageInEnvironmentIterations: number;
  colour: string;
  gridPosition: GridPosition;
};

export type CreateCharacterInput = {
  colour: string;
  grid_position: GridPosition;
  model: string;
};

export type InteractInput = {
  character_id: string;
};

export type PlaygroundSetup = {
  availableModels: string[];
  cellSize: number;
  gridWidth: number;
  gridHeight: number;
  characters: Character[];
}; 