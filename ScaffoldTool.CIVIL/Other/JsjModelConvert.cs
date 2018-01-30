using Autodesk.Revit.DB;
using ScaffoldTool.Common;
using ScaffoldTool.Model;
using ScaffoldTool.Module;
using ScaffoldTool.SFElement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Other
{
    /// <summary>
    /// 转换类
    /// </summary>
    public class JsjModelConvert
    {
        /// <summary>
        /// 序列化转换存在本地。
        /// JsjModel不能序列化
        /// </summary>
        /// <param name="p_JsjModel"></param>
        /// <returns></returns>
        internal static JsjModelSerialize ConvertJsjModel(JsjModel p_JsjModel)
        {
            JsjModelSerialize objResult = new JsjModelSerialize();
            objResult.lstStairPoint = p_JsjModel.lstStairPoint;
            objResult.lstPlatformPoint = p_JsjModel.lstPlatformPoint;           
            objResult.m_strCADPath = p_JsjModel.m_strCADPath;
            objResult.lstRht = p_JsjModel.lstRht;
            objResult.lstXlPlat = p_JsjModel.lstXlPlat;
            objResult.m_OffsetToWall = p_JsjModel.m_OffsetToWall;
            List<ColumnVertexSerialize> lstInner = new List<ColumnVertexSerialize>();
            var first = p_JsjModel.m_InnerPoint;
            if (first != null)
            {
                var current = first;
                do
                {
                    ColumnVertexSerialize v = new ColumnVertexSerialize();
                    v.Location = current.Location.ConvertXyz();
                    v.OutwardVector = current.OutwardVector.ConvertXyz();
                    v.Type = current.Type;
                    v.BackwardDistance = current.BackwardDistance;
                    v.BackwardVector = current.BackwardVector.ConvertXyz();
                    v.ForwardDistance = current.ForwardDistance;
                    v.ForwardVector = current.ForwardVector.ConvertXyz();
                    lstInner.Add(v);
                    current = current.Next;
                } while (first != current);
                objResult.m_InnerPoint = lstInner;
            }
            List<ColumnVertexSerialize> lstOut = new List<ColumnVertexSerialize>();
            first = p_JsjModel.m_OuterPoint;
            if (first != null)
            {
                var current = first;
                do
                {
                    ColumnVertexSerialize v = new ColumnVertexSerialize();
                    v.Location = current.Location.ConvertXyz();
                    v.OutwardVector = current.OutwardVector.ConvertXyz();
                    v.Type = current.Type;
                    v.BackwardDistance = current.BackwardDistance;
                    v.BackwardVector = current.BackwardVector.ConvertXyz();
                    v.ForwardDistance = current.ForwardDistance;
                    v.ForwardVector = current.ForwardVector.ConvertXyz();
                    lstOut.Add(v);
                    current = current.Next;
                } while (first != current);
                objResult.m_OuterPoint = lstOut;
            }
            objResult.m_PasswayWidth = p_JsjModel.m_PasswayWidth;
            List<CrossbarInfoSerialize> lstInnerCrossbarSer = new List<CrossbarInfoSerialize>();
            if (p_JsjModel.lstCrossbar != null)
            {
                foreach (var item in p_JsjModel.lstCrossbar)
                {
                    CrossbarInfoSerialize objCrossBar = new CrossbarInfoSerialize();
                    objCrossBar.m_StartPt = item.m_StartPt.ConvertXyz();
                    objCrossBar.m_EndPt = item.m_EndPt.ConvertXyz();
                    lstInnerCrossbarSer.Add(objCrossBar);
                }
                objResult.lstCrossbar = lstInnerCrossbarSer;
            }
            List<CrossbarInfoSerialize> lstOutCrossbarSer = new List<CrossbarInfoSerialize>();
            if (p_JsjModel.lstOuterCrossbar != null)
            {
                foreach (var item in p_JsjModel.lstOuterCrossbar)
                {
                    CrossbarInfoSerialize objCrossBar = new CrossbarInfoSerialize();
                    objCrossBar.m_StartPt = item.m_StartPt.ConvertXyz();
                    objCrossBar.m_EndPt = item.m_EndPt.ConvertXyz();
                    lstOutCrossbarSer.Add(objCrossBar);
                }
                objResult.lstOuterCrossbar = lstOutCrossbarSer;
            }

            List<SkewInfoSerialize> lstSkew = new List<SkewInfoSerialize>();
            if (p_JsjModel.lstSkew != null)
            {
                foreach (var item in p_JsjModel.lstSkew)
                {
                    SkewInfoSerialize objSkew = new SkewInfoSerialize();
                    objSkew.m_blnStartUp = item.m_blnStartUp;
                    objSkew.m_StartPt = item.m_StartPt.ConvertXyz();
                    objSkew.m_EndPt = item.m_EndPt.ConvertXyz();
                    lstSkew.Add(objSkew);
                }
                objResult.lstSkew = lstSkew;
            }
            List<BoardInfoSerialize> lstBoard = new List<BoardInfoSerialize>();
            if (p_JsjModel.lstBoard != null)
            {
                foreach (var item in p_JsjModel.lstBoard)
                {
                    BoardInfoSerialize objBoard = new BoardInfoSerialize();
                    objBoard.Width = item.Width;
                    List<Xyz> lstXyz = new List<Xyz>();
                    foreach (var t in item.lstLocation)
                    {
                        lstXyz.Add(t.ConvertXyz());
                    }
                    objBoard.lstLocation = lstXyz;
                    lstBoard.Add(objBoard);
                }
                objResult.lstBoard = lstBoard;
            }
            if (p_JsjModel.lstUBar != null)
            {
                List<UBarInfoSerialize> lstUBar = new List<UBarInfoSerialize>();
                foreach (var item in p_JsjModel.lstUBar)
                {
                    lstUBar.Add(new UBarInfoSerialize() { m_Start = item.m_Start.ConvertXyz(), m_End = item.m_End.ConvertXyz() });
                }
                objResult.lstUBar = lstUBar;
            }

            return objResult;
        }

        /// <summary>
        /// 转换成 JsjModel
        /// </summary>
        /// <param name="p_JsjModelSer"></param>
        /// <returns></returns>
        internal static JsjModel ConvertJsjModelSerialize(JsjModelSerialize p_JsjModelSer)
        {
            JsjModel objResult = new JsjModel();
            objResult.m_PasswayWidth = p_JsjModelSer.m_PasswayWidth;
            objResult.m_InnerPoint = GetColVertex(p_JsjModelSer.m_InnerPoint);
            objResult.m_OuterPoint = GetColVertex(p_JsjModelSer.m_OuterPoint);
            objResult.lstStairPoint = p_JsjModelSer.lstStairPoint;
            objResult.lstPlatformPoint = p_JsjModelSer.lstPlatformPoint;
            objResult.lstOutLine = p_JsjModelSer.lstOutLine;            
            objResult.m_strCADPath = p_JsjModelSer.m_strCADPath;
            objResult.lstRht = p_JsjModelSer.lstRht;
            objResult.lstXlPlat = p_JsjModelSer.lstXlPlat;
            objResult.m_OffsetToWall = p_JsjModelSer.m_OffsetToWall;
            if (p_JsjModelSer.lstCrossbar != null)
            {
                List<CrossbarInfo> lstCrossbar = new List<CrossbarInfo>();
                foreach (var item in p_JsjModelSer.lstCrossbar)
                {
                    lstCrossbar.Add(new CrossbarInfo() { m_StartPt = item.m_StartPt.ConvertRvtXYZ(), m_EndPt = item.m_EndPt.ConvertRvtXYZ() });
                }
                objResult.lstCrossbar = lstCrossbar;
            }
            if (p_JsjModelSer.lstOuterCrossbar != null)
            {
                List<CrossbarInfo> lstOutCrossbar = new List<CrossbarInfo>();
                foreach (var item in p_JsjModelSer.lstOuterCrossbar)
                {
                    lstOutCrossbar.Add(new CrossbarInfo() { m_StartPt = item.m_StartPt.ConvertRvtXYZ(), m_EndPt = item.m_EndPt.ConvertRvtXYZ() });
                }
                objResult.lstOuterCrossbar = lstOutCrossbar;
            }
            if (p_JsjModelSer.lstSkew != null)
            {
                List<SkewInfo> lstSkew = new List<SkewInfo>();
                foreach (var item in p_JsjModelSer.lstSkew)
                {
                    lstSkew.Add(new SkewInfo() { m_StartPt = item.m_StartPt.ConvertRvtXYZ(), m_EndPt = item.m_EndPt.ConvertRvtXYZ(), m_blnStartUp = item.m_blnStartUp });
                }
                objResult.lstSkew = lstSkew;
            }

            if (p_JsjModelSer.lstBoard != null)
            {
                List<BoardInfo> lstBoard = new List<BoardInfo>();
                foreach (var item in p_JsjModelSer.lstBoard)
                {
                    BoardInfo objBoard = new BoardInfo();
                    objBoard.blnRectangle = item.blnRectangle;
                    List<XYZ> lstXYZ = new List<XYZ>();
                    foreach (var pt in item.lstLocation)
                    {
                        lstXYZ.Add(pt.ConvertRvtXYZ());
                    }
                    objBoard.lstLocation = lstXYZ;
                    lstBoard.Add(objBoard);
                }
                objResult.lstBoard = lstBoard;
            }
            if (p_JsjModelSer.lstUBar != null)
            {
                List<UBarInfo> lstUbar = new List<UBarInfo>();
                foreach (var item in p_JsjModelSer.lstUBar)
                {
                    lstUbar.Add(new UBarInfo() { m_Start = item.m_Start.ConvertRvtXYZ(), m_End = item.m_End.ConvertRvtXYZ() });
                }
                objResult.lstUBar = lstUbar;
            }

            return objResult;
        }

        private static ColumnVertex GetColVertex(List<ColumnVertexSerialize> p_lstInnerPoint)
        {
            if (p_lstInnerPoint == null)
            {
                return null;
            }
            ColumnVertex start = null;
            ColumnVertex pre = null;
            for (int i = 0; i < p_lstInnerPoint.Count; i++)
            {
                var InnerVertext = p_lstInnerPoint[i];
                if (start == null)
                {
                    start = new ColumnVertex(InnerVertext.Location.ConvertRvtXYZ());
                    start.OutwardVector = InnerVertext.OutwardVector.ConvertRvtXYZ();
                    //start.ForwardDistance = InnerVertext.ForwardDistance;
                    //start.ForwardVector = InnerVertext.ForwardVector.ConvertRvtXYZ();
                    //start.BackwardDistance = InnerVertext.BackwardDistance;
                    //start.BackwardVector = InnerVertext.BackwardVector.ConvertRvtXYZ();
                    pre = start;
                }
                else
                {
                    ColumnVertex now = new ColumnVertex(InnerVertext.Location.ConvertRvtXYZ());
                    now.OutwardVector = InnerVertext.OutwardVector.ConvertRvtXYZ();
                    //now.ForwardDistance = InnerVertext.ForwardDistance;
                    //now.ForwardVector = InnerVertext.ForwardVector.ConvertRvtXYZ();
                    //now.BackwardDistance = InnerVertext.BackwardDistance;
                    //now.BackwardVector = InnerVertext.BackwardVector.ConvertRvtXYZ();
                    now.Previous = pre;
                    pre.Next = now;
                    pre = now;
                }
            }
            pre.Next = start;
            start.Previous = pre;
            return start;
        }

        /// <summary>
        /// 存储为Json
        /// </summary>
        /// <param name="p_Jsj"></param>
        /// <param name="projectPath"></param>
        internal static void SaveToJsonFile(JsjModel p_Jsj, string projectPath)
        {

            string filePath = clsCommon.GetXcDataJsPath(projectPath);
            var fileHelper = new FileHelper(filePath);//文件操作类
            string str = JsonCommon.ObjectToJson<JsjModelSerialize>(JsjModelConvert.ConvertJsjModel(p_Jsj));
            fileHelper.FileWriteLine(str);
        }
        /// <summary>
        /// 存储为Json
        /// </summary>
        /// <param name="p_Jsj"></param>
        /// <param name="projectPath"></param>
        internal static void SaveToJsonFile(JsjModelSerialize p_Jsj, string projectPath)
        {
            //FileInfo fileInfo = new FileInfo(projectPath);
            //string projectName = fileInfo.Name;//获取当前项目文件名称
            //string folderPath = Path.GetDirectoryName(projectPath) + "\\" + projectName;//获取当前所在的文件夹路径
            //string filePath = Path.Combine(Path.GetDirectoryName(projectPath), projectName + ".json");
            string filePath = clsCommon.GetXcDataJsPath(projectPath);
            var fileHelper = new FileHelper(filePath);//文件操作类
            string str = JsonCommon.ObjectToJson<JsjModelSerialize>(p_Jsj);
            fileHelper.FileWriteLine(str);
        }

        /// <summary>
        /// 从Json文件转换成JsjModelSerialize
        /// </summary>
        /// <param name="projectPath"></param>
        /// <returns></returns>
        internal static JsjModelSerialize GetJsonModelFromJsonFile(string projectPath)
        {
            string filePath = clsCommon.GetXcDataJsPath(projectPath);
            if (!File.Exists(filePath))
            {
                return null;
            }
            var fileHelper = new FileHelper(filePath);//文件操作类
            var lstLines = fileHelper.FlieReadLines();
            if (lstLines.Count > 0)
                return JsonCommon.JsonToObject<JsjModelSerialize>(lstLines[0]);
            return null;

        }

        public static List<Line> ConvertRevitLine(List<XcLineSerialize> p_lstXcLine)
        {
            if (p_lstXcLine == null)
            {
                return null;
            }
            List<Line> lstLine = new List<Line>();
            foreach (var item in p_lstXcLine)
            {
                lstLine.Add(Line.CreateBound(item.m_StartPt.ConvertRvtXYZ(), item.m_EndPt.ConvertRvtXYZ()));
            }
            return lstLine;
        }
        public static List<XcLineSerialize> ConvertXcLine(List<Line> p_lstXcLine)
        {
            if (p_lstXcLine == null)
            {
                return null;
            }
            List<XcLineSerialize> lstLine = new List<XcLineSerialize>();
            foreach (var item in p_lstXcLine)
            {
                lstLine.Add(new XcLineSerialize() { m_StartPt = item.GetEndPoint(0).ConvertXyz(), m_EndPt = item.GetEndPoint(1).ConvertXyz() });
            }
            return lstLine;
        }        
    }
}
