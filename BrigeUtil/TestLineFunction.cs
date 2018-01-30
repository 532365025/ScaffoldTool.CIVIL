using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using XC.CommonUtils;

namespace XC
{
    [Transaction(TransactionMode.Manual)]
    class TestLineFunction : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var midline = doc.GetElement(sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "选择桥身(桥体/桥墩/桥中心线):  桥中心线").ElementId);
            var midcurve = GeometryUtil.GetSimpleGeometry<NurbSpline>(midline, new Options()
            {
                DetailLevel = ViewDetailLevel.Fine,
                IncludeNonVisibleObjects = true,
                ComputeReferences = true,
            });

            var tran = new Transaction(doc, "tsetLine");
            tran.Start();
            var testLength = new List<double> { midcurve.Length, midcurve.Length / 5,  midcurve.Length / 2};
            foreach (var length in testLength)
            {
                var targetTransform = midcurve.ComputeDerivatives(length, false);
                var midLocation = targetTransform.Origin;
                var leftDirection = XYZ.BasisZ.CrossProduct(targetTransform.BasisX).Normalize();
                var mainDirection = targetTransform.BasisX;

                var normalLine = Line.CreateBound(midLocation, midLocation + mainDirection * 3);
                var tangentLine = Line.CreateBound(midLocation, midLocation + leftDirection * 6);
                var tangentLine2 = Line.CreateBound(midLocation, midLocation + XYZ.BasisZ * 6);

                var sketchPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, tangentLine.GetEndPoint(0)));
                var sketchPlaneV = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisX, tangentLine.GetEndPoint(0)));
                doc.Create.NewModelCurve(normalLine, sketchPlane);
                doc.Create.NewModelCurve(tangentLine, sketchPlane);
                doc.Create.NewModelCurve(tangentLine2, sketchPlaneV);

                var locationArc = Arc.Create(midLocation, 0.5, 0, Math.PI * 2, XYZ.BasisX, XYZ.BasisY);
                doc.Create.NewModelCurve(locationArc, sketchPlane);
            }


            tran.Commit();


            return Result.Succeeded;
        }
    }
}
