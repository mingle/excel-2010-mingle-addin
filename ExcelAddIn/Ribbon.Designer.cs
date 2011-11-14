using System;

namespace ExcelAddIn
{
    /// <summary>
    /// 
    /// </summary>
    partial class Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase, IAmARibbon
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 
        /// </summary>
        public Ribbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabbMingle = this.Factory.CreateRibbonTab();
            this.groupMingle = this.Factory.CreateRibbonGroup();
            this.buttonLogin = this.Factory.CreateRibbonButton();
            this.buttonFetchCards = this.Factory.CreateRibbonButton();
            this.tabbMingle.SuspendLayout();
            this.groupMingle.SuspendLayout();
            // 
            // tabbMingle
            // 
            this.tabbMingle.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabbMingle.Groups.Add(this.groupMingle);
            this.tabbMingle.Label = "Mingle";
            this.tabbMingle.Name = "tabbMingle";
            // 
            // groupMingle
            // 
            this.groupMingle.Items.Add(this.buttonLogin);
            this.groupMingle.Items.Add(this.buttonFetchCards);
            this.groupMingle.Label = "Mingle";
            this.groupMingle.Name = "groupMingle";
            // 
            // buttonLogin
            // 
            this.buttonLogin.Image = global::ExcelAddIn.Properties.Resources.icon_edit_user;
            this.buttonLogin.Label = "Login";
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.ScreenTip = "provide authentication information for Mingle";
            this.buttonLogin.ShowImage = true;
            this.buttonLogin.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.OnButtonLoginClick);
            // 
            // buttonFetchCards
            // 
            this.buttonFetchCards.Image = global::ExcelAddIn.Properties.Resources.icon_all_items;
            this.buttonFetchCards.Label = "Fetch Cards";
            this.buttonFetchCards.Name = "buttonFetchCards";
            this.buttonFetchCards.ShowImage = true;
            this.buttonFetchCards.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.OnButtonFetchCardsClick);
            // 
            // Ribbon
            // 
            this.Name = "Ribbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tabbMingle);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.OnRibbonLoad);
            this.tabbMingle.ResumeLayout(false);
            this.tabbMingle.PerformLayout();
            this.groupMingle.ResumeLayout(false);
            this.groupMingle.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabbMingle;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupMingle;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonLogin;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonFetchCards;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
