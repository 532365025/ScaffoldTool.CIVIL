using ScaffoldTool.SFElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Model
{
    [Serializable]
    /// <summary>
    /// 返回的脚手架信息
    /// </summary>
    internal class JsjModelSerialize
    {
        /// <summary>
        /// CAD 文件的路径
        /// </summary>
        public string m_strCADPath { get; set; }
        /// <summary>
        /// 内侧集合
        /// </summary>
        public List<ColumnVertexSerialize> m_InnerPoint { get; set; }
        /// <summary>
        /// 外侧集合
        /// </summary>
        public List<ColumnVertexSerialize> m_OuterPoint { get; set; }

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
        public List<CrossbarInfoSerialize> lstCrossbar { get; set; }
        /// <summary>
        /// 外侧横杆
        /// </summary>
        public List<CrossbarInfoSerialize> lstOuterCrossbar { get; set; }

        /// <summary>
        /// 斜杆信息
        /// </summary>
        public List<SkewInfoSerialize> lstSkew { get; set; }

        /// <summary>
        /// 脚手板
        /// </summary>
        public List<BoardInfoSerialize> lstBoard { get; set; }

        /// <summary>
        /// 悬挑主梁
        /// </summary>
        public List<UBarInfoSerialize> lstUBar { get; set; }
        /// <summary>
        /// 悬挑主梁 次梁
        /// </summary>
        public List<UBarInfoSerialize> lstSecondBeam { get; set; }

        /// <summary>
        /// 楼梯立杆坐标
        /// </summary>
        public List<Xyz> lstStairPoint { get; set; }

        /// <summary>
        /// 平台坐标
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
    [Serializable]
    public class XcLineSerialize
    {
        /// <summary>
        /// 起始位置
        /// </summary>
        public Xyz m_StartPt { get; set; }
        /// <summary>
        /// 结束位置
        /// </summary>
        public Xyz m_EndPt { get; set; }
    }


    [Serializable]
    /// <summary>
    /// 横杆信息
    /// </summary>
    public class CrossbarInfoSerialize
    {
        /// <summary>
        /// 横杆起始位置
        /// </summary>
        public Xyz m_StartPt { get; set; }
        /// <summary>
        /// 结束位置
        /// </summary>
        public Xyz m_EndPt { get; set; }
    }
    [Serializable]
    /// <summary>
    /// 脚手板信息
    /// </summary>
    public class BoardInfoSerialize
    {
        public BoardInfoSerialize()
        {
            blnRectangle = true;
        }
        /// <summary>
        /// 脚手板轮廓
        /// </summary>
        public List<Xyz> lstLocation { get; set; }
        ///// <summary>
        ///// 脚手板中心线起始位置
        ///// </summary>
        //public XYZ m_StartPt { get; set; }
        ///// <summary>
        /////  脚手板中心线结束位置
        ///// </summary>
        //public XYZ m_EndPt { get; set; }

        /// <summary>
        /// 脚手板宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 是否矩形 默认是矩形
        /// </summary>
        public bool blnRectangle { get; set; }
    }
    [Serializable]
    /// <summary>
    /// 斜杆信息
    /// </summary>
    public class SkewInfoSerialize
    {
        /// <summary>
        /// 斜杆起始位置
        /// </summary>
        public Xyz m_StartPt { get; set; }
        /// <summary>
        ///  斜杆结束位置
        /// </summary>
        public Xyz m_EndPt { get; set; }

        /// <summary>
        /// 是否是起始位置高
        /// </summary>
        public bool m_blnStartUp { get; set; }
    }

    [Serializable]
    public class Xyz
    {
        public double X;
        public double Y;
        public double Z;
        public Xyz(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    [Serializable]
    public class ColumnVertexSerialize
    {
        // Properties
        public int BackwardDistance { get; set; }
        public Xyz BackwardVector { get; set; }
        public int ForwardDistance { get; set; }
        public Xyz ForwardVector { get; set; }
        public Xyz Location { get; set; }
        public Xyz OutwardVector { get; set; }
        public ColumnType Type { get; set; }
    }

    /// <summary>
    /// 悬挑主梁
    /// </summary>
    [Serializable]
    public class UBarInfoSerialize
    {
        /// <summary>
        /// 起始位置
        /// </summary>
        public Xyz m_Start { get; set; }
        /// <summary>
        /// 结束位置
        /// </summary>
        public Xyz m_End { get; set; }
    }

}
