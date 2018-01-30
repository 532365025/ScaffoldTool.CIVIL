using Autodesk.Revit.DB;
using ScaffoldTool.Common;
using System;

namespace ScaffoldTool.SFElement
{
    public class ColumnVertex
    {
        private ColumnVertex _next, _previous;
        private int _forwardDistance, _backwardDistance;
        private XYZ _forwardVector, _backwardVector, _outwardVector;

        public XYZ Location { get; set; }

        public ColumnType Type
        {
            get
            {
                XYZ a = -_backwardVector.Round(2);
                XYZ b = _forwardVector.Round(2);

                if (a.IsAlmostEqualTo(b))
                {
                    return ColumnType.OnLine;
                }

                if (a.IsAlmostEqualTo(-XYZ.BasisX))
                {
                    double angle = b.AngleTo(XYZ.BasisY);
                    if (angle <= Math.PI / 2)
                    {
                        return ColumnType.OnConcaveCorner;//凹角
                    }
                    else
                    {
                        return ColumnType.OnConvexCorner;//凸角
                    }
                }
                else if (a.IsAlmostEqualTo(XYZ.BasisX))
                {
                    double angle = b.AngleTo(XYZ.BasisY);
                    if (angle <= Math.PI / 2)
                    {
                        return ColumnType.OnConvexCorner;//凸角                 
                    }
                    else
                    {
                        return ColumnType.OnConcaveCorner;//凹角
                    }
                }
                else if (b.IsAlmostEqualTo(-XYZ.BasisX))
                {
                    double angle = a.AngleTo(XYZ.BasisY);
                    if (angle <= Math.PI / 2)
                    {
                        return ColumnType.OnConvexCorner;//凸角                 
                    }
                    else
                    {
                        return ColumnType.OnConcaveCorner;//凹角
                    }
                }

                double cross = a.X * b.Y - a.Y * b.X;
                if (Math.Abs(cross + 1) < 0.01)
                    return ColumnType.OnConcaveCorner;
                else if (Math.Abs(cross - 1) < 0.01)
                    return ColumnType.OnConvexCorner;
                else
                    return ColumnType.OnLine;
            }
        }

        public int ForwardDistance
        {
            get
            {
                XYZ a = _next.Location;
                return a.DistanceTo(Location).ConvetToMm();
                //   return _forwardDistance;
            }
            //set { _forwardDistance = value; }
        }

        public int BackwardDistance
        {
            get
            {
                XYZ a = _previous.Location;
                return a.DistanceTo(Location).ConvetToMm();
                //   return _backwardDistance;
            }
            //set { _backwardDistance = value; }
        }

        public XYZ ForwardVector
        {
            get
            {
                XYZ a = _next.Location;
                return (a - Location).Normalize().Round(2);
                //return _forwardVector;
            }
            //set { _forwardVector = value; }
        }

        public XYZ BackwardVector
        {
            get
            {
                XYZ a = _previous.Location;
                return (a - Location).Normalize().Round(2);
                //return _backwardVector;
            }
            //set { _backwardVector = value; }
        }

        public XYZ OutwardVector
        {
            get
            {
                XYZ a = -_backwardVector;
                XYZ b = _forwardVector;
                if (clsCommon.CheckDirection(a, b, 2))
                {
                    _outwardVector = b.CrossProduct(XYZ.BasisZ);
                }
                else
                {
                    if (clsCommon.CheckDirection(b, XYZ.BasisX, 2) || clsCommon.CheckDirection(b, XYZ.BasisY, 2))
                    {
                        _outwardVector = b.CrossProduct(XYZ.BasisZ);
                    }
                    else if (clsCommon.CheckDirection(a, XYZ.BasisX, 2) || clsCommon.CheckDirection(a, XYZ.BasisY, 2))
                    {
                        _outwardVector = a.CrossProduct(XYZ.BasisZ);
                    }
                }
                return _outwardVector.Round(2);
            }
            set { _outwardVector = value; }
        }

        public ColumnVertex Next
        {
            get { return _next; }
            set
            {
                _next = value;
                if (value == null) return;
                _forwardVector = (_next.Location - this.Location).Normalize();
                _outwardVector = new XYZ(ForwardVector.Y, -ForwardVector.X, 0);
                _forwardDistance = 300 * (int)(_next.Location.DistanceTo(this.Location) * 304.8 / 300 + 0.5);
            }
        }

