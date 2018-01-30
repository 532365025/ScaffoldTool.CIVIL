using Autodesk.Revit.DB;
using CADImport;
using ScaffoldTool.Common;
using ScaffoldTool.SFElement;
using ScaffoldTool.Model;
using ScaffoldTool.WinformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScaffoldTool.Other;

namespace ScaffoldTool.Module
{
    public class ViewController
    {

        public ScaffoldDesignForm objView;
        public ViewController(ScaffoldDesignForm form)
        {
            objView = form as ScaffoldDesignForm;
        }

        #region 返回脚手架数据集

        /// <summary>
        /// 返回脚手架数据集
        /// </summary>
        /// <returns></returns>
        internal JsjModel GetJsjInfo()
        {
            var lstPlatform = GetPlatformPt();
            // 横杆
            List<CrossbarInfo> lstCrossbar = GetCrossbar();
            List<CrossbarInfo> lstOuterCrossbar = GetOuterBar(lstCrossbar);
            // 脚手板
            var lstBoard = GetBoard();
            //斜杆
            List<SkewInfo> lstSkew = GetSkewBar();
            var lstUbar = GetUBar();
            var lstStairPt = GetStair();
            return new JsjModel()
            {                
                lstCrossbar = lstCrossbar,
                lstOuterCrossbar = lstOuterCrossbar,                
                lstSkew = lstSkew,
                lstBoard = lstBoard,
                lstUBar = lstUbar,
                lstStairPoint = lstStairPt,
                lstPlatformPoint = lstPlatform,
                lstOutLine = JsjModelConvert.ConvertXcLine(objView.lstOriginOutLine),                
                m_strCADPath = GlobalData.m_strCADPath,
                lstRht = GetRht(),
                m_OffsetToWall = GlobalData.m_OffsetToWall,
                lstXlPlat = GetXlPlat()
            };
        }
        #endregion

        #region 获取卸料平台
        /// <summary>
        /// 获取卸料平台
        /// </summary>
        /// <returns></returns>
        List<Xyz> GetXlPlat()
        {
            var pts = CADImportUtils.GetEntitys<CADHatch>(objView.cadImage, GlobalData.m_strXlPlatLayerName);
            List<Xyz> lstColPt = new List<Xyz>();
            List<XYZ> lstXYZ = new List<XYZ>();
            foreach (var hatch in pts)
            {
                CADHatch cr = hatch as CADHatch;
                var arc = cr.BoundaryData[0][0] as CAD2DArc;
                Xyz p = new Xyz(arc.CenterPoint.X, arc.CenterPoint.Y, 0);
                bool blnIn = false;

                foreach (var item in lstXYZ)
                {
                    if (item.DistanceTo(p.ConvertRvtXYZ()) < 0.01)
                    {
                        blnIn = true;
                        break;
                    }
                }

                if (blnIn)
                {
                    continue;
                }
                lstXYZ.Add(p.ConvertRvtXYZ());
                lstColPt.Add(p);
            }
            return lstColPt;
        }

        #endregion

        #region 获取人货梯
        /// <summary>
        /// 获取人货梯
        /// </summary>
        /// <returns></returns>
        List<Xyz> GetRht()
        {
            var pts = CADImportUtils.GetEntitys<CADHatch>(objView.cadImage, GlobalData.m_strRhtLayerName);
            List<Xyz> lstColPt = new List<Xyz>();
            List<XYZ> lstXYZ = new List<XYZ>();
            foreach (var hatch in pts)
            {
                CADHatch cr = hatch as CADHatch;
                var arc = cr.BoundaryData[0][0] as CAD2DArc;
                Xyz p = new Xyz(arc.CenterPoint.X, arc.CenterPoint.Y, 0);
                bool blnIn = false;

                foreach (var item in lstXYZ)
                {
                    if (item.DistanceTo(p.ConvertRvtXYZ()) < 0.01)
                    {
                        blnIn = true;
                        break;
                    }
                }

                if (blnIn)
                {
                    continue;
                }
                lstXYZ.Add(p.ConvertRvtXYZ());
                lstColPt.Add(p);
            }
            return lstColPt;
        }

        #endregion

        #region 楼梯额外增加的立杆
        /// <summary>
        /// 楼梯额外增加的立杆
        /// </summary>
        /// <returns></returns>
        internal List<Xyz> GetStair()
        {
            var lines = CADImportUtils.GetEntitys<CADCircle>(objView.cadImage, GlobalData.m_strStairLayerName);
            List<Xyz> lstColPt = new List<Xyz>();
            foreach (var circle in lines)
            {
                CADCircle cr = circle as CADCircle;
                lstColPt.Add(cr.Point.ConvertFeetXYZ().ConvertXyz());
            }
            return lstColPt;
        }
        #endregion

        #region 获取悬挑主梁
        /// <summary>
        /// 获取悬挑主梁
        /// </summary>
        /// <returns></returns>
        internal List<UBarInfo> GetUBar()
        {
            List<UBarInfo> lstUbar = new List<UBarInfo>();
            var UbarLine = CADImportUtils.GetEntitys<CADLine>(objView.cadImage, GlobalData.m_strUbarLayerName);
            foreach (var line in UbarLine)
            {
                CADLine l = line as CADLine;
                bool blnIn = false;
                UBarInfo objUbar = new UBarInfo() { m_Start = l.Point.ConvertFeetXYZ(), m_End = l.Point1.ConvertFeetXYZ() };
                foreach (var item in lstUbar)
                {
                    if (item.m_Start.DistanceTo(objUbar.m_Start) < 0.01 && item.m_End.DistanceTo(objUbar.m_End) < 0.01)
                    {
                        blnIn = true;
                        break;
                    }
                    if (item.m_Start.DistanceTo(objUbar.m_End) < 0.01 && item.m_End.DistanceTo(objUbar.m_Start) < 0.01)
                    {
                        blnIn = true;
                        break;
                    }
                }

                if (blnIn)
                {
                    objView.cadImage.RemoveEntity(line);
                    continue;
                }

                lstUbar.Add(objUbar);
            }
            return lstUbar;
        }
        #endregion

