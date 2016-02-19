using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {


   public bool clicked;
    int tileX = -1;
    int tileY = -1;

    int oldTileX = -1;
    int oldTileY = -1;
    Vector3 mousePos;
    MapTiles map;
    GameObject prevGO;

    int counter = 0;

    BoardManager board;
    void Awake()
    {
        board = FindObjectOfType<BoardManager>();
    }


    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    GameObject clickedGO = MouseCast();
        //    if (clickedGO != null)
        //    {
        //         Debug.Log("click " + clickedGO.GetComponent<Cell>().tileX + " " + clickedGO.GetComponent<Cell>().tileY);
        //        tileX = clickedGO.GetComponent<Cell>().tileX;
        //        tileY = clickedGO.GetComponent<Cell>().tileY;
        //        myLine = clickedGO.GetComponent<LineRenderer>();
        //        clicked = true;
        //    }

        //}


        if (Input.GetMouseButton(0))
        {
           
            GameObject clickedGO = MouseCast();

            if (clickedGO != null)
            {
           
                if((tileX != clickedGO.GetComponent<Cell>().tileX || tileY != clickedGO.GetComponent<Cell>().tileY))
                {
                   // Debug.Log("draged" + clickedGO.GetComponent<Cell>().tileX + " " + clickedGO.GetComponent<Cell>().tileY);
                    tileX = clickedGO.GetComponent<Cell>().tileX;
                    tileY = clickedGO.GetComponent<Cell>().tileY;
                   // Debug.Log("Registered Cell" + tileX + " " +tileY );
                    if ( (tileX != oldTileX || tileY != oldTileY) && oldTileX != -1 && oldTileY != -1 && prevGO != null)
                    {
                        if (!board.Lock)
                        {
                            prevGO.GetComponent<Cell>().setConnection(prevGO.GetComponent<Cell>().tileX, prevGO.GetComponent<Cell>().tileY, clickedGO);
                            Debug.Log("Connect " + prevGO.GetComponent<Cell>().tileX + " " + prevGO.GetComponent<Cell>().tileY + " ----> " + clickedGO.GetComponent<Cell>().tileX + " " + clickedGO.GetComponent<Cell>().tileY);
                        }
                        else
                        {
                            // Reset line position
                            prevGO.GetComponent<Cell>().SetLinePostion(prevGO.transform.position);
                            prevGO = null;
                            tileX = -1;
                            tileY = -1;
                        }
                    }

                    oldTileX = tileX;
                    oldTileY = tileY;
                    clicked = true;
                    prevGO = clickedGO;
                }
                else
                {
                    
                    clicked = false;

                }
            }
            if (prevGO != null  )
            {
                mousePos = Input.mousePosition;
                mousePos.z = 10.0f;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                prevGO.GetComponent<Cell>().SetLinePostion(mousePos);
               // Debug.Log("Update Postion of " +  prevGO.GetComponent<Cell>().tileX + " " + prevGO.GetComponent<Cell>().tileY);
            }


        }

        if (Input.GetMouseButtonUp(0) && prevGO !=null)
        {

            // Debug.Log("Reset position to zero of !" + prevGO.GetComponent<Cell>().tileX + " " + prevGO.GetComponent<Cell>().tileY);
            prevGO.GetComponent<Cell>().SetLinePostion(prevGO.transform.position);
            prevGO = null;
            tileX = -1;
            tileY = -1;
            
        }

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
