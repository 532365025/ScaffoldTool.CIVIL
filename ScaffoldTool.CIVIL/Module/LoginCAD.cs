using CADImport;
using CADImport.RasterImage;
using ScaffoldTool.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Module
{
    public class LoginCAD
    {
        public static CADImage m_LoadCADIamge;
        /// <summary>
        /// 加载CAD文件到m_LoadCADIamge
        /// </summary>
        /// <param name="fileName"></param>
        public static void LoadInCADImage(string fileName)
        {
            m_LoadCADIamge = new CADImage();
            m_LoadCADIamge.InitialNewImage();
            m_LoadCADIamge = CADImage.CreateImageByExtension(fileName);
            
            CADImage.CodePage = System.Text.Encoding.Default.CodePage;
            CADImage.LastLoadedFilePath = Path.GetDirectoryName(fileName);
            CADConst.DefaultSHXParameters.SHXSearchPaths = new CADImport.CADImportForms.SHXForm().SHXPaths;

            if (CADConst.IsWebPath(fileName))
                m_LoadCADIamge.LoadFromWeb(fileName);
            else
                m_LoadCADIamge.LoadFromFile(fileName);
        }
        /// <summary>
        /// 根据CAD文件路径及图层名获取信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static List<CADEntity> GetCadLayerInfo(string fileName, string layerName = null)
        {
            List<CADEntity> lstEntity = new List<CADEntity>();
            if (!File.Exists(fileName))
            {
                return null;
            }
            LoadInCADImage(fileName);
            if (m_LoadCADIamge == null)
            {
                return null;
            }
            var lstCADInfo = CADImportUtils.GetEntitys<CADEntity>(m_LoadCADIamge, layerName);
            if (lstCADInfo.Count > 0)
            {
                foreach (var item in lstCADInfo)
                {
                    lstEntity.Add(item);
                }
            }
            return lstEntity;
        }
    }
}
