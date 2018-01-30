using Autodesk.Revit.DB;
using CADImport;
using ScaffoldTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//public class XYZ
//{
//    public XYZ()
//    {

//    }
//    public XYZ(double x, double y, double z)
//    {
//        this.X = x;
//        this.Y = y;
//        this.Z = z;
//    }
//    public double X { get; set; }
//    public double Y { get; set; }
//    public double Z { get; set; }
//}
public static class ExMethod
{
    /// <summary>
    /// XYZ点转换为DPoint点
    /// </summary>
    /// <param name="p_DPoint"></param>
    /// <returns></returns>
    public static DPoint ConvertDPoint(this XYZ p_DPoint)
    {
        return new DPoint(p_DPoint.X, p_DPoint.Y, p_DPoint.Z);
    }
    /// <summary>
    /// 转换成英尺单位
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static XYZ ConvertFeetXYZ(this DPoint p)
    {
        return new XYZ(p.X.ConvetToFeet(), p.Y.ConvetToFeet(), p.Z.ConvetToFeet());
    }
    /// <summary>
    /// 转换点的单位
    /// </summary>
    /// <param name="p"></param>
    /// <param name="multiple"></param>
    /// <returns></returns>
    public static XYZ ConvertUnitXYZ(this XYZ p, double multiple)
    {
        return new XYZ(p.X * multiple, p.Y * multiple, p.Z * multiple);
    }
    /// <summary>
    /// 按照小数位舍入
    /// </summary>
    /// <param name="p"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static XYZ Round(this XYZ p, int d)
    {
        return new XYZ(Math.Round(p.X, 2), Math.Round(p.Y, 2), Math.Round(p.Z, 2));
    }
    /// <summary>
    /// XYZ点转换为Xyz点
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static Xyz ConvertXyz(this XYZ p)
    {
        return new Xyz(p.X, p.Y, p.Z);
    }
    /// <summary>
    /// Xyz点转换为XYZ点
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static XYZ ConvertRvtXYZ(this Xyz p)
    {
        return new XYZ(p.X, p.Y, p.Z);
    }
    /// <summary>
    /// 转换颜色格式
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static Color ConvertDBColor(this System.Drawing.Color c)
    {
        return new Color(c.R, c.G, c.B);
    }
    /// <summary>
    /// DPiont转换为XYZ点
    /// </summary>
    /// <param name="p_DPoint"></param>
    /// <returns></returns>
    public static XYZ ConvertXYZ(this DPoint p_DPoint)
    {
        return new XYZ(p_DPoint.X, p_DPoint.Y, p_DPoint.Z);
    }
    /// <summary>
    /// 设置XYZ点Z轴
    /// </summary>
    /// <param name="p"></param>
    /// <param name="Z"></param>
    /// <returns></returns>
    public static XYZ SetZ(this XYZ p, double Z)
    {
        return new XYZ(p.X, p.Y, Z);
    }
    /// <summary>
    /// 设置LineZ轴高度
    /// </summary>
    /// <param name="line"></param>
    /// <param name="Z"></param>
    /// <returns></returns>
    public static Line SetZ(this Line line, double Z)
    {
        XYZ p1 = line.GetEndPoint(0);
        XYZ p2 = line.GetEndPoint(1);
        return Line.CreateBound(new XYZ(p1.X, p1.Y, Z), new XYZ(p2.X, p2.Y, Z));
    }
}