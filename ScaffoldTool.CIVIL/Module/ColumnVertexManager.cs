using Autodesk.Revit.DB;
using CADImport;
using ScaffoldTool.Common;
using ScaffoldTool.SFElement;
using ScaffoldTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScaffoldTool.Module
{
    class ColumnVertexManager : ClsBaseManager<CADCircle>, IOprMode
    {
        internal static System.Drawing.Color innerColor = System.Drawing.Color.Red;
        internal static System.Drawing.Color outerColor = System.Drawing.Color.Yellow;
        internal static double diameter = 48.3;

        public ColumnVertex InnerStart;
        public ColumnVertex OuterStart;

        Dictionary<CADEntity, ColumnVertex> entityVertexMap;

        Stack<ColumnVertexUndoArgs> m_undoStack;
        static Dictionary<Keys, XYZ> KeysXyzMap;

        Dictionary<CADEntity, List<CADEntity[]>> m_dicCADEntity = new Dictionary<CADEntity, List<CADEntity[]>>();
        public ColumnVertexManager(CADImage cadImage) : base(cadImage, GlobalData.m_strInnerLayerName)
        {
            this.cadImage = cadImage;
            entityVertexMap = new Dictionary<CADEntity, ColumnVertex>();

            m_undoStack = new Stack<ColumnVertexUndoArgs>();
        }

        public void Create(ColumnVertex v, bool isInner)
        {
            DPoint p = new DPoint(v.Location.X, v.Location.Y, 0);
            CADCircle c = isInner ?
                CADImportUtils.DrawCircle(cadImage, p, diameter, GlobalData.m_strInnerLayerName, innerColor) :
                CADImportUtils.DrawCircle(cadImage, p, diameter, GlobalData.m_strOuterLayerName, outerColor);
            entityVertexMap.Add(c, v);
        }

        public override void SelectedChanged()
        {
            //RefreshDimensionArrArray();
            foreach (var ent in cadImage.SelectedEntities)
            {
                CreateText(ent);
                if (ent != null && entityVertexMap.ContainsKey(ent))
                {
                    var v = entityVertexMap[ent].Previous;
                    CADEntity findEnt = null;
                    foreach (var item in entityVertexMap)
                    {
                        if (item.Value == v)
                        {
                            findEnt = item.Key;
                            break;
                        }
                    }
                    if (findEnt != null)
                    {
                        CreateText(findEnt);
                    }
                }
            }
        }

        private void CreateText(CADEntity ent)
        {
            if (ent != null && entityVertexMap.ContainsKey(ent))
            {
                RemoveDimension(ent);
                CADEntity[] dimensionArray = new CADEntity[2];
                CADCircle c = ent as CADCircle;
                ColumnVertex v = entityVertexMap[ent];
                bool blnRotation = false;
                double angle = v.ForwardVector.AngleTo(XYZ.BasisY);

                if (angle < Math.PI * 0.25 || angle > Math.PI * 0.75)//(clsCommon.CheckDirection(v.ForwardVector, XYZ.BasisY, 2))
                {
                    blnRotation = true;
                }
                dimensionArray[0] = CADImportUtils.DrawText(cadImage,
                    c.Point + new DPoint(v.ForwardDistance * v.ForwardVector.X / 609.9, v.ForwardDistance * v.ForwardVector.Y / 609.9, 0),
                    100,
                    GlobalData.m_strTextLayerName,
                    System.Drawing.Color.White,
                    v.ForwardDistance.ToString(), false, blnRotation);
                //blnRotation = false;
                //if (clsCommon.CheckDirection(v.BackwardVector, XYZ.BasisY, 2))
                //{
                //    blnRotation = true;
                //}
                //dimensionArray[1] = CADImportUtils.DrawText(cadImage,
                //    c.Point + new DPoint(v.BackwardDistance * v.BackwardVector.X / 609.9, v.BackwardDistance * v.BackwardVector.Y / 609.9, 0),
                //    100,
                //    GlobalData.m_strTextLayerName,
                //    System.Drawing.Color.White,
                //    v.BackwardDistance.ToString(), false, blnRotation);

                DPoint v1 = new DPoint(v.Location.X, v.Location.Y, 0);
                DPoint v2 = new DPoint(v.Location.X + v.ForwardVector.X * 150 / 304.8, v.Location.Y + v.ForwardVector.Y * 150 / 304.8, 0);
                //DPoint v3 = new DPoint(v2.X - (v.ForwardVector.X - v.OutwardVector.X) * 50 / 304.8, v2.Y - (v.ForwardVector.Y - v.OutwardVector.Y) * 50 / 304.8, 0);
                dimensionArray[1] = CADImportUtils.DrawLine(cadImage, v1, v2, ent.Color, GlobalData.m_strTextLayerName, 0.5, false);
                //dimensionArray[3] = CADImportUtils.DrawLine(cadImage, v2, v3, ent.Color, GlobalData.m_strTextLayerName, 0.5, false);
                // if (blnInit)
                List<CADEntity[]> dimensionArrArray = new List<CADEntity[]>();
                dimensionArrArray.Add(dimensionArray);
                m_dicCADEntity.Add(ent, dimensionArrArray);
            }
        }

        private void RemoveDimension(CADEntity ent)
        {
            if (m_dicCADEntity.ContainsKey(ent))
            {
                var now = m_dicCADEntity[ent];
                foreach (var dimensionArr in now)
                {
                    for (int i = 0; i < dimensionArr.Length; i++)
                    {
                        if (dimensionArr[i] != null)
                        {
                            cadImage.RemoveEntity(dimensionArr[i]);
                        }
                    }
                }
                m_dicCADEntity.Remove(ent);
            }
        }

        void ShowText()
        {
            var lstTxt = CADImportUtils.GetEntitys<CADText>(cadImage, GlobalData.m_strTextLayerName);
            if (lstTxt.Count == 0 && GlobalData.blnShowText)
            {
                CreateText();
                return;
            }
            foreach (var item in lstTxt)
            {
                item.Visibility = GlobalData.blnShowText;
            }
            lstTxt = CADImportUtils.GetEntitys<CADLine>(cadImage, GlobalData.m_strTextLayerName);
            foreach (var item in lstTxt)
            {
                item.Visibility = GlobalData.blnShowText;
            }
        }

        void CreateText()
        {
            foreach (var item in entityVertexMap)
            {
                CreateText(item.Key);
            }
        }

        public void blnShow(bool p_blnShow)
        {
            ShowText();
        }

        public void DirectionKeyPush(Keys keyCode, int offset)
        {
            if (KeysXyzMap == null)
            {
                KeysXyzMap = new Dictionary<Keys, XYZ> {
                    { Keys.W, XYZ.BasisY },
                    { Keys.S, -XYZ.BasisY },
                    { Keys.A, -XYZ.BasisX },
                    { Keys.D, XYZ.BasisX }
                };
            }
            XYZ direction = KeysXyzMap[keyCode];
            double dx = offset / 304.8 * direction.X;
            double dy = offset / 304.8 * direction.Y;
            CADEntityCollection affectedEntity = new CADEntityCollection();
            List<ColumnVertex> affectedVertex = new List<ColumnVertex>();
            foreach (var ent in cadImage.SelectedEntities)
            {
                RemoveDimension(ent);
                if (ent != null && entityVertexMap.ContainsKey(ent))
                {
                    ColumnVertex v = entityVertexMap[ent];
                    v.Move(direction, offset);
                    cadImage.SetNewPosEntity(dx, dy, 0, ent);
                    affectedEntity.Add(ent);
                    affectedVertex.Add(v);
                }
                else if (ent != null && ent.Layer.Name == GlobalData.m_strPlatfLayerName)
                {
                    CADCircle c = ent as CADCircle;
                    DPoint pt = (c.Point.ConvertFeetXYZ() + (offset / 304.8 * direction)).ConvertDPoint();
                    cadImage.SetNewPosEntity(dx, dy, 0, ent);
                    // CADImportUtils.DrawCircle(cadImage, pt, ColumnVertexManager.diameter, GlobalData.m_strPlatfLayerName, PlatformManager.color, false);

                }
            }
            if (affectedEntity.Count > 0)
                m_undoStack.Push(new ColumnVertexUndoArgs(affectedEntity, affectedVertex, -dx, -dy, ColumnVertexUndoArgs.UndoType.Move));
            SelectedChanged();
        }
        public void DirectionKeyPushs(Keys keyCode, int offset)
        {
            if (KeysXyzMap == null)
            {
                KeysXyzMap = new Dictionary<Keys, XYZ> {
                    { Keys.W, XYZ.BasisY },
                    { Keys.S, -XYZ.BasisY },
                    { Keys.A, -XYZ.BasisX },
                    { Keys.D, XYZ.BasisX }
                };
            }
            XYZ direction = KeysXyzMap[keyCode];
            double dx = offset / 304.8 * direction.X;
            double dy = offset / 304.8 * direction.Y;
            CADEntityCollection affectedEntity = new CADEntityCollection();
            List<ColumnVertex> affectedVertex = new List<ColumnVertex>();
            foreach (var ent in cadImage.SelectedEntities)
            {
                RemoveDimension(ent);
                if (ent != null)
                {
                    //ColumnVertex v = entityVertexMap[ent];
                    //v.Move(direction, offset);
                    cadImage.SetNewPosEntity(dx, dy, 0, ent);
                    affectedEntity.Add(ent);
                    //affectedVertex.Add(v);
                }
                //else if (ent != null && ent.Layer.Name == GlobalData.m_strPlatfLayerName)
                //{
                //    CADCircle c = ent as CADCircle;
                //    DPoint pt = (c.Point.ConvertFeetXYZ() + (offset / 304.8 * direction)).ConvertDPoint();
                //    cadImage.SetNewPosEntity(dx, dy, 0, ent);
                //    // CADImportUtils.DrawCircle(cadImage, pt, ColumnVertexManager.diameter, GlobalData.m_strPlatfLayerName, PlatformManager.color, false);

                //}
            }
            if (affectedEntity.Count > 0)
                m_undoStack.Push(new ColumnVertexUndoArgs(affectedEntity, affectedVertex, -dx, -dy, ColumnVertexUndoArgs.UndoType.Move));
            SelectedChanged();
        }
        public override void Delect()
        {
            //RefreshDimensionArrArray();

            CADEntityCollection affectedEntity = new CADEntityCollection();
            List<ColumnVertex> affectedVertex = new List<ColumnVertex>();
            foreach (var ent in cadImage.SelectedEntities)
            {
                if (ent != null && entityVertexMap.ContainsKey(ent))
                {
                    CADEntity findEnt = null;
                    RemoveDimension(ent);
                    if (ent != null && entityVertexMap.ContainsKey(ent))
                    {
                        var col = entityVertexMap[ent].Previous;

                        foreach (var item in entityVertexMap)
                        {
                            if (item.Value == col)
                            {
                                findEnt = item.Key;
                                break;
                            }
                        }

                    }

                    ColumnVertex v = entityVertexMap[ent];
                    // 保持记录开头信息
                    if (v == OuterStart)
                    {
                        OuterStart = v.Next;
                    }
                    if (v == InnerStart)
                    {
                        InnerStart = v.Next;
                    }
                    v.Next.Previous = v.Previous;
                    v.Previous.Next = v.Next;
                    //v.Next = null;
                    //v.Previous = null;
                    entityVertexMap.Remove(ent);
                    cadImage.RemoveEntity(ent);
                    affectedEntity.Add(ent);
                    affectedVertex.Add(v);

                    if (findEnt != null)
                    {
                        CreateText(findEnt);
                    }
                }
            }
            if (affectedEntity.Count > 0)
                m_undoStack.Push(new ColumnVertexUndoArgs(affectedEntity, affectedVertex, 0, 0, ColumnVertexUndoArgs.UndoType.Delete));
        }

        public void Copy(List<CADEntity> lstEnt, List<XYZ> lstPt)
        {
            CADEntityCollection affectedEntity = new CADEntityCollection();
            List<ColumnVertex> affectedVertex = new List<ColumnVertex>();
            for (int i = 0; i < lstEnt.Count; i++)
            {
                CADEntity origin = lstEnt[i];
                XYZ newPt = lstPt[i];

                ColumnVertex originCol = entityVertexMap[origin];
                if (entityVertexMap.ContainsKey(origin))
                {
                    originCol = entityVertexMap[origin];
                }
                else
                {
                    return;
                }
                string layer = origin.Layer.Name;
                System.Drawing.Color color = origin.Color;

                ColumnVertex newV;
                XYZ direction = (newPt - originCol.Location).Normalize();
                //originCol.ForwardVector = (originCol.Location - originCol.Previous.Location).Normalize();
                // if (direction.AngleTo(originCol.ForwardVector) <= Math.PI / 2)//向前
                bool blnForward = false;
                if (direction.IsAlmostEqualTo(originCol.ForwardVector) || direction.IsAlmostEqualTo(-originCol.BackwardVector))
                {
                    blnForward = true;
                }
                else if (direction.IsAlmostEqualTo(-originCol.BackwardVector))
                {
                    blnForward = false;
                }
                else if (direction.AngleTo(originCol.ForwardVector) <= Math.PI / 4)
                {
                    blnForward = true;
                }
                else if (direction.IsAlmostEqualTo(-originCol.ForwardVector) || direction.IsAlmostEqualTo(originCol.BackwardVector))
                {
                    blnForward = false;
                }
                else if (direction.AngleTo(originCol.ForwardVector) <= Math.PI / 4 * 3)
                {
                    blnForward = false;
                }

                if (blnForward)
                {
                    XYZ nextDirection = (originCol.Next.Location - newPt).Normalize();
                    newV = new ColumnVertex(newPt);
                    newV.Next = originCol.Next;
                    newV.Previous = originCol;
                    //newV.BackwardVector = -direction;
                    //newV.ForwardVector = nextDirection;
                    //newV.BackwardDistance = (newPt.DistanceTo(originCol.Next.Location)).ConvetToMm();
                    //newV.ForwardDistance = (newPt.DistanceTo(originCol.Location)).ConvetToMm();
                    if (clsCommon.CheckDirection(newV.ForwardVector, XYZ.BasisX, 2) || clsCommon.CheckDirection(newV.ForwardVector, XYZ.BasisY, 2))
                    {
                        newV.OutwardVector = newV.OutwardVector.CrossProduct(XYZ.BasisZ);
                    }
                    else
                    {
                        //斜角
                        newV.OutwardVector = -newV.OutwardVector.CrossProduct(XYZ.BasisZ);
                    }
                    originCol.Next.Previous = newV;
                    originCol.Next = newV;
                }
                else
                {
                    XYZ preDirection = (originCol.Previous.Location - newPt).Normalize();
                    newV = new ColumnVertex(newPt);
                    newV.Next = originCol;
                    newV.Previous = originCol.Previous;
                    //newV.ForwardVector = -direction;
                    //newV.BackwardVector = preDirection;
                    //newV.BackwardDistance = (newPt.DistanceTo(originCol.Previous.Location)).ConvetToMm();
                    //newV.ForwardDistance = (newPt.DistanceTo(originCol.Location)).ConvetToMm();
                    //排除斜角
                    if (clsCommon.CheckDirection(newV.ForwardVector, XYZ.BasisX, 2) || clsCommon.CheckDirection(newV.ForwardVector, XYZ.BasisY, 2))
                    {
                        newV.OutwardVector = newV.OutwardVector.CrossProduct(XYZ.BasisZ);
                    }
                    else
                    {
                        newV.OutwardVector = -newV.OutwardVector.CrossProduct(XYZ.BasisZ);
                    }
                    originCol.Previous.Next = newV;
                    originCol.Previous = newV;
                }
                CADCircle c = CADImportUtils.DrawCircle(cadImage, new DPoint(newV.Location.X, newV.Location.Y, 0), 48.3, layer, color, false);
                entityVertexMap.Add(c, newV);
                affectedEntity.Add(c);
                affectedVertex.Add(newV);
            }
            if (affectedEntity.Count > 0)
                m_undoStack.Push(new ColumnVertexUndoArgs(affectedEntity, affectedVertex, 0, 0, ColumnVertexUndoArgs.UndoType.Copy));
            cadImage.SelectedEntities = affectedEntity;
            SelectedChanged();
        }
        public override void Undo()
        {
            //if (ignoreUndo) return;
            if (m_undoStack.Count == 0) return;
            ColumnVertexUndoArgs args = m_undoStack.Pop();
            switch (args.Type)
            {
                case ColumnVertexUndoArgs.UndoType.Move:
                    foreach (var e in args.AffectedEntitice)
                    {
                        cadImage.SetNewPosEntity(args.Dx, args.Dy, 0, e);
                    }
                    XYZ vector = new XYZ(args.Dx, args.Dy, 0);
                    foreach (var v in args.AffectedVertices)
                    {
                        v.Move(vector);
                    }
                    break;
                case ColumnVertexUndoArgs.UndoType.Delete:
                    for (int i = 0; i < args.AffectedEntitice.Count; i++)
                    {
                        CADImportUtils.AddEntityToImage(cadImage, args.AffectedEntitice[i]);
                        args.AffectedVertices[i].Previous.Next = args.AffectedVertices[i];
                        args.AffectedVertices[i].Next.Previous = args.AffectedVertices[i];
                        entityVertexMap.Add(args.AffectedEntitice[i], args.AffectedVertices[i]);
                    }
                    cadImage.SelectedEntities = args.AffectedEntitice;
                    break;
                case ColumnVertexUndoArgs.UndoType.Copy:
                    foreach (var e in args.AffectedEntitice)
                    {
                        cadImage.RemoveEntity(e);
                        entityVertexMap.Remove(e);
                    }
                    foreach (var v in args.AffectedVertices)
                    {
                        v.Next.Previous = v.Previous;
                        v.Previous.Next = v.Next;
                        //v.Next = null;
                        //v.Previous = null;
                    }
                    break;
            }
            SelectedChanged();
        }

        public EnumOprMode GetOprMode()
        {
            return EnumOprMode.ColumnMode;
        }

        public ColumnVertex GetBindingVertex(CADEntity e)
        {
            if (entityVertexMap.ContainsKey(e))
                return entityVertexMap[e];
            else
                return null;
        }

        public void Render(JsjModel jsjModel)
        {
            InnerStart = jsjModel.m_InnerPoint;
            OuterStart = jsjModel.m_OuterPoint;
            // 绘制内排立杆
            if (jsjModel.m_InnerPoint != null)
            {
                ColumnVertex current = jsjModel.m_InnerPoint;
                do
                {
                    CADCircle cCir = CADImportUtils.DrawCircle(cadImage, current.Location.ConvertDPoint(), diameter, GlobalData.m_strInnerLayerName, innerColor);
                    entityVertexMap.Add(cCir, current);
                    current = current.Next;
                } while (current != jsjModel.m_InnerPoint);
            }
            // 绘制外排立杆
            if (jsjModel.m_OuterPoint != null)
            {
                ColumnVertex current = jsjModel.m_OuterPoint;
                do
                {
                    CADCircle cCir = CADImportUtils.DrawCircle(cadImage, current.Location.ConvertDPoint(), diameter, GlobalData.m_strOuterLayerName, outerColor);
                    entityVertexMap.Add(cCir, current);
                    current = current.Next;
                } while (current != jsjModel.m_OuterPoint);
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public override void Clear()
        {
            entityVertexMap.Clear();
            m_dicCADEntity.Clear();
            m_undoStack.Clear();
        }

        public void OtherOpr()
        {
            SelectedChanged();
        }
    }

    class ColumnVertexUndoArgs
    {
        public enum UndoType { Move, Copy, Delete };

        public CADEntityCollection AffectedEntitice;
        public List<ColumnVertex> AffectedVertices;
        public double Dx, Dy;
        public UndoType Type;

        public ColumnVertexUndoArgs(CADEntityCollection affectedEntitice, List<ColumnVertex> affectedVertice, double dx, double dy, UndoType type)
        {
            AffectedEntitice = affectedEntitice;
            AffectedVertices = affectedVertice;
            Dx = dx;
            Dy = dy;
            Type = type;
        }
    }
}
