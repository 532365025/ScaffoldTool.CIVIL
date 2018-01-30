using Autodesk.Revit.DB;

namespace XC.CommonUtils
{
    /// <summary>
    /// 位置偏移工具
    /// </summary>
    public class LocationMoveTool
    {
        public LocationMoveTool(XYZ direction, double distancePerStep)
        {
            this.Direction = direction;
            this.DistancePerStep = distancePerStep;
        }
        public XYZ Direction { private set; get; }
        public double DistancePerStep { private set; get; }

        public XYZ Forward(XYZ currentLocation , int step = 1)
        {
            var newXyz = new XYZ(currentLocation.X, currentLocation.Y, currentLocation.Z);
            return newXyz += Direction * DistancePerStep * step;
        }
        public XYZ Backward(XYZ currentLocation, int step = 1)
        {
            var newXyz = new XYZ(currentLocation.X, currentLocation.Y, currentLocation.Z);
            return newXyz -= Direction * DistancePerStep * step;
        }
    }
}
