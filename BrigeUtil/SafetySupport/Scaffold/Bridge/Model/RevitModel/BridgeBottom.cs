using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XC.SafetySupport.Scaffold.Bridge.Model.RevitModel
{
    class BridgeBottom
    {

        public BridgeBottom(List<BridgeBottomPoint> left, List<BridgeBottomPoint> right)
        {
            this.RigthPoints = right;
            this.LeftPoints = left;
            AllPoints = new List<BridgeBottomPoint>(left.Count + right.Count);
            LeftEdges = new List<BridgeBottomEdge>();
            RightEdges = new List<BridgeBottomEdge>();
        }

        // 中线该点所有探测点
        public List<BridgeBottomPoint> AllPoints { private set; get; }
        /// <summary>
        /// 中线该点右侧所有的探测点
        /// </summary>
        public List<BridgeBottomPoint> RigthPoints { private set; get; }
        /// <summary>
        /// 中线该点左侧所有的探测点
        /// </summary>
        public List<BridgeBottomPoint> LeftPoints { private set; get; }
        /// <summary>
        /// 该底部截面所在中线点
        /// </summary>
        public CurvePoint PointOnMidline { set; get; }
        /// <summary>
        /// 该点左边所有底边
        /// </summary>
        public List<BridgeBottomEdge> LeftEdges { set; get; }
        /// <summary>
        /// 该点右边所有底边
        /// </summary>
        public List<BridgeBottomEdge> RightEdges { set; get; }


        public BridgeBottomEdge GetEdgeAt(BridgeBottomPoint bottomPoint)
        {
            var p = AllPoints.OrderBy(m => Math.Abs(m.OffsetLengthToMidline - bottomPoint.OffsetLengthToMidline)).FirstOrDefault();
            return p.BelongEdge;
        }

        /// <summary>
        /// 取得线段信息
        /// </summary>
        public void Init()
        {
            AllPoints.AddRange(LeftPoints);
            AllPoints.AddRange(RigthPoints);
            AllPoints.ForEach(m => m.BelongBottom = this);

            LeftPoints = LeftPoints.OrderBy(m => m.OffsetLengthToMidline).ToList();
            RigthPoints = RigthPoints.OrderBy(m => m.OffsetLengthToMidline).ToList();

            LeftEdges = GetEdge(LeftPoints);
            RightEdges = GetEdge(RigthPoints);

        }
        #region MyRegion
        private List<BridgeBottomEdge> GetEdge(List<BridgeBottomPoint> points)
        {
            List<BridgeBottomEdge> result = new List<BridgeBottomEdge>(0);
            #region 处理平面 : 两个相邻的线段的切线向量相近则合并为同一条
            var edges1 = new List<BridgeBottomEdge>();
            var start = points[0];
            for (int i = 1; i < points.Count - 1; i++)
            {
                int ia = i - 1;
                int ib = i;
                int ic = i + 1;
                XYZ vab = points[ib].Point - points[ia].Point;
                XYZ vbc = points[ic].Point - points[ib].Point;
                if (vab.IsAlmostEqualTo(vbc))
                {
                    continue;
                }
                else
                {
                    if (start.Point.IsAlmostEqualTo(points[ib].Point))
                    {// 在曲线上的点单独处理
                        start = points[ic];
                        continue;
                    }
                    var edge = new BridgeBottomEdge()
                    {
                        BelongBottom = this,
                        Start = start,
                        End = points[ib],
                    };
                    edges1.Add(edge);
                    start = points[ic];
                }
            }
            result = edges1;
            #endregion
            #region 处理首位线段在无法识别的曲面上
            var edges2 = new List<BridgeBottomEdge>();
            for (int i = 0; i < edges1.Count; i++)
            {
                if (i == 0)
                {
                    if (edges1[i].Start.Point != points.First().Point)
                    {
                        edges2.Add(new BridgeBottomEdge()
                        {
                            Start = points.First(),
                            End = edges1[i].Start,
                            BelongBottom = this,
                        });
                    }
                    edges2.Add(edges1[i]);
                }
                else if (i == edges1.Count - 1)
                {
                    edges2.Add(edges1[i]);
                    if (edges1[i].End.Point != points.Last().Point)
                    {
                        edges2.Add(new BridgeBottomEdge()
                        {
                            Start = edges1[i].End,
                            End = points.Last(),
                            BelongBottom = this,
                        });
                    }
                }
                else
                {
                    edges2.Add(edges1[i]);
                }
            }
            result = edges2;
            #endregion
            #region 连接各个线段
            var edges3 = new List<BridgeBottomEdge>();
            for (int i = 0; i < edges2.Count; i++)
            {
                var e1 = edges2[i];
                    edges3.Add(e1);
                if (i != edges2.Count - 1)
                {
                    var e2 = edges2[i + 1];
                    var e = new BridgeBottomEdge()
                    {
                        BelongBottom = this,
                        End = e2.Start,
                        Start = e1.End,
                    };
                    edges3.Add(e);
                }
            }
            result = edges3;
            #endregion
            return result;
        }

        private bool IsOnSameEdge(List<BridgeBottomPoint> points, int crrent, int another)
        {
            if (another == -1 || another == points.Count)
            {
                return false;
            }
            else
            {
                var d1 = points[crrent].FaceNomal.Normalize().DotProduct(XYZ.BasisZ.Normalize());
                var d2 = points[another].FaceNomal.Normalize().DotProduct(XYZ.BasisZ.Normalize());
                return Math.Abs(d1-d2) < 0.01;
            }
        } 
        #endregion
    }
}
