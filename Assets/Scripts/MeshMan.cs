using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshMan:MonoBehaviour
{
    //nodes of completed shape
    public List<Vector2> nodes;
    public List<Shape> shapes = new List<Shape>();
    public int id;
    public  MeshMan helperShape = null;
    public bool isRendered = true;
    // Use this for initialization


    public void ClearMesh()
    {
        foreach (var ele in nodes)
        {
            Debug.Log(ele.x + " , " + ele.y);
        }
    }

    public static bool operator ==(MeshMan  a, MeshMan b)
    {
        int counter = 0;

        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }
        if (a.nodes.Count != a.nodes.Count)
            return false;

        foreach (var ele in a.nodes)
        {
            if (b.nodes.Contains(ele))
                counter++;
        }

        if (counter == b.nodes.Count)
            return true;
        else
            return false;
    
    }

    public static bool operator !=(MeshMan a, MeshMan b)
    {
        return !(a == b);
    }




}
