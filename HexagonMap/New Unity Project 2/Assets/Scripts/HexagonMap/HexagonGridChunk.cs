using UnityEngine;

namespace HexagonMap
{
    /// <summary>
    /// 
    /// </summary>
    public class HexagonGridChunk : MonoBehaviour
    {
        private HexagonCell[] cells;
        // private HexagonMesh hexMesh;
        public HexagonMesh terrain;
        private Canvas gridCanvas;

        //private static Color color1 = new Color(1f, 0f, 0f);
        //private static Color color2 = new Color(0f, 1f, 0f);
        //private static Color color3 = new Color(0f, 0f, 1f);

        void Awake()
        {
            // hexMesh = GetComponentInChildren<HexagonMesh>();

            this.cells = new HexagonCell[HexagonGrid.chunkSizeWidth * HexagonGrid.chunkSizeHeight];

            this.gridCanvas = GetComponentInChildren<Canvas>();
        }

        void Start()
        {
            // this.hexMesh.Triangulate(cells);
            // this.terrain.Triangulate(cells);
        }

        private void LateUpdate()
        {
            this.Triangulate(cells);
            this.enabled = false;
        }

        public void AddCell(int index, HexagonCell cell)
        {
            this.cells[index] = cell;
            cell.chunk = this;
            cell.transform.SetParent(transform, false);
            cell.uiRect.SetParent(gridCanvas.transform, false);
        }

        /// <summary>
        /// 三角形化网格
        /// </summary>
        /// <param name="cells"></param>
        public void Triangulate(HexagonCell[] cells)
        {
            // 清楚旧数据
            this.terrain.Clear();
            // this.hexMesh.Clear();
            //vertices.Clear();
            //triangles.Clear();
            //colors.Clear();
            for (int i = 0; i < cells.Length; i++)
            {
                this.Triangulate(cells[i]);
            }
            this.terrain.Apply();
            //this.hexMesh.vertices = vertices.ToArray();
            //this.hexMesh.triangles = triangles.ToArray();
            //this.hexMesh.RecalculateNormals();
            //this.hexMesh.colors = colors.ToArray();

            // 设置网格碰撞体
            // this.meshCollider.sharedMesh = this.hexMesh;
        }

        /// <summary>
        /// 三角形化网格
        /// </summary>
        /// <param name="cell"></param>
        private void Triangulate(HexagonCell cell)
        {
            Vector3 center = cell.transform.localPosition;
            for (int i = 0; i < 6; i++)
            {
                this.terrain.AddTriangle(center, cell.Corners[i], cell.Corners[i + 1]);
                this.terrain.AddTriangleColor(cell.Color);
            }
        }

        public void Refresh()
        {
            this.Triangulate(cells);
            // this.terrain.Triangulate(cells);
            this.enabled = true;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="direction"></param>
        ///// <param name="cell"></param>
        ///// <param name="center"></param>
        ///// <param name="e"></param>
        //private void TriangulateWithoutRiver(HexagonDirection direction, HexagonCell cell, Vector3 center, EdgeVertices e)
        //{
        //    TriangulateEdgeFan(center, e, color1);
        //}
    }
}