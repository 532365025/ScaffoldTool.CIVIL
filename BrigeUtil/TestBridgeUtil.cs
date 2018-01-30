using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XC.CommonUtils;
using XC.SafetySupport.Scaffold.Bridge.Model.RevitModel;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class TestBridgeUtil : IExternalCommand
    {
        private double CurevtLengthMin { set; get; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            CurevtLengthMin = commandData.Application.Application.ShortCurveTolerance;
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var bridgeBodyRevitModel = sel.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, "选择桥身(桥体/桥墩/桥中心线):  桥体").Select(m => doc.GetElement(m.ElementId)).ToList();
            var bridgePierRevitModel = sel.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, "选择桥身(桥体/桥墩/桥中心线):  桥墩").Select(m => doc.GetElement(m.ElementId)).ToList();
            var midline = doc.GetElement(sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "选择桥身(桥体/桥墩/桥中心线):  桥中心线").ElementId);

            var tran = new Transaction(doc, "create test arc");
            tran.Start();

            var bridge = BridgeInfoRevitModel.Create(bridgeBodyRevitModel, bridgePierRevitModel, midline, 1200/UnitUtil.PublicToInc);

            testPrintBridgeOutLine(doc, bridge);

            tran.Commit();


            return Result.Succeeded;
        }


        public Line GetCurveInRange(Curve mid, XYZ offset, double lengthInMid, double lengthInOffset, XYZ direction)
        {
            
            var pointInMid = mid.GetEndPoint(0) + direction * lengthInMid;
            var point = pointInMid + offset * lengthInOffset;
            var ps = new XYZ(point.X, point.Y, 0);
            var pe = new XYZ(point.X, point.Y, 1000);
            return Line.CreateBound(ps, pe);
        }

        private Options options = new Options()
        {
            DetailLevel = ViewDetailLevel.Fine,
                IncludeNonVisibleObjects = true,
                ComputeReferences = true,
        };
        #region TEST
        /// <summary>
        /// 输出桥的外轮廓线
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bridge"></param>
        private void testPrintBridgeOutLine(Document doc, BridgeInfoRevitModel bridge)
        {
            foreach (var kv in bridge.GetBridgeBodyOutline())
            {
                var curve = kv.Value;
                var sketchPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, curve.GetEndPoint(0)));
                doc.Create.NewModelCurve(curve, sketchPlane);
            }

            foreach (var kv in bridge.GetBridgePierOutLine())
            {
                var curve = kv.Value;
                var sketchPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, curve.GetEndPoint(0)));
                doc.Create.NewModelCurve(curve, sketchPlane);
            }
        }
        /// <summary>
        /// 输出桥与测试线的外交线(测试线被实体打断的线)
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bottom"></param>
        private void testPrintBottomOutterLine(Document doc, BridgeInfoRevitModel bridge)
        {
            int count = 100;
            double offset = bridge.MidCurve.ApproximateLength / count;
            for (int i = 0; i < count; i++)
            {
                var bottom = bridge.GetBridgeBottomAt(offset * i, 5 / UnitUtil.PublicToInc);

                foreach (var point in bottom.LeftPoints)
                {
                    foreach (var curve in point.OuterLine)
                    {
                        var sketchPlaneV = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisX, curve.Origin));
                        doc.Create.NewModelCurve(curve, sketchPlaneV);
                    }
                }
            }
        }
        /// <summary>
        /// 输出桥的截面下边线
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bottom"></param>
        private void testPrintBottom(Document doc, BridgeInfoRevitModel bridge)
        {
            int count = 100;
            double offset = bridge.MidCurve.ApproximateLength / count;
            for (int i = 0; i < count; i++)
            {
                var bottom = bridge.GetBridgeBottomAt(offset * i, 5 / UnitUtil.PublicToInc);

                foreach (var edge in bottom.LeftEdges)
                {
                    if (edge.Start.Point.DistanceTo(edge.End.Point) < CurevtLengthMin)
                    {
                        continue;
                    }
                    var line = Line.CreateBound(edge.Start.Point, edge.End.Point);
                    var normal = (edge.Start.Point - edge.End.Point).CrossProduct(edge.BelongBottom.PointOnMidline.Tangent);
                    var sketchPlaneV = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(normal, edge.Start.Point));
                    doc.Create.NewModelCurve(line, sketchPlaneV);
                }
            }
        }
        /// <summary>
        /// 随机得到桥面范围内的点, 输出七最下的交线和面的法向量
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bridge"></param>
        private void testPrintTestInfo3(Document doc, BridgeInfoRevitModel bridge)
        {
            var midcurve = bridge.MidCurve;
            var rd = new Random((int)DateTime.UtcNow.ToBinary());
            for (int i = 0; i < 100; i++)
            {
                var distanceToMidline = rd.NextDouble() * bridge.Width / 2;
                var distanceToOrigin = rd.NextDouble() * bridge.MidCurve.Length;

                var pointOnCurve = GeometryUtil.GetCurvePointAt(midcurve, distanceToOrigin);
                var location = pointOnCurve.Location + pointOnCurve.NormalLeft * distanceToMidline;
                var line = Line.CreateBound(new XYZ(location.X, location.Y, 0), new XYZ(location.X, location.Y, 1000));
                var sketchPlanForZ = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisX, location));
                doc.Create.NewModelCurve(line, sketchPlanForZ);

                var pointInfo = bridge.GetBridgeBottomPointAt(location);
                if (pointInfo == null)
                {
                    continue;
                }
                #region 绘制法线
                var normalLine = Line.CreateBound(pointInfo.Point, pointInfo.Point + pointInfo.FaceNomal * 10);
                var sketchPlanForNormal = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(pointInfo.FaceNomal.CrossProduct(XYZ.BasisZ), pointInfo.Point));
                doc.Create.NewModelCurve(normalLine, sketchPlanForNormal);
                #endregion

            }
        }
        /// <summary>
        /// 输出桥的下边线基本属性
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bridge"></param>
        private void testPrintTestInfo2(Document doc, BridgeInfoRevitModel bridge)
        {
            var midcurve = bridge.MidCurve;
            var rd = new Random((int)DateTime.UtcNow.ToBinary());
            var rd2 = new Random((int)DateTime.UtcNow.ToBinary());
            for (int i = 0; i < 10; i++)
            {
                var lengthInOffset = rd2.Next((int)bridge.Width / 2);
                var lengthInMid = rd.Next(Convert.ToInt32(midcurve.Length));
                var pointOnCurve = GeometryUtil.GetCurvePointAt(midcurve, lengthInMid);

                var line = GetCurveInRange(midcurve, pointOnCurve.NormalLeft, lengthInMid, lengthInOffset, pointOnCurve.Tangent);
                var interoption = new SolidCurveIntersectionOptions() { ResultType = SolidCurveIntersectionMode.CurveSegmentsInside };
                var outeroption = new SolidCurveIntersectionOptions() { ResultType = SolidCurveIntersectionMode.CurveSegmentsOutside };
                foreach (var solid in bridge.BodySolid)
                {
                    var intersection = solid.IntersectWithCurve(line, interoption); // 内部线用于求厚度
                    var outersection = solid.IntersectWithCurve(line, outeroption); // 外部县用于求截面特性

                    #region 绘制内部线
                    if (intersection.SegmentCount == 0)
                    {
                        continue;
                    }
                    for (int segment = 0; segment < intersection.SegmentCount; segment++)
                    {
                        Curve curveInside = intersection.GetCurveSegment(segment);
                        var sketchPlanForInsid = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisX, curveInside.GetEndPoint(0)));
                        doc.Create.NewModelCurve(curveInside, sketchPlanForInsid);
                    }
                    #endregion

                    #region 绘制外部线
                    var outList = new List<Curve>();
                    for (int segment = 0; segment < outersection.SegmentCount; segment++)
                    {
                        Curve curveOutside = outersection.GetCurveSegment(segment);
                        outList.Add(curveOutside);
                    }
                    outList.Sort((a, b) => { return (int)(a.GetEndPoint(0).Z - b.GetEndPoint(1).Z); });
                    var curveInLow = outList[0] as Line;
                    var sketchPlanForOutside = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisX, curveInLow.GetEndPoint(0)));
                    doc.Create.NewModelCurve(curveInLow, sketchPlanForOutside);
                    #endregion

                    #region 绘制法线
                    var face = GeometryUtil.GetFaceFromSolidByCurve(curveInLow, solid);
                    var point = curveInLow.GetEndPoint(0).Z > curveInLow.GetEndPoint(1).Z ? curveInLow.GetEndPoint(0) : curveInLow.GetEndPoint(1);
                    XYZ faceNormal = face.ComputeNormal(new UV(point.X, point.Y));
                    var normalLine = Line.CreateBound(point, point + faceNormal * 10);
                    var sketchPlanForNormal = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(faceNormal.CrossProduct(curveInLow.Direction), point));
                    doc.Create.NewModelCurve(normalLine, sketchPlanForNormal);
                    #endregion
                }

            }
        }
        /// <summary>
        /// 输出一个缓边曲线的三个要素: 切向量, 发向量, 位置
        /// </summary>
        /// <param name="midline"></param>
        /// <param name="doc"></param>
        private void testPrintTestInfo1(Element midline, Document doc)
        {
            var midcurve = GeometryUtil.GetSimpleGeometry<NurbSpline>(midline, options);
            var testLength = new List<double> { midcurve.Length / 5, midcurve.Length / 4, midcurve.Length / 3, midcurve.Length / 2 };
            foreach (var length in testLength)
            {
                var targetTransform = midcurve.ComputeDerivatives(length, false);
                var midLocation = targetTransform.Origin;
                var leftDirection = targetTransform.BasisZ.CrossProduct(targetTransform.BasisX);
                var mainDirection = targetTransform.BasisX;

                var normalLine = Line.CreateBound(midLocation, midLocation + mainDirection * 3);
                var tangentLine = Line.CreateBound(midLocation, midLocation + leftDirection * 6);
                var tangentLine2 = Line.CreateBound(midLocation, midLocation + targetTransform.BasisZ * 6);

                var sketchPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, tangentLine.GetEndPoint(0)));
                var sketchPlaneV = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisX, tangentLine.GetEndPoint(0)));
                doc.Create.NewModelCurve(normalLine, sketchPlane);
                doc.Create.NewModelCurve(tangentLine, sketchPlane);
                doc.Create.NewModelCurve(tangentLine2, sketchPlaneV);

                var locationArc = Arc.Create(midLocation, 0.5, 0, Math.PI * 2, XYZ.BasisX, XYZ.BasisY);
                doc.Create.NewModelCurve(locationArc, sketchPlane);
            }
        } 
        #endregion

    }
}
