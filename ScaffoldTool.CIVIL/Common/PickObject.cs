using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Common
{
    public class PickObject
    {
        public static bool TryPickGenericModel(UIDocument uiDoc, out Reference reference)
        {
            try
            {
                reference = uiDoc.Selection.PickObject(ObjectType.Element, new GenericModelSelectionFilter(), "请选择常规模型");
            }
            catch
            {
                reference = null;
            }
            return reference != null;
        }
    }

    class GenericModelSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_GenericModel;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
