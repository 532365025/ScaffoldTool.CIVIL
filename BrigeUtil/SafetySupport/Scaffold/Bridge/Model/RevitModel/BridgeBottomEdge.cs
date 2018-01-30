using Autodesk.Revit.DB;

namespace XC.SafetySupport.Scaffold.Bridge.Model.RevitModel
{
    partial class BridgeBottomEdge
    {
        /// <summary>
        /// 该边的起点
        /// </summary>
        public BridgeBottomPoint Start { set; get; }

        /// <summary>
        /// 该边终点
        /// </summary>
        public BridgeBottomPoint End { set; get; }
        /// <summary>
        /// 该边的发向量
        /// </summary>
        public XYZ Normal { get
            {
                return Start.FaceNomal;
            } }
    }

    partial class BridgeBottomEdge
    {
        /// <summary>
        /// 该边所在底截面
        /// </summary>
        public BridgeBottom BelongBottom { set; get; }
    }
}
