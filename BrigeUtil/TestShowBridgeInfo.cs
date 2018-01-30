using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using XC.CommonUtils;

namespace XC
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class TestShowBridgeInfo : IExternalCommand
    {
        /// <summary>
        /// 显示桥下边的基本情况
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            var sel = uidoc.Selection;
            var bridgeRevitModel = doc.GetElement(sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element));
            var pS = sel.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.Midpoints);
            var pE = sel.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.Midpoints);
            var Mid = Line.CreateBound(pS, pE);
            var vMid = Mid.Direction.Normalize();
            int width = 600;
            var vLeft = Mid.Direction.CrossProduct(XYZ.BasisZ).Normalize();
            var solids = GeometryUtil.TraversingSolid(bridgeRevitModel, options);
            var tran = new Transaction(doc, "create test arc");
            tran.Start();


            var interoption = new SolidCurveIntersectionOptions() { ResultType = SolidCurveIntersectionMode.CurveSegmentsInside };
            var outeroption = new SolidCurveIntersectionOptions() { ResultType = SolidCurveIntersectionMode.CurveSegmentsOutside };
            var rd = new Random((int)DateTime.UtcNow.ToBinary());
            var rd2 = new Random((int)DateTime.UtcNow.ToBinary());
            for (int i = 0; i < 10; i++)
            {
                var lengthInOffset = rd2.Next(width) / 304.8;
                var lengthInMid = rd.Next(Convert.ToInt32(Mid.Length));
                var line = GetCurveInRange(Mid, vLeft, lengthInMid, lengthInOffset);
                foreach (var solid in solids)
                {
                    var intersection = solid.IntersectWithCurve(line, interoption); // 内部线用于求厚度
                    var outersection = solid.IntersectWithCurve(line, outeroption); // 外部县用于求截面特性
                    if (intersection.SegmentCount == 0)
                    {
                        continue;
                    }   
                    #region 绘制内部线
                    for (int segment = 0; segment <= intersection.SegmentCount - 1; segment++)
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

                    // 绘制截面线--一个面在此截面的起始
                    var testPoint = point; // 起点
                    var vNomal = faceNormal;    // 该点所在面的法线向量
                    var vRode = vMid; // 桥中心线在该点的切线向量
                    var pWater = face.Project(testPoint + vRode.Normalize() * 1 / 304.8).XYZPoint;
                    var vWater = pWater - testPoint;
                    var vColck = vNomal.CrossProduct(vWater).Normalize();
                    var lineClock = Line.CreateBound(testPoint, testPoint + vColck * width);
                    var lineAntiClock = Line.CreateBound(testPoint, testPoint - vColck * width);
                    XYZ pia = null, piai = null;
                    foreach (EdgeArray edges in face.EdgeLoops)
                    {
                        foreach (Edge edge in edges)
                        {
                            var c = edge.AsCurve() as Line;
                            IntersectionResultArray resultArray;
                            var setComparisonResult = line.Intersect(lineClock, out resultArray);
                            if (setComparisonResult == SetComparisonResult.Overlap)
                            {
                                pia = resultArray.get_Item(0).XYZPoint;
                            }
                            resultArray = null;
                            setComparisonResult = line.Intersect(lineAntiClock, out resultArray);
                            if (setComparisonResult == SetComparisonResult.Overlap)
                            {
                                piai = resultArray.get_Item(0).XYZPoint;
                            }
                        }
                    }
                    if (pia != null && piai != null)
                    {
                        try
                        {
                            var seLine = Line.CreateBound(pia, piai);
                            if (seLine.Length > 0.4 / 304.8)
                            {
                                doc.Create.NewModelCurve(seLine, sketchPlanForNormal);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

            }

            tran.Commit();

            return Result.Succeeded;
        }

        public Line GetCurveInRange(Line mid, XYZ offset, double lengthInMid, double lengthInOffset)
        {

            var pointInMid = mid.GetEndPoint(0) + mid.Direction * lengthInMid;
            var point = pointInMid + offset * lengthInOffset;
            var ps = new XYZ(point.X, point.Y, 0);
            var pe = new XYZ(point.X, point.Y, 1000);
            return Line.CreateBound(ps, pe);
        }
        Options options = new Options()
        {
            DetailLevel = ViewDetailLevel.Fine,
            ComputeReferences = true,
            IncludeNonVisibleObjects = true
        };
    }
}
