using CADImport;
using CADImport.FaceModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Common
{
    internal static class CADImportUtils
    {
        /// <summary>
        /// 添加实体到窗体
        /// </summary>
        /// <param name="cadImage"></param>
        /// <param name="entity"></param>
        /// <param name="isAddToUndo"></param>
        internal static void AddEntityToImage(CADImage cadImage, CADEntity entity, bool isAddToUndo = true)
        {
            cadImage.Converter.Loads(entity);
            cadImage.CurrentLayout.Entities.Add(entity);
            CADImportFace.SelectObject(entity);
            if (isAddToUndo)
                cadImage.AddToUndoList(entity);
        }
        /// <summary>
        /// 绘制圆圈
        /// </summary>
        /// <param name="cadImage"></param>
        /// <param name="center"></param>
        /// <param name="diameter"></param>
        /// <param name="layer"></param>
        /// <param name="color"></param>
        /// <param name="isAddToUndo"></param>
        /// <returns></returns>
        internal static CADCircle DrawCircle(CADImage cadImage, DPoint center, double diameter, string layer, System.Drawing.Color color, bool isAddToUndo = true)
        {
            CADCircle circle = new CADCircle();
            circle.Color = color;
            circle.Point = center;
            circle.Radius = diameter / 609.6;// diameter/2/304.8
            circle.Layer = cadImage.Converter.LayerByName(layer);
            circle.LineWeight = 1;
            AddEntityToImage(cadImage, circle, isAddToUndo);
            return circle;
        }
        /// <summary>
        /// 绘制文字
        /// </summary>
        /// <param name="cadImage"></param>
        /// <param name="leftBottom"></param>
        /// <param name="height"></param>
        /// <param name="layer"></param>
        /// <param name="color"></param>
        /// <param name="text"></param>
        /// <param name="isAddToUndo"></param>
        /// <param name="blnRotate"></param>
        /// <returns></returns>
        internal static CADText DrawText(CADImage cadImage, DPoint leftBottom, double height, string layer, System.Drawing.Color color, string text, bool isAddToUndo = true, bool blnRotate = false)
        {
            CADText cadText = new CADText();
            cadText.Text = text;
            cadText.Color = color;
            cadText.Layer = cadImage.Converter.LayerByName(layer);
            cadText.Height = height / 304.8;
            cadText.Point = leftBottom;
            if (blnRotate)
                cadText.Rotation = 90;
            AddEntityToImage(cadImage, cadText, isAddToUndo);
            return cadText;
        }
        /// <summary>
        /// 绘制线
        /// </summary>
        /// <param name="cadImage"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="color"></param>
        /// <param name="layer"></param>
        /// <param name="lineWeigth"></param>
        /// <param name="isAddToUndo"></param>
        /// <returns></returns>
        internal static CADLine DrawLine(CADImage cadImage, DPoint point1, DPoint point2, System.Drawing.Color color, string layer, double lineWeigth, bool isAddToUndo = true)
        {
            CADLine line = new CADLine();
            line.Point = point1;
            line.Point1 = point2;
            line.Layer = cadImage.Converter.LayerByName(layer);
            line.Color = color;
            line.LineWeight = lineWeigth;
            AddEntityToImage(cadImage, line, isAddToUndo);
            return line;
        }
        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="cadImage"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        /// <param name="layer"></param>
        /// <param name="lineWeight"></param>
        /// <param name="isAddToUndo"></param>
        /// <returns></returns>
        internal static CADPolyLine DrawRectangle(CADImage cadImage, double x1, double y1, double x2, double y2, System.Drawing.Color color, string layer, double lineWeight, bool isAddToUndo = true)
        {
            if (cadImage == null) return null;
            CADPolyLine polyline = new CADPolyLine();
            polyline.Closed = true;
            CADVertex vertex = new CADVertex();
            vertex.Point = new DPoint(x1, y1, 0);
            polyline.AddEntity(vertex);
            vertex = new CADVertex();
            vertex.Point = new DPoint(x2, y1, 0);
            polyline.AddEntity(vertex);
            vertex = new CADVertex();
            vertex.Point = new DPoint(x2, y2, 0);
            polyline.AddEntity(vertex);
            vertex = new CADVertex();
            vertex.Point = new DPoint(x1, y2, 0);
            polyline.AddEntity(vertex);
            polyline.LineWeight = lineWeight;
            polyline.Color = color;
            polyline.Layer = cadImage.Converter.LayerByName(layer);
            AddEntityToImage(cadImage, polyline, isAddToUndo);
            return polyline;
        }
        /// <summary>
        /// 通过点绘制矩形
        /// </summary>
        /// <param name="cadImage"></param>
        /// <param name="circles"></param>
        /// <param name="color"></param>
        /// <param name="layer"></param>
        /// <param name="lineWeight"></param>
        /// <returns></returns>
        internal static CADPolyLine DrawRectByPoints(CADImage cadImage, List<CADCircle> circles, System.Drawing.Color color, string layer, double lineWeight)
        {
            if (cadImage == null) return null;
            double x1 = double.MaxValue, y1 = double.MaxValue;
            double x2 = double.MinValue, y2 = double.MinValue;
            foreach (var entity in circles)
            {
                if (entity.Point.X - entity.Radius < x1)
                    x1 = entity.Point.X - entity.Radius;
                if (entity.Point.Y - entity.Radius < y1)
                    y1 = entity.Point.Y - entity.Radius;
                if (entity.Point.X + entity.Radius > x2)
                    x2 = entity.Point.X + entity.Radius;
                if (entity.Point.Y + entity.Radius > y2)
                    y2 = entity.Point.Y + entity.Radius;
            }
            return DrawRectangle(cadImage, x1, y1, x2, y2, color, layer, lineWeight);
        }
        /// <summary>
        /// 绘制多边形
        /// </summary>
        /// <param name="cadImage"></param>
        /// <param name="color"></param>
        /// <param name="layer"></param>
        /// <param name="lineWeigth"></param>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        internal static CADPolyLine DrawPolygon(CADImage cadImage, System.Drawing.Color color, string layer, double lineWeigth, params DPoint[] coordinates)
        {
            if (cadImage == null) return null;
            CADPolyLine polyline = new CADPolyLine();
            polyline.Closed = true;
            for (int i = 0; i < coordinates.Length; i++)
            {
                CADVertex vertex = new CADVertex();
                vertex.Point = coordinates[i];
                polyline.AddEntity(vertex);
            }
            polyline.Color = color;
            polyline.Layer = cadImage.Converter.LayerByName(layer);
            polyline.LineWeight = lineWeigth;
            AddEntityToImage(cadImage, polyline);
            return polyline;
        }

        /// <summary>
        /// 矩形阴影
        /// </summary>
        /// <param name="cadImage"></param>
        /// <param name="coordinates"></param>
        /// <param name="layerName"></param>
        /// <param name="color"></param>
        /// <param name="lineWeight"></param>
        /// <param name="isSelect"></param>
        /// <returns></returns>
        public static CADEntity DrawRectHatch(CADImage cadImage, DPoint[] coordinates, string layerName, System.Drawing.Color color, double lineWeight = 0.1)
        {
            CADPolyLine cADPolyLine = new CADPolyLine();
            cADPolyLine.Closed = true;
            for (int i = 0; i < coordinates.Length; i++)
            {
                CADVertex vertex = new CADVertex();
                vertex.Point = coordinates[i];
                cADPolyLine.AddEntity(vertex);
            }
            CAD2DBoundaryList cAD2DBoundaryList = new CAD2DBoundaryList();
            CAD2DPolyline cAD2DPolyline = new CAD2DPolyline();
            cAD2DBoundaryList.BoundaryType = 7;
            int count = cADPolyLine.Count;
            CAD2DPoint cAD2DPoint = default(CAD2DPoint);
            cAD2DPolyline.Bulges = new CADCollection<float>();
            for (int i = 0; i < count; i++)
            {
                CADVertex cADVertex = cADPolyLine.Entities[i] as CADVertex;
                cAD2DPoint.X = cADVertex.Point.X;
                cAD2DPoint.Y = cADVertex.Point.Y;
                bool flag = i <= 0 || !(cAD2DPoint == cAD2DPolyline.EndPoint);
                if (flag)
                {
                    cAD2DPolyline.AddVertex(cAD2DPoint);
                }
            }
            cAD2DPolyline.Closed = true;
            cAD2DBoundaryList.Add(cAD2DPolyline);
            CADHatch cADHatch = new CADHatch();
            cADHatch.Layer = cadImage.Converter.LayerByName(layerName);
            cADHatch.LineWeight = lineWeight;
            cADHatch.BoundaryData.Add(cAD2DBoundaryList);
            cADHatch.Color = color;
            cADHatch.HatchName = "USER";
            HatchPatternData hatchPatternData = new HatchPatternData();
            hatchPatternData.baseP = new DPoint(0.0, 0.0, 0.0);
            hatchPatternData.offset = new DPoint(1.0, 1.0, 0.0);
            hatchPatternData.lineAngle = 40f;
            hatchPatternData.isDash = false;
            hatchPatternData.lines = null;
            hatchPatternData.dashNum = 1;
            cADHatch.HatchPatternData.Add(hatchPatternData);
            AddEntityToImage(cadImage, cADHatch, false);
            return cADHatch;
        }

        /// <summary>
        /// 圆形阴影
        /// </summary>
        /// <param name="cadImage"></param>
        /// <param name="coordinates"></param>
        /// <param name="layerName"></param>
        /// <param name="color"></param>
        /// <param name="lineWeight"></param>
        /// <param name="isSelect"></param>
        /// <returns></returns>
        public static CADEntity DrawCircleHatch(CADImage cadImage, CADCircle circles, double radius, string layerName, System.Drawing.Color color, double lineWeight = 0.1)
        {

            CAD2DBoundaryList cAD2DBoundaryList = new CAD2DBoundaryList();
            CAD2DPolyline cAD2DPolyline = new CAD2DPolyline();
            CAD2DArc arc = new CAD2DArc();
            arc.CenterPoint = circles.Point;
            arc.Radius = radius;
            cAD2DBoundaryList.Add(arc);
            CADHatch cADHatch = new CADHatch();
            cADHatch.Layer = cadImage.Converter.LayerByName(layerName);
            cADHatch.LineWeight = lineWeight;
            cADHatch.BoundaryData.Add(cAD2DBoundaryList);
            cADHatch.Color = color;
            cADHatch.HatchName = "USER";
            HatchPatternData hatchPatternData = new HatchPatternData();
            hatchPatternData.baseP = new DPoint(0.0, 0.0, 0.0);
            hatchPatternData.offset = new DPoint(1.0, 1.0, 0.0);
            hatchPatternData.lineAngle = 40f;
            hatchPatternData.isDash = false;
            hatchPatternData.lines = null;
            hatchPatternData.dashNum = 1;
            cADHatch.HatchPatternData.Add(hatchPatternData);
            AddEntityToImage(cadImage, cADHatch, false);
            return cADHatch;
        }

        public static CADEntity DrawCircleHatch(CADImage cadImage, DPoint p, double radius, string layerName, System.Drawing.Color color, double lineWeight = 0.1)
        {

            CAD2DBoundaryList cAD2DBoundaryList = new CAD2DBoundaryList();
            CAD2DPolyline cAD2DPolyline = new CAD2DPolyline();
            CAD2DArc arc = new CAD2DArc();
            arc.CenterPoint = p;
            arc.Radius = radius;
            cAD2DBoundaryList.Add(arc);
            CADHatch cADHatch = new CADHatch();
            cADHatch.Layer = cadImage.Converter.LayerByName(layerName);
            cADHatch.LineWeight = lineWeight;
            cADHatch.BoundaryData.Add(cAD2DBoundaryList);
            cADHatch.Color = color;
            cADHatch.HatchName = "USER";
            HatchPatternData hatchPatternData = new HatchPatternData();
            hatchPatternData.baseP = new DPoint(0.0, 0.0, 0.0);
            hatchPatternData.offset = new DPoint(1.0, 1.0, 0.0);
            hatchPatternData.lineAngle = 40f;
            hatchPatternData.isDash = false;
            hatchPatternData.lines = null;
            hatchPatternData.dashNum = 1;
            cADHatch.HatchPatternData.Add(hatchPatternData);
            AddEntityToImage(cadImage, cADHatch, false);
            return cADHatch;
        }

        /// <summary>
        /// 根据图层名称获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cadImage"></param>
        /// <param name="strLayerName"></param>
        /// <returns></returns>
        internal static List<CADEntity> GetEntitys<T>(CADImage cadImage, string strLayerName = null)
        {
            List<CADEntity> list = new List<CADEntity>();
            foreach (CADEntity current in cadImage.Converter.Entities)
            {
                if (!string.IsNullOrEmpty(strLayerName) && !current.Layer.Name.Equals(strLayerName))
                {
                    continue;
                }
                if (current is T)
                {
                    list.Add(current);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据图层删除
        /// </summary>
        /// <param name="cadImage"></param>
        /// <param name="strLayerName"></param>
        internal static void ClearByLayerName(CADImage cadImage, string strLayerName)
        {
            cadImage.Converter.Entities.RemoveAll(o => o.Layer.Name.Equals(strLayerName));
        }
    }
}
