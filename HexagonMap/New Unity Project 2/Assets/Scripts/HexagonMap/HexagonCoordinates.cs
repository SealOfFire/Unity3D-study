using System;
using UnityEditor;
using UnityEngine;

namespace HexagonMap
{
    /// <summary>
    /// 六边形网格坐标系统
    /// </summary>
    [Serializable]
    public class HexagonCoordinates// : PropertyDrawer
    {
        [SerializeField]
        private int x, z;

        public int X { get { return x; } }

        public int Z { get { return z; } }

        public int Y { get { return -x - z; } }

        public HexagonCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static HexagonCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexagonCoordinates(x - z / 2, z);
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", this.x, this.z, this.Y);
        }

        public string ToStringOnSeparateLines()
        {
            return string.Format("({0}\n{1}\n{2})", this.x, this.z, this.Y);
        }

        /// <summary>
        /// 坐标转换成网格坐标
        /// </summary>
        /// <param name="position"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static HexagonCoordinates FromPosition(Vector3 position, HexagonGrid grid)
        {
            float x = position.x / (grid.CellInnerRadius * 2f);
            float y = -x;
            float offset = position.z / (grid.CellOuterRadius * 3f);
            x -= offset;
            y -= offset;
            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x - y);
            if (iX + iY + iZ != 0)
            {
                Debug.LogWarning("rounding error!");
                float dX = Mathf.Abs(x - iX);
                float dY = Mathf.Abs(y - iY);
                float dZ = Mathf.Abs(-x - y - iZ);
                if (dX > dY && dX > dZ)
                {
                    iX = -iY - iZ;
                }
                else if (dZ > dY)
                {
                    iZ = -iX - iY;
                }
            }
            return new HexagonCoordinates(iX, iZ);
        }

        //[CustomPropertyDrawer(typeof(HexCoordinates))]
        //public class HexCoordinatesDrawer : PropertyDrawer
        //{
        //}

        //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        //{
        //    HexCoordinates coordinates = new HexCoordinates(property.FindPropertyRelative("x").intValue, property.FindPropertyRelative("z").intValue);
        //    position = EditorGUI.PrefixLabel(position, label);
        //    GUI.Label(position, coordinates.ToString());
        //}

        /// <summary>
        /// 指定单元格到当前单元格坐标距离
        /// </summary>
        /// <param name="cell"></param>
        public int DistanceTo(HexagonCoordinates other)
        {
            // 三个坐标维度上的距离
            int x = this.x < other.x ? other.x - this.x : this.x - other.x;
            int y = this.Y < other.Y ? other.Y - this.Y : this.Y - other.Y;
            int z = this.z < other.z ? other.z - this.z : this.z - other.z;
            return (x + y + x) / 2;
        }
    }
}