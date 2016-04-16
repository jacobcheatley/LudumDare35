using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
public class ArenaPolygon : MonoBehaviour
{
    [Range(3, 12)]
    public int sides;
    [Range(2f, 5f)]
    public float radius;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private PolygonCollider2D polygonCollider2D;
    [HideInInspector] public Vector3[] vertices;

    void Start ()
    {
	    mesh = new Mesh();
	    meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
	    polygonCollider2D = GetComponent<PolygonCollider2D>();
	    GenerateMesh();
    }

	void OnValidate()
	{
        if (mesh != null)
	        GenerateMesh();
	}

    [ContextMenu("Generate")]
    void GenerateMesh()
    {
        mesh.Clear();
        vertices = new Vector3[sides];
        int[] triangles = new int[3 * (sides - 2)];
        Vector3 point = Vector3.right * radius;
        float rotation = 360f / sides;
        for (int i = 0; i < sides; i++)
        {
            vertices[i] = point;
            point = Quaternion.Euler(0, 0, -rotation) * point;
        }
        for (int i = 0; i < sides - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        polygonCollider2D.points = vertices.Select(v => (Vector2)v).ToArray();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ball")
            Debug.Log("Ball exited.");
    }
}