        #region 获取斜杆
        /// <summary>
        /// 获取斜杆
        /// </summary>
        /// <returns></returns>
        private List<SkewInfo> GetSkewBar()
        {
            List<SkewInfo> lstSkew = new List<SkewInfo>();
            var skewLine = CADImportUtils.GetEntitys<CADLine>(objView.cadImage, GlobalData.m_strSkewLayerName);
            foreach (var line in skewLine)
            {
                CADLine l = line as CADLine;
                SkewInfo objSkew = new SkewInfo() { m_blnStartUp = true, m_StartPt = l.Point.ConvertFeetXYZ(), m_EndPt = l.Point1.ConvertFeetXYZ() };
                lstSkew.Add(objSkew);
            }

            return lstSkew;
        }

        #endregion

        #region 获取脚手板
        /// <summary>
        /// 获取脚手板
        /// </summary>
        /// <returns></returns>
        private List<BoardInfo> GetBoard()
        {
            List<BoardInfo> lstBoard = new List<BoardInfo>();
            var boardLine = CADImportUtils.GetEntitys<CADPolyLine>(objView.cadImage, GlobalData.m_strBoardLayerName);
            foreach (var line in boardLine)
            {
                CADPolyLine l = line as CADPolyLine;
                var lst = l.PolyPoints.ToList();

                List<XYZ> lstLocation = new List<XYZ>();
                for (int i = 0; i < 4; i++)
                {
                    lstLocation.Add(lst[i].ConvertFeetXYZ());
                }
                //lstLocation.Add(p1);
                //lstLocation.Add(p2);
                //lstLocation.Add(p3);
                //lstLocation.Add(p4);
                BoardInfo objBoard = new BoardInfo() { Width = objView.PassWayWidth / 2, lstLocation = lstLocation };
                XYZ direction = (lstLocation[3] - lstLocation[0]).Normalize();
                if (direction.IsAlmostEqualTo(XYZ.BasisX) || direction.IsAlmostEqualTo(-XYZ.BasisX)
                    || direction.IsAlmostEqualTo(XYZ.BasisY) || direction.IsAlmostEqualTo(XYZ.BasisY))
                {
                    objBoard.blnRectangle = true;
                }
                else
                {
                    objBoard.blnRectangle = false;
                }
                lstBoard.Add(objBoard);
            }
            return lstBoard;
        }

        #endregion

        #region 获取平台立杆点
        /// <summary>
        /// 获取平台立杆点
        /// </summary>
        /// <returns></returns>
        List<Xyz> GetPlatformPt()
        {
            var lines = CADImportUtils.GetEntitys<CADCircle>(objView.cadImage, GlobalData.m_strPlatfLayerName);
            List<Xyz> lstPt = new List<Xyz>();
            foreach (var line in lines)
            {
                lstPt.Add((line as CADCircle).Point.ConvertFeetXYZ().ConvertXyz());
            }
            return lstPt;
        }
        #endregion

        #region 获取横杆
        /// <summary>
        /// 获取横杆
        /// </summary>
        /// <returns></returns>
        private List<CrossbarInfo> GetCrossbar()
        {
            var lines = CADImportUtils.GetEntitys<CADLine>(objView.cadImage, GlobalData.m_strCrossLayerName);
            List<CrossbarInfo> lstCrossbar = new List<CrossbarInfo>();
            foreach (var line in lines)
            {
                CADLine l = line as CADLine;
                if (l.Point.ConvertFeetXYZ().DistanceTo(l.Point1.ConvertFeetXYZ()) < 0.01)
                {
                    continue;
                }
                CrossbarInfo objCrossbar = new CrossbarInfo() { m_StartPt = l.Point.ConvertFeetXYZ(), m_EndPt = l.Point1.ConvertFeetXYZ() };
                lstCrossbar.Add(objCrossbar);
            }

            return lstCrossbar;
        }

        #endregion

        #region 生成斜拉杆

        /// <summary>
        /// 生成斜拉杆
        /// </summary>
        internal void m_mthCreateXlg()
        {
            if (objView.InnerStart == null || objView.OuterStart == null)
            {
                return;
            }
            var lstHaveSkewBar = GetSkewBar();
            var first = objView.OuterStart;
            var current = first;
            int step = 0;
            List<SkewInfo> lstSkew = new List<SkewInfo>();
            do
            {
                bool blnNeed = false;
                if (current.Type == ColumnType.OnConvexCorner || current.Type == ColumnType.OnConcaveCorner)
                {
                    blnNeed = true;
                }
                if (current.Next.Type == ColumnType.OnConvexCorner || current.Next.Type == ColumnType.OnConcaveCorner)
                {
                    blnNeed = true;
                }
                step++;
                if (step == GlobalData.m_XlgStep)
                {
                    blnNeed = true;
                }
                if (blnNeed)
                {
                    step = 0;
                    SkewInfo objSkew = new SkewInfo() { m_blnStartUp = true, m_StartPt = current.Location, m_EndPt = current.Next.Location };
                    lstSkew.Add(objSkew);
                }
                current = current.Next;
            } while (current != first);

            foreach (var item in lstSkew)
            {
                XYZ ps = item.m_StartPt;
                XYZ pe = item.m_EndPt;
                if (blnCheckSkewBarExist(ps, pe, lstHaveSkewBar))
                {
                    continue;
                }
                XYZ direction = (item.m_EndPt - item.m_StartPt).Normalize().CrossProduct(XYZ.BasisZ);
                DPoint p1 = new DPoint(ps.X, ps.Y, 0);
                DPoint p2 = new DPoint(pe.X, pe.Y, 0);

                CADImportUtils.DrawLine(objView.cadImage, p1, p2, SkewRowManager.color, GlobalData.m_strSkewLayerName, SkewRowManager.lineWeight);
            }
            objView.cadPictBox.Refresh();
        }
        bool blnCheckSkewBarExist(XYZ start, XYZ end, List<SkewInfo> lstSkewBar)
        {
            foreach (var item in lstSkewBar)
            {
                if (item.m_StartPt.DistanceTo(start) <= 0.001 && item.m_EndPt.DistanceTo(end) <= 0.001)
                {
                    return true;
                }
                if (item.m_StartPt.DistanceTo(end) <= 0.001 && item.m_EndPt.DistanceTo(start) <= 0.001)
                {
                    return true;
                }
            }
            return false;

        }
        #endregion

