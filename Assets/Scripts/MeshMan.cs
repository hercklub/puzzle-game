using UnityEngine;
using System.Collections;

public class MeshMan : MonoBehaviour {

    // Use this for initialization
    void MouseCast()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.transform.name);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseCast();

        }
       
    }

}
