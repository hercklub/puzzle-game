using UnityEngine;
using System.Collections;

public class DrawTriangle : MonoBehaviour {

    public Mesh mesh;
    public Vector3[] vertices;
    public int[] triangles;



    void InitMesh()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "TST";
        vertices = new Vector3[3];
        triangles = new int[3];
    }

    void Awake()
    {
        InitMesh();

    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
