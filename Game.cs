using System.Numerics;
using System.Collections.Generic;
using System;

public class Game
{
    public static int WIDTH = 20; 
    public static int HEIGHT = 20; 

    public List<Vector2> snakeParts = new List<Vector2>();
    public Vector2 fruit;
    public SnakeHead snakeHead;
    private Random random = new Random();

    public Game()
    {
        snakeHead = new SnakeHead(new Vector2(5, 4), 4);
        snakeParts.Add(new Vector2(4, 4));
        snakeParts.Add(new Vector2(3, 4));
        fruit = new Vector2(random.Next(1, Game.WIDTH), random.Next(1, Game.HEIGHT)); 
    }
}