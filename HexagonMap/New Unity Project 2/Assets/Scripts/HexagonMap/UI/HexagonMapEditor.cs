using UnityEngine;
using UnityEngine.EventSystems;

namespace HexagonMap.UI
{
	public class HexagonMapEditor : MonoBehaviour
	{
		public HexagonGrid hexagonGrid;
		public Material terrainMaterial;
		// private Color activeColor;
		// private HexagonCell previousCell, searchToCell, searchFromCell;
		private HexagonCell previousCell;

		/// <summary>
		/// 单位预设体
		/// </summary>
		public HexagonUnit unitPrefab;

		/// <summary>
		/// 显示网格边线
		/// </summary>
		/// <param name="visible"></param>
		public void ShowGrid (bool visible)
		{
			if (visible) {
				this.terrainMaterial.EnableKeyword ("GRID_ON");
			} else {
				this.terrainMaterial.DisableKeyword ("GRID_ON");
			}
		}

		public void SetEditMode (bool toggle)
		{
			this.enabled = toggle;
		}

		void Awake ()
		{
			this.ShowGrid (true);
			this.SetEditMode (false);
		}

		// Use this for initialization
		private void Start ()
		{

		}

		// Update is called once per frame
		private void Update ()
		{
			if (!EventSystem.current.IsPointerOverGameObject ()) {
				// 鼠标邮件点击判断选择的单元格
				if (Input.GetMouseButton (0)) {
					this.HandleInput ();
					return;
				}
				// 按下u键的时候创建单位
				if (Input.GetKeyDown (KeyCode.U)) {
					if (Input.GetKey (KeyCode.LeftShift)) {
						this.DestroyUnit ();
					} else {
						this.CreateUnit ();
					}
					return;
				}
			}
			this.previousCell = null;
		}

		/// <summary>
		/// 
		/// </summary>
		private void HandleInput ()
		{
			HexagonCell currentCell = this.GetCellUnderCursor ();
			if (currentCell) {
				if (this.previousCell && previousCell != currentCell) {
					// 检测拖拽
				} else {
					//  没有拖拽
				}
				// EditCells (currentCell);
				previousCell = currentCell;
			} else {
				previousCell = null;
			}
		}

		/// <summary>
		/// 获取当前点击的单元格
		/// </summary>
		/// <returns>The cell under cursor.</returns>
		private HexagonCell GetCellUnderCursor ()
		{
			return this.hexagonGrid.GetCell (Camera.main.ScreenPointToRay (Input.mousePosition));
		}

		/// <summary>
		/// 创建单位
		/// </summary>
		private void CreateUnit ()
		{
			HexagonCell cell = this.GetCellUnderCursor ();
			// 选中的单元格不是空
			// 当前单元格上没有其他单位时才能创建单位
			if (cell && !cell.Unit) {
				// 单位放到地图上
				this.hexagonGrid.AddUnit (Instantiate<HexagonUnit> (this.unitPrefab), cell, Random.Range (0f, 360f));
			}
		}

		/// <summary>
		/// 移除单位
		/// </summary>
		private void DestroyUnit ()
		{
			HexagonCell cell = this.GetCellUnderCursor ();
			if (cell && cell.Unit) {
				this.hexagonGrid.RemoveUnit (cell.Unit);
			}
		}
	}
}