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




    public void setConnection(int fromTileX, int fromTileY, GameObject toGo) {
        //Debug.Log("SET CONNECTION " + fromTileX + " " + fromTileY + " " + map.graph[fromTileX, fromTileY].isConnected);
        int newTileX = toGo.GetComponent<Cell>().tileX;
        int newTileY = toGo.GetComponent<Cell>().tileY;
        
       
        if ((!map.graph[fromTileX, fromTileY].isConnected || !map.graph[newTileX, newTileY].isConnected)
             || ReferenceEquals(map.isNodeInShape(new Vector2(fromTileX, fromTileY)),map.isNodeInShape(new Vector2(newTileX, newTileY))))
        {

            // Draw line between object and lock it
            if (toGo != null && toGo.GetComponent<Cell>() != null)
            {

                isNeighbour = false;
                bool crossConnect = false;

                // FIX WHEN THERE IS NO NODE IN GIVEN DIRECTION
                // Chceck for cross paths
                Vector2 from = new Vector2(tileX, tileY);
                Vector2 to = new Vector2(newTileX, newTileY);
                int tempDir = map.DeltaMovement(from, to);
                Vector2 dirVec = to - from;
                if (tempDir % 2 == 0)
                {
                    if (map.tiles[tileX + (int)dirVec.x, tileY] != 0 && map.tiles[tileX, tileY + (int)dirVec.y] != 0)
                    {
                        if (map.graph[tileX + (int)dirVec.x, tileY].isConnected &&
                            map.graph[tileX, tileY + (int)dirVec.y].isConnected)
                        {
                            crossConnect = true;
                        }
                    }

                }

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
                                map.graph[fromTileX, fromTileY].isConnected = true;
                                makeConnection(newTileX, newTileY, fromTileX, fromTileY, toGo);
                               // Debug.Log(" LOCKED: " + fromTileX + " " + fromTileY);
                                break;
                            }
                            else if (map.vertices.Count > 0 && map.vertices[0] == new Vector2(newTileX, newTileY))
                            {
                                isNeighbour = true;
                                map.graph[fromTileX, fromTileY].isConnected = true;
                                makeConnection(newTileX, newTileY, fromTileX, fromTileY, toGo);
                              //  Debug.Log(" LOCKED: " + fromTileX + " " + fromTileY);
                                break;
                            }
                        }
                    }
                }

            }
            else
            {

                myLine.SetPosition(0, transform.position);
                myLine.SetPosition(1, transform.position);
                // Set Line to origin
                // myLine.SetPosition(0, v3);
                Debug.Log("ALREADY CONNECTED");
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
        else
        {
            Debug.Log("WBABABABABA");
            myLine.SetPosition(0, transform.position);
            myLine.SetPosition(1, transform.position);
        }

    }

    bool connectingToAnotherShape(int x, int y, int origX, int origY)
    {
        MeshMan shape;
        List<MeshMan> shapes = map.NodeInShapes(new Vector2(x, y));

        shape = map.GetBiggestPolygon(ref shapes);
        if (map.vertices.Count == 0) // first element, add to list of verices
        {
            map.vertices.Add(new Vector2(origX, origY));
        }
        map.endNode = new Vector2(x, y);

        // find index of end node
        var index = shape.nodes.FindIndex(a => a == map.endNode);
        // temporary List of nodes in shape wich we need to add to shape to conncet
        List<Vector2> connectingNodes = new List<Vector2>();
        List<Vector2> connectingNodes1 = new List<Vector2>();

        List<Vector2> tempVertices_1 = new List<Vector2>(map.vertices);
        List<Vector2> tempVertices_2 = new List<Vector2>(map.vertices);

        Debug.Log(  "STAR AND END " + map.startNode + " " + map.endNode);
        if (!ReferenceEquals(map.isNodeInShape(map.startNode), map.isNodeInShape(map.endNode)))
        {
            return false;
        }
        if (map.startNode == map.endNode)
            return false;
        // Nedd to remember shape
        //if (map.startNode != map.endNode)
        //{
        //    Debug.Log("NOT sAME SHAPE");
        //}

        //Debug.Log("START NODE" + map.startNode);
        //Debug.Log("END NODE  " + map.endNode);

        // Find Path from start to end node
        //Debug.Log("Connecting nodes " );
        // when something goes wrong , dont crash whole PC!!! (fcking BSOD)
        // int fckUpCounter = 0;


        // get Last element from vertices
        Vector2 prevEle = map.vertices[map.vertices.Count - 1];

        // Make both paths
        map.YetDunno(shape, ref connectingNodes, prevEle, index, ref tempVertices_1, 1);
        map.YetDunno(shape, ref connectingNodes1, prevEle, index, ref tempVertices_2, -1);

        //Debug.Log("VERTICES +1 ");
        //foreach (var ele in tempVertices_1)
        //{
        //    Debug.Log(ele);
        //}
        //Debug.Log("VERTICES -1 ");
        //foreach (var ele in tempVertices_2)
        //{
        //    Debug.Log(ele);
        //}

        MeshMan meshData = null;
        Debug.Log("ID : S " + shape.id);


        //if (map.separateShapes)
        //Get helper shape to update , if no exiting , create new
        if (map.separateShapes)
        {
            if (!shape.isRendered)
            {
                Debug.Log("Updating");
                meshData = shape.GetComponent<MeshMan>();
            }
            else
            {
                Debug.Log("Creating");
                GameObject GO = Instantiate(map.trianglePref, map.trianglePref.transform.position, map.trianglePref.transform.rotation) as GameObject;
                meshData = GO.GetComponent<MeshMan>();

                meshData.id = map.ShapeId;
                map.ShapeId++;

            }
        }

        // bigger polygon is union of those two...
        if (map.separateShapes)
        {
            if (map.AreOfPolygon(tempVertices_1) > map.AreOfPolygon(tempVertices_2))
            {
                map.vertices = new List<Vector2>(tempVertices_2);
                meshData.nodes = new List<Vector2>(tempVertices_1);
                connectingNodes = connectingNodes1;

            }
            else
            {
                map.vertices = new List<Vector2>(tempVertices_1);
                meshData.nodes = new List<Vector2>(tempVertices_2);
            }
        }
        else
        {
            // FEJKS !!!!!!!!!!
            // Vertices != Shape ... vertices define actual rendered shape
            //                       shape define just connecting part ... lot of rewrting to correct it.. IMA LAZY
            if (map.AreOfPolygon(tempVertices_1) < map.AreOfPolygon(tempVertices_2))
            {
                map.vertices = new List<Vector2>(tempVertices_2);
                

            }
            else
            {
                map.vertices = new List<Vector2>(tempVertices_1);
                connectingNodes = connectingNodes1;
            }

        }
         // RESET Z POSITION OF ORIGINAL SHAPE
        foreach (var ele in shape.nodes)
        {
            Vector3 tempPos = map.graph[(int)ele.x,(int)ele.y].Go.transform.position;
            tempPos.z = 0f;
            map.graph[(int)ele.x, (int)ele.y].Go.transform.position = tempPos;
        }


        foreach (var ele in connectingNodes)
        {
            //  Filling shape
            int temp = map.DeltaMovement(prevEle, ele);
            if (map.currentDir != temp)
            {
                map.shape.Add(prevEle);
                map.currentDir = temp;
            }
            prevEle = ele;
            // Debug.Log("PREV ELE:" + prevEle);

        }

        /// Fix missing nodes in shape 
        if (map.DeltaMovement(prevEle, map.startNode) != map.currentDir)
        {
            if (!map.shape.Contains(prevEle))
            {
                // Debug.Log("ADDED prevELE" + prevEle);
                map.shape.Add(prevEle);
            }
        }
        if (map.DeltaMovement(prevEle, map.startNode) != map.fistDirShape)
        {
            if (!map.shape.Contains(map.startNode))
            {
                Debug.Log("ADDED startELE" + map.startNode);
                map.shape.Add(map.startNode);
            }
        }

        // Debug.Log("AREA OF SHAPE :" + map.AreOfPolygon(map.vertices));
        // Draw mesh
        //Debug.Log("VERTICES");
        foreach (var ele in map.vertices)
        {
            Vector3 tempPos = map.graph[(int)ele.x, (int)ele.y].Go.transform.position;
            tempPos.z = -2f;
            map.graph[(int)ele.x, (int)ele.y].Go.transform.position = tempPos;

        }



        MeshMan tempMM = map.CreateMesh();
        tempMM.shapes.AddRange(shape.shapes);
        // Pass reference to helper shape 
        if (map.separateShapes)
        {
            shape.helperShape = meshData;
            tempMM.helperShape = meshData;
            meshData.isRendered = false;

            Debug.Log("CONNECTED " + meshData.id + " ---> LEFT  S" + tempMM.id + "  RIGHT S" + shape.id);
        }
        else
        {
            Destroy(shape.gameObject);
        }
        map.ClearPath(false);
        return true;

    }
    void makeConnection(int x, int y,int origX,int origY ,GameObject target_go)
    {

       // Snaping line to node
        Vector3 target = map.TileToWorldCoord(x,y);
        target.z = -1;
        myLine.SetPosition(1, target);
       //Debug.Log("target" + x + " " +y);
        target_go.GetComponent<SpriteRenderer>().color = Color.black;
        // Hide node behind shape
        Vector3 tempPos = target_go.transform.position;
        tempPos.z = -2f;
        target_go.transform.position = tempPos;


        if (map.isNodeInShape(new Vector2(origX, origY)) != null)
        {
            map.startNode = new Vector2(origX, origY);
            Debug.Log("START NODE");

        }

        //MeshMan shape;
        List<MeshMan> shapes = map.NodeInShapes(new Vector2(x, y));
        if ( shapes.Count != 0 )
        {

            if (connectingToAnotherShape(x, y, origX, origY))
                return;
            else
            {
                myLine.SetPosition(0, transform.position);
                myLine.SetPosition(1, transform.position);
                
            }


        }


        //GetComponent<SpriteRenderer>().color = Color.red;
        
        //ADING CONNECTED VERTICES TO THE LIST AND AFTER SHAPE IS COPMLETED ,FILL AREA WITH TRIANGLES
        if (map.vertices.Count == 0) // first insert in list of verices
        {
            map.vertices.Add(new Vector2(origX, origY));
            map.vertices.Add(new Vector2(x, y));
            map.currentDir = map.DeltaMovement(new Vector2(origX, origY), new Vector2(x, y));
            
            map.fistDirShape = map.currentDir;
            Debug.Log(map.fistDirShape);
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
                map.CreateMesh();

                // Enable conecting nodes of already drawn shape

                //foreach (var ele in map.vertices)
                //{
                //    map.graph[(int)ele.x, (int)ele.y].isConnected = false;
                //}

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

    // Move it to separate script....
    

    void AutoFinishShape()
    {


    }
    public void SetLinePostion(Vector3 pos)
    {

        if (myLine != null)
        {


            //Debug.Log(" MOVE " + tileX + " " + tileY);
            Vector3 temp = transform.position;
            temp.z = -1f;
            pos.z = -1f;
            myLine.SetPosition(0, temp);
            myLine.SetPosition(1, pos);




        }

    }
    //v3 = Input.mousePosition;
    //        v3.z = 10.0f;
    //        v3 = Camera.main.ScreenToWorldPoint(v3);

}
