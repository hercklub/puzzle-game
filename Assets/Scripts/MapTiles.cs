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
	// Use this for initialization
	void Start () {
        GenerateMapData();
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
                graph[i, j] = new Node();
                graph[i, j].x = i;
                graph[i, j].y = j;

            }
        }

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                if (i < mapSizeX - 1)
                    graph[i, j].edges.Add(graph[i + 1, j]);
                if (i  > 0)
                    graph[i, j].edges.Add(graph[i - 1, j]);

                if (j < mapSizeY - 1)
                    graph[i, j].edges.Add(graph[i , j + 1]);
                if (j > 0)
                    graph[i, j].edges.Add(graph[i , j - 1]);
            }
        }

    }

    void GenerateMapData() // be able to input from matrix
    {
        tiles = new int[mapSizeX, mapSizeY];
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                tiles[i, j] = 0;
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
                TileType tt = tilesTypes[tiles[i, j]];
                radius = tt.tileSprite.GetComponent<SpriteRenderer>().bounds.size.x;
                GameObject go = (GameObject)Instantiate(tt.tileSprite,new Vector3 ((radius + tileOffset) * i, (radius + tileOffset) * j,0),Quaternion.identity);

                Cell ct = go.GetComponent<Cell>();
                ct.tileX = i;
                ct.tileY = j;
                ct.map = this;
            }
        }

    }
}
