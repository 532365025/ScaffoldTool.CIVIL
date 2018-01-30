namespace ScaffoldTool.WinformUI
{
    partial class ScaffoldDesignForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScaffoldDesignForm));
            this.styleManager1 = new DevComponents.DotNetBar.StyleManager();
            this.styleManager2 = new DevComponents.DotNetBar.StyleManager();
            this.buttonItem14 = new DevComponents.DotNetBar.ButtonItem();
            this.mainRibbonControl = new DevComponents.DotNetBar.RibbonControl();
            this.ribbonPanel1 = new DevComponents.DotNetBar.RibbonPanel();
            this.ribContrlColumn = new DevComponents.DotNetBar.RibbonBar();
            this.rUp = new DevComponents.DotNetBar.CheckBoxItem();
            this.rDown = new DevComponents.DotNetBar.CheckBoxItem();
            this.rLeft = new DevComponents.DotNetBar.CheckBoxItem();
            this.rRight = new DevComponents.DotNetBar.CheckBoxItem();
            this.btnMoveColumn = new DevComponents.DotNetBar.ButtonItem();
            this.btnCopyColumn = new DevComponents.DotNetBar.ButtonItem();
            this.ribar_Crossbar = new DevComponents.DotNetBar.RibbonBar();
            this.btnCreateCrossbar = new DevComponents.DotNetBar.ButtonItem();
            this.ribar_Column = new DevComponents.DotNetBar.RibbonBar();
            this.btnCreateColumn = new DevComponents.DotNetBar.ButtonItem();
            this.btnItem_Undo = new DevComponents.DotNetBar.ButtonItem();
            this.btnItem_Redo = new DevComponents.DotNetBar.ButtonItem();
            this.ribTabItem = new DevComponents.DotNetBar.RibbonTabItem();
            this.comboBoxEx1 = new DevComponents.DotNetBar.ComboBoxItem();
            this.office2007StartButton1 = new DevComponents.DotNetBar.Office2007StartButton();
            this.itemContainer1 = new DevComponents.DotNetBar.ItemContainer();
            this.itemContainer2 = new DevComponents.DotNetBar.ItemContainer();
            this.itemContainer3 = new DevComponents.DotNetBar.ItemContainer();
            this.buttonItem2 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem3 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem4 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem5 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem6 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem7 = new DevComponents.DotNetBar.ButtonItem();
            this.galleryContainer1 = new DevComponents.DotNetBar.GalleryContainer();
            this.labelItem8 = new DevComponents.DotNetBar.LabelItem();
            this.buttonItem8 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem9 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem10 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem11 = new DevComponents.DotNetBar.ButtonItem();
            this.itemContainer4 = new DevComponents.DotNetBar.ItemContainer();
            this.buttonItem12 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem13 = new DevComponents.DotNetBar.ButtonItem();
            this.btnItem = new DevComponents.DotNetBar.ButtonItem();
            this.qatCustomizeItem1 = new DevComponents.DotNetBar.QatCustomizeItem();
            this.styleManager3 = new DevComponents.DotNetBar.StyleManager();
            this.mainRibbonControl.SuspendLayout();
            this.ribbonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cadPictBox
            // 
            this.cadPictBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cadPictBox.Location = new System.Drawing.Point(0, 154);
            this.cadPictBox.Size = new System.Drawing.Size(1498, 687);
            this.cadPictBox.VirtualSize = new System.Drawing.Size(423, 423);
            this.cadPictBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cadPictBox_KeyDown);
            this.cadPictBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cadPictBox_PreviewKeyDown);
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue;
            // 
            // styleManager2
            // 
            this.styleManager2.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue;
            // 
            // buttonItem14
            // 
            this.buttonItem14.Name = "buttonItem14";
            this.buttonItem14.SubItemsExpandWidth = 14;
            this.buttonItem14.Text = "buttonItem14";
            // 
            // mainRibbonControl
            // 
            this.mainRibbonControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.mainRibbonControl.BackgroundStyle.Class = "";
            this.mainRibbonControl.CaptionVisible = true;
            this.mainRibbonControl.Controls.Add(this.ribbonPanel1);
            this.mainRibbonControl.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.btnItem_Undo,
            this.btnItem_Redo,
            this.ribTabItem,
            this.comboBoxEx1});
            this.mainRibbonControl.KeyTipsFont = new System.Drawing.Font("Tahoma", 7F);
            this.mainRibbonControl.Location = new System.Drawing.Point(0, 0);
            this.mainRibbonControl.Name = "mainRibbonControl";
            this.mainRibbonControl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.mainRibbonControl.QuickToolbarItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.office2007StartButton1,
            this.btnItem,
            this.qatCustomizeItem1});
            this.mainRibbonControl.Size = new System.Drawing.Size(1498, 154);
            this.mainRibbonControl.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.mainRibbonControl.TabGroupHeight = 14;
            this.mainRibbonControl.TabIndex = 2;
            this.mainRibbonControl.Text = "ribbonControl1";
            // 
            // ribbonPanel1
            // 
            this.ribbonPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonPanel1.Controls.Add(this.ribContrlColumn);
            this.ribbonPanel1.Controls.Add(this.ribar_Crossbar);
            this.ribbonPanel1.Controls.Add(this.ribar_Column);
            this.ribbonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ribbonPanel1.Location = new System.Drawing.Point(0, 58);
            this.ribbonPanel1.Name = "ribbonPanel1";
            this.ribbonPanel1.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ribbonPanel1.Size = new System.Drawing.Size(1498, 94);
            // 
            // 
            // 
            this.ribbonPanel1.Style.Class = "";
            // 
            // 
            // 
            this.ribbonPanel1.StyleMouseDown.Class = "";
            // 
            // 
            // 
            this.ribbonPanel1.StyleMouseOver.Class = "";
            this.ribbonPanel1.TabIndex = 1;
            // 
            // ribContrlColumn
            // 
            this.ribContrlColumn.AutoOverflowEnabled = true;
            // 
            // 
            // 
            this.ribContrlColumn.BackgroundMouseOverStyle.Class = "";
            // 
            // 
            // 
            this.ribContrlColumn.BackgroundStyle.Class = "";
            this.ribContrlColumn.ContainerControlProcessDialogKey = true;
            this.ribContrlColumn.Dock = System.Windows.Forms.DockStyle.Left;
            this.ribContrlColumn.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.rUp,
            this.rDown,
            this.rLeft,
            this.rRight,
            this.btnMoveColumn,
            this.btnCopyColumn});
            this.ribContrlColumn.Location = new System.Drawing.Point(141, 0);
            this.ribContrlColumn.Name = "ribContrlColumn";
            this.ribContrlColumn.Size = new System.Drawing.Size(232, 91);
            this.ribContrlColumn.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribContrlColumn.TabIndex = 2;
            this.ribContrlColumn.Text = "立杆操作";
            // 
            // 
            // 
            this.ribContrlColumn.TitleStyle.Class = "";
            // 
            // 
            // 
            this.ribContrlColumn.TitleStyleMouseOver.Class = "";
            this.ribContrlColumn.ItemClick += new System.EventHandler(this.ribContrlColumn_ItemClick);
            // 
            // rUp
            // 
            this.rUp.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.rUp.Checked = true;
            this.rUp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rUp.Name = "rUp";
            this.rUp.Text = "上";
            // 
            // rDown
            // 
            this.rDown.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.rDown.Name = "rDown";
            this.rDown.Text = "下";
            // 
            // rLeft
            // 
            this.rLeft.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.rLeft.Name = "rLeft";
            this.rLeft.Text = "左";
            // 
            // rRight
            // 
            this.rRight.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.rRight.Name = "rRight";
            this.rRight.Text = "右";
            // 
            // btnMoveColumn
            // 
            this.btnMoveColumn.Name = "btnMoveColumn";
            this.btnMoveColumn.SubItemsExpandWidth = 14;
            this.btnMoveColumn.Text = "移动";
            this.btnMoveColumn.Click += new System.EventHandler(this.btnMoveColumn_Click);
            // 
            // btnCopyColumn
            // 
            this.btnCopyColumn.Name = "btnCopyColumn";
            this.btnCopyColumn.SubItemsExpandWidth = 14;
            this.btnCopyColumn.Text = "复制";
            this.btnCopyColumn.Click += new System.EventHandler(this.btnCopyColumn_Click);
            // 
            // ribar_Crossbar
            // 
            this.ribar_Crossbar.AutoOverflowEnabled = true;
            // 
            // 
            // 
            this.ribar_Crossbar.BackgroundMouseOverStyle.Class = "";
            // 
            // 
            // 
            this.ribar_Crossbar.BackgroundStyle.Class = "";
            this.ribar_Crossbar.ContainerControlProcessDialogKey = true;
            this.ribar_Crossbar.Dock = System.Windows.Forms.DockStyle.Left;
            this.ribar_Crossbar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.btnCreateCrossbar});
            this.ribar_Crossbar.Location = new System.Drawing.Point(72, 0);
            this.ribar_Crossbar.Name = "ribar_Crossbar";
            this.ribar_Crossbar.Size = new System.Drawing.Size(69, 91);
            this.ribar_Crossbar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribar_Crossbar.TabIndex = 1;
            this.ribar_Crossbar.Text = "横杆";
            // 
            // 
            // 
            this.ribar_Crossbar.TitleStyle.Class = "";
            // 
            // 
            // 
            this.ribar_Crossbar.TitleStyleMouseOver.Class = "";
            // 
            // btnCreateCrossbar
            // 
            this.btnCreateCrossbar.Name = "btnCreateCrossbar";
            this.btnCreateCrossbar.SubItemsExpandWidth = 14;
            this.btnCreateCrossbar.Text = "生成横杆";
            // 
            // ribar_Column
            // 
            this.ribar_Column.AutoOverflowEnabled = true;
            // 
            // 
            // 
            this.ribar_Column.BackgroundMouseOverStyle.Class = "";
            // 
            // 
            // 
            this.ribar_Column.BackgroundStyle.Class = "";
            this.ribar_Column.ContainerControlProcessDialogKey = true;
            this.ribar_Column.Dock = System.Windows.Forms.DockStyle.Left;
            this.ribar_Column.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.btnCreateColumn});
            this.ribar_Column.Location = new System.Drawing.Point(3, 0);
            this.ribar_Column.Name = "ribar_Column";
            this.ribar_Column.Size = new System.Drawing.Size(69, 91);
            this.ribar_Column.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribar_Column.TabIndex = 0;
            this.ribar_Column.Text = "立杆";
            // 
            // 
            // 
            this.ribar_Column.TitleStyle.Class = "";
            // 
            // 
            // 
            this.ribar_Column.TitleStyleMouseOver.Class = "";
            // 
            // btnCreateColumn
            // 
            this.btnCreateColumn.Name = "btnCreateColumn";
            this.btnCreateColumn.SubItemsExpandWidth = 14;
            this.btnCreateColumn.Text = "生成立杆";
            // 
            // btnItem_Undo
            // 
            this.btnItem_Undo.Icon = ((System.Drawing.Icon)(resources.GetObject("btnItem_Undo.Icon")));
            this.btnItem_Undo.Name = "btnItem_Undo";
            this.btnItem_Undo.Text = "buttonItem1";
            this.btnItem_Undo.Click += new System.EventHandler(this.btnItem_Undo_Click);
            // 
            // btnItem_Redo
            // 
            this.btnItem_Redo.Icon = ((System.Drawing.Icon)(resources.GetObject("btnItem_Redo.Icon")));
            this.btnItem_Redo.Name = "btnItem_Redo";
            this.btnItem_Redo.Text = "buttonItem15";
            this.btnItem_Redo.Click += new System.EventHandler(this.btnItem_Redo_Click);
            // 
            // ribTabItem
            // 
            this.ribTabItem.Checked = true;
            this.ribTabItem.Name = "ribTabItem";
            this.ribTabItem.Panel = this.ribbonPanel1;
            this.ribTabItem.Text = "生成";
            // 
            // comboBoxEx1
            // 
            this.comboBoxEx1.DropDownHeight = 106;
            this.comboBoxEx1.DropDownWidth = 400;
            this.comboBoxEx1.ItemHeight = 16;
            this.comboBoxEx1.Name = "comboBoxEx1";
            // 
            // office2007StartButton1
            // 
            this.office2007StartButton1.AutoExpandOnClick = true;
            this.office2007StartButton1.CanCustomize = false;
            this.office2007StartButton1.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.Image;
            this.office2007StartButton1.Icon = ((System.Drawing.Icon)(resources.GetObject("office2007StartButton1.Icon")));
            this.office2007StartButton1.ImagePaddingHorizontal = 2;
            this.office2007StartButton1.ImagePaddingVertical = 2;
            this.office2007StartButton1.Name = "office2007StartButton1";
            this.office2007StartButton1.ShowSubItems = false;
            this.office2007StartButton1.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.itemContainer1,
            this.itemContainer1});
            this.office2007StartButton1.Text = "&File";
            this.office2007StartButton1.Click += new System.EventHandler(this.office2007StartButton1_Click_1);
            // 
            // itemContainer1
            // 
            // 
            // 
            // 
            this.itemContainer1.BackgroundStyle.Class = "RibbonFileMenuContainer";
            this.itemContainer1.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            this.itemContainer1.Name = "itemContainer1";
            this.itemContainer1.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.itemContainer2,
            this.itemContainer4});
            // 
            // itemContainer2
            // 
            // 
            // 
            // 
            this.itemContainer2.BackgroundStyle.Class = "RibbonFileMenuTwoColumnContainer";
            this.itemContainer2.ItemSpacing = 0;
            this.itemContainer2.Name = "itemContainer2";
            this.itemContainer2.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.itemContainer3,
            this.galleryContainer1});
            // 
            // itemContainer3
            // 
            // 
            // 
            // 
            this.itemContainer3.BackgroundStyle.Class = "RibbonFileMenuColumnOneContainer";
            this.itemContainer3.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            this.itemContainer3.MinimumSize = new System.Drawing.Size(120, 0);
            this.itemContainer3.Name = "itemContainer3";
            this.itemContainer3.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonItem2,
            this.buttonItem3,
            this.buttonItem4,
            this.buttonItem5,
            this.buttonItem6,
            this.buttonItem7});
            // 
            // buttonItem2
            // 
            this.buttonItem2.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem2.Name = "buttonItem2";
            this.buttonItem2.SubItemsExpandWidth = 24;
            this.buttonItem2.Text = "&New";
            // 
            // buttonItem3
            // 
            this.buttonItem3.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem3.Name = "buttonItem3";
            this.buttonItem3.SubItemsExpandWidth = 24;
            this.buttonItem3.Text = "&Open...";
            // 
            // buttonItem4
            // 
            this.buttonItem4.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem4.Name = "buttonItem4";
            this.buttonItem4.SubItemsExpandWidth = 24;
            this.buttonItem4.Text = "&Save...";
            // 
            // buttonItem5
            // 
            this.buttonItem5.BeginGroup = true;
            this.buttonItem5.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem5.Name = "buttonItem5";
            this.buttonItem5.SubItemsExpandWidth = 24;
            this.buttonItem5.Text = "S&hare...";
            // 
            // buttonItem6
            // 
            this.buttonItem6.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem6.Name = "buttonItem6";
            this.buttonItem6.SubItemsExpandWidth = 24;
            this.buttonItem6.Text = "&Print...";
            // 
            // buttonItem7
            // 
            this.buttonItem7.BeginGroup = true;
            this.buttonItem7.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem7.Name = "buttonItem7";
            this.buttonItem7.SubItemsExpandWidth = 24;
            this.buttonItem7.Text = "&Close";
            // 
            // galleryContainer1
            // 
            // 
            // 
            // 
            this.galleryContainer1.BackgroundStyle.Class = "RibbonFileMenuColumnTwoContainer";
            this.galleryContainer1.EnableGalleryPopup = false;
            this.galleryContainer1.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            this.galleryContainer1.MinimumSize = new System.Drawing.Size(180, 240);
            this.galleryContainer1.MultiLine = false;
            this.galleryContainer1.Name = "galleryContainer1";
            this.galleryContainer1.PopupUsesStandardScrollbars = false;
            this.galleryContainer1.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.labelItem8,
            this.buttonItem8,
            this.buttonItem9,
            this.buttonItem10,
            this.buttonItem11});
            // 
            // labelItem8
            // 
            this.labelItem8.BorderSide = DevComponents.DotNetBar.eBorderSide.Bottom;
            this.labelItem8.BorderType = DevComponents.DotNetBar.eBorderType.Etched;
            this.labelItem8.CanCustomize = false;
            this.labelItem8.Name = "labelItem8";
            this.labelItem8.PaddingBottom = 2;
            this.labelItem8.PaddingTop = 2;
            this.labelItem8.Stretch = true;
            this.labelItem8.Text = "Recent Documents";
            // 
            // buttonItem8
            // 
            this.buttonItem8.Name = "buttonItem8";
            this.buttonItem8.Text = "&1. Short News 5-7.rtf";
            // 
            // buttonItem9
            // 
            this.buttonItem9.Name = "buttonItem9";
            this.buttonItem9.Text = "&2. Prospect Email.rtf";
            // 
            // buttonItem10
            // 
            this.buttonItem10.Name = "buttonItem10";
            this.buttonItem10.Text = "&3. Customer Email.rtf";
            // 
            // buttonItem11
            // 
            this.buttonItem11.Name = "buttonItem11";
            this.buttonItem11.Text = "&4. example.rtf";
            // 
            // itemContainer4
            // 
            // 
            // 
            // 
            this.itemContainer4.BackgroundStyle.Class = "RibbonFileMenuBottomContainer";
            this.itemContainer4.HorizontalItemAlignment = DevComponents.DotNetBar.eHorizontalItemsAlignment.Right;
            this.itemContainer4.Name = "itemContainer4";
            this.itemContainer4.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonItem12,
            this.buttonItem13});
            // 
            // buttonItem12
            // 
            this.buttonItem12.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem12.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonItem12.Name = "buttonItem12";
            this.buttonItem12.SubItemsExpandWidth = 24;
            this.buttonItem12.Text = "Opt&ions";
            // 
            // buttonItem13
            // 
            this.buttonItem13.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem13.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonItem13.Name = "buttonItem13";
            this.buttonItem13.SubItemsExpandWidth = 24;
            this.buttonItem13.Text = "E&xit";
            // 
            // btnItem
            // 
            this.btnItem.Name = "btnItem";
            this.btnItem.Text = "智能设计";
            // 
            // qatCustomizeItem1
            // 
            this.qatCustomizeItem1.Name = "qatCustomizeItem1";
            // 
            // styleManager3
            // 
            this.styleManager3.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue;
            // 
            // ScaffoldDesignForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1498, 841);
            this.Controls.Add(this.mainRibbonControl);
            this.Name = "ScaffoldDesignForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "市政模块_盘扣式脚手架智能设计";
            this.Load += new System.EventHandler(this.ScaffoldDesignForm_Load);
            this.Controls.SetChildIndex(this.cadPictBox, 0);
            this.Controls.SetChildIndex(this.mainRibbonControl, 0);
            this.mainRibbonControl.ResumeLayout(false);
            this.mainRibbonControl.PerformLayout();
            this.ribbonPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.StyleManager styleManager1;
        private DevComponents.DotNetBar.StyleManager styleManager2;
        //private DevComponents.DotNetBar.TextBoxItem textBoxItem1;
        private DevComponents.DotNetBar.ButtonItem buttonItem14;
        private DevComponents.DotNetBar.RibbonControl mainRibbonControl;
        private DevComponents.DotNetBar.RibbonPanel ribbonPanel1;
        private DevComponents.DotNetBar.RibbonBar ribar_Crossbar;
        private DevComponents.DotNetBar.ButtonItem btnCreateCrossbar;
        private DevComponents.DotNetBar.RibbonBar ribar_Column;
        private DevComponents.DotNetBar.ButtonItem btnCreateColumn;
        private DevComponents.DotNetBar.ButtonItem btnItem_Undo;
        private DevComponents.DotNetBar.ButtonItem btnItem_Redo;
        private DevComponents.DotNetBar.RibbonTabItem ribTabItem;
        private DevComponents.DotNetBar.Office2007StartButton office2007StartButton1;
        private DevComponents.DotNetBar.ButtonItem btnItem;
        private DevComponents.DotNetBar.QatCustomizeItem qatCustomizeItem1;
        private DevComponents.DotNetBar.StyleManager styleManager3;
        private DevComponents.DotNetBar.RibbonBar ribContrlColumn;
        private DevComponents.DotNetBar.ButtonItem btnMoveColumn;
        private DevComponents.DotNetBar.ButtonItem btnCopyColumn;
        internal DevComponents.DotNetBar.CheckBoxItem rUp;
        internal DevComponents.DotNetBar.CheckBoxItem rDown;
        internal DevComponents.DotNetBar.CheckBoxItem rLeft;
        internal DevComponents.DotNetBar.CheckBoxItem rRight;
        internal DevComponents.DotNetBar.ComboBoxItem comboBoxEx1;
        private DevComponents.DotNetBar.ItemContainer itemContainer1;
        private DevComponents.DotNetBar.ItemContainer itemContainer2;
        private DevComponents.DotNetBar.ItemContainer itemContainer3;
        private DevComponents.DotNetBar.ButtonItem buttonItem2;
        private DevComponents.DotNetBar.ButtonItem buttonItem3;
        private DevComponents.DotNetBar.ButtonItem buttonItem4;
        private DevComponents.DotNetBar.ButtonItem buttonItem5;
        private DevComponents.DotNetBar.ButtonItem buttonItem6;
        private DevComponents.DotNetBar.ButtonItem buttonItem7;
        private DevComponents.DotNetBar.GalleryContainer galleryContainer1;
        private DevComponents.DotNetBar.LabelItem labelItem8;
        private DevComponents.DotNetBar.ButtonItem buttonItem8;
        private DevComponents.DotNetBar.ButtonItem buttonItem9;
        private DevComponents.DotNetBar.ButtonItem buttonItem10;
        private DevComponents.DotNetBar.ButtonItem buttonItem11;
        private DevComponents.DotNetBar.ItemContainer itemContainer4;
        private DevComponents.DotNetBar.ButtonItem buttonItem12;
        private DevComponents.DotNetBar.ButtonItem buttonItem13;
    }
}