        #region 脚手板
        /// <summary>
        /// 脚手板
        /// </summary>
        internal void m_mthScaffoldboard()
        {
            if (objView.InnerStart == null || objView.OuterStart == null)
            {
                return;
            }
            var lstHaveBoard = GetBoard();
            List<List<XYZ>> lstBoard = new List<List<XYZ>>();
            var first = objView.InnerStart;
            if (first.Type == ColumnType.OnConcaveCorner)
            {
                first = first.Next;
            }
            var current = first;
            do
            {
                List<XYZ> lstPt = null;
                var next = current.Next;
                if (current.Type == ColumnType.OnConvexCorner && Math.Round(current.BackwardVector.DotProduct(current.ForwardVector), 1) == 0)
                {
                    XYZ p1 = current.Location - objView.dPassWayWidth * current.BackwardVector;
                    XYZ p2 = p1 - objView.dPassWayWidth * current.ForwardVector;
                    XYZ p3 = current.Location - objView.dPassWayWidth * current.ForwardVector;
                    lstPt = new List<XYZ>();
                    lstPt.Add(current.Location);
                    lstPt.Add(p1);
                    lstPt.Add(p2);
                    lstPt.Add(p3);
                    //lstPt.Add((current.Location + p1) / 2);
                    //lstPt.Add((p2 + p3) / 2);
                    lstPt.Add(lstPt[0]);
                    lstPt.Add(lstPt[2]);
                    lstBoard.Add(lstPt);
                }

                //是否是斜方向判断
                if (clsCommon.CheckDirection(current.ForwardVector, XYZ.BasisX, 2) || clsCommon.CheckDirection(current.ForwardVector, XYZ.BasisY, 2))
                {
                    lstPt = new List<XYZ>();
                    lstPt.Add(current.Location);
                    current.OutwardVector = current.ForwardVector.CrossProduct(XYZ.BasisZ);
                    lstPt.Add(current.Location + current.OutwardVector * objView.dPassWayWidth);
                    lstPt.Add(current.Next.Location + current.OutwardVector * objView.dPassWayWidth);
                    lstPt.Add(current.Next.Location);
                    lstPt.Add(lstPt[0]);
                    lstPt.Add(lstPt[2]);
                }
                else //斜方向
                {
                    current = current.Next;
                    continue;
                }
                lstBoard.Add(lstPt);
                current = current.Next;
                if (first == current)
                {
                    break;
                }
                if (current.Type == ColumnType.OnConcaveCorner)
                {
                    if (current.Location.DistanceTo(new XYZ(-42.0240618186077, 89.1781922389176, 0)) < 0.01)
                    {

                    }
                    if (Math.Round(current.ForwardVector.DotProduct(current.BackwardVector), 2) == 0)
                    {
                        current = current.Next;
                        continue;
                    }

                    XYZ p1 = current.Location;
                    XYZ p2 = current.Next.Location;
                    XYZ p3 = null;
                    XYZ p4 = null;
                    var outer = objView.OuterStart;
                    do
                    {
                        if (p2.DistanceTo(outer.Location) <= GlobalData.m_dPassWayWidth + 0.01)
                        {
                            p3 = outer.Location;
                        }
                        if (p1.DistanceTo(outer.Location) <= GlobalData.m_dPassWayWidth + 0.01)
                        {
                            p4 = outer.Location;
                        }
                        outer = outer.Next;
                    } while (outer != objView.OuterStart);
                    if (p3 == null || p4 == null || p3 == p4)
                    {
                        current = current.Next;
                        continue;
                    }
                    lstPt = new List<XYZ>();
                    lstPt.Add(p1);
                    lstPt.Add(p2);
                    lstPt.Add(p3);
                    lstPt.Add(p4);
                    lstPt.Add(lstPt[0]);
                    lstPt.Add(lstPt[2]);
                    lstBoard.Add(lstPt);
                    current = current.Next;
                }
            } while (first != current);
            CreateBoard(lstBoard, lstHaveBoard);
            objView.cadPictBox.Invalidate();
        }

        /// <summary>
        /// 创建脚手板
        /// </summary>
        /// <param name="lstPt"></param>
        /// <param name="lstHaveBoard"></param>
        void CreateBoard(List<List<XYZ>> lstPt, List<BoardInfo> lstHaveBoard)
        {
            foreach (var item in lstPt)
            {
                if (blnCheckBoardExist(item, lstHaveBoard))
                {
                    continue;
                }

                //DPoint p1 = new DPoint(item[0].X, item[0].Y, 0);
                //DPoint p2 = new DPoint(item[1].X, item[1].Y, 0);
                //ScaffoldcadPictBoxUtil.DrawLine(objView.cadImage, p1, p2, System.Drawing.Color.DarkBlue, GlobalData.m_strBoardLayerName, 3);
                List<DPoint> lstDPt = new List<DPoint>();
                foreach (var pt in item)
                {
                    lstDPt.Add(pt.ConvertDPoint());
                }

                CADImportUtils.DrawPolygon(objView.cadImage, BoardManager.color, GlobalData.m_strBoardLayerName, BoardManager.lineWeight, lstDPt.ToArray());
            }
        }

