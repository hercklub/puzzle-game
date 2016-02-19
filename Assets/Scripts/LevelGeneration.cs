using UnityEngine;
using System.Collections;
using System;

public class LevelGeneration
{

    int width;
    int height;

    string seed;
    bool useRandomSeed;

    [Range(0, 100)]
    int randomFillPercent;

    int[,] map;

    public LevelGeneration(int _w,int _h,string _seed,bool _useSeed,int _fill,ref int[,] _map)
    {
        width = _w;
        height = _h;
        seed = _seed;
        useRandomSeed = _useSeed;
        randomFillPercent = _fill;
        map = _map;

    }

    public void GenerateMap()
    {
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
           // SmoothMap();
        }
    }


    void RandomFillMap()
    {
       
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }
        
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                
                map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 5)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }




}