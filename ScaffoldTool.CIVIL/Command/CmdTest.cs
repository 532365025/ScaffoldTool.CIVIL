using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using ScaffoldTool.WinformUI;
using ScaffoldTool.Model;
using ScaffoldTool.Common;
using ScaffoldTool.SFElement;

namespace ScaffoldTool.Command
{
    [Transaction(TransactionMode.Manual)]
    public class CmdTest : IExternalCommand
    {
        double topZ = 0;
        Document _doc = null;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            _doc = doc;
            if (string.IsNullOrWhiteSpace(doc.PathName))
            {
                return Result.Cancelled;
            }
            //var config = new LoadConfig().m_mthLoadConfig(doc.PathName);
            //if (config == null)
            //{
            //    TaskDialog.Show("星层科技", "请先设置参数 ");
            //    return Result.Cancelled;
            //}
            //GlobalInit.Init(config);
            int passWay = 900;
            //GlobalData.m_dPassWayWidth = passWay / 304.8;//900/304.8
            //GlobalData.m_XlgStep = 5;//斜拉杆
            //GlobalData.m_dLenScale = double.Parse(config.MGXTBZ);
            if (doc.ActiveView is View3D)
            {

            }
            else
            {
                var view3D = new FilteredElementCollector(doc).OfClass(typeof(View3D))
                    .Where(o => o.Name.Contains("三维")).FirstOrDefault();
                if (view3D != null)
                {
                    uiDoc.ActiveView = view3D as View3D;
                }
            }

            Reference reference;
            if (!PickObject.TryPickGenericModel(uiDoc, out reference))
            {
                return Result.Cancelled;
            }
            GlobalData.m_strProjectPath = doc.PathName;

            FamilyInstance familyInstance = doc.GetElement(reference) as FamilyInstance;
#if DEBUG
            List<Line> lstTestLine = new List<Line> { Line.CreateBound(XYZ.Zero, new XYZ(20, 0, 0)), Line.CreateBound(new XYZ(20, 0, 0), new XYZ(20, 20, 0)), Line.CreateBound(new XYZ(20, 20, 0), new XYZ(0, 20, 0)), Line.CreateBound(new XYZ(0, 20, 0), new XYZ(0, 0, 0)) };
            List<Line> lstTestLine_out = new List<Line> { Line.CreateBound(XYZ.Zero, new XYZ(30, 0, 0)), Line.CreateBound(new XYZ(30, 0, 0), new XYZ(30, 30, 0)), Line.CreateBound(new XYZ(30, 30, 0), new XYZ(0, 30, 0)), Line.CreateBound(new XYZ(0, 30, 0), new XYZ(0, 0, 0)) };
            ScaffoldDesignForm form = new ScaffoldDesignForm();
            form.lstEnlargeLoop = lstTestLine_out;
            form.lstOriginOutLine = lstTestLine;
            form.ShowDialog();
# endif
            Transaction ts = new Transaction(doc, "智能设计");
            ts.Start();
            CreateCircle(new JsjModel());

            //CreateJsjView objCtview = new CreateJsjView(uiDoc);
            //ViewPlan vp = objCtview.CreateView(JsjModel, outlineCtrl.OriginalLoop, outlineCtrl.EnlargeLoop);

            ts.Commit();
            //if (vp != null)
            //    uiDoc.ActiveView = vp;
            return Result.Succeeded;
        }


        void CreateCircle(JsjModel p_ObjJsj)
        {
            if (p_ObjJsj.m_InnerPoint == null || p_ObjJsj.m_OuterPoint == null)
            {
                return;
            }
            var lstModelLines = new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_Lines).OfClass(typeof(CurveElement))?.ToElements();
            if (lstModelLines != null && lstModelLines.Count > 0)
            {
                foreach (var item in lstModelLines)
                {
                    if (item is ModelLine || item is ModelCurve)
                    {
                        _doc.Delete(item.Id);
                    }
                }
            }
            ColumnVertex first = p_ObjJsj.m_InnerPoint;
            var current = first;
            do
            {
                DrawCircle(_doc, current.Location.SetZ(topZ), 100);
                current = current.Next;

            } while (current != first);

            first = p_ObjJsj.m_OuterPoint;
            current = first;
            do
            {
                DrawCircle(_doc, current.Location.SetZ(topZ), 100);
                current = current.Next;
            } while (current != first);

            Element p_Type = null;
            if (p_ObjJsj.lstUBar != null)
            {
                foreach (var item in p_ObjJsj.lstUBar)
                {
                    DrawLine(_doc, Line.CreateBound(item.m_Start.SetZ(topZ), item.m_End.SetZ(topZ)));
                }
            }           
            if (p_ObjJsj.lstStairPoint != null)
            {
                foreach (var item in p_ObjJsj.lstStairPoint)
                {
                    DrawCircle(_doc, item.ConvertRvtXYZ().SetZ(topZ), 100);
                }

            }

            if (p_ObjJsj.lstPlatformPoint != null)
            {
                foreach (var item in p_ObjJsj.lstPlatformPoint)
                {
                    DrawCircle(_doc, item.ConvertRvtXYZ().SetZ(topZ), 100);
                }
            }

        }
        public void testLine(List<Line> lstline)
        {
            foreach (var item in lstline)
            {
                DrawLine(_doc, item);
            }

        }
        #region 绘制测试函数
        public void DrawCircle(Document doc, XYZ pos, double radius)
        {
            ModelCurve line = doc.Create.NewModelCurve(Arc.Create(pos, radius / 304.8, 0, Math.PI * 2, XYZ.BasisX, XYZ.BasisY),
                   SketchPlane.Create(doc, MyElement.CreatePlane(XYZ.BasisZ, new XYZ(0, 0, pos.Z))));
        }

        public void DrawLine(Document doc, Line line)
        {
            doc.Create.NewModelCurve(line, SketchPlane.Create(doc, MyElement.CreatePlane(XYZ.BasisZ, new XYZ(0, 0, line.Origin.Z))));
        }

        public void DrawLine(Document doc, Line line, string p_strLine, ref Element type)
        {
            var cv = doc.Create.NewModelCurve(line, SketchPlane.Create(doc, MyElement.CreatePlane(XYZ.BasisZ, new XYZ(0, 0, line.Origin.Z))));
            type = ClsDetailLine.GetLineStyle(doc, p_strLine, type, cv);
            cv.LineStyle = type;
            cv.LookupParameter("线样式").Set(type.Id);
        }

        #endregion
    }
}
