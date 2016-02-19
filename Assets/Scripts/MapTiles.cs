using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MapTiles : MonoBehaviour
{


    public TileType[] tilesTypes;
    public int[,] tiles;

    public GameObject trianglePref;

    [HideInInspector]
    public Node[,] graph;

    [HideInInspector]
    public List<Vector2> vertices; // all veritces that makes shape

    [HideInInspector]
    public List<Vector2> shape; // only corners of shape

    // for shape recognition
    [HideInInspector]
    public int fistDirShape;
    [HideInInspector]
    public int currentDir = -1;


    public float tileOffset;


    public bool onlyNearestNeighbour;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    public bool separateShapes = false;
    public bool FFA = false;

    public int mapSizeX;
    public int mapSizeY;

    int[,] matrix;
    float radius;


    public Vector2 startNode = new Vector2(-1,-1);
    public Vector2 endNode = new Vector2(-1, -1);
    public int pathDirection;

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

    public Queue<GameManager.Level> levels = new Queue<GameManager.Level>();

    [HideInInspector]
    public int ShapeId = 1;
    BoardManager board;
    FindSolution solutionFinder;
    UiShapeManager ui;
    // Use this for initialization
    void Awake()
    {
        board = FindObjectOfType<BoardManager>();
        solutionFinder = FindObjectOfType<FindSolution>();
        ui = FindObjectOfType<UiShapeManager>();
        /*
        matrix = new int[5, 5] {
        {1,0,1,0,1},
        {0,1,0,1,0},
        {1,0,1,0,1},
        {0,1,0,1,0},
        {1,0,1,0,1},
        };*/
        /*
        matrix = new int[5, 5] {
        {1,1,1,1,1},
        {1,1,1,1,1},
        {1,1,0,1,1},
        {1,1,1,1,1},
        {1,1,1,1,1}
        };*/



        mapSizeX = 5;
        mapSizeY = 5;
        List<BoardManager.ShapeToComplete> temp = new List<BoardManager.ShapeToComplete>();
        temp.Add(new BoardManager.ShapeToComplete(new Shape.Rectangle(false)));
        temp.Add(new BoardManager.ShapeToComplete(new Shape.Diamond(true)));
        temp.Add(new BoardManager.ShapeToComplete(new Shape.Rectangle(true)));

        matrix = new int[5, 5] {
        {1,1,1,1,1},
        {1,1,1,1,1},
        {1,1,1,1,1},
        {1,1,1,1,1},
        {1,1,1,1,1}
        };
        levels.Enqueue(new GameManager.Level(mapSizeX, mapSizeY, matrix, temp));
        temp.Clear();


        mapSizeX = 5;
        mapSizeY = 5;

        temp.Add(new BoardManager.ShapeToComplete(new Shape.Rectangle(true)));
        temp.Add(new BoardManager.ShapeToComplete(new Shape.Rectangle(false)));
        temp.Add(new BoardManager.ShapeToComplete(new Shape.Rectangle(true)));

        matrix = new int[5, 5] {
        {1,1,1,1,1},
        {1,1,1,1,1},
        {1,1,0,1,1},
        {1,1,1,1,1},
        {1,1,1,1,1}
        };
        levels.Enqueue(new GameManager.Level(mapSizeX, mapSizeY, matrix, temp));
        matrix = new int[3, 3] {
        {1,1,1},
        {1,1,1},
        {1,1,1}
        };

        //matrix = new int[mapSizeX, mapSizeY];
        ////int _w,int _h,string _seed,bool _useSeed,int _fill,ref int[,] _map
        //LevelGeneration gen = new LevelGeneration(mapSizeX, mapSizeY, seed, useRandomSeed, randomFillPercent, ref matrix);
        //gen.GenerateMap();

        //GenerateMapData(matrix);
        //GenerateVisuals();
        //GenerateGraph();
        ////FindAllTriangles(new Vector2(0,0),new Vector2(mapSizeX,mapSizeY));
        //PlaceCamera();

        LoadLevel();

    }

    public void UnloadLevel()
    {
        if (tiles == null)
            return;
        ClearAll();
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                // 8 way connection
                if (tiles[i, j] != 0)
                {
                    if (graph[i, j].Go != null)
                        Destroy(graph[i, j].Go);

                }
            }
        }
        board.winConditionShapes.Clear();
        ui.UiShape.Clear();
    }

   public void LoadLevel()
    {
        UnloadLevel();
        
        if (levels.Count > 0)
        { 
            GameManager.Level level = levels.Dequeue();
        
            matrix = level.matrix;
            mapSizeX = level.mapSizeX;
            mapSizeY = level.mapSizeY;
            
            board.winConditionShapes.AddRange(level.winConditionShapes);
            ui.SetStartingShapes();
            

            // add matrix and win condition
            GenerateMapData(matrix);
            GenerateVisuals();
            GenerateGraph();
            //FindAllTriangles(new Vector2(0,0),new Vector2(mapSizeX,mapSizeY));
            PlaceCamera();
        }
        else
        {
            Debug.Log("No more levels");
        }
    }
    void Start()
    {
   
        //solutionFinder.FindAllSolutions();
    }

    /*
    void Traverse(MeshMan node,int key)
    {


        if (node.leftChild != null && node.id != key)
        {
            Traverse(node.leftChild,key);
        }


        if (node.rightChild != null && node.id != key)
        {
            Traverse(node.rightChild,key);
        }

        if (node.id == key)
        {
            Debug.Log(node.id + "==" + key);
        }

        if (node.isRendered)
        {
            Debug.Log("RENDERED SHAPES S" + node.id);
        }
        else
        {
            Debug.Log("NOT RENDERED S" + node.id);
            node.nodes.Clear();
            Destroy(node.gameObject);
        }



    }
    */
    GameObject MouseCast()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Clicked Mesh Collider
            GameObject go = MouseCastMesh();
            GameObject clickedGO = MouseCast();
            if (go != null && clickedGO == null)
            {
                // Clear Mesh and Collider
                go.GetComponent<MeshFilter>().mesh = null;

                MeshMan manager = go.GetComponent<MeshMan>();
                Debug.Log("CLEAR MESH");

                //// FIND HELPER SHAPE IN WHICH IS AT LEAST ONE POINT OF SHAPE WE WANT TO DELTE
                //MeshMan helper = null;
                //foreach (var ele in FindObjectsOfType<MeshMan>())
                //{
                //    if (!ele.isRendered)
                //    {
                //        if (ele.nodes.Contains(manager.nodes[0]))
                //        {
                //            helper = ele;
                //            break;
                //        }

                //    }
                //}

                

                    Vector2 top = manager.nodes[0];
                    Vector2 bot = manager.nodes[0];
                    FindBoundingBounds(ref bot, ref top, ref manager.nodes);

                    Debug.Log("BOUNDING BOUNDS " + bot + "  " + top);
                // DELETE INSIDE POINTS
                    for (int j = (int)bot.y; j < (int)top.y; j++)
                    {
                        for (int i = (int)bot.x; i < (int)top.x; i++)
                        {
                            if (PointInPolygon(new Vector2(i, j), ref manager.nodes))
                            {
                                if (tiles[i, j] != 0)
                                {

                                graph[i, j].Go.GetComponent<Cell>().DeleteLines();
                                graph[i, j].Go.GetComponent<SpriteRenderer>().color = Color.white;
                                graph[i, j].isConnected = false;

                                Vector3 tempPos = graph[i, j].Go.transform.position;
                                tempPos.z = 0f;
                                graph[i, j].Go.transform.position = tempPos;

                            }

                            }
                        }
                    }
                
                bool cointinsEle = false;
                foreach (var ele in manager.nodes)
                {
                    Node toDelete = graph[(int)ele.x, (int)ele.y];
                    Debug.Log(ele.x + " " + ele.y);
                    toDelete.isConnected = false;

                    // Delete lines

                    //LineRenderer line = toDelete.Go.GetComponent<LineRenderer>();
                    //line.SetPosition(0, Vector3.zero);
                    //line.SetPosition(1, Vector3.zero);
                    toDelete.Go.GetComponent<Cell>().DeleteLines();
                    // Enable nodes
                    int numOfNodesShapes = NodesInShapesNum(ele);


                    if ( (cointinsEle && numOfNodesShapes <= 2) || (!cointinsEle && numOfNodesShapes <=1) )
                    {
                        toDelete.Go.GetComponent<SpriteRenderer>().color = Color.white;
                        toDelete.isConnected = false;
                        Vector3 tempPos = toDelete.Go.transform.position;
                        tempPos.z = 0f;
                        toDelete.Go.transform.position = tempPos;

                    }
                    else
                    {
                        Debug.Log("FCK : " + NodesInShapesNum(ele));
                    }
                }
                board.DeleteShapes(manager.shapes);
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

    bool isEdge(Node a, Node b)
    {
        foreach (Edge c in a.edges)
        {
            if (c.node.x == b.x && c.node.y == b.y)
                return true;
        }
        return false;
    }
    void FindAllTriangles(Vector2 start, Vector2 end)
    {
        for (int i = (int)start.x, c = 0; i < (int)end.x; i++)
        {
            for (int j = (int)start.y; j < (int)end.y; j++, c++)
            {
                if (tiles[i, j] != 0)
                {
                    Node u = graph[i, j];
                    foreach (Edge v in graph[i, j].edges) // traverse all edges of element
                    {

                        foreach (Edge w in v.node.edges)
                        {
                            if (isEdge(v.node, w.node) && isEdge(w.node, u))
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
                                Vector2 res = findNearestNeighbour(i, j, x, y, onlyNearestNeighbour);
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
    Vector2 findNearestNeighbour(int x, int y, int i, int j, bool onlyOneStep)
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


                    go.GetComponentInChildren<TextMesh>().text = i + ", " + j; 

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
       
        // Debug.Log("VERTICES: " + vertices.Count);
        if (clearShape)
        {

            foreach (var ele in vertices)
            {
                Debug.Log("CLEEEARRR!!!!" + ele);
                Node toDelete = graph[(int)ele.x, (int)ele.y];
                if (!isNodeInShape(ele))
                {
                    toDelete.isConnected = false;
                    //Debug.Log(ele.x + " " + ele.y);
                    // Delete lines

                    //LineRenderer line = toDelete.Go.GetComponent<LineRenderer>();
                    //line.SetPosition(0, Vector3.zero);
                    //line.SetPosition(1, Vector3.zero);
                    
                    // Enable nodes

                    toDelete.Go.GetComponent<SpriteRenderer>().color = Color.white;

                    Vector3 tempPos = toDelete.Go.transform.position;
                    tempPos.z = 0f;
                    toDelete.Go.transform.position = tempPos;
                }
                toDelete.Go.GetComponent<Cell>().DeleteLines();
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
            arr[i] = TileToWorldCoord((int)arr[i].x, (int)arr[i].y);

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
            if (from.y < to.y)
                return N;
            else
                return S;

        }


    }
    void PlaceCamera()

    {
        // CENTER TO MIDDLE
        float width = (radius * (mapSizeX - 1) + (mapSizeX - 1) * tileOffset) / 2;
        float heigth = ( radius * (mapSizeY -1 ) + (mapSizeY -1 ) * tileOffset ) * 0.6f;

        //CENTER TO BOOTOM
        //float toBotom = ((mapSizeY/2) * radius + ((mapSizeY ) /2) * tileOffset) /2;
        Vector3 pos = new Vector3(width, heigth, -10f);


        Camera.main.transform.position = pos;

        float pixelsPerUnity = 1;
        float circleSize = radius / pixelsPerUnity;
        float aspect = Camera.main.aspect;

        float screenHeigth = (float)Screen.height;
        float screenWidth = (float)Screen.width;

        // To fit one circle in screen
        //Debug.Log(circleSize);
        float ortSizeCircle = ((circleSize / 2) / aspect) * ((mapSizeX));
        float ortSizeOffet = ((tileOffset / 2) / aspect) * ((mapSizeX - 1));
        Camera.main.orthographicSize = ortSizeCircle + ortSizeOffet;

    }



    public MeshMan CreateMesh()
    {

        //MESH GO
        GameObject GO = Instantiate(trianglePref, trianglePref.transform.position, trianglePref.transform.rotation) as GameObject;
        MeshMan meshData = GO.GetComponent<MeshMan>();
        meshData.nodes = new List<Vector2>(vertices);

        meshData.id = ShapeId;
        ShapeId++;



        Vector2 top = shape[0];
        Vector2 bot = shape[0];

        FindBoundingBounds(ref bot,ref top,ref shape);

        Debug.Log("BOUNDING BOUNDS " + bot + "  " + top);

        for (int j = (int)bot.y; j < (int)top.y; j++)
        {
            for (int i = (int)bot.x; i < (int)top.x; i++)
            {
                if (PointInPolygon(new Vector2(i, j), ref shape))
                {
                    if (tiles[i,j] != 0)
                        graph[i, j].isConnected = true;
                }
            }
        }

        // JUST FOR TESTING PURPOSES
        // *********************************************************************************
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                int tile = tiles[i, j];
                if (tile != 0)
                {
                    if (graph[i, j].isConnected)
                    {
                        graph[i, j].Go.GetComponent<SpriteRenderer>().color = Color.blue;
                    }

                }
            }
        }

        // *********************************************************************************



        // MESH RENDERER

        MeshRenderer mshRen = GO.GetComponent<MeshRenderer>();

        ShapeRecognition shapeRec = FindObjectOfType<ShapeRecognition>();
        Shape recognizedShape = shapeRec.ValidShape();
        if (!FFA)
        {
            board.ProcessShape(recognizedShape);

        }
        
        
        if (recognizedShape != null)
        {
            //Debug.Log("VALID SHAPE, FILL WITH OTHER COLLOR");
            mshRen.material = valid;   
            meshData.shapes.Add(recognizedShape);
        }

        else
        {
            mshRen.material = invalid;
        }

        //Aray of nodes to list of vertices in world coordinates
        Vector2[] vertices2D = vertices.ToArray();
        TileToWroldArray(ref vertices2D);

        // Fill shape with triagnels
        Triangulator tr = new Triangulator(vertices2D);
        int[] triangles = tr.Triangulate();

        // Convert to Vector3
        Vector3[] verticesV = new Vector3[vertices2D.Length];
        for (int i = 0; i < verticesV.Length; i++)
        {
            verticesV[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        Mesh msh = new Mesh();
        msh.vertices = verticesV;
        msh.triangles = triangles;

        msh.RecalculateNormals();
        msh.RecalculateBounds();

        MeshFilter filter = GO.GetComponent<MeshFilter>();


        GO.AddComponent<MeshCollider>();

        GO.GetComponent<MeshCollider>().sharedMesh = msh;
        filter.mesh = msh;
        /*
        foreach (var ele in map.vertices)
        {

            Debug.Log(ele);
        }

        for (int i = 0; i < triangles.Length; i++)
        {

            Debug.Log(triangles[i]);
        }
        */
        return meshData;
    }

    public MeshMan isNodeInShape(Vector2 node)
    {
        foreach (var shape in FindObjectsOfType<MeshMan>())
        {
            foreach (var ele in shape.nodes)
            {
                if (node == ele)
                {
                    return shape;
                }
            }

        }

        return null;
    }


    public MeshMan isNodeInShape(Vector2 node, ref List<MeshMan> shapes)
    {
        foreach (var shape in shapes)
        {
            foreach (var ele in shape.nodes)
            {
                if (node == ele)
                {
                    return shape;
                }
            }

        }

        return null;
    }

    public List<MeshMan> NodeInShapes(Vector2 node)
    {
        List<MeshMan> result = new List<MeshMan>();
        foreach (var shape in FindObjectsOfType<MeshMan>())
        {

            foreach (var ele in shape.nodes)
            {
                if (node == ele)
                {
                    result.Add(shape);
                    break;
                }
            }

        }
        return result;
    }

    public int NodesInShapesNum(Vector2 node)
    {
        int counter = 0;
        foreach (var shape in FindObjectsOfType<MeshMan>())
        {
            foreach (var ele in shape.nodes)
            {
                if (node == ele)
                {
                    Debug.Log("ELE" + ele + "IN SHAPE : S " + shape.id);
                    counter++;
                    break;
                }
            }

        }
        return counter;
    }

    public double AreOfPolygon(List<Vector2> polygon)
    {
        int i, j;
        double area = 0;

        for (i = 0; i < polygon.Count; i++)
        {
            j = (i + 1) % polygon.Count;
            area += polygon[i].x * polygon[j].y;
            area -= polygon[i].y * polygon[j].x;
        }

        area /= 2;
        return (area < 0 ? -area : area);

    }
    /// <summary>
    /// Finds path to connect shape with nodes from already drawn shape (polygon);
    /// Reason for this function is to find 2 possible paths from start to end Node , one wich we are going to draw as new shape
    /// 
    /// </summary>
    /// <param name="polygon">Drawn shape</param>
    /// <param name="connectingNodes">List of Nodes needed to connect to complete shape</param>
    /// <param name="prevEle">Last connect node from map.vertices</param>
    /// <param name="i"> index where map.endNode is in map.vertices</param>
    /// <param name="locVertices">temporary  vertices of finished shape</param>
    /// <param name="locShape">>temporary  vertices of finished shape</param>
    public void YetDunno(MeshMan polygon, ref List<Vector2> connectingNodes, Vector2 prevEle, int i, ref List<Vector2> locVertices, int Dir)
    {
        int fckUpCounter = 0;
        while (polygon.nodes[i] != startNode)
        {
            connectingNodes.Add(polygon.nodes[i]);
            i += Dir;
            // For list to be cyclic
            if (i >= polygon.nodes.Count)
            {
                i = 0;
            }
            if (i < 0)
            {
                i = polygon.nodes.Count - 1;
            }

            fckUpCounter++;
            if (fckUpCounter > 30)
            {
                Debug.Log("FUUUUUUUUUUUUUUUUUUUUUUUUUUUCK");
                break;
            }

        }

        // Joining path to already existing vertices and shape
        foreach (var ele in connectingNodes)
        {
            locVertices.Add(ele);
        }
    }

    public void FindBoundingBounds(ref Vector2 bottom, ref Vector2 top, ref List<Vector2> shape)
    {
        foreach (var ele in shape)
        {
            if (ele.x < bottom.x )
            {
                bottom.x = ele.x;
            }
            if (ele.y < bottom.y)
            {
                bottom.y = ele.y;
            }


            if (ele.x > top.x)
            {
                top.x = ele.x;
            }

            if (ele.y > top.y)
            {
                top.y = ele.y;
            }
        }

    }

    public MeshMan GetBiggestPolygon(ref List<MeshMan> shapes)
    {
        double area;
        double max = 0;
        MeshMan maxAreaShape = shapes[0];
        foreach (var ele in shapes)
        {
            area = AreOfPolygon(ele.nodes);
            if (area > max)
            {
                maxAreaShape = ele;
                max = area;
            }
        }
        return maxAreaShape;
    }
    public bool PointInPolygon(Vector2 point , ref List<Vector2> polygon)
    {
        int numOfVert = polygon.Count;
        bool retCode = false;
        for (int i = 0, j = numOfVert - 1; i < numOfVert; j = i++)
        {
            if (((polygon[i].y >= point.y) != (polygon[j].y >= point.y)) &&
        (point.x <= (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
            {
                retCode = !retCode;
            }
        }

        return retCode;



    }
    public bool AreAllNodesConnected()
    {
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                int tile = tiles[i, j];
                if (tile != 0)
                {
                    if (!graph[i, j].isConnected)
                    {
                        return false;
                    }

                }
            }
        }

        return true;
    }
    public void ClearAll()
    {
        foreach (var shape in FindObjectsOfType<MeshMan>())
        {
            Destroy(shape.gameObject);
        }

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {

                Node toDelete = graph[i, j];
                if (toDelete != null)
                {
                    toDelete.isConnected = false;
                    toDelete.Go.GetComponent<Cell>().DeleteLines();
                    toDelete.Go.GetComponent<SpriteRenderer>().color = Color.white;
                    Vector3 tempPos = toDelete.Go.transform.position;
                    tempPos.z = 0f;
                    toDelete.Go.transform.position = tempPos;
                }

            }
        }
        board.numOfvalidShape = 0;
        board.totalNumOfShapes = 0;


    }
}
