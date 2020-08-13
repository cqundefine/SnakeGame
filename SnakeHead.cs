using System.Numerics;

public class SnakeHead
{
    public Vector2 pos;
    public int rot; // 1 - UP // 2 - RIGHT // 3 - DOWN // 4 - LEFT

    public SnakeHead(Vector2 pos, int rot)
    {
        this.pos = pos;
        this.rot = rot;
    }
}