using CADImport;
using ScaffoldTool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Module
{
    /// <summary>
    /// 操作基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClsBaseManager<T>
    {
        public CADImage cadImage = null;
        public string layer = null;
        public Stack<RowUndoArgs> undoStack = null;
        public ClsBaseManager(CADImage cadImage, string LayerName)
        {
            this.cadImage = cadImage;
            this.layer = LayerName;
            undoStack = new Stack<RowUndoArgs>();
        }

        /// <summary>
        /// 清空
        /// </summary>
        public virtual void Clear()
        {
            var lstCAD = CADImportUtils.GetEntitys<T>(cadImage, layer);
            foreach (var line in lstCAD)
            {
                cadImage.RemoveEntity(line);
            }
            undoStack.Clear();
        }
        /// <summary>
        /// 撤销
        /// </summary>
        public virtual void Undo()
        {
            if (undoStack.Count == 0) return;
            RowUndoArgs args = undoStack.Pop();
            switch (args.Type)
            {
                case RowUndoArgs.UndoType.Add:
                    cadImage.Undo();
                    break;
                case RowUndoArgs.UndoType.Delect:
                    foreach (var ent in args.AffectedRow)
                    {
                        CADImportUtils.AddEntityToImage(cadImage, ent, false);
                    }
                    break;
            }
        }
        /// <summary>
        /// 删除选择项目
        /// </summary>
        public virtual void Delect()
        {
            List<CADEntity> affectedRow = new List<CADEntity>();
            foreach (var ent in cadImage.SelectedEntities)
            {
                if (ent != null && ent.Layer.Name == layer)
                {
                    cadImage.RemoveEntity(ent);
                    affectedRow.Add(ent);
                }
            }
            if (affectedRow.Count > 0)
                undoStack.Push(new RowUndoArgs(affectedRow, RowUndoArgs.UndoType.Delect));
        }

        /// <summary>
        /// 获取当前层的CAD对象
        /// </summary>
        /// <returns></returns>
        public virtual dynamic GetCADEntities()
        {
            return CADImportUtils.GetEntitys<T>(cadImage, layer);
        }

        public virtual void SelectedChanged()
        {

        }
    }
}
