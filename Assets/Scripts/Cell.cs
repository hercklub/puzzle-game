using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Cell : MonoBehaviour {
    LineRenderer myLine;
    public int tileX;
    public int tileY;
    public MapTiles map;

    bool isConnected;



    bool isNeighbour = false;
    Vector3 v3; // mouse position


    void Start()
    {
        myLine = GetComponent<LineRenderer>();
    }


    LineRenderer CreteLineRenderer() {
       LineRenderer temp = new GameObject().AddComponent<LineRenderer>();
       temp.SetWidth(0.15f, 0.15f);
       temp.gameObject.transform.parent = this.transform; // set line GO as child of NODE

       return temp;

    }
    public void DeleteLines()
    {
        // TODO: do not delete line if its shared between meshes
        //foreach (Transform line in transform)
        //{
        //    Destroy(line.gameObject);
        //    Debug.Log("fuck");
        //}
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.zero);

    }




   public void setConnection(int fromTileX,int fromTileY, GameObject toGo) {
        //Debug.Log("SET CONNECTION " + fromTileX + " " + fromTileY + " " + map.graph[fromTileX, fromTileY].isConnected);

        if (!map.graph[fromTileX, fromTileY].isConnected)
        {
            
            // Draw line between object and lock it
            if (toGo != null && toGo.GetComponent<Cell>() != null)
            {
          
                isNeighbour = false;
                bool crossConnect = false;
                int newTileX = toGo.GetComponent<Cell>().tileX;
                int newTileY = toGo.GetComponent<Cell>().tileY;
               
                // FIX WHEN THERE IS NO NODE IN GIVEN DIRECTION
                // Chceck for cross paths
               /* Vector2 from = new Vector2(tileX, tileY);
                Vector2 to = new Vector2(newTileX, newTileY);
                int tempDir = map.DeltaMovement(from, to);
                Vector2 dirVec = to - from;
                if (tempDir % 2 == 0)
                {
                    if (map.graph[tileX + (int)dirVec.x, tileY].isConnected &&
                        map.graph[tileX, tileY + (int)dirVec.y].isConnected)
                    {
                        crossConnect = true;
                    }

                }
                */
                if (!crossConnect)
                {
                    foreach (var tile in map.graph[fromTileX, fromTileY].edges)
                    {
                        // Check if node is neighbour and if node can be connected (not already in path)


                        if (tile.node.x == newTileX && tile.node.y == newTileY)
                        {
                            // TO FIX (so fucking discusting !!!) ... but working :)
                            if (!map.vertices.Contains(new Vector2(newTileX, newTileY)))
                            {
                                isNeighbour = true;
                                makeConnection(newTileX, newTileY, fromTileX, fromTileY, toGo);
                                map.graph[fromTileX, fromTileY].isConnected = true;
                                Debug.Log( " LOCKED: " +fromTileX + " " + fromTileY);
                                break;
                            }
                            else if (map.vertices.Count > 0 && map.vertices[0] == new Vector2(newTileX, newTileY))
                            {
                                isNeighbour = true;
                                makeConnection(newTileX, newTileY, fromTileX, fromTileY, toGo);
                                map.graph[fromTileX, fromTileY].isConnected = true;
                                Debug.Log(" LOCKED: " + fromTileX + " " + fromTileY);
                                break;
                            }
                        }
                    }
                }

            }
            else
            {
                // Set Line to origin
               // myLine.SetPosition(0, v3);
               // Debug.Log("ALREADY CONNECTED");
                /*
                if (prevNodeGO != null)
                {
                    prevNodeGO.GetComponent<SpriteRenderer>().color = Color.white;
                }
                */

                //go.GetComponent<Cell>().isConnected = false;

            }

            if (!isNeighbour)
            {
                // Set Line to origin
                myLine.SetPosition(0, transform.position);
                myLine.SetPosition(1, transform.position);
                //Debug.Log("NOT NEIGHBOUR!");
                /*
                if (prevNodeGO != null)
                {
                    prevNodeGO.GetComponent<SpriteRenderer>().color = Color.white;
                }
                //go.GetComponent<Cell>().isConnected = false;
                */
            }

   
        }

    }
    void makeConnection(int x, int y,int origX,int origY ,GameObject target_go)
    {
        
       Vector2 target = map.TileToWorldCoord(x,y);
       myLine.SetPosition(1, target);
       //Debug.Log("target" + x + " " +y);



        target_go.GetComponent<SpriteRenderer>().color = Color.black;
        //GetComponent<SpriteRenderer>().color = Color.red;
        
        //ADING CONNECTED VERTICES TO THE LIST AND AFTER SHAPE IS COPMLETED ,FILL AREA WITH TRIANGLES
        if (map.vertices.Count == 0) // first insert in list of verices
        {
            map.vertices.Add(new Vector2(origX, origY));
            map.vertices.Add(new Vector2(x, y));
            //map.shape.Add(new Vector2(origX, origY));
            map.currentDir = map.DeltaMovement(new Vector2(origX, origY), new Vector2(x, y));
            map.fistDirShape = map.currentDir;
        }
        else {

            // ADDING JUST CORNERS OF SHAPE
            int temp = map.DeltaMovement(new Vector2(origX, origY), new Vector2(x, y));
            // Debug.Log(temp);



            if (map.currentDir != temp )
            {
                //Debug.Log(temp  + " != " + map.currentDir);
                map.shape.Add(new Vector2(origX, origY));
                map.currentDir = temp;
            }


            // start == end ... shape is connected
            if (map.vertices.Contains(new Vector2(x, y)) && map.vertices.Contains(new Vector2(origX, origY))) 
            {
                // In case player didnt start in corner
                if (map.fistDirShape != temp)
                {
                    map.shape.Add(new Vector2(x, y));
                }

                // Draw mesh
                CreateMesh();

                // Enable conecting nodes of already drawn shape
                /*
                foreach (var ele in map.vertices)
                {
                    map.graph[(int)ele.x, (int)ele.y].isConnected = false;
                }
                */
                map.ClearPath(false);
               

                /*
                    foreach (var ele in map.shape)
                    {
                        Debug.Log(ele.x + " " + ele.y);
                    }
                    */
      

            }
            // not continuing alrady started shape... clear unfinished shape
            else if ( !map.vertices.Contains(new Vector2(origX, origY)) )
            {
               
                map.ClearPath(true);

                map.currentDir = map.DeltaMovement(new Vector2(origX, origY), new Vector2(x, y));
                map.fistDirShape = map.currentDir;
                map.vertices.Add(new Vector2(origX, origY));
                map.vertices.Add(new Vector2(x, y));
               // map.shape.Add(new Vector2(origX, origY));

            }

            else {
                map.vertices.Add(new Vector2(x, y));
            }
       
            
        }

       Debug.Log("Conection made: "+ "[" + origX +","+origY+"] ---> " + "[" + x + "," + y + "]" );
    }



   GameObject MouseCast()
    { 
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
           
             return hit.collider.gameObject;
        }
        return null;
    }
    void CreateMesh()
    {

        //MESH GO
        GameObject GO = Instantiate(map.trianglePref, map.trianglePref.transform.position, map.trianglePref.transform.rotation) as GameObject;
        MeshMan meshData = GO.GetComponent<MeshMan>();
        meshData.nodes = new List<Vector2>(map.vertices);


        // MESH RENDERER

        MeshRenderer mshRen = GO.GetComponent<MeshRenderer>();

        ShapeRecognition shapeRec = FindObjectOfType<ShapeRecognition>();
        if (shapeRec.ValidShape())
        {
            //Debug.Log("VALID SHAPE, FILL WITH OTHER COLLOR");
            mshRen.material = map.valid;
        }

        else
        {
            mshRen.material = map.invalid;
        }

        //Aray of nodes to list of vertices in world coordinates
        Vector2[] vertices2D = map.vertices.ToArray();
        map.TileToWroldArray(ref vertices2D);

        // Fill shape with triagnels
        Triangulator tr = new Triangulator(vertices2D);
        int[] triangles = tr.Triangulate();

        // Convert to Vector3
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        Mesh msh = new Mesh();
        msh.vertices = vertices;
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

    }

    void AutoFinishShape()
    {


    }
    public void SetLinePostion(Vector3 pos)
    {

        if (myLine != null)
        {


            if (!map.graph[tileX, tileY].isConnected)
            {
                Debug.Log(" MOVE " + tileX + " " + tileY);
                myLine.SetPosition(0, transform.position);
                myLine.SetPosition(1, pos);
            }
            else {
                Debug.Log( "CANT MOVE "+tileX + " " + tileY);
            }


        }

    }
    //v3 = Input.mousePosition;
    //        v3.z = 10.0f;
    //        v3 = Camera.main.ScreenToWorldPoint(v3);

}
