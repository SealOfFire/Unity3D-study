using UnityEngine;
using UnityEngine.UI;

namespace HexagonMap
{
    /// <summary>
    /// 六边形单元格
    /// </summary>
    public class HexagonCell : MonoBehaviour
    {
        public RectTransform uiRect;

        private HexagonGrid grid;

        private Color color;

        /// <summary>
        /// 相邻的单元格
        /// </summary>
        [SerializeField]
        private HexagonCell[] neighbors = new HexagonCell[6];

        private Vector3[] corners = new Vector3[7];
        private HexagonCoordinates coordinates;

        public HexagonGridChunk chunk;
        public Vector3[] Corners { get { return this.corners; } }

        public HexagonCoordinates Coordinates { get { return this.coordinates; } set { this.coordinates = value; } }

        public Color Color { get { return this.color; } set { this.color = value; } }

        /// <summary>
        /// 路径上的下一个单元格位置
        /// </summary>
        public HexagonCell PathFrom { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SearchHeuristic { get; set; }

        public HexagonCell NextWithSamePriority { get; set; }

        /// <summary>
        /// 检索优先级
        /// </summary>
        public int SearchPriority
        {
            get
            {
                return this.distance + this.SearchHeuristic;
            }
        }

        /// <summary>
        /// 距离
        /// </summary>
        private int distance;

        /// <summary>
        /// 距离
        /// </summary>
        public int Distance
        {
            get
            {
                return this.distance;
            }
            set
            {
                distance = value;
                // UpdateDistanceLabel();
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Initialize(HexagonGrid grid)
        {
            this.grid = grid;

            // 初始化六个定点
            this.corners[0] = new Vector3(0f, 0f, grid.CellOuterRadius) + this.transform.localPosition;
            this.corners[1] = new Vector3(grid.CellInnerRadius, 0f, 0.5f * grid.CellOuterRadius) + this.transform.localPosition;
            this.corners[2] = new Vector3(grid.CellInnerRadius, 0f, -0.5f * grid.CellOuterRadius) + this.transform.localPosition;
            this.corners[3] = new Vector3(0f, 0f, -grid.CellOuterRadius) + this.transform.localPosition;
            this.corners[4] = new Vector3(-grid.CellInnerRadius, 0f, -0.5f * grid.CellOuterRadius) + this.transform.localPosition;
            this.corners[5] = new Vector3(-grid.CellInnerRadius, 0f, 0.5f * grid.CellOuterRadius) + this.transform.localPosition;
            this.corners[6] = new Vector3(0f, 0f, grid.CellOuterRadius) + this.transform.localPosition;
        }

        private void Refresh()
        {
            if (this.chunk)
            {
                this.chunk.Refresh();
                for (int i = 0; i < neighbors.Length; i++)
                {
                    HexagonCell neighbor = neighbors[i];
                    if (neighbor != null && neighbor.chunk != chunk)
                    {
                        neighbor.chunk.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定方向上的相邻单元格
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public HexagonCell GetNeighbor(HexagonDirection direction)
        {
            return this.neighbors[(int)direction];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="cell"></param>
        public void SetNeighbor(HexagonDirection direction, HexagonCell cell)
        {
            this.neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }

        /// <summary>
        /// 禁用高亮显示
        /// </summary>
        public void DisableHighlight()
        {
            Image highlight = uiRect.GetChild(0).GetComponent<Image>();
            highlight.enabled = false;
        }

        /// <summary>
        /// 启用高亮显示
        /// </summary>
        public void EnableHighlight(Color color)
        {
            Image highlight = uiRect.GetChild(0).GetComponent<Image>();
            highlight.color = color;
            highlight.enabled = true;
        }
    }
}