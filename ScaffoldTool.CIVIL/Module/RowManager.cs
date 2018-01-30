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
    /// 横杆
    /// </summary>
    class RowManager : ClsBaseManager<CADLine>, IOprMode
    {
        internal static System.Drawing.Color color = System.Drawing.Color.White;
        internal static double lineWeight = 0.2;

        public RowManager(CADImage cadImage) : base(cadImage, GlobalData.m_strCrossLayerName)
        {

        }
        public override void SelectedChanged()
        {
            List<CADCircle> columnCircle = cadImage.SelectedEntities.Where(e => e is CADCircle /*&& e.Layer.Name == "外排立杆" || e.Layer.Name == "内排立杆"*/)
                .Cast<CADCircle>().ToList();
            if (columnCircle.Count == 2)
            {
                var UbarLine = CADImportUtils.GetEntitys<CADLine>(cadImage, layer);
                XYZ p1 = columnCircle[0].Point.ConvertFeetXYZ();
                XYZ p2 = columnCircle[1].Point.ConvertFeetXYZ();
                bool blnIn = checkIn(UbarLine, p1, p2);
                if (blnIn)
                {
                    return;
                }
                CADImportUtils.DrawLine(cadImage, columnCircle[0].Point, columnCircle[1].Point, color, layer, lineWeight, false);
                undoStack.Push(new RowUndoArgs(null, RowUndoArgs.UndoType.Add));
            }
            else if (columnCircle.Count > 2)
            {
                var UbarLine = CADImportUtils.GetEntitys<CADLine>(cadImage, layer);
                List<XYZ> lstPts = new List<XYZ>();
                foreach (var item in columnCircle)
                {
                    lstPts.Add(item.Point.ConvertFeetXYZ());
                }
                List<CADEntity> affectedRow = new List<CADEntity>();
                foreach (var pt in lstPts)
                {
                    var pEnd = findXYZ(lstPts, pt, XYZ.BasisX);
                    if (pEnd != null)
                    {
                        bool blnIn = checkIn(UbarLine, pt, pEnd);
                        if (!blnIn)
                        {
                            var add = CADImportUtils.DrawLine(cadImage, pt.ConvertDPoint(), pEnd.ConvertDPoint(), color, layer, lineWeight, false);
                            affectedRow.Add(add);
                            UbarLine.Add(add);
                        }
                    }
                    pEnd = findXYZ(lstPts, pt, -XYZ.BasisX);
                    if (pEnd != null)
                    {
                        bool blnIn = checkIn(UbarLine, pt, pEnd);
                        if (!blnIn)
                        {
                            var add = CADImportUtils.DrawLine(cadImage, pt.ConvertDPoint(), pEnd.ConvertDPoint(), color, layer, lineWeight, false);
                            affectedRow.Add(add);
                            UbarLine.Add(add);
                        }
                    }
                    pEnd = findXYZ(lstPts, pt, XYZ.BasisY);
                    if (pEnd != null)
                    {
                        bool blnIn = checkIn(UbarLine, pt, pEnd);
                        if (!blnIn)
                        {
                            var add = CADImportUtils.DrawLine(cadImage, pt.ConvertDPoint(), pEnd.ConvertDPoint(), color, layer, lineWeight, false);
                            affectedRow.Add(add);
                            UbarLine.Add(add);
                        }
                    }
                    pEnd = findXYZ(lstPts, pt, -XYZ.BasisY);
                    if (pEnd != null)
                    {
                        bool blnIn = checkIn(UbarLine, pt, pEnd);
                        if (!blnIn)
                        {
                            var add = CADImportUtils.DrawLine(cadImage, pt.ConvertDPoint(), pEnd.ConvertDPoint(), color, layer, lineWeight, false);
                            affectedRow.Add(add);
                            UbarLine.Add(add);
                        }
                    }
                }
                if (affectedRow.Count > 0)
                {
                    undoStack.Push(new RowUndoArgs(affectedRow, RowUndoArgs.UndoType.Add));
                }
            }
        }

        XYZ findXYZ(List<XYZ> lstPts, XYZ pt, XYZ direction)
        {
            return lstPts.Where(o => o.DistanceTo(pt) > 0.5 && o.DistanceTo(pt) < 10 && (o - pt).Normalize().Round(2).IsAlmostEqualTo(direction))?.OrderBy(o => o.DistanceTo(pt))?.FirstOrDefault();
        }

        private bool checkIn(List<CADEntity> UbarLine, XYZ p1, XYZ p2)
        {
            bool blnIn = false;
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

            return blnIn;
        }

        public EnumOprMode GetOprMode()
        {
            return EnumOprMode.RowMode;
        }

        public void Render(JsjModel jsjModel)
        {
            foreach (var crossInfo in jsjModel.lstCrossbar)
            {
                CADImportUtils.DrawLine(cadImage, crossInfo.m_StartPt.ConvertDPoint(), crossInfo.m_EndPt.ConvertDPoint(), color, layer, lineWeight, false);
            }
        }

        public void OtherOpr()
        {
            SelectedChanged();
        }
    }

    public class RowUndoArgs
    {
        public enum UndoType { Add, Delect };

        public List<CADEntity> AffectedRow;
        public UndoType Type;

        public RowUndoArgs(List<CADEntity> affectedRow, UndoType type)
        {
            AffectedRow = affectedRow;
            Type = type;
        }
    }
}
