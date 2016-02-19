using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindSolution:MonoBehaviour {

    int mapSizeX;
    int mapSizeY;
    MapTiles.Node [,] graph;
    int[,] tiles;
    MapTiles map;


    public class Solution
    {
        public List<Shape> shapes;
        public Solution()
        {
            shapes = new List<Shape>();
        }
    }

    List<Solution> solutions = new List<Solution>();

    void ClearTempShape()
    {
        for (int i = 0 + 1; i < mapSizeX; i++)
        {
            for (int j = 0 + 1; j < mapSizeY; j++)
            {
                int tile = tiles[i, j];
                if (tile != 0)
                {
                    graph[i, j].isConnected = false;

                }
            }
        }


    }
    
    void setShape(List<Vector2> shape)
    {


        Vector2 top = shape[0];
        Vector2 bot = shape[0];
        map.FindBoundingBounds(ref bot, ref top, ref shape);

        //Debug.Log("BOUNDING BOUNDS " + bot + "  " + top);

        for (int j = (int)bot.y; j < (int)top.y; j++)
        {
            for (int i = (int)bot.x; i < (int)top.x; i++)
            {
                if (map.PointInPolygon(new Vector2(i, j), ref shape))
                {
                    if (tiles[i, j] != 0)
                    {
                        //Debug.Log("SET" + i + "," + j);
                        graph[i, j].isConnected = true;
                        break;
                    }

                }
            }
        }

        foreach (Vector2 point in shape)
        {
            int tile = tiles[(int)point.x, (int)point.y];
            if (tile != 0)
            {
                graph[(int)point.x, (int)point.y].isConnected = true;

            }
        }

    }


    Vector2 getMovementDirection(Vector2 from, Vector2 to)
    {
        int direction = map.DeltaMovement(from, to);
        Vector2 movementDirection = new Vector2();
        switch (direction)
        {
            case 1:
                movementDirection = new Vector2(1, 0);
                break;
            case 2:
                movementDirection = new Vector2(1, 1);
                break;
            case 3:
                movementDirection = new Vector2(0, 1);
                break;
            case 4:
                movementDirection = new Vector2(-1, 1);
                break;
            case 5:
                movementDirection = new Vector2(-1, 0);
                break;
            case 6:
                movementDirection = new Vector2(1, -1);
                break;
            case 7:
                movementDirection = new Vector2(0, -1);
                break;
            case 8:
                movementDirection = new Vector2(-1, -1);
                break;
        }

        //Debug.Log( from + " " + to + " " + movementDirection);
        return movementDirection;

    }
    Queue<Vector2> pointsToTraverse = new Queue<Vector2>();
   // Stack<Vector2> pointsToTraverse = new Stack<Vector2>();
    bool isValidShape(List<Vector2> points)
    {

        if (points.Count != 4)
        {
            Debug.Log("NOT RECTANGLE");
            return false;
        }
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 nextPoint;

            if (i == points.Count - 1)
            {
                nextPoint = points[0];
            }
            else
            {
                nextPoint = points[i + 1];
            }


            Vector2 step = getMovementDirection(points[i], nextPoint);
            Vector2 origin = points[i];
   

            int j = 0;
            while (origin != nextPoint)
            {


                if (tiles[(int)origin.x, (int)origin.y] == 0)
                {
                    Debug.Log("NOT VALID" + origin);
                    return false;
                }

                origin += step;
                // POINTS TO TRAVERSE NEXT
                if (origin != points[0] && nextPoint != points[0])
                {
                   // Debug.Log(origin);
                    pointsToTraverse.Enqueue(origin);
                   // pointsToTraverse.Push(origin);
                }
                if (j > 10)
                {
                    Debug.Log("BU BU");
                    return false;
                }

                j++;
            }



        }
        return true;


        
    }
    int counter = 0;
    void FindAllPosiibleRectangleTopVertices(Vector2 bot)
    {
        // have to be at least square of size 1
        if ((int)bot.x + 1 >= mapSizeX || (int)bot.y + 1 >= mapSizeY)
            return;
        
        for (int i = (int)bot.x + 1; i < mapSizeX; i++)
        {
            for (int j = (int)bot.y + 1; j < mapSizeY; j++)
            {
                int tile = tiles[i, j];
                if (tile != 0)
                {
                    //if (!checkWinCondition())
                    //{
                        if (tiles[(int)bot.x, (int)bot.y] != 0 && tiles[i, (int)bot.y] != 0 && tiles[i, j] != 0 && tiles[(int)bot.x, j] != 0)
                        {
                            Debug.Log("POSIIBLE RECTANGLE " + bot + " " + new Vector2(i, bot.y) + " " + new Vector2(i, j) + " " + new Vector2(bot.x, j));
                            List<Vector2> toCheck = new List<Vector2>();
                            toCheck.Add(bot);
                            toCheck.Add(new Vector2(i, bot.y));
                            toCheck.Add(new Vector2(i, j));
                            toCheck.Add(new Vector2(bot.x, j));
                            if (isValidShape(toCheck))
                            {
                             setShape(toCheck);
                            FindAllPosiibleRectangleTopVertices(pointsToTraverse.Dequeue());
                            //FindAllPosiibleRectangleTopVertices(pointsToTraverse.Pop());


                            
                            // break;
                            // Check Win Condition
                            }
                        }
                    //}
                    //else {
                    //    Debug.Log("FINISHED");
                    //    return;
                    //}

                }
            }
        }

    }

    bool checkWinCondition()
    {
        for (int i = 0 + 1; i < mapSizeX; i++)
        {
            for (int j = 0 + 1; j < mapSizeY; j++)
            {
                int tile = tiles[i, j];
                if (tile != 0)
                {
                    if (!graph[i, j].isConnected)
                        return false;

                }
            }
        }
        return true;
    }

    public void FindAllSolutions()
    {

        map = FindObjectOfType<MapTiles>();
        mapSizeX = map.mapSizeX;
        mapSizeY = map.mapSizeY;

        tiles = map.tiles;
        graph = map.graph;

        int availebleShapes = 3;
        List<BoardManager.ShapeToComplete> winConditionShapes = new List<BoardManager.ShapeToComplete>();

        //winConditionShapes.Add(new BoardManager.ShapeToComplete(new Shape.Rectangle(true)));
        //winConditionShapes.Add(new BoardManager.ShapeToComplete(new Shape.Rectangle(false)));
        //winConditionShapes.Add(new BoardManager.ShapeToComplete(new Shape.Rectangle(true)));

        //GenerateSquare(new Vector2(0,0),1);

        Solution sol = new Solution(); ;

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                int tile = tiles[i, j];
                if (tile != 0)
                {
                    //Debug.Log("---------POSIBLE CANDIDATES FOR: " + new Vector2(i, j));
                   
                    //sol.Add(); // shapes of valid solution

                }
            }
        }

        FindAllPosiibleRectangleTopVertices(new Vector2(0, 0));
        solutions.Add(sol);


    }
    void GenerateRectangle(Vector2 bottom, int size)
    {
        Vector2 top = bottom + Vector2.one * size;
        Debug.Log(top);

        List<Vector2> shape = new List<Vector2>();


    }
}
