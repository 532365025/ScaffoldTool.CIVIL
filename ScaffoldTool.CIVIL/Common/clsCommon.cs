using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CADImport;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScaffoldTool.Common
{
    public class clsCommon
    {
        public delegate void SeletedCADEntities(CADImage cadImage);

        public static CurveArray CreateCurve(List<XYZ> points)
        {
            CurveArray array = new CurveArray();
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i].DistanceTo(points[i + 1]) < 0.01)
                {
                    points[i + 1] = points[i];
                    continue;
                }
                array.Append(Line.CreateBound(points[i], points[i + 1]));
            }
            if (points[points.Count - 1].DistanceTo(points[0]) > 0.01)
            {
                array.Append(Line.CreateBound(points[points.Count - 1], points[0]));
            }
            return array;
        }

        /// <summary>
        /// 选择信息 底部显示
        /// </summary>
        public static SeletedCADEntities Seleted_CADEntities;

        public static bool GetIntersection(Autodesk.Revit.DB.Curve line1, Autodesk.Revit.DB.Curve line2, out XYZ p_xyz)
        {
            p_xyz = null;
            IntersectionResultArray results;//交点数组
            SetComparisonResult result = line1.Intersect(line2, out results);

            if (result != Autodesk.Revit.DB.SetComparisonResult.Overlap)//重叠，没有重叠就是平行
                return false;

            if (results == null || results.Size != 1)//没有交点或者交点不是1个
                return false;

            IntersectionResult iResult = results.get_Item(0);//取得交点
            XYZ intersectionPoint = iResult.XYZPoint;//取得交点坐标

            p_xyz = intersectionPoint;
            return true;
        }

        public static bool CheckDirection(XYZ pointTemp1, XYZ pointTemp2, int v)
        {
            XYZ point1 = pointTemp1 + pointTemp2;//相反方向相加抵消0
            XYZ point2 = pointTemp1 - pointTemp2;//相同方向相加也为0
            double x = Math.Round(point1.DistanceTo(new XYZ()), v);
            double y = Math.Round(point2.DistanceTo(new XYZ()), v);
            if (x == 0 || y == 0)
            {
                return true;
            }
            return false;
        }

        public static void DrawCircle(Document doc, XYZ pos, double radius)
        {
            ModelCurve line = doc.Create.NewModelCurve(Arc.Create(pos, radius / 304.8, 0, Math.PI * 2, XYZ.BasisX, XYZ.BasisY),
                   SketchPlane.Create(doc, MyElement.CreatePlane(XYZ.BasisZ, new XYZ(0, 0, pos.Z))));
        }

        public static void DrawLine(Document doc, Line line)
        {
            doc.Create.NewModelCurve(line, SketchPlane.Create(doc, MyElement.CreatePlane(XYZ.BasisZ, new XYZ(0, 0, line.Origin.Z))));
        }
        /// <summary>
        /// 删除Json文件
        /// </summary>
        /// <param name="projectPath"></param>
        public static void DelJsonFile(string projectPath)
        {
            string filePath = GetXcDataJsPath(projectPath);
            if (!File.Exists(filePath))
            {
                return;
            }
            string strFolder = Path.Combine(GetXcDataFolder(projectPath), "bak");
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            FileInfo f = new FileInfo(projectPath);
            string fileName = f.Name.Replace(".rvt", "");
            string strBakfile = Path.Combine(strFolder, string.Format("{0}_{1}.json", fileName, string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now)));
            File.Copy(filePath, strBakfile, true);
            File.Delete(filePath);
        }
        /// <summary>
        /// 获取Json文件夹路径
        /// </summary>
        /// <param name="projectPath"></param>
        /// <returns></returns>
        public static string GetXcDataJsPath(string projectPath)
        {
            string strFolder = Path.GetDirectoryName(projectPath);
            FileInfo f = new FileInfo(projectPath);
            string fileName = f.Name.Replace(".rvt", "");
            if (!Directory.Exists(Path.Combine(strFolder, fileName)))
            {
                Directory.CreateDirectory(Path.Combine(strFolder, fileName));
            }
            return Path.Combine(strFolder, fileName, fileName + ".json");
        }

        /// <summary>
        /// 获取数据文件夹
        /// </summary>
        /// <param name="projectPath"></param>
        /// <returns></returns>
        public static string GetXcDataFolder(string projectPath)
        {
            // string strFolder = Path.GetDirectoryName(projectPath);
            FileInfo f = new FileInfo(projectPath);
            string fileName = f.Name.Replace(".rvt", "");
            string strFolder = Path.Combine(Path.GetDirectoryName(projectPath), fileName);
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            return strFolder;
        }


        public static bool PickOneLinkElem(Document doc, out ElementId elementId)
        {
            elementId = ElementId.InvalidElementId;
            try
            {
                Reference reference = new UIDocument(doc).Selection.PickObject(ObjectType.Element, LinkElementPickFilter.Single, "请选择链接图元");
                Element elem = doc.GetElement(reference);
                elementId = (elem != null) ? elem.Id : ElementId.InvalidElementId;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string GetLinkCADFilePath(Document doc, ElementId linkId)
        {
            try
            {
                Element elem1 = doc.GetElement(linkId);
                Element elem2 = doc.GetElement(elem1.GetTypeId());
                ModelPath path = ModelPathUtils.ConvertUserVisiblePathToModelPath(doc.PathName);
                TransmissionData data = TransmissionData.ReadTransmissionData(path);
                foreach (ElementId id in data.GetAllExternalFileReferenceIds())
                {
                    ExternalFileReference fileReferenceData = data.GetDesiredReferenceData(id);
                    string filePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(fileReferenceData.GetAbsolutePath());
                    if (filePath.EndsWith("\\" + elem2.Name))
                    {
                        return filePath;
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

    }
    internal class LinkElementPickFilter : ISelectionFilter
    {
        public static LinkElementPickFilter Single = new LinkElementPickFilter();

        public bool AllowElement(Element elem)
        {
            return ((elem != null) && (elem is ImportInstance));
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }

}