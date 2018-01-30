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
    /// 悬挑主梁
    /// </summary>
    class UbarManager : ClsBaseManager<CADLine>, IOprMode
    {
        internal static System.Drawing.Color color = System.Drawing.Color.White;
        internal static double lineWeight = 0.2;
        public UbarManager(CADImage cadImage) : base(cadImage, GlobalData.m_strUbarLayerName)
        {

        }
        public void OtherOpr()
        {

        }
        public EnumOprMode GetOprMode()
        {
            return EnumOprMode.UbarMode;
        }

        public void Render(JsjModel jsjModel)
        {
            if (jsjModel.lstUBar != null)
            {
                foreach (var ubarInfo in jsjModel.lstUBar)
                {
                    CADImportUtils.DrawLine(cadImage, ubarInfo.m_Start.ConvertDPoint(), ubarInfo.m_End.ConvertDPoint(), color, GlobalData.m_strUbarLayerName, lineWeight, false);
                }
            }
        }
    }
}
