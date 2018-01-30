using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XC.CommonUtils;

namespace XC.SafetySupport.Scaffold.Bridge.Model.RevitModel
{
    /// <summary>
    /// 桥梁模型信息, 模型为Revit模型, 传入单位确保为Revit单位
    /// </summary>
    partial class BridgeInfoRevitModel
    {
        public static BridgeInfoRevitModel Create(List<Element> body, List<Element> pier, Element midline, double width)
        {
            BridgeInfoRevitModel model = new BridgeInfoRevitModel()
            {
                Body = body,
                Pier = pier,
                Midline = midline,
                Width = width,
                BodySolid = new List<Solid>(),
                BodyFace = new List<Face>(),
            };

            foreach (var item in body)
            {
                model.BodySolid.AddRange(GeometryUtil.TraversingSolid(item, options));
            }

            foreach (var solid in model.BodySolid)
            {
                foreach (Face face in solid.Faces)
                {
                    model.BodyFace.Add(face);
                }
            }

            model.MidCurve = GeometryUtil.GetSimpleGeometry<NurbSpline>(midline, options);

            return model;
        }

        /// <summary>
        /// 桥梁的Revit模型
        /// </summary>
        public List<Element> Body { private set; get; }
        /// <summary>
        /// 桥墩Revit模型
        /// </summary>
        public List<Element> Pier { private set; get; }
        /// <summary>
        /// 桥中心线Revit模型
        /// </summary>
        public Element Midline { private set; get; }
        /// <summary>
        /// 桥面宽度
        /// </summary>
        public double Width { private set; get; }
        /// <summary>
        /// 桥中心线
        /// </summary>
        public NurbSpline MidCurve { private set; get; }
        /// <summary>
        /// 桥身Revit实体
        /// </summary>
        public List<Solid> BodySolid { private set; get; }
        /// <summary>
        /// 桥身Revit实体面
        /// </summary>
        public List<Face> BodyFace { private set; get; }

        /// <summary>
        /// 桥身, 桥墩, 桥中线
        /// </summary>
        /// <param name="body"></param>
        /// <param name="pier"></param>
        /// <param name="midline"></param>
        /// <returns></returns>


        private BridgeInfoRevitModel() { } // 禁用构造方法
        private static readonly Options options = new Options()
        {
            DetailLevel = ViewDetailLevel.Fine,
            ComputeReferences = true,
            IncludeNonVisibleObjects = true
        };

    }

    partial class BridgeInfoRevitModel
    {
        /// <summary>
        /// 求某一点的厚度, 根据一个点做一个垂直线, 得到与桥实体相交的所有内部线段, 并求和
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public double GetBridgeThicknessAt(XYZ location)
        {
            double thickness = 0.0;
            var interoption = new SolidCurveIntersectionOptions() { ResultType = SolidCurveIntersectionMode.CurveSegmentsInside };
            var testLine = GetTestLineAt(location);
            foreach (var solid in this.BodySolid)
            {
                var intersection = solid.IntersectWithCurve(testLine, interoption);
                for (int segment = 0; segment <= intersection.SegmentCount - 1; segment++)
                {
                    Curve curveInside = intersection.GetCurveSegment(segment);
                    thickness += curveInside.Length;
                }
            }
            return thickness;
        }
        /// <summary>
        /// 求桥底某一点的特性: 法向量..厚度
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public BridgeBottomPoint GetBridgeBottomPointAt(XYZ location)
        {
            var interoption = new SolidCurveIntersectionOptions() { ResultType = SolidCurveIntersectionMode.CurveSegmentsInside };
            var outeroption = new SolidCurveIntersectionOptions() { ResultType = SolidCurveIntersectionMode.CurveSegmentsOutside };
            var testLine = GetTestLineAt(location);
            var outterList = new List<KeyValuePair<Line, Solid>>();

            foreach (var solid in this.BodySolid)
            {
                var intersection = solid.IntersectWithCurve(testLine, interoption); // 内部线用于检测是否有交点
                var outersection = solid.IntersectWithCurve(testLine, outeroption); // 外部线用于求截面特性
                if (intersection.SegmentCount == 0)
                {
                    continue;
                }
                for (int segment = 0; segment < outersection.SegmentCount; segment++)
                {
                    var curve = outersection.GetCurveSegment(segment) as Line;
                    outterList.Add(new KeyValuePair<Line, Solid>(curve, solid));
                }
            }
            if (outterList.Count == 0)
            {
                return null;
            }
            double outLength = outterList.Sum(m => m.Key.Length == testLine.Length ? 0 : m.Key.Length);
            var thickness = testLine.Length - outLength;// 内部线长度 = 总长 - 外部线长度

            var lowKV = outterList.OrderBy(m => m.Key.GetEndPoint(0).Z).First();// 得到最低的外部线段, 测试与其相交的面
            var curveInLow = lowKV.Key;
            var face = GeometryUtil.GetFaceFromSolidByCurve(lowKV.Key, lowKV.Value);
            var locationOnface = curveInLow.GetEndPoint(0).Z > curveInLow.GetEndPoint(1).Z ? curveInLow.GetEndPoint(0) : curveInLow.GetEndPoint(1);
            var faceNormal = face.ComputeNormal(new UV(locationOnface.X, locationOnface.Y));

            var point = new BridgeBottomPoint()
            {
                FaceNomal = faceNormal,
                Point = locationOnface,
                Face = face,
                Thickness = thickness,
                OuterLine = outterList.Select(m => m.Key).ToList(),
            };

            return point;
        }

