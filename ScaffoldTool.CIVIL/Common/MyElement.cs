using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Common
{
    public enum RevitVersion
    {
        Revit2016,
        Revit2017,
        Revit2018
    }
    /// <summary>
    /// 从不同版本创建元素
    /// </summary>
    public class MyElement
    {
        /// <summary>
        /// 加载程序集
        /// </summary>
        public static Assembly assembly = Assembly.Load("RevitAPI");
        /// <summary>
        /// 获取Revit版本
        /// </summary>
        public static RevitVersion version = GetVersion();
        public static RevitVersion GetVersion()
        {
            if (assembly.FullName.Contains("Version=18"))
            {
                return RevitVersion.Revit2018;
            }
            else if (assembly.FullName.Contains("Version=17"))
            {
                return RevitVersion.Revit2017;
            }
            else if (assembly.FullName.Contains("Version=16"))
            {
                return RevitVersion.Revit2016;
            }
            return RevitVersion.Revit2016;
        }
        /// <summary>
        /// 通过两个点创建平面
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static Plane CreatePlane(XYZ normal, XYZ origin)
        {

            Plane plane = null;
            object[] pms = new object[] { normal, origin };
            if (version == RevitVersion.Revit2016 || version == RevitVersion.Revit2017)
            {
                Type type = assembly.GetType("Autodesk.Revit.DB.Plane");
                object objPlane = Activator.CreateInstance(type, pms);
                plane = objPlane as Plane;
            }
            else if (version == RevitVersion.Revit2018)
            {
                Type type = assembly.GetType("Autodesk.Revit.DB.Plane");
                MethodInfo method = type.GetMethod("CreateByNormalAndOrigin");
                plane = method.Invoke(null, pms) as Plane;
            }
            return plane;
        }
    }
}
