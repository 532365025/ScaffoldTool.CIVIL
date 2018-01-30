using Autodesk.Revit.DB;
using ScaffoldTool.SFElement;
using System;
using System.Collections.Generic;

namespace ScaffoldTool.Model
{
    /// <summary>
    /// 返回的脚手架信息
    /// </summary>
    public class JsjModel
    {
        /// <summary>
        /// CAD 文件的路径
        /// </summary>
        public string m_strCADPath { get; set; }

        /// <summary>
        /// 内侧集合
        /// </summary>
        public ColumnVertex m_InnerPoint { get; set; }
        /// <summary>
        /// 外侧集合
        /// </summary>
        public ColumnVertex m_OuterPoint { get; set; }

        /// <summary>
        /// 通道宽度
        /// </summary>
        public int m_PasswayWidth { get; set; }
        /// <summary>
        /// 与墙距离默认300
        /// </summary>
        public int m_OffsetToWall { get; set; }
        /// <summary>
        /// 横杆
        /// </summary>
        public List<CrossbarInfo> lstCrossbar { get; set; }
        /// <summary>
        /// 外侧横杆
        /// </summary>
        public List<CrossbarInfo> lstOuterCrossbar { get; set; }

        /// <summary>
        /// 斜杆信息
        /// </summary>
        public List<SkewInfo> lstSkew { get; set; }

        /// <summary>
        /// 脚手板
        /// </summary>
        public List<BoardInfo> lstBoard { get; set; }

        /// <summary>
        /// 悬挑主梁
        /// </summary>
        public List<UBarInfo> lstUBar { get; set; }

        /// <summary>
        /// 悬挑主梁 次梁
        /// </summary>
        //public List<UBarInfoSerialize> lstSecondBeam { get; set; }


        /// <summary>
        /// 楼梯立杆坐标
        /// </summary>
        public List<Xyz> lstStairPoint { get; set; }
        /// <summary>
        /// 平台立杆坐标
        /// </summary>
        public List<Xyz> lstPlatformPoint { get; set; }

        /// <summary>
        /// 建筑轮廓
        /// </summary>
        public List<XcLineSerialize> lstOutLine { get; set; }

        /// <summary>
        /// 人货梯立杆坐标
        /// </summary>
        public List<Xyz> lstRht { get; set; }

        /// <summary>
        /// 卸料平台坐标
        /// </summary>
        public List<Xyz> lstXlPlat { get; set; }
    }

    /// <summary>
    /// 横杆信息
    /// </summary>
    public class CrossbarInfo
    {
        /// <summary>
        /// 横杆起始位置
        /// </summary>
        public XYZ m_StartPt { get; set; }
        /// <summary>
        /// 结束位置
        /// </summary>
        public XYZ m_EndPt { get; set; }
    }

    /// <summary>
    /// 脚手板信息
    /// </summary>
    public class BoardInfo
    {
        public BoardInfo()
        {
            blnRectangle = true;
        }
        /// <summary>
        /// 脚手板轮廓
        /// </summary>
        public List<XYZ> lstLocation { get; set; }

        /// <summary>
        /// 脚手板宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 是否矩形 默认是矩形
        /// </summary>
        public bool blnRectangle { get; set; }
    }

    /// <summary>
    /// 斜杆信息
    /// </summary>
    public class SkewInfo
    {
        /// <summary>
        /// 斜杆起始位置
        /// </summary>
        public XYZ m_StartPt { get; set; }
        /// <summary>
        ///  斜杆结束位置
        /// </summary>
        public XYZ m_EndPt { get; set; }

        /// <summary>
        /// 是否是起始位置高
        /// </summary>
        public bool m_blnStartUp { get; set; }


    }

    /// <summary>
    /// 悬挑主梁
    /// </summary>
    public class UBarInfo
    {
        /// <summary>
        /// 起始位置
        /// </summary>
        public XYZ m_Start { get; set; }
        /// <summary>
        /// 结束位置
        /// </summary>
        public XYZ m_End { get; set; }
    }   
}