using System;
using System.Collections.Generic;
using UnityEngine;

namespace HexagonMap
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexagonMesh : MonoBehaviour
    {
        #region 成员

        /// <summary>
        /// 绘制六边形的形状
        /// </summary>
        private Mesh hexMesh;

        /// <summary>
        /// 单元格颜色
        /// </summary>
        [NonSerialized]
        private List<Color> colors;

        /// <summary>
        /// 绘制六边形的定点列表
        /// </summary>
        [NonSerialized]
        private List<Vector3> vertices;

        /// <summary>
        /// 绘制六边形的三角形列表
        /// </summary>
        [NonSerialized]
        private List<int> triangles;

        /// <summary>
        /// 纹理坐标
        /// </summary>
        [NonSerialized]
        private List<Vector2> uvs;

        [NonSerialized]
        private List<Vector2> uv2s;

        [NonSerialized]
        private List<Vector3> terrainTypes;

        /// <summary>
        /// 用于点击的碰撞检测
        /// </summary>
        private MeshCollider meshCollider;

        /// <summary>
        /// 是否使用碰撞
        /// </summary>
        public bool useCollider;

        /// <summary>
        /// 是否可以设置颜色
        /// </summary>
        public bool useColors;

        /// <summary>
        /// 是否使用纹理坐标
        /// </summary>
        public bool useUVCoordinates;

        public bool useUV2Coordinates;

        public bool useTerrainTypes;

        #endregion

        #region 方法



        #region u3d反射方法

        void Awake()
        {
            // 添加一个碰撞体到网格上
            if (this.useCollider)
            {
                this.meshCollider = gameObject.AddComponent<MeshCollider>();
            }

            // 初始化绘制
            this.hexMesh = new Mesh();
            this.GetComponent<MeshFilter>().mesh = this.hexMesh;
            hexMesh.name = "Hexagon Mesh";
            //this.vertices = new List<Vector3>();
            //this.triangles = new List<int>();


        }

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.gray;
        //    DrawMesh();
        //}

        //void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = Color.green;
        //    DrawMesh();
        //}

        #endregion

        ///// <summary>
        ///// 三角形化网格
        ///// </summary>
        ///// <param name="cells"></param>
        //public void Triangulate(HexagonCell[] cells)
        //{
        //    // 清楚旧数据
        //    this.hexMesh.Clear();
        //    vertices.Clear();
        //    triangles.Clear();
        //    colors.Clear();
        //    for (int i = 0; i < cells.Length; i++)
        //    {
        //        this.Triangulate(cells[i]);
        //    }
        //    this.hexMesh.vertices = vertices.ToArray();
        //    this.hexMesh.triangles = triangles.ToArray();
        //    this.hexMesh.RecalculateNormals();
        //    this.hexMesh.colors = colors.ToArray();

        //    // 设置网格碰撞体
        //    this.meshCollider.sharedMesh = this.hexMesh;
        //}

        ///// <summary>
        ///// 三角形化网格
        ///// </summary>
        ///// <param name="cell"></param>
        //private void Triangulate(HexagonCell cell)
        //{
        //    Vector3 center = cell.transform.localPosition;
        //    for (int i = 0; i < 6; i++)
        //    {
        //        AddTriangle(center, cell.Corners[i], cell.Corners[i + 1]);
        //        AddTriangleColor(cell.Color);
        //    }
        //}

        /// <summary>
        /// 添加一个三角形
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        /// <summary>
        /// 为三角形添加颜色
        /// </summary>
        /// <param name="color"></param>
        public void AddTriangleColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        //public float sideLength = 2;
        //public float angleDegree = 100;

        //private void DrawMesh()
        //{
        //    Mesh mesh = Create(sideLength, angleDegree);
        //    int[] tris = mesh.triangles;
        //    Gizmos.DrawLine(mesh.vertices[tris[0]], mesh.vertices[tris[1]]);
        //    Gizmos.DrawLine(mesh.vertices[tris[0]], mesh.vertices[tris[2]]);
        //    Gizmos.DrawLine(mesh.vertices[tris[1]], mesh.vertices[tris[2]]);
        //}

        //private Mesh Create(float sideLength, float angleDegree)
        //{
        //    Mesh mesh = new Mesh();
        //    Vector3[] vertices = new Vector3[3];

        //    float angle = Mathf.Deg2Rad * angleDegree;
        //    float halfAngle = angle / 2;
        //    vertices[0] = Vector3.zero;
        //    float cosA = Mathf.Cos(halfAngle);
        //    float sinA = Mathf.Sin(halfAngle);
        //    vertices[1] = new Vector3(cosA * sideLength, 0, sinA * sideLength);
        //    vertices[2] = new Vector3(cosA * sideLength, 0, -sinA * sideLength);

        //    int[] triangles = new int[3];
        //    triangles[0] = 0;
        //    triangles[1] = 1;
        //    triangles[2] = 2;

        //    mesh.vertices = vertices;
        //    mesh.triangles = triangles;

        //    Vector2[] uvs = new Vector2[vertices.Length];
        //    for (int i = 0; i < uvs.Length; i++)
        //    {
        //        uvs[i] = Vector2.zero;
        //    }
        //    mesh.uv = uvs;

        //    return mesh;
        //}

        public void Clear()
        {
            //this.hexMesh.Clear();
            //vertices.Clear();
            //colors.Clear();
            //triangles.Clear();

            this.hexMesh.Clear();
            this.vertices = ListPool<Vector3>.Get();
            if (this.useColors)
            {
                this.colors = ListPool<Color>.Get();
            }
            if (this.useUVCoordinates)
            {
                this.uvs = ListPool<Vector2>.Get();
            }
            if (useUV2Coordinates)
            {
                this.uv2s = ListPool<Vector2>.Get();
            }
            if (this.useTerrainTypes)
            {
                this.terrainTypes = ListPool<Vector3>.Get();
            }
            this.triangles = ListPool<int>.Get();
        }

        public void Apply()
        {
            //this.hexMesh.SetVertices(vertices);
            //this.hexMesh.SetColors(colors);
            //this.hexMesh.SetTriangles(triangles, 0);
            //this.hexMesh.RecalculateNormals();
            //// 设置网格碰撞体
            //this.meshCollider.sharedMesh = this.hexMesh;

            this.hexMesh.SetVertices(vertices);
            ListPool<Vector3>.Add(vertices);
            if (this.useColors)
            {
                this.hexMesh.SetColors(this.colors);
                ListPool<Color>.Add(this.colors);
            }
            if (this.useUVCoordinates)
            {
                this.hexMesh.SetUVs(0, this.uvs);
                ListPool<Vector2>.Add(this.uvs);
            }
            if (useUV2Coordinates)
            {
                hexMesh.SetUVs(1, this.uv2s);
                ListPool<Vector2>.Add(this.uv2s);
            }
            if (useTerrainTypes)
            {
                this.hexMesh.SetUVs(2, terrainTypes);
                ListPool<Vector3>.Add(terrainTypes);
            }
            this.hexMesh.SetTriangles(triangles, 0);
            ListPool<int>.Add(this.triangles);
            this.hexMesh.RecalculateNormals();

            // 设置网格碰撞体
            if (this.useCollider)
            {
                this.meshCollider.sharedMesh = this.hexMesh;
            }
        }

        public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            this.uvs.Add(uv1);
            this.uvs.Add(uv2);
            this.uvs.Add(uv3);
        }

        public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
        {
            this.uvs.Add(uv1);
            this.uvs.Add(uv2);
            this.uvs.Add(uv3);
            this.uvs.Add(uv4);
        }

        public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
        {
            this.uvs.Add(new Vector2(uMin, vMin));
            this.uvs.Add(new Vector2(uMax, vMin));
            this.uvs.Add(new Vector2(uMin, vMax));
            this.uvs.Add(new Vector2(uMax, vMax));
        }

        public void AddTriangleTerrainTypes(Vector3 types)
        {
            this.terrainTypes.Add(types);
            this.terrainTypes.Add(types);
            this.terrainTypes.Add(types);
        }

        public void AddQuadTerrainTypes(Vector3 types)
        {
            this.terrainTypes.Add(types);
            this.terrainTypes.Add(types);
            this.terrainTypes.Add(types);
            this.terrainTypes.Add(types);
        }
        #endregion

    }
}