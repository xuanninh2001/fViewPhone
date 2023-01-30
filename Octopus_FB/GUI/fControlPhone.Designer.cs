namespace Octopus_FB
{
    partial class fControlPhone
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
            this.components = new System.ComponentModel.Container();
            this.PanelPhone = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // PanelPhone
            // 
            this.PanelPhone.AutoScroll = true;
            this.PanelPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelPhone.Location = new System.Drawing.Point(0, 0);
            this.PanelPhone.Name = "PanelPhone";
            this.PanelPhone.Size = new System.Drawing.Size(1184, 661);
            this.PanelPhone.TabIndex = 0;
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // fControlPhone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.PanelPhone);
            this.Name = "fControlPhone";
            this.Text = "fControlPhone";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.fControlPhone_FormClosed);
            this.Load += new System.EventHandler(this.fControlPhone_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel PanelPhone;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}