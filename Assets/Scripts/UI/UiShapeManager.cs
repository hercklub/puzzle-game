using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UiShapeManager : MonoBehaviour {
    public List<Image> UiShape = new List<Image>();
    BoardManager board;

    public Sprite square;
    public Sprite triangle;
    public Sprite rectangle;
    public Sprite diamond;

    // Use this for initialization
    void Start () {
        //UiShape.AddRange(GetComponentsInChildren<Image>());
	}

    public void SetStartingShapes()
    {
        foreach (Transform tra in gameObject.transform)
        {
            UiShape.Add(tra.gameObject.GetComponent<Image>());
        }
       

        Debug.Log(UiShape.Count);
        board = FindObjectOfType<BoardManager>();
        int i = 0;
        foreach (var shape in board.winConditionShapes)
        {

            if (shape.element.GetType() == typeof(Shape.Rectangle))
            {
                if (shape.element.type == true)
                {
                    UiShape[i].sprite = square;
                }
                else
                {
                    UiShape[i].sprite = rectangle;
                }
            }
            else if (shape.element.GetType() == typeof(Shape.Triangle))
            {
                if (shape.element.type == true)
                {
                    UiShape[i].sprite = triangle;
                }
                else
                {
                    UiShape[i].sprite = triangle;
                }
            }
            else if (shape.element.GetType() == typeof(Shape.Diamond))
            {
                if (shape.element.type == true)
                {
                    UiShape[i].sprite = diamond;
                }
                else
                {
                    UiShape[i].sprite = diamond;
                }
            }
            i++;
        }
    }

    public void MarkAsFinished(int index)
    {
        UiShape[index].color = Color.clear;
    }
    public void MarkAsUnFinished(int index)
    {
        UiShape[index].color = Color.white;
    }
}
