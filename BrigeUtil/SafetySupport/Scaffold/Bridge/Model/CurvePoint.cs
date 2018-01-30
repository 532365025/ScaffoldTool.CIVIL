using Autodesk.Revit.DB;

namespace XC.SafetySupport.Scaffold.Bridge.Model
{
    /// <summary>
    /// 用来存储在桥中心线上的某个点的属性
    /// </summary>
    public class CurvePoint
    {
        /// <summary>
        /// 该点切线向量
        /// </summary>
        public XYZ Tangent { set; get; }
        /// <summary>
        /// 左偏移向量
        /// </summary>
        public XYZ NormalLeft { set; get; }
        /// <summary>
        /// 右偏移向量
        /// </summary>
        public XYZ NormalRight { set; get; }
        /// <summary>
        /// 该点位置
        /// </summary>
        public XYZ Location { set; get; }
        /// <summary>
        /// 冲桥中心线起到该点的曲线距离
        /// </summary>
        public double Distance { set; get; }

        /// <summary>
        /// 从一个Transform中提取属性, 属性对应详见curve.ComputeDerivatives
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static CurvePoint FromTransform(Transform transform)
        {
            var p = new CurvePoint();
            p.Location = transform.Origin;
            p.Tangent = transform.BasisX.Normalize();
            p.NormalLeft = XYZ.BasisZ.CrossProduct(transform.BasisX).Normalize();
            p.NormalRight = -p.NormalLeft;

            return p;
        }
    }
}