        public ColumnVertex Previous
        {
            get { return _previous; }
            set
            {
                _previous = value;
                if (value == null) return;
                _backwardVector = (_previous.Location - this.Location).Normalize();
                _backwardDistance = 300 * (int)(_previous.Location.DistanceTo(this.Location) * 304.8 / 300 + 0.5);
            }
        }

        public ColumnVertex(XYZ location/*, ColumnType type*/)
        {
            Location = location;
            //Type = type;
        }

        public ColumnVertex CreateForward(double offset)
        {
            offset /= 304.8;
            XYZ location = this.Location + this.ForwardVector * offset;
            return new ColumnVertex(location/*, ColumnType.OnLine*/);
        }

        public ColumnVertex CreateBackward(double offset)
        {
            offset /= 304.8;
            XYZ location = this.Location + this.BackwardVector * offset;
            return new ColumnVertex(location/*, ColumnType.OnLine*/);
        }

        //public ColumnVertex[] CreateOutward()
        //{
        //    ColumnVertex[] result = null;
        //    ColumnVertex v1, v2, v3;
        //    switch (this.Type)
        //    {
        //        case ColumnType.OnLine:
        //            //if (_next.Type != ColumnType.OnConcaveCorner)
        //            {
        //                v1 = new ColumnVertex(this.Location + this.OutwardVector * GlobalData.m_dPassWayWidth);
        //                result = new ColumnVertex[] { v1 };
        //            }
        //            break;
        //        case ColumnType.OnConvexCorner:
        //            if (Math.Round(this.ForwardVector.DotProduct(this.BackwardVector), 2) == 0)
        //            {
        //                v1 = new ColumnVertex(this.Location - this.ForwardVector * GlobalData.m_dPassWayWidth/*, ColumnType.OnLine*/);
        //                v2 = new ColumnVertex(this.Location - (this.ForwardVector + this.BackwardVector) * GlobalData.m_dPassWayWidth/*, ColumnType.OnConvexCorner*/);
        //                v3 = new ColumnVertex(this.Location - BackwardVector * GlobalData.m_dPassWayWidth/*, ColumnType.OnLine*/);
        //                if (_next.Type != ColumnType.OnConcaveCorner)
        //                    result = new ColumnVertex[] { v1, v2, v3 };
        //                else
        //                    result = new ColumnVertex[] { v1, v2 };
        //            }
        //            else//斜角情况下。
        //            {
        //                v1 = new ColumnVertex(this.Location + this.ForwardVector.CrossProduct(XYZ.BasisZ) * GlobalData.m_dPassWayWidth);
        //                result = new ColumnVertex[] { v1 };
        //            }
        //            break;
        //        case ColumnType.OnConcaveCorner:
        //            if (_next.Type == ColumnType.OnConvexCorner)
        //            {
        //                v1 = new ColumnVertex(this.Location + this.OutwardVector * GlobalData.m_dPassWayWidth);
        //                result = new ColumnVertex[] { v1 };
        //            }
        //            break;
        //    }
        //    return result;
        //}

        public void Move(XYZ direction, double offset)
        {
            Move(direction * offset / 304.8);
        }

        public void Move(XYZ vector)
        {
            this.Location += vector;
            XYZ v = (_next.Location - this.Location).Normalize();
            int d = 300 * (int)(_next.Location.DistanceTo(this.Location) * 304.8 / 300 + 0.5);
            _forwardVector = v;
            _next._backwardVector = -v;
            _forwardDistance = _next._backwardDistance = d;
            v = (_previous.Location - this.Location).Normalize();
            d = 300 * (int)(_previous.Location.DistanceTo(this.Location) * 304.8 / 300 + 0.5);
            _backwardVector = v;
            _previous._forwardVector = -v;
            _backwardDistance = _previous._forwardDistance = d;
        }
    }

    public enum ColumnType
    {
        /// <summary>
        /// 位于边上的立杆点
        /// </summary>
        OnLine,
        /// <summary>
        /// 位于凸角处的立杆点
        /// </summary>
        OnConvexCorner,
        /// <summary>
        /// 位于凹角处的立杆点
        /// </summary>
        OnConcaveCorner
    };
}