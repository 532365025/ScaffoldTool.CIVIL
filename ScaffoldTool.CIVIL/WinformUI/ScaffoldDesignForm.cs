using Autodesk.Revit.DB;
using CADImport;
using ScaffoldTool.Common;
using ScaffoldTool.Model;
using ScaffoldTool.Module;
using ScaffoldTool.SFElement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Viewer.AddEntitys;

namespace ScaffoldTool.WinformUI
{
    public partial class ScaffoldDesignForm : Viewer.CADForm
    {
        int moveDiection = 300;//按AWSD键移动距离
        /// <summary>
        /// 内侧立杆点
        /// </summary>
        internal ColumnVertex OriginStart = null;
        internal ColumnVertex OriginOuter = null;
        ColumnVertexManager vertexBehavior;
        public ColumnVertex InnerStart
        {
            get { return vertexBehavior.InnerStart; }
            set
            {
                vertexBehavior.InnerStart = value;
            }
        }
        public ColumnVertex OuterStart
        {
            get
            {
                return vertexBehavior.OuterStart;

            }
            set
            {
                vertexBehavior.OuterStart = value;

            }
        }
        /// <summary>
        /// 悬挑次梁
        /// </summary>
        internal List<UBarInfoSerialize> lstSecondBeam { get; set; }
        /// <summary>
        /// 原始轮廓线
        /// </summary>
        public List<Line> lstOriginOutLine { get; set; }
        /// <summary>
        /// 原始轮廓扩大后的线
        /// </summary>
        public List<Line> lstEnlargeLoop { get; set; }

        ViewController controller = null;

        //internal CADPictureBox ViewBox { get { return cadPictBox; } }
        /// <summary>
        /// 通道宽度
        /// </summary>
        internal int PassWayWidth = 900;
        /// <summary>
        /// 通道宽度
        /// </summary>
        internal double dPassWayWidth = 0;
        /// <summary>
        /// 返回数据集
        /// </summary>
        internal JsjModel m_objResultJsjModel { get; set; }
        /// <summary>
        /// 操作模式
        /// </summary>
        internal EnumOprMode m_oprMode { get; set; }
        public int SetDistance { get; set; }
        List<IOprMode> lstIOprMode = null;
        /// <summary>
        /// 操作模式
        /// </summary>       
        public ScaffoldDesignForm()
        {
            InitializeComponent();
            this.ControlBox = false;
            this.Text = string.Empty;
        }

        private void ScaffoldDesignForm_Load(object sender, EventArgs e)
        {
            EntitysUtility.CreateCircle(cadImage, 3, new DPoint(40, 40, 0));
            
            AddOutline(lstOriginOutLine, true);
            AddOutline(lstEnlargeLoop, false);
            cadImage.GetExtents();
            cadPictBox.Refresh();
        }

        private void btnItem_Undo_Click(object sender, EventArgs e)
        {
            cadImage.Undo();
            cadPictBox.Invalidate();
        }

