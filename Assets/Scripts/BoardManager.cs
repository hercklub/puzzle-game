using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

    public int numOfvalidShape = 0;
    public int totalNumOfShapes = 0;
    public bool invalidShape = false;
    public bool Lock = false;
    MapTiles map;
    UiShapeManager ui;
    public class ShapeToComplete
    {
        public Shape element;

        public bool finished = false;

        public ShapeToComplete(Shape _S)
        {
            element = _S;
        }

    }
    
    public  List<ShapeToComplete> winConditionShapes = new List<ShapeToComplete>();
    public Shape excessShape ;

    void Start()
    {
        
        map = FindObjectOfType<MapTiles>();
        ui = FindObjectOfType<UiShapeManager>();
    }

    bool AddCompletedShape(ref Shape recShape)
    {

        Debug.Log("WIN CONDITION COUNT" + winConditionShapes.Count);
        totalNumOfShapes++;
        int i = 0;
        foreach (var it in winConditionShapes)
        {
            if (it.element.GetType() == recShape.GetType())
            {
                if (it.element.type == recShape.type)
                {
                    if (!it.finished)
                    {
                        Debug.Log(it.element.GetType().Name + "  DONE" + it.element.type);
                        numOfvalidShape++;
                        ui.MarkAsFinished(i);
                        it.finished = true;
                        if (CheckWinCondition())
                        {
                            Debug.Log("LEVEL FINISHED");
                            //allValidShapes = false;
                        }
                        return true;

                    }
                  
                }
                
            }
            i++;
        }
        excessShape = recShape;
        Lock = true;
        return false;

    }
    bool CheckWinCondition()
    {
        foreach (var it in winConditionShapes)
        {
            if (!it.finished)
            {
                return false;
            }

        }
        if (totalNumOfShapes != numOfvalidShape)
            return false;
        for (int i = 0; i < map.mapSizeX; i++)
        {
            for (int j = 0; j < map.mapSizeY; j++)
            {
                int tile = map.tiles[i, j];
                if (tile != 0)
                {
                    if (!map.graph[i, j].isConnected)
                    {

                        return false;
                    }

                }
            }
        }


        return true;
    }
    public void DeleteShapes(List<Shape> shapes)
    {
        if (excessShape != null)
        {
            foreach (var shape in shapes)
            {

                if (excessShape.GetType() == shape.GetType())
                {
                    if (excessShape.type == shape.type)
                    {
                        Lock = false;
                        excessShape = null;
                        shapes.Remove(shape);
                        totalNumOfShapes--;
                        break;

                    }
                }
            }
        }
        
        foreach (var shape in shapes)
        {
            int i = 0;
            totalNumOfShapes--;     
            foreach (var it in winConditionShapes)
            {
                if (it.element.GetType() == shape.GetType())
                {
                    if (it.element.type == shape.type)
                    {
                        if (it.finished)
                        {
                            Debug.Log(it.element.GetType().Name + "  CLEARED" + it.element.type);
                            it.finished = false;
                            ui.MarkAsUnFinished(i);
                            numOfvalidShape--;
                            break;
                        }
                    }
                 
                }
                i++;
            }

            
        }




    } 
    public void ProcessShape(Shape recShape)
    {
        if (recShape != null )
        {
            //Debug.Log( " PROCESSING SHAPE "+ recShape.GetType().Name);
            if (AddCompletedShape(ref recShape))
            {
                Debug.Log("ADD SHAPE");
                foreach (var temp in winConditionShapes)
                {
                    Debug.Log(temp.element.GetType().Name + " TYPE " + temp.element.type + " " + temp.finished);
                }
            }
            else
            {
                //numOfvalidShape++;
                Debug.Log("WRONG SHAPE ");

            }
            
        }

        else
        {
        
        }


    }
}
