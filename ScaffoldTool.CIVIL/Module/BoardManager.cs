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
    /// 脚手板
    /// </summary>
    internal class BoardManager : ClsBaseManager<CADPolyLine>, IOprMode
    {
        internal static System.Drawing.Color color = System.Drawing.Color.DeepPink;
        internal static double lineWeight = 0.1;

        public BoardManager(CADImage cadImage) : base(cadImage, GlobalData.m_strBoardLayerName)
        {

        }

        public EnumOprMode GetOprMode()
        {
            return EnumOprMode.BoardMode;
        }

        public override void SelectedChanged()
        {
            List<CADCircle> columnCircle = cadImage.SelectedEntities.Where(e => e is CADCircle/*&& e.Layer.Name == "外排立杆" || e.Layer.Name == "内排立杆"*/)
                .Cast<CADCircle>().ToList();
            if (columnCircle.Count == 4)
            {
                #region 判断重复问题
                List<XYZ> lstPt = new List<XYZ>();
                foreach (var circle in columnCircle)
                {
                    lstPt.Add(circle.Point.ConvertFeetXYZ());
                }
                var UbarLine = CADImportUtils.GetEntitys<CADPolyLine>(cadImage, layer);
                foreach (var line in UbarLine)
                {
                    CADPolyLine polyLine = line as CADPolyLine;

                    bool blnSame = false;
                    for (int j = 0; j < 4; j++)
                    {
                        bool blnIn = false;
                        for (int i = 0; i < 4; i++)
                        {
                            var pt = polyLine.PolyPoints[i].ConvertFeetXYZ();
                            if (pt.DistanceTo(lstPt[j]) < 0.03)
                            {
                                blnIn = true;
                                break;
                            }
                        }
                        if (!blnIn)
                        {
                            blnSame = false;
                            break;
                        }
                        else
                        {
                            blnSame = true;
                        }
                    }
                    if (blnSame)
                    {
                        return;
                    }
                }

                #endregion

                var lstDPt = CreatePolyline(columnCircle);
                lstDPt.Add(lstDPt[0]);
                lstDPt.Add(lstDPt[2]);
                CADImportUtils.DrawPolygon(cadImage, color, layer, lineWeight, lstDPt.ToArray());

                undoStack.Push(new RowUndoArgs(null, RowUndoArgs.UndoType.Add));
            }
        }

        private List<DPoint> CreatePolyline(List<CADCircle> columnCircle)
        {
            List<XYZ> lstPt = new List<XYZ>();
            foreach (var circle in columnCircle)
            {
                lstPt.Add(circle.Point.ConvertFeetXYZ());
            }
            XYZ p1 = lstPt[0];
            XYZ p2 = null;
            XYZ p3 = null;
            int findIdx = -1;
            for (int i = 1; i < lstPt.Count; i++)
            {
                if (lstPt[0].DistanceTo(lstPt[i]).ConvetToMm() == GlobalData.m_dPassWayWidth.ConvetToMm())
                {
                    p2 = lstPt[i];
                    findIdx = i;
                    break;
                }
            }
            if (findIdx < 0)//间距不是900;
            {
                List<DPoint> lstDPoints = new List<DPoint>();
                Line line1 = Line.CreateBound(lstPt[0], lstPt[2]);
                Line line2 = Line.CreateBound(lstPt[1], lstPt[3]);
                XYZ ip = null;
                if (clsCommon.GetIntersection(line1, line2, out ip))
                {
                    foreach (var item in lstPt)
                    {
                        lstDPoints.Add(item.ConvertDPoint());
                    }
                }
                else
                {
                    line1 = Line.CreateBound(lstPt[0], lstPt[1]);
                    line2 = Line.CreateBound(lstPt[2], lstPt[3]);
                    if (!clsCommon.GetIntersection(line1, line2, out ip))
                    {
                        lstDPoints.Add(lstPt[0].ConvertDPoint());
                        lstDPoints.Add(lstPt[1].ConvertDPoint());
                        lstDPoints.Add(lstPt[3].ConvertDPoint());
                        lstDPoints.Add(lstPt[2].ConvertDPoint());
                    }
                    else
                    {
                        lstDPoints.Add(lstPt[0].ConvertDPoint());
                        lstDPoints.Add(lstPt[3].ConvertDPoint());
                        lstDPoints.Add(lstPt[1].ConvertDPoint());
                        lstDPoints.Add(lstPt[2].ConvertDPoint());
                    }
                    double len1 = lstDPoints[0].ConvertFeetXYZ().DistanceTo(lstDPoints[1].ConvertFeetXYZ());
                    double len2 = lstDPoints[0].ConvertFeetXYZ().DistanceTo(lstDPoints[3].ConvertFeetXYZ());
                    if (len1 > len2)
                    {
                        var t = lstDPoints[1];
                        lstDPoints[1] = lstDPoints[3];
                        lstDPoints[3] = t;
                    }
                }
                return lstDPoints;
            }

            if (findIdx == 1)
            {
                p3 = lstPt[2];
            }
            else
            {
                p3 = lstPt[1];
            }
            XYZ p4 = null;
            for (int i = 1; i < lstPt.Count; i++)
            {
                if (lstPt[i] != p2 && lstPt[i] != p3)
                {
                    p4 = lstPt[i];
                    break;
                }
            }

            //p3与p4顺序。

            Line l1 = Line.CreateBound(p1, p3);
            Line l2 = Line.CreateBound(p2, p4);
            XYZ inter = null;
            if (!clsCommon.GetIntersection(l1, l2, out inter))
            {
                inter = p3;
                p3 = p4;
                p4 = inter;
            }
            List<DPoint> lstDPoint = new List<DPoint>();
            lstDPoint.Add(p1.ConvertDPoint());
            lstDPoint.Add(p2.ConvertDPoint());
            lstDPoint.Add(p3.ConvertDPoint());
            lstDPoint.Add(p4.ConvertDPoint());
            return lstDPoint;
        }

        public void Render(JsjModel jsjModel)
        {
            if (jsjModel.lstBoard != null)
            {
                foreach (var board in jsjModel.lstBoard)
                {
                    DPoint dp0 = board.lstLocation[0].ConvertDPoint();
                    DPoint dp1 = board.lstLocation[1].ConvertDPoint();
                    DPoint dp2 = board.lstLocation[2].ConvertDPoint();
                    DPoint dp3 = board.lstLocation[3].ConvertDPoint();
                    DPoint[] dpArray = new DPoint[] { dp0, dp1, dp2, dp3, dp0, dp2 };
                    CADImportUtils.DrawPolygon(cadImage, color, layer, lineWeight, dpArray);
                }
            }
        }

        public void OtherOpr()
        {
            SelectedChanged();
        }
    }
}
