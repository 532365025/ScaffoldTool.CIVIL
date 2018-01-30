using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Model
{
    internal interface IOprMode
    {
        /// <summary>
        /// 获取操作模型
        /// </summary>
        /// <returns></returns>
        EnumOprMode GetOprMode();
        /// <summary>
        /// 选中的时候
        /// </summary>
        void SelectedChanged();

        /// <summary>
        /// 撤销
        /// </summary>
        void Undo();
        /// <summary>
        /// 删除 Delete的时候
        /// </summary>
        void Delect();
        /// <summary>
        /// 加载 渲染
        /// </summary>
        /// <param name="jsjModel"></param>
        void Render(JsjModel jsjModel);
        /// <summary>
        /// 清空
        /// </summary>
        void Clear();
        /// <summary>
        /// 其他操作
        /// </summary>
        void OtherOpr();
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <returns></returns>
        dynamic GetCADEntities();

    }
}
