import pygame

from agent import Agent

# Initialize Pygame
pygame.init()

# Screen dimensions
WIDTH, HEIGHT = 500, 500
GRID_SIZE = 10
CELL_SIZE = WIDTH // GRID_SIZE

# Colors
BLACK = (0, 0, 0)
BLUE = (0, 0, 255)
GREEN = (0, 128, 0)
WHITE = (255, 255, 255)

# Initialize sprite
agent = Agent(GRID_SIZE // 2, GRID_SIZE // 2)

# Set up the display
screen = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption("AI Playground")

# Main loop
running = True
while running:
  for event in pygame.event.get():
    if event.type == pygame.KEYDOWN:
      if event.key == pygame.K_ESCAPE:
        running = False
   
  # Clear screen
  screen.fill(GREEN)

  # Draw grid
  for x in range(0, WIDTH, CELL_SIZE):
    pygame.draw.line(screen, BLACK, (x, 0), (x, HEIGHT))
  for y in range(0, HEIGHT, CELL_SIZE):
    pygame.draw.line(screen, BLACK, (0, y), (WIDTH, y))

  # Draw sprite
  pygame.draw.rect(
    screen,
    BLUE,
    (sprite.x * CELL_SIZE, sprite.y * CELL_SIZE, CELL_SIZE, CELL_SIZE),
  )

  # Update display
  pygame.display.flip()

# Quit Pygame
pygame.quit()