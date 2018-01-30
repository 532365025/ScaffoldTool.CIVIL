using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class DoubleEx
{

    /// <summary>
    /// 扩展方法
    /// </summary>
    /// <param name="p_Value">值</param>
    /// <param name="num">保留小数位数</param>
    /// <returns></returns>
    public static double Round(this double p_Value, int num)
    {
        return Math.Round(p_Value, 2);
    }
    /// <summary>
    /// 英尺转换成mm
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static int ConvetToMm(this double d)
    {
        return (int)Math.Round(d * 304.8);
    }

    #region 扩展方法 mm与英尺转换

    /// <summary>
    /// mm转换成英尺
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static double ConvetToFeet(this double d)
    {
        return d / 304.8;
    }
    /// <summary>
    /// mm转换成英尺
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static double ConvetToFeet(this int d)
    {
        return d / 304.8;
    }

    #endregion

}


