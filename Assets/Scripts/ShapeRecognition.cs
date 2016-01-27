using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShapeRecognition : MonoBehaviour {
    // Use this for initialization
    MapTiles map;



    public const int square = 1;
    public const int rectangle = 2;
    public const int diamond = 3;
    public const int diamondRec = 4;

    void Start () {
        map = FindObjectOfType<MapTiles>();
        
    }

    public bool ValidShape()
    {

        Debug.Log(map.shape.Count);
        if (map.shape.Count == 3) // Triangle
        {

            return true;
        }
        if (map.shape.Count == 4) // Square
        {
            Debug.Log("4 GON");
            // direction between first 2 nodes
            int dirType = map.DeltaMovement(map.shape[0], map.shape[1]);
            //Debug.Log("INIT: " + dirType + " " + map.shape[0] +" " + map.shape[1]);
            //lenght , in case of square , all sides will be equal
            float length = Vector2.Distance(map.shape[0],map.shape[1]);
            //determine if shape have diffrent side lenghts
            bool rectangleFlag = false;
            /*                   Direction types

                                    - only vertical and horzintal directions between nodes

                                    * * *
                                    * * *
                                    * * *     
             */

            /*                    -only diagonal direcitions

                                        *
                                      * * *
                                    * * * * *
                                      * * *
                                        *


             */
            
            if (dirType % 2 != 0)
            {

                if (CheckAllNodesDirection(1, ref rectangleFlag,length))
                {
                    if (!rectangleFlag)
                        Debug.Log("SQUARE");
                    else
                        Debug.Log("RECTANGLE");
                    return true;

                }
    
            }
            else
            {

                if (CheckAllNodesDirection(0, ref rectangleFlag,length))
                {
                    if (!rectangleFlag)
                        Debug.Log("DIAMOND");
                    else
                        Debug.Log("DIAMOND RECT");
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    bool CheckAllNodesDirection(int type, ref bool isRec,float len)
    {
        Vector2 oldEle = new Vector2(-1,-1);
        foreach (Vector2 ele in map.shape)
        {
            if (oldEle != new Vector2(-1, -1))
            {
               Debug.Log(map.DeltaMovement(ele, oldEle));
                if (map.DeltaMovement(ele, oldEle) % 2 != type)
                {
                    Debug.Log(type + "   " + ele + " " + oldEle);
                    
                    return false;
                }

                if (len != Vector2.Distance(ele, oldEle))
                {

                    isRec = true;
                }
            }
            oldEle = ele;

        }

        // special case for connection from last -> first
        if (map.DeltaMovement(map.shape[map.shape.Count - 1], map.shape[0]) % 2 != type)
        {
            return false;
        }


            return true;
    }
    
	void Update () {
	
	}
}
