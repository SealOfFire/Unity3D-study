using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexagonMap
{
    /// <summary>
    /// 单位
    /// </summary>
    public class HexagonUnit : MonoBehaviour
    {
        /// <summary>
        /// 单位所在的单元格
        /// </summary>
        private HexagonCell location;

        /// <summary>
        /// T单位的朝向
        /// </summary>
        private float orientation;

        /// <summary>
        /// 单位所在的单元格
        /// </summary>
        public HexagonCell Location
        {
            get
            {
                return this.location;
            }
            set
            {
                if (this.location)
                {
                    this.location.Unit = null;
                }
                this.location = value;
                value.Unit = this;
                this.transform.localPosition = value.Position;
            }
        }

        /// <summary>
        /// T单位的朝向
        /// </summary>
        public float Orientation
        {
            get { return this.orientation; }
            set
            {
                this.orientation = value;
                this.transform.localRotation = Quaternion.Euler(0f, value, 0f);
            }
        }

        /// <summary>
        ///  移动路径
        /// </summary>
        private List<HexagonCell> pathToTravel;

        /// <summary>
        /// 
        /// </summary>
        private const float travelSpeed = 4f;

        /// <summary>
        /// 旋转的速度
        /// </summary>
        private const float rotationSpeed = 180f;

        /// <summary>
        /// 验证位置
        /// </summary>
        public void ValidateLocation()
        {
            this.transform.localPosition = this.location.Position;
        }

        /// <summary>
        /// 移除单位
        /// </summary>
        public void Die()
        {
            this.location.Unit = null;
            Destroy(this.gameObject);
        }

        /// <summary>
        /// 目的地单元格是否可以进入
        /// </summary>
        /// <returns><c>true</c> if this instance is valid destination the specified cell; otherwise, <c>false</c>.</returns>
        /// <param name="cell">Cell.</param>
        public bool IsValidDestination(HexagonCell cell)
        {
            // 目标单元格不在水底
            // return !cell.IsUnderwater;
            // 目标单元格内没有其他单位
            return !cell.Unit;
        }

        /// <summary>
        /// 沿着路径移动
        /// </summary>
        /// <param name="path">Path.</param>
        public void Travel(List<HexagonCell> path)
        {
            // 直接将单位移动到目标单元格
            this.Location = path[path.Count - 1];
            //
            this.pathToTravel = path;
            StopAllCoroutines();
            StartCoroutine(this.TravelPath());
        }

        /// <summary>
        /// 旋转朝向
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private IEnumerator LookAt(Vector3 point)
        {
            point.y = transform.localPosition.y;
            Quaternion fromRotation = transform.localRotation;
            Quaternion toRotation =
            Quaternion.LookRotation(point - transform.localPosition);
            float angle = Quaternion.Angle(fromRotation, toRotation);

            if (angle > 0f)
            {
                float speed = rotationSpeed / angle;
                for (float t = Time.deltaTime; t < 1f; t += Time.deltaTime)
                {
                    transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, t);
                    yield return null;
                }
            }
            transform.LookAt(point);
            orientation = transform.localRotation.eulerAngles.y;
        }

        /// <summary>
        ///  表现移动动画的协程
        /// </summary>
        /// <returns>The path.</returns>
        private IEnumerator TravelPath()
        {
            Vector3 a, b, c = this.pathToTravel[0].Position;
            transform.localPosition = c;
            yield return LookAt(pathToTravel[1].Position);
            float t = Time.deltaTime * travelSpeed;
            for (int i = 1; i < this.pathToTravel.Count; i++)
            {
                //Vector3 a = this.pathToTravel[i - 1].Position;
                //Vector3 b = this.pathToTravel[i].Position;
                a = c;
                b = (this.pathToTravel[i - 1].Position + this.pathToTravel[i].Position) * 0.5f;
                c = (b + pathToTravel[i].Position) * 0.5f;
                for (; t < 1f; t += Time.deltaTime * travelSpeed)
                {
                    this.transform.localPosition = Bezier.GetPoint(a, b, c, t);
                    Vector3 d = Bezier.GetDerivative(a, b, c, t);
                    d.y = 0f; // TODO
                    transform.localRotation = Quaternion.LookRotation(d);
                    yield return null;
                }
                t -= 1f;
            }

            a = c;
            b = this.pathToTravel[pathToTravel.Count - 1].Position;
            c = b;
            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                this.transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f; // TODO
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            this.transform.localPosition = location.Position;
            this.orientation = transform.localRotation.eulerAngles.y;

            ListPool<HexagonCell>.Add(pathToTravel);
            pathToTravel = null;
        }

        private void OnDrawGizmos()
        {
            if (pathToTravel == null || pathToTravel.Count == 0)
            {
                return;
            }
            Vector3 a, b, c = this.pathToTravel[0].Position;
            for (int i = 1; i < this.pathToTravel.Count; i++)
            {
                //Vector3 a = this.pathToTravel[i - 1].Position;
                //Vector3 b = this.pathToTravel[i].Position;
                a = c;
                b = (this.pathToTravel[i - 1].Position + this.pathToTravel[i].Position) * 0.5f;
                c = (b + pathToTravel[i].Position) * 0.5f;
                for (float t = 0f; t < 1f; t += 0.1f)
                {
                    Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
                    // Gizmos.DrawSphere(Vector3.Lerp(a, b, t), 2f);
                }
            }

            a = c;
            b = this.pathToTravel[pathToTravel.Count - 1].Position;
            c = b;
            for (float t = 0f; t < 1f; t += 0.1f)
            {
                Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
                // Gizmos.DrawSphere(Vector3.Lerp(a, b, t), 2f);
            }
        }

        private void OnEnable()
        {
            if (this.location)
            {
                this.transform.localPosition = location.Position;
            }
        }
    }
}