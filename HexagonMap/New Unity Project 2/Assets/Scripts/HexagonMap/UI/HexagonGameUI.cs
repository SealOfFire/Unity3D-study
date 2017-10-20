using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HexagonMap.UI
{
	public class HexagonGameUI : MonoBehaviour
	{
		/// <summary>
		/// 地图
		/// </summary>
		public HexagonGrid hexagonGrid;

		/// <summary>
		///  当前光标选定的单元格
		/// </summary>
		private HexagonCell currentCell;

		/// <summary>
		/// 选中的单位
		/// </summary>
		private HexagonUnit selectedUnit;

		// Use this for initialization
		private void Start ()
		{
		
		}
	
		// Update is called once per frame
		private void Update ()
		{
			if (!EventSystem.current.IsPointerOverGameObject ()) {
				if (Input.GetMouseButtonDown (0)) {
					// 选择一个单元格
					this.DoSelection ();
				} else if (this.selectedUnit) {
					if (Input.GetMouseButtonDown (1)) {
						this.DoMove ();
					} else {
						// 有选中的单元格的时候
						this.DoPathfinding ();
					}
				}
			}
		}

		/// <summary>
		/// 当前选中的单元格更新，并返回选中的单元格是否变更
		/// </summary>
		/// <returns><c>true</c>, if current cell was updated, <c>false</c> otherwise.</returns>
		private bool UpdateCurrentCell ()
		{
			HexagonCell cell = this.hexagonGrid.GetCell (Camera.main.ScreenPointToRay (Input.mousePosition));
			if (cell != this.currentCell) {
				this.currentCell = cell;
				return true;
			}
			return false;
		}

		/// <summary>
		///  选择一个单元格
		///  如果存在当前的单元格，占据单元格的单位将成为所选单位。如果这个单元格没有单位的话，那么我们最终就不选择单位。
		/// </summary>
		private void DoSelection ()
		{
			this.hexagonGrid.ClearPath ();
			this.UpdateCurrentCell ();
			if (this.currentCell) {
				this.selectedUnit = this.currentCell.Unit;
			}
		}

		/// <summary>
		/// 查询路径
		/// </summary>
		private void DoPathfinding ()
		{
			if (this.UpdateCurrentCell ()) {
				if (this.currentCell && this.selectedUnit.IsValidDestination (this.currentCell)) {
					this.hexagonGrid.FindPath (this.selectedUnit.Location, currentCell, 24);
				} else {
					this.hexagonGrid.ClearPath ();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toggle">If set to <c>true</c> toggle.</param>
		public void SetEditMode (bool toggle)
		{
			this.enabled = !toggle;
			// this.hexagonGrid.ShowUI (!toggle);
			this.hexagonGrid.ClearPath ();
		}

		/// <summary>
		/// 移动单位
		/// </summary>
		private void DoMove ()
		{
			if (this.hexagonGrid) {
				this.selectedUnit.Location = currentCell;
				this.hexagonGrid.ClearPath ();
			}
		}
	}
}