using UnityEngine;

namespace HexagonMap
{
    /// <summary>
    /// 相机控制
    /// </summary>
    public class HexagonMapCamera : MonoBehaviour
    {
        /// <summary>
        /// 旋转接头
        /// </summary>
        private Transform swivel;

        /// <summary>
        /// 相机架
        /// </summary>
        private Transform stick;

        /// <summary>
        /// 焦距 0~1:从远到近
        /// </summary>
        private float zoom = 1f;

        /// <summary>
        /// 相机的最大最小焦距位置
        /// </summary>
        public float stickMinZoom, stickMaxZoom;

        /// <summary>
        /// 旋转接头的最大和最小变量
        /// </summary>
        public float swivelMinZoom, swivelMaxZoom;

        /// <summary>
        /// 相机的移动速度 根据放大缩小不同移动速度不同
        /// </summary>
        public float moveSpeedMinZoom, moveSpeedMaxZoom;

        /// <summary>
        /// 摄影机旋转速度
        /// </summary>
        public float rotationSpeed;

        /// <summary>
        /// 旋转的角度
        /// </summary>
        private float rotationAngle;

        /// <summary>
        /// 
        /// </summary>
        public HexagonGrid grid;

        #region u3d反射方法

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            this.swivel = transform.GetChild(0);
            this.stick = swivel.GetChild(0);
        }

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            // 鼠标滚轮调整焦距
            float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            if (zoomDelta != 0f)
            {
                this.AdjustZoom(zoomDelta);
            }

            // 旋转摄影机
            float rotationDelta = Input.GetAxis("Rotation");
            if (rotationDelta != 0f)
            {
                this.AdjustRotation(rotationDelta);
            }

            // 移动相机
            float xDelta = Input.GetAxis("Horizontal");
            float zDelta = Input.GetAxis("Vertical");
            if (xDelta != 0f || zDelta != 0f)
            {
                this.AdjustPosition(xDelta, zDelta);
            }
        }

        #endregion

        /// <summary>
        /// 调整焦距
        /// </summary>
        /// <param name="delta">鼠标滚轮的调整滚动程度</param>
        private void AdjustZoom(float delta)
        {
            this.zoom = Mathf.Clamp01(zoom + delta);
            // 焦距位置
            float distance = Mathf.Lerp(this.stickMinZoom, this.stickMaxZoom, this.zoom);
            stick.localPosition = new Vector3(0f, 0f, distance);
            // 旋转镜头
            float angle = Mathf.Lerp(this.swivelMinZoom, this.swivelMaxZoom, this.zoom);
            swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
            // TODO 
            /*
             你可以通过调整鼠标滚轮的灵敏度来调整焦距的变化速度。可以在Edit/Project Settings/Input中找到。例如，将它从默认的0.1降为0.025导致更慢、更平滑的调焦过程。
             */
        }

        /// <summary>
        /// 移动相机
        /// </summary>
        /// <param name="xDelta">x轴的移动变化</param>
        /// <param name="zDelta">z周的移动变化</param>
        void AdjustPosition(float xDelta, float zDelta)
        {
            Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
            float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
            float distance =
            Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
            damping * Time.deltaTime;

            Vector3 position = transform.localPosition;
            position += direction * distance;
            transform.localPosition = position;
            // 防止镜头移动到网格外
            transform.localPosition = ClampPosition(position);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector3 ClampPosition(Vector3 position)
        {
            float xMax = (grid.chunkCountWidth * HexagonGrid.chunkSizeWidth - 0.5f) * (2f * this.grid.CellInnerRadius);
            position.x = Mathf.Clamp(position.x, 0f, xMax);
            float zMax = (grid.chunkCountHeight * HexagonGrid.chunkSizeHeight - 1) * (1.5f * this.grid.CellOuterRadius);
            position.z = Mathf.Clamp(position.z, 0f, zMax);

            //float xMax = (this.grid.Width - 0.5f) * (2f * this.grid.CellInnerRadius);
            //position.x = Mathf.Clamp(position.x, 0f, xMax);
            //float zMax = (this.grid.Height - 1) * (1.5f * this.grid.CellOuterRadius);
            //position.z = Mathf.Clamp(position.z, 0f, zMax);

            return position;
        }

        /// <summary>
        /// 旋转摄影机
        /// </summary>
        /// <param name="delta"></param>
        private void AdjustRotation(float delta)
        {
            rotationAngle += delta * rotationSpeed * Time.deltaTime;
            if (rotationAngle < 0f)
            {
                rotationAngle += 360f;
            }
            else if (rotationAngle >= 360f)
            {
                rotationAngle -= 360f;
            }
            transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
        }
    }
}