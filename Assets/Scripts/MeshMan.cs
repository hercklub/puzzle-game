using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshMan : MonoBehaviour {
    //nodes of completed shape
    public List<Vector2> nodes;

    // Use this for initialization


    public void ClearMesh()
    {
        foreach (var ele in nodes)
        {
            Debug.Log(ele.x + " , " + ele.y);
        }
    }



}
