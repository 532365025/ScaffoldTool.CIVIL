using Autodesk.Revit.DB;
using CADImport;
using ScaffoldTool.Common;
using ScaffoldTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Module
{
    /// <summary>
    /// 斜杆
    /// </summary>
    internal class SkewRowManager : ClsBaseManager<CADLine>, IOprMode
    {
        internal static System.Drawing.Color color = System.Drawing.Color.DarkOrange;
        internal static double lineWeight = 1;

        public SkewRowManager(CADImage cadImage) : base(cadImage, GlobalData.m_strSkewLayerName)
        {

        }

        public void OtherOpr()
        {
            SelectedChanged();
        }

        public EnumOprMode GetOprMode()
        {
            return EnumOprMode.SkewRowMode;
        }

        public void Render(JsjModel jsjModel)
        {
            if (jsjModel.lstSkew != null)
            {
                foreach (var skewInfo in jsjModel.lstSkew)
                {
                    CADImportUtils.DrawLine(cadImage, skewInfo.m_StartPt.ConvertDPoint(), skewInfo.m_EndPt.ConvertDPoint(), color, layer, lineWeight, false);
                }
            }
        }

        public override void SelectedChanged()
        {
            List<CADCircle> columnCircle = cadImage.SelectedEntities.Where(e => e is CADCircle  /*&& e.Layer.Name == "外排立杆" || e.Layer.Name == "内排立杆"*/).Cast<CADCircle>().ToList();
            if (columnCircle.Count == 2)
            {
                bool blnIn = false;
                var UbarLine = CADImportUtils.GetEntitys<CADLine>(cadImage, GlobalData.m_strSkewLayerName);
                XYZ p1 = columnCircle[0].Point.ConvertFeetXYZ();
                XYZ p2 = columnCircle[1].Point.ConvertFeetXYZ();
                foreach (var ent in UbarLine)
                {
                    CADLine line = ent as CADLine;
                    if (p1.DistanceTo(line.Point.ConvertFeetXYZ()) < 0.1 && p2.DistanceTo(line.Point1.ConvertFeetXYZ()) < 0.1)
                    {
                        blnIn = true;
                        break;
                    }
                    if (p1.DistanceTo(line.Point1.ConvertFeetXYZ()) < 0.1 && p2.DistanceTo(line.Point.ConvertFeetXYZ()) < 0.1)
                    {
                        blnIn = true;
                        break;
                    }
                }
                if (blnIn)
                {
                    return;
                }
                CADImportUtils.DrawLine(cadImage, columnCircle[0].Point, columnCircle[1].Point, color, layer, lineWeight, false);
                undoStack.Push(new RowUndoArgs(null, RowUndoArgs.UndoType.Add));
            }
        }

    }
}
