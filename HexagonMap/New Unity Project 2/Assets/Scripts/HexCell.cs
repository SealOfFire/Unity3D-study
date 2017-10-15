using UnityEngine;

public class HexCell : MonoBehaviour
{

	public HexCoordinates coordinates;
	public Color color;

	[SerializeField] 
	private HexCell[] neighbors;

	// Use this for initialization
	private void Start ()
	{
		
	}
	
	// Update is called once per frame
	private void Update ()
	{
		
	}

	/// <summary>
	/// 获取相邻的格
	/// </summary>
	/// <returns>The neighbor.</returns>
	/// <param name="direction">Direction.</param>
	public HexCell GetNeighbor (HexDirection direction)
	{ 
		return neighbors [(int)direction]; 
	}

	/// <summary>
	/// 设置相邻的格
	/// </summary>
	/// <param name="direction">Direction.</param>
	/// <param name="cell">Cell.</param>
	public void SetNeighbor (HexDirection direction, HexCell cell)
	{ 
		neighbors [(int)direction] = cell; 
		cell.neighbors[(int)direction.Opposite()] = this; 
	}
}
