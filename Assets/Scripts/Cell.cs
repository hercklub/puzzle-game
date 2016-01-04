using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {
    LineRenderer myLine;
    public int tileX;
    public int tileY;
    public MapTiles map;

    bool isNeighbour = false;
    Vector3 v3; // mouse position
    void Start()
    {
        myLine = GetComponent<LineRenderer>();

    }
    void OnMouseDrag()
    {
        
        v3 = Input.mousePosition;
        v3.z = 10.0f;
        v3 = Camera.main.ScreenToWorldPoint(v3);

     
        myLine.SetPosition(0,transform.position);
        myLine.SetPosition(1,v3);

       // Debug.Log("click " + tileX + " " + tileY);
    }
    void OnMouseUp()
    {
        GameObject go = MouseCast();

           // Draw line between object and lock it
        if (go != null && go.GetComponent<Cell>() != null )
        {
            isNeighbour = false;
            // Check if node is neighbour
            foreach (var tile in map.graph[tileX, tileY].edges)
            {
                if (tile.x == go.GetComponent<Cell>().tileX && tile.y == go.GetComponent<Cell>().tileY)
                {
                    isNeighbour = true;
                    makeConnection(go.GetComponent<Cell>().tileX, go.GetComponent<Cell>().tileY);
                    break;
                }
            }
            

        }
        else 
        {
            // Set Line to origin
            myLine.SetPosition(0, v3);
        }

        if (!isNeighbour) {
            // Set Line to origin
            myLine.SetPosition(0, v3);
        }
    }
    void makeConnection(int x, int y)
    {
       Vector2 target = map.TileToWorldCoord(x,y);
        myLine.SetPosition(1, target);


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
