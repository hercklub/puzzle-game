using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapTiles : MonoBehaviour
{

    
    public TileType[] tilesTypes;
    int[,] tiles;

    public GameObject trianglePref;

    [HideInInspector]
    public Node[,] graph;

    [HideInInspector]
    public List<Vector2> vertices;

    [HideInInspector]
    public List<Vector2> shape;

    // for shape recognition
    [HideInInspector]
    public int fistDirShape;
    [HideInInspector]
    public int currentDir = -1;
    [HideInInspector]
    public float tileOffset;


    public bool onlyNearestNeighbour;

    int mapSizeX = 5;
    int mapSizeY = 5;
    int[,] matrix;



    public const int E = 1;
    public const int NE = 2;
    public const int N = 3;
    public const int NW = 4;
    public const int W = 5;
    public const int SE = 6;
    public const int S = 7;
    public const int SW = 8;

    public Material invalid;
    public Material valid;
    // Use this for initialization
    void Awake()
    {
       
    }


    void Start()
    {
        /*
        matrix = new int[5, 5] {
        {1,0,1,0,1},
        {0,1,0,1,0},
        {1,0,1,0,1},
        {0,1,0,1,0},
        {1,0,1,0,1},
        };*/
        
        matrix = new int[5, 5] {
        {1,1,1,1,1},
        {1,1,1,1,1},
        {1,1,0,1,1},
        {1,1,1,1,1},
        {1,1,1,1,1},
        };
        /*
        matrix = new int[5, 5] {
        {0,1,1,1,0},
        {1,0,0,0,1},
        {1,0,0,0,1},
        {1,0,0,0,1},
        {0,1,1,1,0},
        };
        */
        GenerateMapData(matrix);
        GenerateVisuals();
        GenerateGraph();     
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Clicked Mesh Collider
            GameObject go = MouseCastMesh();
            if (go != null)
            {
                // Clear Mesh and Collider
                go.GetComponent<MeshFilter>().mesh = null;
                
                MeshMan manager = go.GetComponent<MeshMan>();

                foreach (var ele in manager.nodes)
                {
                    Node toDelete = graph[(int)ele.x,(int)ele.y];
                    toDelete.isConnected = false;

                    // Delete lines

                    //LineRenderer line = toDelete.Go.GetComponent<LineRenderer>();
                    //line.SetPosition(0, Vector3.zero);
                    //line.SetPosition(1, Vector3.zero);

                    toDelete.Go.GetComponent<Cell>().DeleteLines();
                    // Enable nodes
                    toDelete.Go.GetComponent<SpriteRenderer>().color = Color.white;

                }
                Destroy(go);
                vertices.Clear();
                shape.Clear();

            }
        }
    }

    public class Edge
    {
        public Node node;
    }
    public class Node
    {
        public List<Edge> edges;

        public int x;
        public int y;
        public GameObject Go;
        public bool isConnected;
        public Node()
        {
            edges = new List<Edge>();
        }
    }

    public Vector3 TileToWorldCoord(int tileX, int tileY)
    {
        TileType tt = tilesTypes[tiles[tileX, tileY]];
        float radius = tt.tileSprite.GetComponent<SpriteRenderer>().bounds.size.x;
        return new Vector3((radius + tileOffset) * tileX, (radius + tileOffset) * tileY);
    }
    /*
    bool isEdge(Node a, Node b)
    {
        foreach (Edge c in a.edges)
        {
            if (c.node.x == b.x && c.node.y == b.y)
                return true;
        }
        return false;
    }
    void FindAllTriangles(Vector2 start,Vector2 end,bool onlyConnected)
    {
        for (int i = (int)start.x, c = 0; i < (int)end.x; i++)
        {
            for (int j = (int)start.y; j <(int) end.y; j++, c++)
            {
                if (tiles[i, j] != 0 && graph[i,j].isConnected)
                {
                    Node u = graph[i,j];
                    foreach (Edge v in graph[i, j].edges) // traverse all edges of element
                    {
                        if (v.node.isConnected)
                        {
                            foreach (Edge w in v.node.edges)
                            {
                                if (isEdge(v.node, w.node) && isEdge(w.node, u) && w.node.isConnected)
                                {
                                    Debug.Log("FOUND TRIANGLE" + "[" + u.x + "  " + u.y + "]"
                                                               + "[" + v.node.x + "  " + v.node.y + "]"
                                                               + "[" + w.node.x + "  " + w.node.y + "]"
                                        );
                                }
                            }
                        }
                    }

                }
            }
        }
    }
    */

    void GenerateGraph()
    {
       

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
                                Vector2 res = findNearestNeighbour(i, j, x, y,onlyNearestNeighbour);
                                if (res != new Vector2(-1, -1))
                                {
                                    Edge newEdge = new Edge();
                                    newEdge.node = graph[(int)res.x, (int)res.y];
                                    graph[i, j].edges.Add(newEdge);
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
    
    /// <summary>
    /// For given node finds nearest node to wich it can be connected to.
    /// </summary>
    /// <param name="x">X postion in graph</param>
    /// <param name="y">Y positon in graph</param>
    /// <param name="i">X direction )</param>
    /// <param name="j">Y direction</param>
    /// <returns>Coordinates of nearest neighbour</returns>
    Vector2 findNearestNeighbour(int x, int y, int i, int j,bool onlyOneStep)
    {
        //Debug.Log(x + " " + y + " " + i + " " + j);
        int x1 = x;
        int y1 = y;

        while (x1 < mapSizeX && y1 < mapSizeY && x1 >= 0 && y1 >= 0)
        {
            //Debug.Log("TEMP: " + x1 + "," + y1 + " || " + i + " " + j);
            if (tiles[x1, y1] == 1 && (x1 != x || y1 != y))
            {
                return new Vector2(x1, y1);

            }
            if (onlyOneStep && (x1 != x || y1 != y))
                break;

            x1 += i;
            y1 += j;
        }
        return new Vector2(-1, -1);

    }
    void GenerateMapData(int[,] matrix) // be able to input from matrix
    {
        tiles = new int[mapSizeX, mapSizeY];
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                tiles[i, j] = matrix[i, j];
            }
        }


    }
    void GenerateVisuals()
    {
        float radius;
        graph = new Node[mapSizeX, mapSizeY];
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

                    // GRAPH INIT
                    graph[i, j] = new Node();
                    graph[i, j].x = i;
                    graph[i, j].y = j;
                    graph[i, j].Go = go;
                    graph[i, j].isConnected = false;

                }
            }
        }

    }
    public void ClearPath(bool clearShape)
    {
        if (clearShape)
        {
            foreach (var ele in vertices)
            {
                Node toDelete = graph[(int)ele.x, (int)ele.y];
                toDelete.isConnected = false;
                Debug.Log(ele.x + " " + ele.y);
                // Delete lines

                //LineRenderer line = toDelete.Go.GetComponent<LineRenderer>();
                //line.SetPosition(0, Vector3.zero);
                //line.SetPosition(1, Vector3.zero);
                toDelete.Go.GetComponent<Cell>().DeleteLines();

                // Enable nodes
                toDelete.Go.GetComponent<SpriteRenderer>().color = Color.white;
            }
            vertices.Clear();
            shape.Clear();
        }
        else
        {
            vertices.Clear();
            shape.Clear();

        }
    }
    GameObject MouseCastMesh()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public void TileToWroldArray(ref Vector2[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = TileToWorldCoord((int)arr[i].x,(int)arr[i].y);

        }
        
    }

    public int DeltaMovement(Vector2 from, Vector2 to)
    {

        if (from.x > to.x)
        {
            if (from.y > to.y)
                return NW;
            else if (from.y < to.y)
                return SW;
            else
                return W;
        }
        else if (from.x < to.x)
        {
            if (from.y > to.y)
                return NE;
            else if (from.y < to.y)
                return SE;
            else
                return E;

        }
        else {
            if (from.y > to.y)
                return N;
            else
                return S;

        }


    }
}