        bool blnCheckBoardExist(List<XYZ> lstPt, List<BoardInfo> lstHaveBoard)
        {
            foreach (var item in lstHaveBoard)
            {
                if (lstPt[0].DistanceTo(item.lstLocation[0]) <= 0.001 && lstPt[2].DistanceTo(item.lstLocation[2]) <= 0.001)
                {
                    return true;
                }
                if (lstPt[0].DistanceTo(item.lstLocation[2]) <= 0.001 && lstPt[2].DistanceTo(item.lstLocation[0]) <= 0.001)
                {
                    return true;
                }
                if (lstPt[0].DistanceTo(item.lstLocation[1]) <= 0.001 && lstPt[2].DistanceTo(item.lstLocation[3]) <= 0.001)
                {
                    return true;
                }
                if (lstPt[0].DistanceTo(item.lstLocation[3]) <= 0.001 && lstPt[2].DistanceTo(item.lstLocation[1]) <= 0.001)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 横杆
        /// <summary>
        /// 横杆
        /// </summary>
        internal void m_mthCreateCroosbar()
        {
            if (objView.InnerStart == null || objView.OuterStart == null)
            {
                return;
            }
            var lstCrossbar = GetCrossbar();

            var first = objView.InnerStart;

            var current = first;
            do
            {
                if (current.Type == ColumnType.OnConcaveCorner)
                {
                    current = current.Next;
                    continue;
                }
                XYZ d1 = current.OutwardVector;
                XYZ d2 = current.ForwardVector;
                int count = Math.Round(d1.DotProduct(d2), 1) == 0 ? 2 : 1;
                var findList = FindOuterPoint(current, count);
                if (findList != null && findList.Count > 0)
                {
                    foreach (var item in findList)
                    {
                        if (blnCheckCrossBarExist(current.Location, item.Location, lstCrossbar))
                        {
                            continue;
                        }
                        AddCrossbarLine(current.Location, item.Location);
                    }
                }
                current = current.Next;
            } while (first != current);

            m_mthCreateInnerAndOuterBar(lstCrossbar);
            CreatePlatformCrossbar(lstCrossbar);
            objView.cadPictBox.Refresh();
        }

        /// <summary>
        ///平台的横杆
        ///平台立杆之间没有考虑。
        /// </summary>
        void CreatePlatformCrossbar(List<CrossbarInfo> lstCrossbar)
        {
            if (objView.InnerStart == null || objView.OuterStart == null)
            {
                return;
            }
            var cadCircles = CADImportUtils.GetEntitys<CADCircle>(objView.cadImage, GlobalData.m_strPlatfLayerName);
            List<XYZ> lstPlatfPts = new List<XYZ>();
            foreach (var pt in cadCircles)
            {
                lstPlatfPts.Add((pt as CADCircle).Point.ConvertFeetXYZ());
            }
            foreach (var item in lstPlatfPts)
            {
                XYZ PlatPt = item;

                List<XYZ> lstNearPt = new List<XYZ>();
                var first = objView.InnerStart;
                var current = first;
                do
                {
                    if (Math.Abs(PlatPt.DistanceTo(current.Location) - GlobalData.m_dPassWayWidth) < 0.01)
                    {
                        lstNearPt.Add(current.Location);
                    }
                    current = current.Next;
                } while (current != first);

                foreach (var pt in lstNearPt)
                {
                    if (blnCheckCrossBarExist(PlatPt, pt, lstCrossbar))
                    {
                        continue;
                    }
                    AddCrossbarLine(PlatPt, pt);
                }
            }
        }
        public void AddCrossbarLine(XYZ p_p1, XYZ p_p2)
        {
            DPoint p1 = new DPoint(p_p1.X, p_p1.Y, 0);
            DPoint p2 = new DPoint(p_p2.X, p_p2.Y, 0);
            CADImportUtils.DrawLine(objView.cadImage, p1, p2, RowManager.color, GlobalData.m_strCrossLayerName, RowManager.lineWeight);
        }
        List<ColumnVertex> FindOuterPoint(ColumnVertex innerPoint, int count)
        {
            List<ColumnVertex> result = new List<ColumnVertex>();
            var first = objView.OuterStart;
            var current = first;
            double distance = (objView.PassWayWidth + 30) / 304.8;
            do
            {
                if (innerPoint.Location.DistanceTo(current.Location) <= distance)
                {
                    //垂直或者平行方向.
                    XYZ direction = (current.Location - innerPoint.Location).Normalize();
                    if (direction.IsAlmostEqualTo(innerPoint.ForwardVector) || direction.IsAlmostEqualTo(-innerPoint.ForwardVector) ||
                        Math.Round(direction.DotProduct(innerPoint.ForwardVector), 1) == 0 || Math.Round(direction.DotProduct(innerPoint.BackwardVector), 1) == 0)
                    {
                        result.Add(current);
                        if (result.Count == count)
                        {
                            return result;
                        }
                    }
                }
                current = current.Next;
            } while (first != current);

            return result;
        }

        ColumnVertex FindOuterPoint(ColumnVertex innerPoint, Line OutVectorLine)
        {
            var first = objView.OuterStart;
            var current = first;
            double maxDistance = GlobalData.m_dPassWayWidth + 0.1;
            OutVectorLine.MakeUnbound();
            do
            {
                if (innerPoint.Location.DistanceTo(current.Location) <= maxDistance && OutVectorLine.Distance(current.Location) <= 0.03)
                {
                    return current;
                }
                current = current.Next;
            } while (current != first);
            return null;
        }
        bool blnCheckCrossBarExist(XYZ start, XYZ end, List<CrossbarInfo> lstCrossbar)
        {
            foreach (var item in lstCrossbar)
            {
                if (item.m_StartPt.DistanceTo(start) <= 0.001 && item.m_EndPt.DistanceTo(end) <= 0.001)
                {
                    return true;
                }
                if (item.m_StartPt.DistanceTo(end) <= 0.001 && item.m_EndPt.DistanceTo(start) <= 0.001)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 内外侧横杆
        /// <summary>
        /// 内外侧横杆
        /// </summary>
        void m_mthCreateInnerAndOuterBar(List<CrossbarInfo> lstCrossbar)
        {
            if (objView.InnerStart == null || objView.OuterStart == null)
            {
                return;
            }
            var first = objView.InnerStart;
            var current = first;
            do
            {
                var next = current.Next;
                if (blnCheckCrossBarExist(current.Location, next.Location, lstCrossbar))
                {
                    current = current.Next;
                    continue;
                }
                AddCrossbarLine(current.Location, next.Location);
                current = current.Next;
            } while (first != current);

            first = objView.OuterStart;
            current = first;
            do
            {
                var next = current.Next;
                if (blnCheckCrossBarExist(current.Location, next.Location, lstCrossbar))
                {
                    current = current.Next;
                    continue;
                }
                AddCrossbarLine(current.Location, next.Location);
                current = current.Next;
            } while (first != current);
        }
        List<CrossbarInfo> GetOuterBar(List<CrossbarInfo> lstCrossbar)
        {
            List<CrossbarInfo> outerBarList = new List<CrossbarInfo>();
            var first = objView.OuterStart;
            if (first == null)
            {
                return null;
            }
            var current = first;
            do
            {
                var next = current.Next;
                //if (blnCheckCrossBarExist(current.Location, next.Location, lstCrossbar))
                //{
                //    current = current.Next;
                //    continue;
                //}
                if (current.Location.DistanceTo(next.Location) < 0.01)
                {
                    continue;
                }
                outerBarList.Add(new CrossbarInfo { m_StartPt = current.Location, m_EndPt = next.Location });
                current = current.Next;
            } while (first != current);
            return outerBarList;
        }
        #endregion

        #region 隐藏
        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="p_select"></param>
        /// <param name="blnShow"></param>
        internal void Show(int p_select, bool blnShow)
        {
            //横杆
            //斜杆
            //脚手板
            //悬挑主梁
            string strLayerName = null;
            switch (p_select)
            {
                //case 0:
                //    HideOrShowByLayerName("外排立杆", blnShow);
                //    HideOrShowByLayerName("内排立杆", blnShow);
                //    objView.cadPictBox.Refresh();
                //    return ;
                case 0:
                    strLayerName = GlobalData.m_strCrossLayerName;
                    HideOrShowByLayerName<CADLine>(strLayerName, blnShow);
                    break;
                case 1:
                    strLayerName = GlobalData.m_strSkewLayerName;
                    HideOrShowByLayerName<CADLine>(strLayerName, blnShow);
                    break;
                case 2:
                    strLayerName = GlobalData.m_strBoardLayerName;
                    HideOrShowByLayerName<CADPolyLine>(strLayerName, blnShow);
                    break;
                case 3:
                    strLayerName = GlobalData.m_strUbarLayerName;
                    HideOrShowByLayerName<CADLine>(strLayerName, blnShow);
                    break;
                case 4:
                    objView.ShowText(blnShow);
                    break;
                default: return;
            }

            objView.cadPictBox.Refresh();
        }

        private void HideOrShowByLayerName<T>(string p_strlayerName, bool blnShow)
        {
            var lines = CADImportUtils.GetEntitys<T>(objView.cadImage, p_strlayerName);
            foreach (var line in lines)
            {
                line.Visibility = blnShow;
            }
        }
        #endregion

        #region 创建悬挑主梁
        /// <summary>
        /// 创建悬挑主梁
        /// </summary>
        internal void CreateUbar()
        {
            if (objView.InnerStart == null || objView.OuterStart == null)
            {
                return;
            }
            var lstUbar = GetUBar();
            var first = objView.InnerStart;
            // List<ColumnVertex> lstInnerPts = new List<ColumnVertex>();
            var lstStairPt = GetStair();//楼梯立杆
            var current = first;
            double len = 0;
            do
            {

                if (current.Type == ColumnType.OnLine)
                {
                    XYZ ptEnd = current.Location - 2000 / 304.8 * current.OutwardVector;
                    Line line = Line.CreateBound(current.Location, ptEnd);
                    XYZ p = null;
                    var resultLine = FindNearLine(line, current.ForwardVector, out p);
                    if (resultLine != null)
                    {
                        XYZ p1 = null;
                        XYZ p2 = null;
                        var find = FindOuterPoint(current, line);
                        if (find != null)
                        {
                            XYZ direction = (find.Location - p).Normalize();
                            p1 = find.Location;// + 100 / 304.8 * direction;
                            len = p1.DistanceTo(p) + 100.ConvetToFeet();
                            p2 = p - len * GlobalData.m_dLenScale * direction;
                            if (lstStairPt.Count > 0)
                            {
                                //悬挑到楼梯
                                var stairPt = lstStairPt.OrderBy(o => o.ConvertRvtXYZ().DistanceTo(p1)).FirstOrDefault();
                                if (stairPt != null && stairPt.ConvertRvtXYZ().DistanceTo(p1) <= GlobalData.m_dPassWayWidth + 0.01)
                                {
                                    len = p1.DistanceTo(p) + 100.ConvetToFeet();
                                    p1 = p1 + GlobalData.m_dPassWayWidth * direction;
                                    p2 = p - len * GlobalData.m_dLenScale * direction;
                                }
                            }
                            else if (find.Type == ColumnType.OnConcaveCorner)//悬挑多根立杆
                            {
                                XYZ lastOne = find.Location;
                                int totalLen = 0;
                                if (clsCommon.CheckDirection(direction, find.ForwardVector, 2))
                                {
                                    var tempCurrent = find;
                                    while (clsCommon.CheckDirection(direction, tempCurrent.ForwardVector, 2))
                                    {
                                        totalLen += tempCurrent.ForwardDistance;
                                        if (totalLen > 1200)//不超过1200
                                        {
                                            break;
                                        }
                                        lastOne = tempCurrent.Next.Location;
                                        tempCurrent = tempCurrent.Next;
                                    }
                                }
                                else if (clsCommon.CheckDirection(direction, find.BackwardVector, 2))
                                {
                                    var tempCurrent = find;
                                    while (clsCommon.CheckDirection(direction, tempCurrent.BackwardVector, 2))
                                    {
                                        totalLen += tempCurrent.BackwardDistance;
                                        if (totalLen > 1200)//不超过1200
                                        {
                                            break;
                                        }
                                        lastOne = tempCurrent.Previous.Location;
                                        tempCurrent = tempCurrent.Previous;
                                    }
                                }
                                p1 = lastOne;
                                len = p1.DistanceTo(p) + 100.ConvetToFeet();
                                p2 = p - len * GlobalData.m_dLenScale * direction;
                            }
                        }
                        else
                        {
                            //这种情况理论上 不应该出现
                            double len1 = current.Location.DistanceTo(p) + GlobalData.m_dPassWayWidth + 100.ConvetToFeet();
                            p1 = p + len1 * current.OutwardVector;
                            p2 = p - len1 * GlobalData.m_dLenScale * current.OutwardVector;
                        }

                        if (blnCheckExistInUbar(p1, p2, lstUbar))
                        {
                            current = current.Next;
                            continue;
                        }
                        CADImportUtils.DrawLine(objView.cadImage, p1.ConvertDPoint(), p2.ConvertDPoint(),
                            UbarManager.color, GlobalData.m_strUbarLayerName, UbarManager.lineWeight);
                    }
                }
                #region 凸点
                else
                {


                    //凸点和凹点的主梁
                    XYZ direction = (current.BackwardVector + current.ForwardVector).Normalize();
                    if (current.Type == ColumnType.OnConcaveCorner)
                    {
                        current = current.Next;
                        //凹点不做
                        continue;
                    }

                    //lstInnerPts.Add(current);
                    //计算外侧立杆点

                    //按2米阳角布置角部悬挑构造
                    int nextLen = 0;
                    int preLen = 0;
                    var tempNow = current;
                    while (tempNow.ForwardVector.IsAlmostEqualTo(tempNow.Next.ForwardVector))
                    {
                        nextLen += tempNow.ForwardDistance;
                        tempNow = tempNow.Next;
                    }
                    tempNow = current;
                    while (tempNow.BackwardVector.IsAlmostEqualTo(tempNow.Previous.BackwardVector))
                    {
                        preLen += tempNow.BackwardDistance;
                        tempNow = tempNow.Previous;
                    }
                    if (preLen < 2000 || nextLen < 2000)
                    {
                        //阳角
                        if (current.Previous.Type == ColumnType.OnConcaveCorner)//阳角之前的阴角.
                        {
                            XYZ vector = current.BackwardVector;
                            XYZ ptEnd = current.Location + 3000 / 304.8 * vector;
                            Line line = Line.CreateBound(current.Location, ptEnd);
                            XYZ p = null;
                            var resultLine = FindNearLine(line, current.ForwardVector, out p);
                            if (resultLine != null)
                            {
                                XYZ p1 = null;
                                XYZ p2 = null;
                                var find = FindOuterPoint(current, line);
                                if (find != null)
                                {
                                    p1 = find.Location;// + 100 / 304.8 * direction;
                                    len = p1.DistanceTo(p) + 100.ConvetToFeet();
                                    p2 = p + len * GlobalData.m_dLenScale * vector;

                                    if (blnCheckExistInUbar(p1, p2, lstUbar))
                                    {
                                        current = current.Next;
                                        continue;
                                    }
                                    CADImportUtils.DrawLine(objView.cadImage, p1.ConvertDPoint(), p2.ConvertDPoint(),
                                        UbarManager.color, GlobalData.m_strUbarLayerName, UbarManager.lineWeight);
                                }

                            }
                        }
                        current = current.Next;
                        continue;
                    }
                    len = Math.Sqrt(GlobalData.m_dPassWayWidth * GlobalData.m_dPassWayWidth * 2);// + GlobalData.m_dExtendLength;
                    XYZ pStart = current.Location - len * direction;
                    XYZ pTempEnd = current.Location + GlobalData.m_dPassWayWidth * direction;
                    Line lineTemp = Line.CreateBound(current.Location, pTempEnd);
                    XYZ intersectPt = null;//与边界的交点
                    foreach (var line in objView.lstOriginOutLine)
                    {
                        if (clsCommon.GetIntersection(lineTemp, line, out intersectPt))
                        {
                            break;
                        }
                    }
                    if (intersectPt != null)
                    {
                        double outerLen = pStart.DistanceTo(intersectPt) + 100.ConvetToFeet();//外侧长度
                        XYZ pEnd = intersectPt + outerLen * GlobalData.m_dLenScale * direction;
                        CADImportUtils.DrawLine(objView.cadImage, pStart.ConvertDPoint(), pEnd.ConvertDPoint(),
                         UbarManager.color, GlobalData.m_strUbarLayerName, UbarManager.lineWeight);
                    }
                }
                #endregion

                current = current.Next;
            } while (first != current);


            #region 四个顶角45度的悬挑槽钢 
            //var leftBottom = (from m in lstInnerPts
            //                  orderby m.Location.X, m.Location.Y
            //                  select m).FirstOrDefault();
            //var leftTop = (from m in lstInnerPts
            //               orderby m.Location.Y descending, m.Location.X ascending
            //               select m).FirstOrDefault();

            //var rightTop = (from m in lstInnerPts
            //                orderby m.Location.X descending, m.Location.Y descending
            //                select m).FirstOrDefault();
            //var rightBottm = (from m in lstInnerPts
            //                  orderby m.Location.Y ascending, m.Location.X descending
            //                  select m).FirstOrDefault();

            //lstInnerPts.Clear();

            //lstInnerPts.Add(leftBottom);
            //lstInnerPts.Add(leftTop);
            //lstInnerPts.Add(rightTop);
            //lstInnerPts.Add(rightBottm);
            //foreach (var item in lstInnerPts)
            //{
            //    current = item;
            //    //    计算外侧立杆点
            //    XYZ direction = (current.BackwardVector + current.ForwardVector).Normalize();

            //    double len = Math.Sqrt(GlobalData.m_dPassWayWidth * GlobalData.m_dPassWayWidth * 2);// + GlobalData.m_dExtendLength;
            //    XYZ pStart = current.Location - len * direction;
            //    XYZ pTempEnd = current.Location + GlobalData.m_dPassWayWidth * direction;
            //    Line lineTemp = Line.CreateBound(current.Location, pTempEnd);
            //    XYZ intersectPt = null;//与边界的交点
            //    foreach (var line in objView.lstOriginOutLine)
            //    {
            //        if (clsCommon.GetIntersection(lineTemp, line, out intersectPt))
            //        {
            //            break;
            //        }
            //    }
            //    if (intersectPt != null)
            //    {
            //        double outerLen = pStart.DistanceTo(intersectPt);//外侧长度
            //        XYZ pEnd = intersectPt + outerLen * GlobalData.m_dLenScale * direction;
            //        ScaffoldcadPictBoxUtil.DrawLine(objView.cadImage, pStart.ConvertDPoint(), pEnd.ConvertDPoint(),
            //         UbarManager.color, GlobalData.m_strUbarLayerName, UbarManager.lineWeight);
            //    }
            //} 
            #endregion


            objView.cadPictBox.Refresh();


        }

        #endregion

        #region 判断悬挑主梁是否已经存在
        /// <summary>
        /// 判断悬挑主梁是否已经存在
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="lstUbar"></param>
        /// <returns></returns>
        bool blnCheckExistInUbar(XYZ start, XYZ end, List<UBarInfo> lstUbar)
        {
            bool blnIn = false;
            foreach (var item in lstUbar)
            {
                if (start.DistanceTo(item.m_Start) < 0.03 && end.DistanceTo(item.m_End) < 0.03)
                {
                    blnIn = true;
                    break;
                }
                if (end.DistanceTo(item.m_Start) < 0.03 && start.DistanceTo(item.m_End) < 0.03)
                {
                    blnIn = true;
                    break;
                }
            }
            return blnIn;
        }
        #endregion

        #region 查找最近的轮廓线
        /// <summary>
        /// 查找最近的轮廓线
        /// </summary>
        /// <param name="p_line"></param>
        /// <param name="direction"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        Line FindNearLine(Line p_line, XYZ direction, out XYZ p)
        {
            p = null;
            XYZ start = p_line.GetEndPoint(0);
            // p_line.MakeUnbound();
            List<Line> lstLine = new List<Line>();
            foreach (var line in objView.lstOriginOutLine)
            {
                if (clsCommon.GetIntersection(p_line, line, out p))
                {
                    lstLine.Add(line);
                }
            }
            if (lstLine.Count == 0)
            {
                return null;
            }
            var findline = lstLine.OrderBy(l => l.Distance(start)).FirstOrDefault();
            clsCommon.GetIntersection(p_line, findline, out p);
            return findline;
        }

        #endregion

        #region 添加立杆
        /// <summary>
        /// 添加立杆
        /// </summary>
        internal void AddColPostion()
        {
            if (objView.cadImage.SelectedEntities.Count == 0)
            {
                return;
            }

            int v = objView.SetDistance;
            DPoint Addp = new DPoint(v / 304.8, 0, 0);
            if (objView.rUp.Checked)
            {
                Addp = new DPoint(0, v / 304.8, 0);
            }
            else if (objView.rDown.Checked)
            {
                Addp = new DPoint(0, -v / 304.8, 0);
            }
            else if (objView.rLeft.Checked)
            {
                Addp = new DPoint(-v / 304.8, 0, 0);
            }
            var lstAllPt = GetAllCADCirclePts();
            List<CADEntity> lstEnt = new List<CADEntity>();
            List<XYZ> lstPt = new List<XYZ>();
            foreach (var ent in objView.cadImage.SelectedEntities)
            {
                if (ent is CADCircle)
                {
                    CADCircle cr = ent as CADCircle;
                    DPoint point = cr.Point + Addp;

                    //防止重复创建立杆
                    bool blnIn = false;
                    foreach (var item in lstAllPt)
                    {
                        if (item.DistanceTo(point.ConvertFeetXYZ()) < 0.01)
                        {
                            blnIn = true;
                            break;
                        }
                    }
                    if (blnIn)
                    {
                        continue;
                    }

                    lstEnt.Add(cr);
                    lstPt.Add(point.ConvertFeetXYZ());
                }
            }
            objView.Copy(lstEnt, lstPt);
            objView.cadPictBox.Refresh();
        }

        #endregion

        /// <summary>
        /// 获取所有立杆点 防止重复
        /// </summary>
        /// <returns></returns>
        List<XYZ> GetAllCADCirclePts()
        {
            List<XYZ> lstAllpt = new List<XYZ>();
            var lstCircles = CADImportUtils.GetEntitys<CADCircle>(objView.cadImage);
            foreach (var item in lstCircles)
            {
                lstAllpt.Add((item as CADCircle).Point.ConvertFeetXYZ());
            }
            return lstAllpt;
        }

        #region 保存Json
        /// <summary>
        /// 保存Json
        /// </summary>
        /// <param name="p_Jsj"></param>
        internal void SaveToFile(JsjModel p_Jsj)
        {
            JsjModelConvert.SaveToJsonFile(p_Jsj, GlobalData.m_strProjectPath);

        }
        #endregion

        #region 从Json文件获取
        /// <summary>
        /// 从Json文件获取
        /// </summary>
        /// <returns></returns>
        internal JsjModelSerialize GetModelFromJsonFile()
        {
            return JsjModelConvert.GetJsonModelFromJsonFile(GlobalData.m_strProjectPath);
        }
        #endregion

        /// <summary>
        /// 删除横杆
        /// </summary>
        internal void DeleteAllCrossBar()
        {
            var ents = CADImportUtils.GetEntitys<CADLine>(objView.cadImage, GlobalData.m_strCrossLayerName);
            foreach (var ent in ents)
            {
                objView.cadImage.RemoveEntity(ent);
            }
        }
        /// <summary>
        /// 删除所有斜拉杆
        /// </summary>
        internal void DeleteAllSkewbar()
        {
            var ents = CADImportUtils.GetEntitys<CADLine>(objView.cadImage, GlobalData.m_strSkewLayerName);
            foreach (var ent in ents)
            {
                objView.cadImage.RemoveEntity(ent);
            }
        }
        /// <summary>
        /// 删除所有脚手板
        /// </summary>
        internal void DeleteAllBoard()
        {
            var ents = CADImportUtils.GetEntitys<CADPolyLine>(objView.cadImage, GlobalData.m_strBoardLayerName);
            foreach (var ent in ents)
            {
                objView.cadImage.RemoveEntity(ent);
            }
        }
        /// <summary>
        /// 删除所有悬挑主梁
        /// </summary>
        internal void DeleteAllUbar()
        {
            var ents = CADImportUtils.GetEntitys<CADLine>(objView.cadImage, GlobalData.m_strUbarLayerName);
            foreach (var ent in ents)
            {
                objView.cadImage.RemoveEntity(ent);
            }
        }
        /// <summary>
        /// 删除所有楼梯立杆
        /// </summary>
        internal void DeleteAllStairBar()
        {
            var ents = CADImportUtils.GetEntitys<CADCircle>(objView.cadImage, GlobalData.m_strStairLayerName);
            foreach (var ent in ents)
            {
                objView.cadImage.RemoveEntity(ent);
            }
        }

        /// <summary>
        /// 删除所有外侧立杆
        /// </summary>
        internal void DeleteAllOuterPt()
        {
            var ents = CADImportUtils.GetEntitys<CADCircle>(objView.cadImage, GlobalData.m_strOuterLayerName);
            foreach (var ent in ents)
            {
                objView.cadImage.RemoveEntity(ent);
            }
        }
        /// <summary>
        /// 删除所有平台立杆
        /// </summary>
        internal void DeleteAllPlatform()
        {
            var ents = CADImportUtils.GetEntitys<CADCircle>(objView.cadImage, GlobalData.m_strPlatfLayerName);
            foreach (var ent in ents)
            {
                objView.cadImage.RemoveEntity(ent);
            }
        }
        /// <summary>
        /// 人货梯
        /// </summary>
        internal void DeleteAllRht()
        {
            var ents = CADImportUtils.GetEntitys<CADHatch>(objView.cadImage, GlobalData.m_strRhtLayerName);
            foreach (var ent in ents)
            {
                objView.cadImage.RemoveEntity(ent);
            }
        }

        #region 删除所有设计内容

        /// <summary>
        /// 删除所有设计内容
        /// </summary>
        internal void DeleteAll()
        {
            objView.comboBoxEx1.SelectedIndex = 0;
            //var ents = ScaffoldcadPictBoxUtil.GetEntitys<CADLine>(objView.cadImage, GlobalData.m_strCrossLayerName);
            //foreach (var ent in ents)
            //{
            //    objView.cadImage.RemoveEntity(ent);
            //}
            //ents = ScaffoldcadPictBoxUtil.GetEntitys<CADPolyLine>(objView.cadImage, GlobalData.m_strBoardLayerName);
            //foreach (var ent in ents)
            //{
            //    objView.cadImage.RemoveEntity(ent);
            //}
            //ents = ScaffoldcadPictBoxUtil.GetEntitys<CADLine>(objView.cadImage, GlobalData.m_strSkewLayerName);
            //foreach (var ent in ents)
            //{
            //    objView.cadImage.RemoveEntity(ent);
            //}
            //ents = ScaffoldcadPictBoxUtil.GetEntitys<CADLine>(objView.cadImage, GlobalData.m_strUbarLayerName);
            //foreach (var ent in ents)
            //{
            //    objView.cadImage.RemoveEntity(ent);
            //}
            //ents = ScaffoldcadPictBoxUtil.GetEntitys<CADCircle>(objView.cadImage, GlobalData.m_strStairLayerName);
            //foreach (var ent in ents)
            //{
            //    objView.cadImage.RemoveEntity(ent);
            //}
            //DeleteAllPlatform();
            //DeleteAllRht();

            objView.ClearDesgin();
            objView.cadPictBox.Refresh();
        }
        #endregion

        #region 删除Json文件

        /// <summary>
        /// 删除Json文件
        /// </summary>
        /// <param name="projectPath"></param>
        internal void DelJsonFile(string projectPath)
        {
            clsCommon.DelJsonFile(projectPath);
        }

        #endregion

        #region 复制为平台
        /// <summary>
        /// 复制为平台
        /// </summary>
        public void CreatePlatform()
        {
            if (objView.cadImage.SelectedEntities.Count == 0)
            {
                return;
            }
            int v = objView.SetDistance;
            DPoint Addp = new DPoint(v / 304.8, 0, 0);
            if (objView.rUp.Checked)
            {
                Addp = new DPoint(0, v / 304.8, 0);
            }
            else if (objView.rDown.Checked)
            {
                Addp = new DPoint(0, -v / 304.8, 0);
            }
            else if (objView.rLeft.Checked)
            {
                Addp = new DPoint(-v / 304.8, 0, 0);
            }
            var lstAllPt = GetAllCADCirclePts();
            foreach (var ent in objView.cadImage.SelectedEntities)
            {
                if (ent is CADCircle)
                {
                    CADCircle cr = ent as CADCircle;
                    DPoint point = cr.Point + Addp;


                    //防止重复创建立杆
                    bool blnIn = false;
                    foreach (var item in lstAllPt)
                    {
                        if (item.DistanceTo(point.ConvertFeetXYZ()) < 0.01)
                        {
                            blnIn = true;
                            break;
                        }
                    }
                    if (blnIn)
                    {
                        continue;
                    }

                    CADCircle c = CADImportUtils.DrawCircle(objView.cadImage, point, 48.3, GlobalData.m_strPlatfLayerName, System.Drawing.Color.Blue, false);
                }
            }
            objView.cadPictBox.Refresh();
        }
        #endregion

        #region 移动
        /// <summary>
        /// 移动
        /// </summary>
        public void Move()
        {
            if (objView.cadImage.SelectedEntities.Count == 0)
            {
                return;
            }
            int space = 0;
            if (!int.TryParse(objView.SetDistance.ToString(), out space))
            {
                return;
            }
            System.Windows.Forms.Keys key;
            if (objView.rUp.Checked)
            {
                key = System.Windows.Forms.Keys.W;
            }
            else if (objView.rDown.Checked)
            {
                key = System.Windows.Forms.Keys.S;
            }
            else if (objView.rLeft.Checked)
            {
                key = System.Windows.Forms.Keys.A;
            }
            else if (objView.rRight.Checked)
            {
                key = System.Windows.Forms.Keys.D;
            }
            else
            {
                return;
            }
            objView.DirectionKeyPush(key, space);
        }
        #endregion

        /// <summary>
        /// 设置人货梯
        /// </summary>
        internal void SetAsRht()
        {
            objView.Create(EnumOprMode.RhtMode);
            objView.cadPictBox.Refresh();
        }
        /// <summary>
        /// 设置为楼梯
        /// </summary>
        internal void SetAsStair()
        {
            objView.Create(EnumOprMode.StairsMode);
            objView.cadPictBox.Refresh();
        }

        /// <summary>
        /// 设置为卸料平台
        /// </summary>
        internal void SetAsXlPlat()
        {
            objView.Create(EnumOprMode.XlPlat);
            objView.cadPictBox.Refresh();
        }
    }
}
