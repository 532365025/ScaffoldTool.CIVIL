using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Common
{
    public class ClsDetailLine
    {
        public static Element GetLineStyle(Document doc, string lineStyleName, Element LineType, DetailCurve detailCurve, Color c = null)
        {
            if (LineType != null)
            {
                return LineType;
            }
            if (LineType == null)
            {
                var styles = detailCurve.GetLineStyleIds();
                foreach (var styleId in styles)
                {
                    var LineStyle = doc.GetElement(styleId);
                    if (LineStyle.Name.Equals(lineStyleName))
                    {
                        LineType = LineStyle;
                    }
                }
            }
            if (LineType == null)
            {
                LineType = CreateLineStyle(doc, lineStyleName, c);
            }
            return LineType;
        }
        public static Element GetLineStyle(Document doc, string lineStyleName, Element LineType, CurveElement detailCurve, Color c = null)
        {
            if (LineType != null)
            {
                return LineType;
            }
            if (LineType == null)
            {
                var styles = detailCurve.GetLineStyleIds();
                foreach (var styleId in styles)
                {
                    var LineStyle = doc.GetElement(styleId);
                    if (LineStyle.Name.Equals(lineStyleName))
                    {
                        LineType = LineStyle;
                    }
                }
            }
            if (LineType == null)
            {
                LineType = CreateLineStyle(doc, lineStyleName, c);
            }
            return LineType;
        }
        public static Element CreateLineStyle(Document doc, string p_strName, Color c)
        {
            Category lineCat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Lines);
            Category newCat = doc.Settings.Categories.NewSubcategory(lineCat, p_strName);
            //Color newColor = new Color(255, 0, 0);
            if (c != null)
            {
                newCat.LineColor = c;
            }
            newCat.SetLineWeight(1, GraphicsStyleType.Projection);

            doc.Regenerate();
            FilteredElementCollector temc = new FilteredElementCollector(doc);
            temc.OfClass(typeof(GraphicsStyle));
            GraphicsStyle mgs = temc.First(m => (m as GraphicsStyle).GraphicsStyleCategory.Name == p_strName) as GraphicsStyle;

            return mgs;
        }
    }
}
