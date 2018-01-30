using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using XC.SafetySupport.Scaffold.Bridge.Model;

namespace XC.CommonUtils
{
    /// <summary>
    /// 简单的集合工具
    /// </summary>
    public class GeometryUtil
    {
        /// <summary>
        /// 根据在和实体相连的一个线段取得该线段接触的面
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static Face GetFaceFromSolidByCurve(Curve curve, Solid solid)
        {
            Face FFF = null;
            int count = 0;
            foreach (Face face in solid.Faces)
            {
                var isOverlap = face.Intersect(curve);
                if (isOverlap == SetComparisonResult.Overlap)
                {
                    count++;
                    FFF = face;
                }
            }
            return FFF;
        }

        /// <summary>
        /// 遍历族实例的所有实体
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="options"></param>
        /// <param name="action"></param>
        public static IEnumerable<Solid> TraversingSolid(Element instance, Options options)
        {

            var fi = instance;
            var geoEle = fi.get_Geometry(options);
            foreach (GeometryObject geoObj in geoEle)
            {
                if (geoObj is Solid)
                {
                    Solid solid = geoObj as Solid;
                    if (solid.SurfaceArea == 0) continue;
                    yield return solid;
                }
                else if (geoObj is GeometryInstance)
                {
                    GeometryInstance gi = geoObj as GeometryInstance;
                    GeometryElement ge = gi.GetInstanceGeometry();
                    foreach (GeometryObject geoObj2 in ge)
                    {
                        if (geoObj2 is Solid)
                        {
                            Solid solid = geoObj2 as Solid;
                            if (solid.Faces.Size > 0)
                            {
                                yield return solid;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 遍历一个实体上某种面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static IEnumerable<T> TraversingSolidFace<T>(Solid solid) where T :class
        {
            foreach (Face face in solid.Faces)
            {
                if (face is T)
                {
                    yield return face as T;
                }
            }
        }
        /// <summary>
        /// 遍历一个面上的所有边
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static IEnumerable<Curve> TraversingFaceEdge(Face face)
        {
            for (int i = 0; i < face.EdgeLoops.Size; i++)
            {
                var edges = face.EdgeLoops.get_Item(i);
                for (int j = 0; j < edges.Size; j++)
                {
                    yield return edges.get_Item(j).AsCurve();
                }
            }
        }

        /// <summary>
        /// 得到简单的集合属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moduleLine"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T GetSimpleGeometry<T>(Element moduleLine, Options options) where T :class
        {
            var geo = moduleLine.get_Geometry(options);

            var g = geo.FirstOrDefault();
            if (g is T && g != null)
            {
                return g as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取某点的切线向量, 后一个点的坐标 - 前一个点的坐标
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static CurvePoint GetCurvePointAt(Curve curve, double length)
        {
            double normalized = length / curve.Length; // 必须归一化, 不然会有较大误差
            var transform = curve.ComputeDerivatives(normalized, true);
            var pInfo = CurvePoint.FromTransform(transform);
            pInfo.Distance = length;
            return pInfo;
        }
        
        /// <summary>
        /// 得到一个面的所有边
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static IEnumerable<Line> GetFaceEdgeLine(Face face)
        {
            foreach (EdgeArray edges in face.EdgeLoops)
            {
                foreach (Edge edge in edges)
                {
                    var c = edge.AsCurve() as Line;
                    yield return c;
                }
            }
        }
        
        /// <summary>
        /// 测试一个点是否在线上
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool IsPointOnCurve(Curve curve, XYZ point)
        {
            return curve.Project(point).XYZPoint.IsAlmostEqualTo(point);
        }
        
        /// <summary>
        /// 得到交点集合, 只针对一定有交点的两个边
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IEnumerable<XYZ> GetIntersectFormCurveAndCurve(Curve left, Curve right)
        {
            IntersectionResultArray arr;
            if (left.Intersect(right, out arr) == SetComparisonResult.Overlap)
            {
                var iterator = arr.ForwardIterator();
                for (int i = 0; i < arr.Size; i++)
                {
                    yield return arr.get_Item(i).XYZPoint;
                }
            }
            else
            {
                yield break;
            }
        }
    }
}
