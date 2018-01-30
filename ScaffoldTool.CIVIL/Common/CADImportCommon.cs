
using Autodesk.Revit.DB;
using CADImport;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Common
{
    public class CADImportCommon
    {
        public static List<XYZ> MoveXYZs(List<XYZ> lst_XYZ, XYZ direction)
        {
            for (int i = 0; i < lst_XYZ.Count; i++)
            {
                lst_XYZ[i] += direction;
            }
            return lst_XYZ;
        }
        public static List<DPoint> MoveDPoints(List<DPoint> lst_DPoint, DPoint direction)
        {
            for (int i = 0; i < lst_DPoint.Count; i++)
            {
                lst_DPoint[i] += direction;
            }
            return lst_DPoint;
        }        
        public static DPoint GetDirection(DPoint startPoint, DPoint endPoint)
        {
            DPoint p1 = endPoint - startPoint;
            p1.Normalize();
            return  p1;
        }
    }
}
