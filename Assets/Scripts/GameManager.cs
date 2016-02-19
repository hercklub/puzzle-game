using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public class Level
    {
        public Level(int _X,int _Y , int [,] _G , List<BoardManager.ShapeToComplete> _W )
        {
            mapSizeX = _X;
            mapSizeY = _Y;
            matrix = _G;
            winConditionShapes.AddRange(_W);
        }

        public int mapSizeX;
        public int mapSizeY;
        public int[,] matrix;
        public List<BoardManager.ShapeToComplete> winConditionShapes = new List<BoardManager.ShapeToComplete>() ;

    }
	

}
