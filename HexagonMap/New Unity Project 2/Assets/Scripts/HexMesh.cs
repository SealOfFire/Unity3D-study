using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
	private Mesh hexMesh;
	private List<Vector3> vertices;
	private List<int> triangles;
	private MeshCollider meshCollider;
	private List<Color> colors;

	void Awake ()
	{
		// this.hexGrid = GetComponentInChildren<HexGrid>();

		GetComponent<MeshFilter> ().mesh = hexMesh = new Mesh ();
		this.meshCollider = gameObject.AddComponent<MeshCollider> ();
		this.hexMesh.name = "Hex Mesh";
		this.vertices = new List<Vector3> ();
		this.triangles = new List<int> ();
		this.colors = new List<Color> ();
	}

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Triangulate (HexCell[] cells)
	{
		this.hexMesh.Clear ();
		this.vertices.Clear ();
		this.triangles.Clear ();
		this.colors.Clear ();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate (cells [i]);
		}
		this.hexMesh.vertices = vertices.ToArray ();
		this.hexMesh.triangles = triangles.ToArray ();
		this.hexMesh.RecalculateNormals ();
		this.hexMesh.colors = colors.ToArray ();

		this.meshCollider.sharedMesh = hexMesh;
	}

	void Triangulate (HexCell cell)
	{
		Vector3 center = cell.transform.localPosition;
		for (int i = 0; i < 6; i++) {
			AddTriangle (center, center + HexMetrics.corners [i], center + HexMetrics.corners [i + 1]);
			AddTriangleColor (cell.color);
		}
	}

	void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;
		this.vertices.Add (v1);
		this.vertices.Add (v2);
		this.vertices.Add (v3);
		this.triangles.Add (vertexIndex);
		this.triangles.Add (vertexIndex + 1);
		this.triangles.Add (vertexIndex + 2);
	}

	void AddTriangleColor (Color color)
	{
		colors.Add (color);
		colors.Add (color);
		colors.Add (color);
	}
		
}