        private void btnItem_Redo_Click(object sender, EventArgs e)
        {
            cadImage.Redo();
            cadPictBox.Invalidate();
        }
        /// <summary>
        /// 快捷键设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cadPictBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!this.Focused)
                this.Focus();
            bool ctrlDown = false;
            int cn = 2;
            if (e.Shift)
                cn = 5;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    MoveEntity(0, cn, 0, cn);
                    break;
                case Keys.Down:
                    MoveEntity(0, -cn, 0, -cn);
                    break;
                case Keys.Left:
                    MoveEntity(cn, 0, cn, 0);
                    break;
                case Keys.Right:
                    MoveEntity(-cn, 0, -cn, 0);
                    break;
                case Keys.ControlKey:
                    ctrlDown = true;
                    break;
                case Keys.Delete:
                    cadImage.RemoveEntity(cadImage.SelectedEntities);
                    break;
                case Keys.Escape:
                    ClearSelection();
                    break;
                case Keys.W:
                    MoveEntity(0, moveDiection, 0, moveDiection);
                    break;
                case Keys.S:
                    MoveEntity(0, -moveDiection, 0, -moveDiection);
                    break;
                case Keys.A:
                    MoveEntity(moveDiection, 0, moveDiection, 0);
                    break;
                case Keys.D:
                    MoveEntity(-moveDiection, 0, -moveDiection, 0);
                    break;
                case Keys.Z:
                    if (ctrlDown)
                    {
                        /// 触发Behavior.Undo
                        cadImage.Undo();                        
                    }
                    break;
            }
            cadPictBox.Invalidate();
        }
        /// <summary>
        /// 激活方向键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cadPictBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                e.IsInputKey = true;
        }
        public void ShowText(bool p_blnShow)
        {
            vertexBehavior.blnShow(p_blnShow);
        }
        /// <summary>
        /// 复制立杆
        /// </summary>
        /// <param name="Old"></param>
        /// <param name="ptNew"></param>
        public void Copy(List<CADEntity> Old, List<XYZ> ptNew)
        {
            var objFind = lstIOprMode.Find(o => o.GetOprMode() == EnumOprMode.ColumnMode);
            if (objFind != null)
            {
                var obj = objFind as ColumnVertexManager;
                obj.Copy(Old, ptNew);
            }
        }
        /// <summary>
        /// 删除全部
        /// </summary>
        public void ClearAll()
        {
            cadImage.Converter.Entities.Clear();
            foreach (var item in lstIOprMode)
            {
                item.Clear();
            }
            vertexBehavior.InnerStart = null;  
            vertexBehavior.OuterStart = null;
        }
        /// <summary>
        /// 删除设计元素
        /// </summary>
        public void ClearDesgin()
        {
            foreach (var item in lstIOprMode)
            {
                item.Clear();
            }
        }
        IOprMode GetOprMode()
        {
            return lstIOprMode.Find(o => o.GetOprMode() == m_oprMode);
        }
        IOprMode GetOprMode(EnumOprMode opr)
        {
            return lstIOprMode.Find(o => o.GetOprMode() == opr);
        }
        public void Create(EnumOprMode opr)
        {
            GetOprMode(opr)?.OtherOpr();
        }
        public void DirectionKeyPush(Keys keyCode, int offset = 300)
        {
            /// 触发Behavior.DirectionKeyPush
            vertexBehavior.DirectionKeyPush(keyCode, offset);
            this.Invalidate();
        }
        #region 绘制相关方法
        internal const string SystemLayerName = "SYS_LAYER";
        public void AddOutline(List<Line> outline, bool isOrigin)
        {
            foreach (var line in outline)
            {
                DPoint p1 = new DPoint(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, 0);
                DPoint p2 = new DPoint(line.GetEndPoint(1).X, line.GetEndPoint(1).Y, 0);
                if (isOrigin)
                    CADImportUtils.DrawLine(cadImage, p1, p2, System.Drawing.Color.White, SystemLayerName, 2);
                else
                    CADImportUtils.DrawLine(cadImage, p1, p2, System.Drawing.Color.Green, SystemLayerName, 0.5);
            }
        }

        internal void AddColumnVertex(ColumnVertex start, bool isInner)
        {
            if (isInner)
                vertexBehavior.InnerStart = start;
            else
                vertexBehavior.OuterStart = start;
            ColumnVertex current = start;
            do
            {
                vertexBehavior.Create(current, isInner);
                current = current.Next;
            } while (current != start);
        }
        #endregion       
        private void ribContrlColumn_ItemClick(object sender, EventArgs e)
        {

        }

        private void btnMoveColumn_Click(object sender, EventArgs e)
        {
            controller.Move();
        }

        private void btnCopyColumn_Click(object sender, EventArgs e)
        {
            controller.AddColPostion();
        }

        private void office2007StartButton1_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("欢迎使用！");
        }
    }
}
