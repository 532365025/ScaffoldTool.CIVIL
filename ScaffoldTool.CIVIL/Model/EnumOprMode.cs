using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Model
{
    /// <summary>
    /// 操作模式
    /// </summary>
    public enum EnumOprMode
    {
        /// <summary>
        /// 立杆
        /// </summary>
        ColumnMode = 0,
        /// <summary>
        /// 横杆
        /// </summary>
        RowMode,
        /// <summary>
        /// 斜杆
        /// </summary>
        SkewRowMode,
        /// <summary>
        /// 脚手板模式
        /// </summary>
        BoardMode,
        /// <summary>
        /// 悬挑主梁
        /// </summary>
        UbarMode,
        /// <summary>
        /// 楼梯
        /// </summary>
        StairsMode,

        /// <summary>
        /// 平台
        /// </summary>
        PlatForm,
        /// <summary>
        /// 人货梯
        /// </summary>
        RhtMode,
        /// <summary>
        /// 卸料平台
        /// </summary>
        XlPlat,
    }
}
