using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace XC.SafetySupport.Scaffold.Bridge.Model.RevitModel
{
    /// <summary>
    /// 桥底边点特性
    /// </summary>
    partial class  BridgeBottomPoint
    {
        /// <summary>
        /// 桥底边点
        /// </summary>
        public XYZ Point { set; get; }
        /// <summary>
        /// 桥底边点所在平面
        /// </summary>
        public Face Face { set; get; }
        /// <summary>
        /// 该点对应桥厚度
        /// </summary>
        public double Thickness { set; get; }
        /// <summary>
        /// 该点面法向量
        /// </summary> 
        public XYZ FaceNomal { set; get; }
    }

    partial class BridgeBottomPoint
    {
        /// <summary>
        /// 该点所在截面
        /// </summary>
        public BridgeBottom BelongBottom { set; get; }
        /// <summary>
        /// 该点所在截面的边
        /// </summary>
        public BridgeBottomEdge BelongEdge { set; get; }
        /// <summary>
        /// 该点距离探测起点的距离
        /// </summary>
        public double OffsetLengthToMidline { set; get; }
        /// <summary>
        /// 该点相对于中点线的向量
        /// </summary>
        public XYZ OffsetVectorToMidline { set; get; }
        /// <summary>
        /// 该点的外相交线
        /// </summary>
        public List<Line> OuterLine { set; get; }
        
    }
}
