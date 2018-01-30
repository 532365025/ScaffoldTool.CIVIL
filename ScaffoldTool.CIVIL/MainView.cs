using CADImport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Viewer.AddEntitys;

namespace CADImportCommon
{
    public partial class MainView : Viewer.CADForm
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string layerName = "桥梁平面中心线";
        List<Curve> lstCurve = new List<Curve>();
        List<CADEntity> lstEntity = new List<CADEntity>();
        List<XYZ> lstPoint = new List<XYZ>();
        public MainView()
        {
            InitializeComponent();
            this.cadBoxForm = new CADForm();
            this.SuspendLayout();
            this.cadBoxForm.BackColor = System.Drawing.Color.Black;
            this.cadBoxForm.DoubleBuffering = true;
            this.cadBoxForm.Location = new System.Drawing.Point(0, 0);
            this.cadBoxForm.Name = "scaffoldViewBox1";
            this.cadBoxForm.Ortho = false;
            this.cadBoxForm.Position = new System.Drawing.Point(0, 0);
            this.cadBoxForm.ScrollBars = CADImport.FaceModule.ScrollBarsShow.Automatic;
            this.cadBoxForm.Size = new Size(1335, 696);
            this.cadBoxForm.TabIndex = 0;
            this.cadBoxForm.VirtualSize = new Size(0, 0);
            this.cadBoxForm.Dock = DockStyle.Fill;            
            this.panelBox_Elevation.Controls.Add(cadBoxForm);
            this.ResumeLayout(false);
            this.cadBoxForm.Render();
        }

        private void CADImprot_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //CADImportUtils.DrawCircle()
            //cadBoxForm.AddCircle(new List<XYZ> { new XYZ(50,50,0),new XYZ(0,0,0),new XYZ(200,200,200) },2000);
            //cadBoxForm.Refresh();
            //cadBoxForm.ClearAll();
            string fileName = string.Empty;
            if (DialogResult.OK == openFileDialog1.ShowDialog())
                fileName = openFileDialog1.FileName;
            if (string.IsNullOrEmpty(fileName))
            {
                cadBoxForm.LoadFile(fileName);
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cadBoxForm.AddOutline(new List<Line>{Line.CreateBound(XYZ.Zero,new XYZ(0,20,0)),Line.CreateBound(new XYZ(15, 25, 0),new XYZ(500,500,0)) },false);
            cadBoxForm.Refresh();
        }

        private void CADImprot_Click(object sender, EventArgs e)
        {
            
        }

        private void CADImprot_MouseClick(object sender, MouseEventArgs e)
        {
            this.X = e.X;
            this.Y = e.Y;
            cadBoxForm.AddCircle(new List<XYZ> { new XYZ(this.X, this.Y,0), new XYZ(0,50,0), new XYZ(20, 20,0) }, 2000);
            cadBoxForm.Refresh();
        }

        private void btnLogPlaneLine_Click(object sender, EventArgs e)
        {
            string fileName = string.Empty;
            if (DialogResult.OK == openFileDialog1.ShowDialog())
                fileName = openFileDialog1.FileName;
            lstEntity = cadBoxForm.GetCadLayerInfo(fileName, layerName);
            //List<CADEntity> lstEntity = Utils.GetEntitys<CADEntity>(cadImage, layerName);
            List<XYZ> lstTempPoint = new List<XYZ>();
            foreach (var item in lstEntity)
            {
                foreach (var dp in (item as CADPolyLine).PolyPoints)
                {
                    //XYZ p = new XYZ(dp.X / 0.3048,dp.Y / 0.3048,0);
                    lstTempPoint.Add(dp.ConvertFeetXYZ());                    
                }
            }
            //lstTempPoint = CheckPoint(lstTempPoint);
            int index = lstTempPoint.Count / 2;
            XYZ dirction = lstTempPoint[index].DistanceTo(XYZ.Zero) * (-lstTempPoint[index]).Normalize();
            lstPoint= Common.MoveXYZs(lstTempPoint, dirction);

            foreach (var entity in lstEntity)
            {
                CADPolyLine polyLine = entity as CADPolyLine; 
                List<Line> lstLine = new List<Line>();
                List<XYZ> lstPolyPoint = new List<XYZ>();
                foreach (var dp in polyLine.PolyPoints)
                {
                    lstPolyPoint.Add(dp.ConvertFeetXYZ());
                }
                List<XYZ> lstNewPoint = Common.MoveXYZs(lstPolyPoint, dirction);
                for (int i = 0; i < lstNewPoint.Count-1; i++)
                {
                    try
                    {
                        lstLine.Add(Line.CreateBound(lstNewPoint[i], lstNewPoint[i + 1]));
                    }
                    catch
                    {
                        
                    }
                }
                if (lstLine.Count>0)
                {
                    cadBoxForm.AddLine(lstLine, true);
                    foreach (var line in lstLine)
                    {
                        XYZ p1 = line.GetEndPoint(0).ConvertUnitXYZ(100);
                        XYZ p2 = line.GetEndPoint(1).ConvertUnitXYZ(100);
                        EntitysUtility.CreateLine(cadImage,new DPoint(p1.X, p1.Y, p1.Z), new DPoint(p2.X, p2.Y, p2.Z));
                        cadPictBox.Invalidate();
                    }
                }
            }
        }
        List<XYZ> CheckPoint(List<XYZ> lstPoint)
        {
            if (lstPoint.Count<10)
            {
                return lstPoint;
            }
            int index = lstPoint.Count / 2;
            foreach (var p in lstPoint)
            {
                if (p.DistanceTo(lstPoint[index])>80)
                {
                    lstPoint.Remove(p);
                }
            }
            return lstPoint;
        }

        private void btnVertical_Click(object sender, EventArgs e)
        {
            ChoiceCreateVerticalMode choiceForm = new ChoiceCreateVerticalMode();            
            if (choiceForm.ShowDialog()!=DialogResult.OK)
            {
                return;
            }
            if (choiceForm.isLogin)
            {
                string fileName = string.Empty;
                if (DialogResult.OK == openFileDialog1.ShowDialog())
                    fileName = openFileDialog1.FileName;
                //List<CADEntity> lstEntity = cadBoxForm.GetCadLayerInfo(fileName, layerName);
            }
            else
            {
                XYZ p1 = lstPoint[0];
                XYZ p2 = lstPoint[lstPoint.Count - 1];
                EntitysUtility.CreateLine(cadImage,new DPoint(p1.X,0,0),new DPoint(p2.X,0,0));
                cadPictBox.Invalidate();
            }
        }
    }
}
