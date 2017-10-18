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
        [SerializeField, SetProperty("CellOuterRadius")]
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
        [SerializeField, SetProperty("PrefabCell")]
        private HexagonCell prefabCell;

        /// <summary>
        /// 六边形网格铺设方向
        /// </summary>
        [SerializeField, SetProperty("Direction")]
        private CellOrientation cellOrientation = CellOrientation.Horizontal;

        /// <summary>
        /// 绘制网格地图的模型
        /// </summary>
        private HexagonMesh hexMesh;

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

        #endregion


        #endregion

        #region 属性

        /// <summary>
        /// 网格的外径
        /// </summary>
        public float CellOuterRadius { get { return this.cellOuterRadius; } set { if (value <= 0) this.cellOuterRadius = 10f; else this.cellOuterRadius = value; } }

        public float CellInnerRadius { get { return this.cellOuterRadius * 0.866025404f; } }

        ///// <summary>
        ///// 宽度网格数
        ///// </summary>
        //public int Width
        //{
        //    get { return this.width; }
        //    set { if (value <= 0) this.width = 1; else this.width = value; }
        //}

        ///// <summary>
        ///// 高度网格数
        ///// </summary>
        //public int Height
        //{
        //    get { return this.height; }
        //    set { if (value <= 0) this.height = 1; else this.height = value; }
        //}

        /// <summary>
        /// 六边形网格铺设方向
        /// </summary>
        public CellOrientation Direction { get { return this.cellOrientation; } set { this.cellOrientation = value; } }

        public HexagonCell PrefabCell { get { return this.prefabCell; } set { this.prefabCell = value; } }

        #endregion

        #region 方法

        #region u3d反射方法

        private void Awake()
        {
            // this.hexMesh = this.GetComponentInChildren<HexagonMesh>();

            // 计算网格总数
            this.cellCountHeight = this.chunkCountHeight * HexagonGrid.chunkSizeHeight;
            this.cellCountWidth = this.chunkCountWidth * HexagonGrid.chunkSizeWidth;

            //
            this.CreateChunks();
            this.CreateCells();
        }

        // Use this for initialization
        private void Start()
        {
            // 绘制地图网格
            // this.hexMesh.Triangulate(this.cells);
        }

        // Update is called once per frame
        private void Update()
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
        private void CreateCells()
        {
            this.cells = new HexagonCell[this.cellCountHeight * this.cellCountWidth];
            int index = 0; // 单元格序号
            for (int heightIndex = 0; heightIndex < this.cellCountHeight; heightIndex++)
            {
                for (int widthIndex = 0; widthIndex < this.cellCountWidth; widthIndex++)
                {
                    this.CreateCell(widthIndex, heightIndex, index++, this.cellOuterRadius);
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
        public void CreateCell(int widthIndex, int heightIndex, int index, float outerRadius)
        {
            Vector3 position; // 单元格的中心位置
            position.y = 0f;
            // 根据网格方向不同，设置不同的网格排列方式
            switch (this.cellOrientation)
            {
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
            this.cells[index] = Instantiate<HexagonCell>(this.prefabCell);
            // 设置单元格的位置
            // this.cells[index].transform.SetParent(this.transform, false);
            this.cells[index].transform.localPosition = position;
            this.cells[index].Coordinates = HexagonCoordinates.FromOffsetCoordinates(widthIndex, heightIndex);
            this.cells[index].Initialize(this);
            this.cells[index].Color = this.defaultColor;
            // 设置标签
            Text label = Instantiate<Text>(cellLabelPrefab);
            this.cells[index].uiRect = label.rectTransform;
            // label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            // label.text = x.ToString() + "\n" + z.ToString();

            // this.cells[index].uiRect = label.rectTransform;
            // 设置相邻的单元格
            if (widthIndex > 0)
            {
                this.cells[index].SetNeighbor(HexagonDirection.W, this.cells[index - 1]);
            }
            if (heightIndex > 0)
            {
                if ((heightIndex & 1) == 0)
                {
                    this.cells[index].SetNeighbor(HexagonDirection.SE, this.cells[index - this.cellCountWidth]);
                    if (widthIndex > 0)
                    {
                        this.cells[index].SetNeighbor(HexagonDirection.SW, this.cells[index - this.cellCountWidth - 1]);
                    }
                }
                else
                {
                    this.cells[index].SetNeighbor(HexagonDirection.SW, this.cells[index - this.cellCountWidth]);
                    if (widthIndex < this.cellCountWidth - 1)
                    {
                        this.cells[index].SetNeighbor(HexagonDirection.SE, this.cells[index - this.cellCountWidth + 1]);
                    }
                }
            }
            //
            this.AddCellToChunk(widthIndex, heightIndex, this.cells[index]);
        }

        /// <summary>
        /// 单元格添加到指定的块
        /// </summary>
        /// <param name="widthIndex"></param>
        /// <param name="heightIndex"></param>
        /// <param name="cell"></param>
        private void AddCellToChunk(int widthIndex, int heightIndex, HexagonCell cell)
        {
            int chunkX = widthIndex / HexagonGrid.chunkSizeWidth;
            int chunkZ = heightIndex / HexagonGrid.chunkSizeHeight;
            HexagonGridChunk chunk = this.chunks[chunkX + chunkZ * this.chunkCountWidth];

            int localX = widthIndex - chunkX * HexagonGrid.chunkSizeWidth;
            int localZ = heightIndex - chunkZ * HexagonGrid.chunkSizeHeight;
            chunk.AddCell(localX + localZ * HexagonGrid.chunkSizeWidth, cell);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateChunks()
        {
            this.chunks = new HexagonGridChunk[this.chunkCountWidth * this.chunkCountHeight];

            for (int z = 0, i = 0; z < this.chunkCountHeight; z++)
            {
                for (int x = 0; x < this.chunkCountWidth; x++)
                {
                    HexagonGridChunk chunk = chunks[i++] = Instantiate<HexagonGridChunk>(chunkPrefab);
                    chunk.transform.SetParent(transform);
                }
            }
        }

        #region 处理点击

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="position"></param>
        //void TouchCell(Vector3 position)
        //{
        //    // 点击的位置
        //    position = transform.InverseTransformPoint(position);
        //    // 点击的位置转换成网格坐标
        //    HexagonCoordinates coordinates = HexagonCoordinates.FromPosition(position, this);
        //    Debug.Log("touched at " + coordinates);
        //}

        /// <summary>
        /// 获取点击位置的单元格
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public HexagonCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            HexagonCoordinates coordinates = HexagonCoordinates.FromPosition(position, this);
            // Debug.Log("touched at " + coordinates);
            int index = coordinates.X + coordinates.Z * this.cellCountWidth + coordinates.Z / 2;
            // Debug.Log(this.cells.Length);
            Debug.Log("touched at " + coordinates + "index:" + index);
            return this.cells[index];
            // return null;
        }

        #endregion

        ///// <summary>
        ///// 地图上每个单元格到指定单元格的距离
        ///// </summary>
        ///// <param name="cell"></param>
        //public void FindDistancesTo(HexagonCell cell)
        //{
        //    this.StopAllCoroutines();
        //    this.StartCoroutine(Search(cell));

        //    //for (int i = 0; i < cells.Length; i++)
        //    //{
        //    //    this.cells[i].Distance = cell.Coordinates.DistanceTo(cells[i].Coordinates);
        //    //}
        //}

        /// <summary>
        /// 查找两个单元格之间的路径
        /// </summary>
        /// <param name="fromCell"></param>
        /// <param name="toCell"></param>
        public void FindPath(HexagonCell fromCell, HexagonCell toCell)
        {
            StopAllCoroutines();
            StartCoroutine(Search(fromCell, toCell));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private IEnumerator Search(HexagonCell fromCell, HexagonCell toCell)
        {
            if (this.searchFrontier == null)
            {
                this.searchFrontier = new HexagonCellPriorityQueue();
            }
            else
            {
                this.searchFrontier.Clear();
            }

            for (int i = 0; i < cells.Length; i++)
            {
                //yield return delay;
                //cells[i].Distance = cell.Coordinates.DistanceTo(cells[i].Coordinates);
                cells[i].Distance = int.MaxValue;
                // 清除所有高亮显示
                cells[i].DisableHighlight();
            }
            // 高亮起点和终点
            fromCell.EnableHighlight(Color.blue);
            toCell.EnableHighlight(Color.red);
            WaitForSeconds delay = new WaitForSeconds(1 / 60f);
            // Queue<HexagonCell> frontier = new Queue<HexagonCell>();
            // List<HexagonCell> frontier = new List<HexagonCell>();
            fromCell.Distance = 0;
            // frontier.Enqueue(cell);
            // frontier.Add(fromCell);
            this.searchFrontier.Enqueue(fromCell);
            while (this.searchFrontier.Count > 0)
            {
                yield return delay;
                // HexagonCell current = frontier.Dequeue();
                // frontier.RemoveAt(0);
                //HexagonCell current = frontier[0];
                HexagonCell current = searchFrontier.Dequeue();
                // 找到目标单元格时结束循环
                if (current == toCell)
                {
                    current = current.PathFrom;
                    while (current != fromCell)
                    {
                        current.EnableHighlight(Color.white);
                        current = current.PathFrom;
                    }
                    break;
                }

                for (HexagonDirection d = HexagonDirection.NE; d <= HexagonDirection.NW; d++)
                {
                    HexagonCell neighbor = current.GetNeighbor(d);
                    if (neighbor == null)
                    {
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
                    int distance = current.Distance;
                    //if (current.HasRoadThroughEdge(d))
                    //{
                    //    distance += 1;
                    //}
                    //else
                    //{
                    //    distance += 10;
                    //}
                    distance += 1;

                    if (neighbor.Distance == int.MaxValue)
                    {
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        neighbor.SearchHeuristic = neighbor.Coordinates.DistanceTo(toCell.Coordinates);
                        // frontier.Add(neighbor);
                        this.searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                    }
                    // frontier.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                    // frontier.Sort((x, y) => x.SearchPriority.CompareTo(y.SearchPriority));
                }
            }

            //for (int i = 0; i < cells.Length; i++)
            //{
            //    yield return delay;
            //    this.cells[i].Distance =
            //    cell.Coordinates.DistanceTo(cells[i].Coordinates);
            //}
        }

        ///// <summary>
        ///// 根据坐标获取单元格
        ///// </summary>
        ///// <returns>The cell.</returns>
        ///// <param name="x">The x coordinate.</param>
        ///// <param name="y">The y coordinate.</param>
        ///// <param name="z">The z coordinate.</param>
        //public HexCell GetCell(int x, int y, int z)
        //{
        //    throw new System.NotImplementedException();
        //}

        #endregion
    }
}