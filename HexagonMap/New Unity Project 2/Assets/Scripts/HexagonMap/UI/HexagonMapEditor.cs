using UnityEngine;

namespace HexagonMap.UI
{
    public class HexagonMapEditor : MonoBehaviour
    {
        public HexagonGrid hexagonGrid;
        public Material terrainMaterial;
        // private Color activeColor;
        private HexagonCell previousCell, searchToCell, searchFromCell;

        /// <summary>
        /// 显示网格边线
        /// </summary>
        /// <param name="visible"></param>
        public void ShowGrid(bool visible)
        {
            if (visible)
            {
                this.terrainMaterial.EnableKeyword("GRID_ON");
            }
            else
            {
                this.terrainMaterial.DisableKeyword("GRID_ON");
            }
        }

        void Awake()
        {
            this.ShowGrid(true);
        }

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                HandleInput();
            }
            else
            {
                previousCell = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleInput()
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                HexagonCell currentCell = hexagonGrid.GetCell(hit.point);
                // this.hexGrid.TouchCell(hit.point);
                if (Input.GetKey(KeyCode.LeftShift) && this.searchToCell != currentCell)
                {
                    // 清除旧的检索路径起点
                    if (this.searchFromCell)
                    {
                        this.searchFromCell.DisableHighlight();
                    }
                    // 当前选中的单元格设置为新的检索起点
                    this.searchFromCell = currentCell;
                    this.searchFromCell.EnableHighlight(Color.blue);
                    if (searchToCell)
                    {
                        this.hexagonGrid.FindPath(this.searchFromCell, this.searchToCell, 24);
                    }
                }
                else if (searchFromCell && searchFromCell != currentCell)
                {
                    searchToCell = currentCell;
                    this.hexagonGrid.FindPath(searchFromCell, currentCell, 24);
                }
                previousCell = currentCell;
            }
            else
            {
                previousCell = null;
            }
        }
    }
}