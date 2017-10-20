using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HexagonMap
{
	/// <summary>
	/// 六边形网格控件
	/// </summary>
	public class HexagonGrid : MonoBehaviour
	{
		#region 成员

		/// <summary>
		/// 六边形网格铺设方向
		/// </summary>
		public enum CellOrientation
		{
			/// <summary>
			/// 水平
			/// </summary>
			Horizontal,

			/// <summary>
			/// 垂直
			/// </summary>
			Vertical
		}

		/// <summary>
		/// 网格的外径
		/// </summary>
		[SerializeField, SetProperty ("CellOuterRadius")]
		private float cellOuterRadius = 10f;

		/// <summary>
		/// 网格内的所有单元格
		/// </summary>
		private HexagonCell[] cells;

		///// <summary>
		///// 宽度网格数
		///// </summary>
		//[SerializeField, SetProperty("Width")]
		//private int width = 6;

		///// <summary>
		///// 高度网格数
		///// </summary>
		//[SerializeField, SetProperty("Height")]
		//private int height = 6;

		/// <summary>
		/// 单元格预制 用于批量创建单元格
		/// </summary>
		[SerializeField, SetProperty ("PrefabCell")]
		private HexagonCell prefabCell;

		/// <summary>
		/// 六边形网格铺设方向
		/// </summary>
		[SerializeField, SetProperty ("Direction")]
		private CellOrientation cellOrientation = CellOrientation.Horizontal;

		/// <summary>
		/// 绘制网格地图的模型
		/// </summary>
		// private HexagonMesh hexMesh;

		#region 地图分块

        /// <summary>
        /// 宽度网格数
        /// </summary>
        public int cellCountWidth;

		/// <summary>
		/// 高度网格数
		/// </summary>
		public int cellCountHeight;

		/// <summary>
		/// 分块数
		/// </summary>
		public const int chunkSizeWidth = 5, chunkSizeHeight = 5;

		/// <summary>
		/// 每一块的大小
		/// </summary>
		public int chunkCountWidth = 4, chunkCountHeight = 3;

		/// <summary>
		/// 所有的块
		/// </summary>
		private HexagonGridChunk[] chunks;

		/// <summary>
		/// 
		/// </summary>
		public HexagonGridChunk chunkPrefab;

		public Color defaultColor = Color.green;
		public Color touchedColor = Color.magenta;

		public Text cellLabelPrefab;

		/// <summary>
		/// 路径检索优先级队列
		/// </summary>
		private HexagonCellPriorityQueue searchFrontier;

		/// <summary>
		/// 检索的路径是否存在
		/// </summary>
		private bool currentPathExists;

		/// <summary>
		/// 检索边界
		/// </summary>
		private int searchFrontierPhase;

		/// <summary>
		/// 查找路径的起始,终结单元格
		/// </summary>
		private HexagonCell currentPathFrom, currentPathTo;

		#endregion

		/// <summary>
		/// 地图上的单位列表
		/// </summary>
		List<HexagonUnit> units = new List<HexagonUnit> ();

		#endregion

		#region 属性

		/// <summary>
		/// 网格的外径
		/// </summary>
		public float CellOuterRadius {
			get { return this.cellOuterRadius; }
			set {
				if (value <= 0)
					this.cellOuterRadius = 10f;
				else
					this.cellOuterRadius = value;
			}
		}

		/// <summary>
		/// 网格内径
		/// </summary>
		/// <value>The cell inner radius.</value>
		public float CellInnerRadius { get { return this.cellOuterRadius * 0.866025404f; } }

		/// <summary>
		/// 六边形网格铺设方向
		/// </summary>
		public CellOrientation Direction { get { return this.cellOrientation; } set { this.cellOrientation = value; } }

		public HexagonCell PrefabCell { get { return this.prefabCell; } set { this.prefabCell = value; } }

		/// <summary>
		/// 
		/// </summary>
		/// <value><c>true</c> if this instance has path; otherwise, <c>false</c>.</value>
		public bool HasPath{ get { return this.currentPathExists; } }

		#endregion

		#region 方法

		#region u3d反射方法

		private void Awake ()
		{
			// this.hexMesh = this.GetComponentInChildren<HexagonMesh>();

			// 计算网格总数
			this.cellCountHeight = this.chunkCountHeight * HexagonGrid.chunkSizeHeight;
			this.cellCountWidth = this.chunkCountWidth * HexagonGrid.chunkSizeWidth;

			//
			this.CreateChunks ();
			this.CreateCells ();
		}

		// Use this for initialization
		private void Start ()
		{
			// 绘制地图网格
			// this.hexMesh.Triangulate(this.cells);
		}

		// Update is called once per frame
		private void Update ()
		{
			//// 点击处理
			//if (Input.GetMouseButton(0))
			//{
			//    this.HandleInput();
			//}
		}

		#endregion

		/// <summary>
		/// 初始化单元格（位置）
		/// </summary>
		private void CreateCells ()
		{
			this.cells = new HexagonCell[this.cellCountHeight * this.cellCountWidth];
			int index = 0; // 单元格序号
			for (int heightIndex = 0; heightIndex < this.cellCountHeight; heightIndex++) {
				for (int widthIndex = 0; widthIndex < this.cellCountWidth; widthIndex++) {
					this.CreateCell (widthIndex, heightIndex, index++, this.cellOuterRadius);
				}
			}
		}

		/// <summary>
		/// 创建网格
		/// </summary>
		/// <param name="widthIndex">网格宽方向上的位置</param>
		/// <param name="heightIndex">网格高方向上的位置</param>
		/// <param name="index">网格的索引</param>
		/// <param name="outerRadius">网格的外径</param>
		public void CreateCell (int widthIndex, int heightIndex, int index, float outerRadius)
		{
			Vector3 position; // 单元格的中心位置
			position.y = 0f;
			// 根据网格方向不同，设置不同的网格排列方式
			switch (this.cellOrientation) {
			case CellOrientation.Horizontal:
				position.x = (widthIndex + heightIndex * 0.5f - heightIndex / 2) * (this.CellInnerRadius * 2f);
				position.z = heightIndex * (this.cellOuterRadius * 1.5f);
				break;
			case CellOrientation.Vertical:
                    // TODO 这部分没有实现
                    // position.x = heightIndex * (this.cellOuterRadius * 1.5f);
                    // position.z = (widthIndex + heightIndex * 0.5f - heightIndex / 2) * (this.CellInnerRadius * 2f);
				position.x = (widthIndex + heightIndex * 0.5f - heightIndex / 2) * (this.CellInnerRadius * 2f);
				position.z = heightIndex * (this.cellOuterRadius * 1.5f);
				break;
			default:
				position.x = widthIndex * outerRadius;
				position.z = heightIndex * outerRadius;
				break;
			}

			// 创建网格
			this.cells [index] = Instantiate<HexagonCell> (this.prefabCell);
			// 设置单元格的位置
			// this.cells[index].transform.SetParent(this.transform, false);
			this.cells [index].transform.localPosition = position;
			this.cells [index].Coordinates = HexagonCoordinates.FromOffsetCoordinates (widthIndex, heightIndex);
			this.cells [index].Initialize (this);
			this.cells [index].Color = this.defaultColor;
			// 设置标签
			Text label = Instantiate<Text> (cellLabelPrefab);
			this.cells [index].uiRect = label.rectTransform;
			// label.rectTransform.SetParent(gridCanvas.transform, false);
			label.rectTransform.anchoredPosition = new Vector2 (position.x, position.z);
			// label.text = x.ToString() + "\n" + z.ToString();

			// this.cells[index].uiRect = label.rectTransform;
			// 设置相邻的单元格
			if (widthIndex > 0) {
				this.cells [index].SetNeighbor (HexagonDirection.W, this.cells [index - 1]);
			}
			if (heightIndex > 0) {
				if ((heightIndex & 1) == 0) {
					this.cells [index].SetNeighbor (HexagonDirection.SE, this.cells [index - this.cellCountWidth]);
					if (widthIndex > 0) {
						this.cells [index].SetNeighbor (HexagonDirection.SW, this.cells [index - this.cellCountWidth - 1]);
					}
				} else {
					this.cells [index].SetNeighbor (HexagonDirection.SW, this.cells [index - this.cellCountWidth]);
					if (widthIndex < this.cellCountWidth - 1) {
						this.cells [index].SetNeighbor (HexagonDirection.SE, this.cells [index - this.cellCountWidth + 1]);
					}
				}
			}
			//
			this.AddCellToChunk (widthIndex, heightIndex, this.cells [index]);
		}

		/// <summary>
		/// 单元格添加到指定的块
		/// </summary>
		/// <param name="widthIndex"></param>
		/// <param name="heightIndex"></param>
		/// <param name="cell"></param>
		private void AddCellToChunk (int widthIndex, int heightIndex, HexagonCell cell)
		{
			int chunkX = widthIndex / HexagonGrid.chunkSizeWidth;
			int chunkZ = heightIndex / HexagonGrid.chunkSizeHeight;
			HexagonGridChunk chunk = this.chunks [chunkX + chunkZ * this.chunkCountWidth];

			int localX = widthIndex - chunkX * HexagonGrid.chunkSizeWidth;
			int localZ = heightIndex - chunkZ * HexagonGrid.chunkSizeHeight;
			chunk.AddCell (localX + localZ * HexagonGrid.chunkSizeWidth, cell);
		}

		/// <summary>
		/// 
		/// </summary>
		private void CreateChunks ()
		{
			this.chunks = new HexagonGridChunk[this.chunkCountWidth * this.chunkCountHeight];

			for (int z = 0, i = 0; z < this.chunkCountHeight; z++) {
				for (int x = 0; x < this.chunkCountWidth; x++) {
					HexagonGridChunk chunk = chunks [i++] = Instantiate<HexagonGridChunk> (chunkPrefab);
					chunk.transform.SetParent (transform);
				}
			}
		}

		#region 处理点击

		/// <summary>
		/// 获取点击位置的单元格
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public HexagonCell GetCell (Vector3 position)
		{
			position = transform.InverseTransformPoint (position);
			HexagonCoordinates coordinates = HexagonCoordinates.FromPosition (position, this);
			// Debug.Log("touched at " + coordinates);
			int index = coordinates.X + coordinates.Z * this.cellCountWidth + coordinates.Z / 2;
			// Debug.Log(this.cells.Length);
			Debug.Log ("touched at " + coordinates + "index:" + index);
			return this.cells [index];
			// return null;
		}

		/// <summary>
		/// 判断拾取射线是否选中单元格
		/// </summary>
		/// <returns>The cell.</returns>
		/// <param name="ray">Ray.</param>
		public HexagonCell GetCell (Ray ray)
		{
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				return GetCell (hit.point);
			}
			return null;
		}

		#endregion

		/// <summary>
		/// 查找两个单元格之间的路径
		/// </summary>
		/// <param name="fromCell"></param>
		/// <param name="toCell"></param>
		public void FindPath (HexagonCell fromCell, HexagonCell toCell, int speed)
		{
			// 测试寻路的时间花费
			//System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			//sw.Start();
			this.ClearPath (); // 清除上次的检索结果
			this.currentPathFrom = fromCell;
			this.currentPathTo = toCell;
			this.currentPathExists = this.Search (fromCell, toCell, speed);
			this.ShowPath (speed); // 高亮检索到的路径
			//sw.Stop();
			//Debug.Log(sw.ElapsedMilliseconds);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		private bool Search (HexagonCell fromCell, HexagonCell toCell, int speed)
		{
			this.searchFrontierPhase += 2;

			if (this.searchFrontier == null) {
				this.searchFrontier = new HexagonCellPriorityQueue ();
			} else {
				this.searchFrontier.Clear ();
			}

			// toCell.EnableHighlight(Color.red);
			fromCell.SearchPhase = searchFrontierPhase;
			fromCell.Distance = 0;
			this.searchFrontier.Enqueue (fromCell);
			while (this.searchFrontier.Count > 0) {
				// yield return delay;
				HexagonCell current = searchFrontier.Dequeue ();
				current.SearchPhase += 1;

				// 找到目标单元格时结束循环
				if (current == toCell) {
					return true;
				}

				// 计算移动到目的需要的回合数

				int currentTurn = current.Distance / speed;
				for (HexagonDirection d = HexagonDirection.NE; d <= HexagonDirection.NW; d++) {
					HexagonCell neighbor = current.GetNeighbor (d);
					if (neighbor == null || neighbor.SearchPhase > searchFrontierPhase) {
						continue;
					}
					if (neighbor.Unit) {
						//  单元格中有单位的时候跳过这个单元格
						continue;
					}
					// 避开有水的地方
					//if (neighbor.IsUnderwater)
					//{
					//    continue;
					//}
					// 避开悬崖
					//if (current.GetEdgeType(neighbor) == HexEdgeType.Cliff)
					//{
					//    continue;
					//}
					// 沿着道路移动更快
					// int distance = current.Distance;
					// 根据移动成本计算
					int moveCost = 1;
					//if (current.HasRoadThroughEdge(d))
					//{
					//    moveCost = 1;
					//}
					//else if (current.Walled != neighbor.Walled)
					//{
					//    continue;
					//}
					//else
					//{
					//    moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
					//    moveCost += neighbor.UrbanLevel + neighbor.FarmLevel +
					//    neighbor.PlantLevel;
					//}
					int distance = current.Distance + moveCost;
					// distance += 1;

					// 计算移动到目的需要的回合数
					int turn = distance / speed;
					if (turn > currentTurn) {
						distance = turn * speed + moveCost;
					}


					//if (neighbor.Distance == int.MaxValue)
					if (neighbor.SearchPhase < this.searchFrontierPhase) {
						neighbor.SearchPhase = this.searchFrontierPhase;
						neighbor.Distance = distance;
						// neighbor.SetLabel(turn.ToString());
						neighbor.PathFrom = current;
						neighbor.SearchHeuristic = neighbor.Coordinates.DistanceTo (toCell.Coordinates);
						this.searchFrontier.Enqueue (neighbor);
					} else if (distance < neighbor.Distance) {
						neighbor.Distance = distance;
						// neighbor.SetLabel(turn.ToString());
						neighbor.PathFrom = current;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// 高亮路径
		/// </summary>
		/// <param name="speed"></param>
		private void ShowPath (int speed)
		{
			if (currentPathExists) {
				HexagonCell current = this.currentPathTo;
				while (current != currentPathFrom) {
					int turn = current.Distance / speed;
					current.SetLabel (turn.ToString ());
					current.EnableHighlight (Color.white);
					current = current.PathFrom;
				}
			}
			currentPathFrom.EnableHighlight (Color.blue);
			currentPathTo.EnableHighlight (Color.red);
		}

		/// <summary>
		/// 清除路径
		/// </summary>
		public void ClearPath ()
		{
			if (currentPathExists) {
				HexagonCell current = currentPathTo;
				while (current != currentPathFrom) {
					current.SetLabel (null);
					current.DisableHighlight ();
					current = current.PathFrom;
				}
				current.DisableHighlight ();
				currentPathExists = false;
			}
			currentPathFrom = currentPathTo = null;
		}

		#region 单位处理方法

		/// <summary>
		/// 清除所有的单位
		/// </summary>
		private void ClearUnits ()
		{
			for (int i = 0; i < this.units.Count; i++) {
				this.units [i].Die ();
			}
		}

		/// <summary>
		/// 向网格添加单位
		/// </summary>
		/// <param name="unit">Unit.</param>
		/// <param name="location">Location.</param>
		/// <param name="orientation">Orientation.</param>
		public void AddUnit (HexagonUnit unit, HexagonCell location, float orientation)
		{
			this.units.Add (unit);
			unit.transform.SetParent (this.transform, false);
			unit.Location = location;
			unit.Orientation = orientation;
		}

		/// <summary>
		/// 从网格上移除单位
		/// </summary>
		/// <param name="unit">Unit.</param>
		public void RemoveUnit (HexagonUnit unit)
		{
			this.units.Remove (unit);
			unit.Die ();
		}

		#endregion

		#endregion
	}
}