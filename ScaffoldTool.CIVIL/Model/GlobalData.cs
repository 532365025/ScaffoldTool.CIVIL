namespace ScaffoldTool.Model
{
    public class GlobalData
    {        
        /// <summary>
        /// 卸料平台
        /// </summary>
        internal static readonly string m_strXlPlatLayerName = "卸料平台";

        /// <summary>
        /// 文字
        /// </summary>
        internal static readonly string m_strTextLayerName = "文字";

        /// <summary>
        /// 立杆
        /// </summary>
        internal static readonly string m_strInnerLayerName = "内排立杆";
        internal static readonly string m_strOuterLayerName = "外排立杆";
        /// <summary>
        /// 楼梯
        /// </summary>
        internal static readonly string m_strStairLayerName = "楼梯";
        /// <summary>
        /// 脚手板 
        /// </summary>
        internal static readonly string m_strBoardLayerName = "脚手板";
        /// <summary>
        /// 横杆
        /// </summary>
        internal static readonly string m_strCrossLayerName = "横杆";
        /// <summary>
        /// 斜杆
        /// </summary>
        internal static readonly string m_strSkewLayerName = "斜杆";
        /// <summary>
        /// 悬挑主梁
        /// </summary>
        internal static readonly string m_strUbarLayerName = "悬挑主梁";
        /// <summary>
        /// 平台
        /// </summary>
        internal static readonly string m_strPlatfLayerName = "平台";
        /// <summary>
        /// 人货梯
        /// </summary>
        internal static readonly string m_strRhtLayerName = "人货梯";

        /// <summary>
        /// 过段宽度
        /// </summary>
        internal static double m_dPassWayWidth = 0;

        /// <summary>
        /// 悬挑主梁伸出长度
        /// </summary>
        internal static double m_dExtendLength = 0;
        /// <summary>
        /// 悬挑主梁 长度比例1.25
        /// </summary>
        internal static double m_dLenScale = 1.25;

        /// <summary>
        /// revit 项目路径
        /// </summary>
        internal static string m_strProjectPath = null;

        /// <summary>
        /// 斜拉杆 步距
        /// </summary>
        internal static int m_XlgStep = 5;

        /// <summary>
        /// CAD 文件 默认的墙轮廓线图层 [墙线]
        /// </summary>
        internal static string m_strLoadCADLayerName = "墙线";
        /// <summary>
        /// CAD 文件 [轮廓]
        /// </summary>
        internal static string m_strCADOutLineLayerName = "轮廓";

        /// <summary>
        /// CAD路径
        /// </summary>
        internal static string m_strCADPath = null;
        /// <summary>
        /// 内杆距墙距离
        /// </summary>
        internal static int m_OffsetToWall = 300;
        /// <summary>
        /// 是否显示文字
        /// </summary>
        internal static bool blnShowText = false;
    }
}