        /// <summary>
        /// 获取中线上某一点的截面数据
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public BridgeBottom GetBridgeBottomAt(double length, double clear)
        {
            var pointOnCurve = GeometryUtil.GetCurvePointAt(MidCurve, length);

            var left = GetBridgeBottom(pointOnCurve.Location, pointOnCurve.Tangent, pointOnCurve.NormalLeft, clear);
            var right = GetBridgeBottom(pointOnCurve.Location, pointOnCurve.Tangent, pointOnCurve.NormalRight, clear);
            var bottom = new BridgeBottom(left, right)
            {
                PointOnMidline = pointOnCurve,
            };
            bottom.Init();
            return bottom;
        }

        /// <summary>
        /// 通过一个点获取一个截面的底边信息, 冲中心线向两边每隔一段距离放探测线, 根据探测结果获得面和线
        /// </summary>
        /// <param name="startLocation">桥的中线线上一点</param>
        /// <param name="crossSectionNormal">桥的中心线切线方向</param>
        /// <param name="awayFromStart">桥边远离桥中线的方向</param>
        /// <returns></returns>
        public List<BridgeBottomPoint> GetBridgeBottom(XYZ startLocation, XYZ crossSectionNormal, XYZ awayFromStart, double clear)
        {
            // 确定步长并得到每步在底部面上的点
            double distance = clear;
            int detectorNum = (int)(Width / 2 / distance) + 1;
            var mover = new LocationMoveTool(awayFromStart, distance);
            var bottoms = new List<BridgeBottomPoint>(detectorNum);
            for (int i = 0; i < detectorNum; i++)
            {
                var location = mover.Forward(startLocation, i);
                var bottomPoint = GetBridgeBottomPointAt(location);
                if (bottomPoint == null)
                {
                    continue;
                }
                bottomPoint.OffsetLengthToMidline = location.DistanceTo(startLocation);
                bottomPoint.OffsetVectorToMidline = awayFromStart;
                bottoms.Add(bottomPoint);
            }
            return bottoms;
        }

        private static Line GetTestLineAt(XYZ location, double heighest = 1000, double lowest = 0)
        {
            var ps = new XYZ(location.X, location.Y, lowest);
            var pe = new XYZ(location.X, location.Y, heighest);
            return Line.CreateBound(ps, pe);
        }
    }

    partial class BridgeInfoRevitModel
    {
        /// <summary>
        /// 获取桥梁的外轮廓
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<BridgeCurveType, Curve>> GetBridgeBodyOutline()
        {
            var start = GeometryUtil.GetCurvePointAt(MidCurve, 0);
            var end = GeometryUtil.GetCurvePointAt(MidCurve, MidCurve.Length);

            var left = MidCurve.CreateOffset(-Width / 2, XYZ.BasisZ);
            var right = MidCurve.CreateOffset(Width / 2, XYZ.BasisZ);
            var midStartToLeft = Line.CreateBound(start.Location, start.Location + start.NormalLeft * Width / 2);
            var midStartToRight = Line.CreateBound(start.Location, start.Location + start.NormalRight * Width / 2);
            var midEndToLeft = Line.CreateBound(end.Location, end.Location + end.NormalLeft * Width / 2);
            var midEndToRight = Line.CreateBound(end.Location, end.Location + end.NormalRight * Width / 2);

            yield return new KeyValuePair<BridgeCurveType, Curve>(BridgeCurveType.桥中心线, MidCurve);
            yield return new KeyValuePair<BridgeCurveType, Curve>(BridgeCurveType.桥左边界线, left);
            yield return new KeyValuePair<BridgeCurveType, Curve>(BridgeCurveType.桥右边界线, right);
            yield return new KeyValuePair<BridgeCurveType, Curve>(BridgeCurveType.桥中心线起始点至桥左边界线, midStartToLeft);
            yield return new KeyValuePair<BridgeCurveType, Curve>(BridgeCurveType.桥中心线起始点至桥右边界线, midStartToRight);
            yield return new KeyValuePair<BridgeCurveType, Curve>(BridgeCurveType.桥中心线终止点至桥左边界线, midEndToLeft);

        }

        /// <summary>
        /// 返回桥墩轮廓
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<int, Curve>> GetBridgePierOutLine()
        {
            foreach (Element pierEle in Pier)
            {
                var solid = GeometryUtil.TraversingSolid(pierEle, options).OrderBy(m => m.GetBoundingBox().Max.Z).Last();
                var face = GeometryUtil.TraversingSolidFace<PlanarFace>(solid).OrderBy(m => (m.EdgeLoops.get_Item(0).get_Item(0).AsCurve()).GetEndPoint(0).Z).Last();
                var curves = GeometryUtil.TraversingFaceEdge(face).Select(m => new KeyValuePair<int, Curve>(pierEle.Id.IntegerValue, m));
                foreach (var item in curves)
                {
                    yield return item;
                }
            }
            yield break;
        }
    }

    enum BridgeCurveType
    {
        桥中心线,
        桥左边界线,
        桥右边界线,
        桥中心线起始点至桥左边界线,
        桥中心线起始点至桥右边界线,
        桥中心线终止点至桥左边界线,
        桥中心线终止点至桥右边界线,
    }
}
