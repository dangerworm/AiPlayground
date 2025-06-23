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
  available_models: string[];
  characters: Character[];
  cell_size: number;
  grid_width: number;
  grid_height: number;
}; 