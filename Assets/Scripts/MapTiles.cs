using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapTiles : MonoBehaviour {

    public TileType[] tilesTypes;
    int [,] tiles;
    public Node[,] graph;

    public float tileOffset;

    int mapSizeX = 5;
    int mapSizeY = 5;
    int [,] matrix;
    // Use this for initialization
    void Start () {
        matrix = new int[5, 5] {
        {1,0,0,0,1},
        {0,0,0,0,0},
        {0,0,0,0,0},
        {0,0,0,0,0},
        {1,0,0,0,1},
        };
        GenerateMapData(matrix);
        GenerateVisuals();
        GenerateGraph();

    }


    public class Node {
        public List<Node> edges;

        public int x;
        public int y;
        public Node()
        {
            edges = new List<Node>();
        }
    }

    public Vector2 TileToWorldCoord(int tileX, int tileY)
    {
        TileType tt = tilesTypes[tiles[tileX, tileY]];
        float radius = tt.tileSprite.GetComponent<SpriteRenderer>().bounds.size.x;
        return new Vector2((radius + tileOffset) * tileX , (radius + tileOffset) * tileY);
    }

    void GenerateGraph()
    {
        graph = new Node[mapSizeX,mapSizeY];

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                if (tiles[i, j] != 0)
                {
                    //Debug.Log(i + " " +j );
                    graph[i, j] = new Node();
                    graph[i, j].x = i;
                    graph[i, j].y = j;
                }
                
     
            }
        }

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                // 8 way connection
                if (tiles[i, j] != 0)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            //Debug.Log(i + " " + j + " " + x  + " " + y);
                            if (x != 0 || y != 0)
                            {
                                Vector2 res = findNearestNeighbour(i, j, x, y);
                                if (res != new Vector2(-1, -1))
                                {                           
                                graph[i, j].edges.Add(graph[(int)res.x, (int)res.y]);
                                 }
                            }


                        }

                    }
                }
                
                /*
                if (i < mapSizeX - 1)
                {
  
                    graph[i, j].edges.Add(graph[i + 1, j]);
                }
                if (i  > 0)
                    graph[i, j].edges.Add(graph[i - 1, j]);

                if (j < mapSizeY - 1)
                    graph[i, j].edges.Add(graph[i , j + 1]);
                if (j > 0)
                    graph[i, j].edges.Add(graph[i , j - 1]);


                if (i < mapSizeX - 1 && j < mapSizeY - 1)
                    graph[i, j].edges.Add(graph[i+1, j + 1]);

                if (i < mapSizeX - 1 && j > 0)
                    graph[i, j].edges.Add(graph[i + 1, j - 1]);

                if (i > 0 && j < mapSizeY - 1)
                    graph[i, j].edges.Add(graph[i - 1, j + 1]);

                if (i > 0 && j > 0)
                    graph[i, j].edges.Add(graph[i - 1, j - 1]);

       */


            }
        }

    }

    Vector2 findNearestNeighbour(int x, int y , int i ,int j)
    {
        //Debug.Log(x + " " + y + " " + i + " " + j);
        int x1 = x;
        int y1 = y;

        while (x1 < mapSizeX && y1 < mapSizeY && x1 >= 0 && y1 >= 0)
        {
            //Debug.Log("TEMP: " + x1 + "," + y1 + " || " + i + " " + j);
            if (tiles[x1, y1] == 1 && (x1 != x || y1 != y))
            {
                return new Vector2(x1,y1);
            }

            x1 += i;
            y1 += j;
        }
        return new Vector2(-1,-1);
        
    }
    void GenerateMapData(int [,] matrix) // be able to input from matrix
    {
        tiles = new int[mapSizeX, mapSizeY];
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                tiles[i, j] = matrix[i,j];
            }
        }


    }
    void GenerateVisuals()
    {
        float radius;
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                int tile = tiles[i, j];
                if (tile != 0)
                {
                    TileType tt = tilesTypes[tile];
                    radius = tt.tileSprite.GetComponent<SpriteRenderer>().bounds.size.x;
                    GameObject go = (GameObject)Instantiate(tt.tileSprite, new Vector3((radius + tileOffset) * i, (radius + tileOffset) * j, 0), Quaternion.identity);
                    Cell ct = go.GetComponent<Cell>();
                    ct.tileX = i;
                    ct.tileY = j;
                    ct.map = this;
                }
            }
        }

    }
}
