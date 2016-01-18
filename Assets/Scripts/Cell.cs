using UnityEngine;
using System.Collections;


public class Cell : MonoBehaviour {
    LineRenderer myLine;
    public int tileX;
    public int tileY;
    public MapTiles map;
    bool isConnected;
    GameObject prevNodeGO;


    bool isNeighbour = false;
    Vector3 v3; // mouse position
    void Start()
    {
        myLine = GetComponent<LineRenderer>();

    }
    void OnMouseDrag()
    {   
        if (!map.graph[tileX, tileY].isConnected)
        {
            v3 = Input.mousePosition;
            v3.z = 10.0f;
            v3 = Camera.main.ScreenToWorldPoint(v3);

            myLine.SetPosition(0, transform.position);
            myLine.SetPosition(1, v3);

            //Debug.Log("click " + tileX + " " + tileY);

            //setConnection();
        }
    }
    void OnMouseUp()
    {
        setConnection();
    }

    void setConnection() {
        if (!map.graph[tileX, tileY].isConnected)
        {
            GameObject toGo = MouseCast();

            // Draw line between object and lock it
            if (toGo != null && toGo.GetComponent<Cell>() != null)
            {
                isNeighbour = false;
                // Check if node is neighbour
                foreach (var tile in map.graph[tileX, tileY].edges)
                {
                    if (tile.node.x == toGo.GetComponent<Cell>().tileX && tile.node.y == toGo.GetComponent<Cell>().tileY)
                    {
                        isNeighbour = true;
                        makeConnection(toGo.GetComponent<Cell>().tileX, toGo.GetComponent<Cell>().tileY, tileX, tileY, toGo);
                        map.graph[tileX, tileY].isConnected = true;
                        break;
                    }
                }


            }
            else
            {
                // Set Line to origin
                myLine.SetPosition(0, v3);
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
                myLine.SetPosition(0, v3);
                /*
                if (prevNodeGO != null)
                {
                    prevNodeGO.GetComponent<SpriteRenderer>().color = Color.white;
                }
                //go.GetComponent<Cell>().isConnected = false;
                */
            }
            if (toGo != null)
            {
                prevNodeGO = toGo;
            }
        }

    }
    void makeConnection(int x, int y,int origX,int origY ,GameObject target_go)
    {
       Vector2 target = map.TileToWorldCoord(x,y);
       myLine.SetPosition(1, target);

       target_go.GetComponent<SpriteRenderer>().color = Color.red;
        //GetComponent<SpriteRenderer>().color = Color.red;

        if (map.vertices.Count == 0) // first insert in list of verices
        {
            map.vertices.Add(map.TileToWorldCoord(origX, origY));
            map.vertices.Add(map.TileToWorldCoord(x, y));
        }
        else {


            if (map.vertices.Contains(map.TileToWorldCoord(x,y))) // start == end ... shape is connected
            {
                Vector2[] vertices2D = map.vertices.ToArray();
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

                GameObject GO = Instantiate(map.trianglePref,map.trianglePref.transform.position,map.trianglePref.transform.rotation) as GameObject;
                MeshFilter filter = GO.GetComponent<MeshFilter>();
           

                GO.AddComponent<MeshCollider>();

                GO.GetComponent<MeshCollider>().sharedMesh = msh;
                filter.mesh = msh;
                map.vertices.Clear();

                foreach (var ele in map.vertices)
                {

                    Debug.Log(ele);
                }

                for (int i = 0; i < triangles.Length; i++)
                {

                    Debug.Log(triangles[i]);
                }
                
            }
            else {
                map.vertices.Add(map.TileToWorldCoord(x, y));
            }
       
            
        }

       Debug.Log("Conection made: "+ "[" + origX +","+origY+"] ---> " + "[" + x + "," + y + "]" );
    }

    void destroyConnection() {
        return;
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
}
