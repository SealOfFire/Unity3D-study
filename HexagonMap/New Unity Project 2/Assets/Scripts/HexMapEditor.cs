using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapEditor : MonoBehaviour
{

	public Color[] colors;
	public HexGrid hexGrid;
	private Color activeColor;

	void Awake ()
	{
		SelectColor (0);
	}


	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButton (0)) {
			HandleInput ();
		}
	}

	void HandleInput ()
	{
		Ray inputRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (inputRay, out hit)) {
			this.hexGrid.TouchCell (hit.point,this.activeColor);
		}
	}

	public void SelectColor (int index)
	{
		this.activeColor = this.colors [index];
	}
}
