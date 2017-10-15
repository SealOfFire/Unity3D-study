using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{

	public int width = 6;
	public int height = 6;

	public HexCell cellPrefab;
	public Text cellLabelPrefab;

	public Color defaultColor = Color.white;
	public Color touchedColor = Color.magenta;

	public HexCell[] cells;
	private Canvas gridCanvas;
	public HexMesh hexMesh;

	void Awake ()
	{
		this.gridCanvas = GetComponentInChildren<Canvas> ();
		//
		this.hexMesh = GetComponentInChildren<HexMesh> ();
		//
		this.cells = new HexCell[height * width];
		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				this.CreateCell (x, z, i++);
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		this.hexMesh.Triangulate (cells);
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void CreateCell (int x, int z, int i)
	{
		Vector3 position;
		//position.x = x * 10f;
		//position.y = 0f;
		//position.z = z * 10f;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		// position.x = (x + z * 0.5f) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = this.cells [i] = Instantiate<HexCell> (this.cellPrefab);
		cell.transform.SetParent (transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates (x, z);
		cell.color = defaultColor;
		//
		// 设置邻居
		if (x > 0) { 
			cell.SetNeighbor (HexDirection.W, cells [i - 1]); 
		} 
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor (HexDirection.SE, cells [i - width]);
				if (x > 0) {
					cell.SetNeighbor (HexDirection.SW, cells [i - width - 1]);
				}
			} else {
				cell.SetNeighbor (HexDirection.SW, cells [i - width]);
				if (x < width - 1) {
					cell.SetNeighbor (HexDirection.SE, cells [i - width + 1]);
				}
			}
		}
		//
		Text label = Instantiate<Text> (this.cellLabelPrefab);
		label.rectTransform.SetParent (gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = new Vector2 (position.x, position.z);
		// label.text = x.ToString() + "\n" + z.ToString();
		label.text = cell.coordinates.ToStringOnSeparateLines ();
	}

	public void TouchCell (Vector3 position, Color color)
	{
		position = transform.InverseTransformPoint (position);
		// Debug.Log ("touched at " + position);
		HexCoordinates coordinates = HexCoordinates.FromPosition (position);        
		Debug.Log ("touched at " + coordinates.ToString ());
		int index = coordinates.X + coordinates.Z * this.width + coordinates.Z / 2;
		HexCell cell = this.cells [index];
		cell.color = color;
		this.hexMesh.Triangulate (this.cells);
	}
}
