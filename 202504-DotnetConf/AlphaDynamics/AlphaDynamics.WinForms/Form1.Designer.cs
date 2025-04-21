namespace AlphaDynamics.WinForms;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.ListBox crewListBox;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.crewListBox = new System.Windows.Forms.ListBox();
        this.SuspendLayout();

        // 
        // crewListBox
        // 
        this.crewListBox.FormattingEnabled = true;
        this.crewListBox.ItemHeight = 15;
        this.crewListBox.Location = new System.Drawing.Point(12, 12);
        this.crewListBox.Name = "crewListBox";
        this.crewListBox.Size = new System.Drawing.Size(260, 200);
        this.crewListBox.TabIndex = 0;

        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(284, 231);
        this.Controls.Add(this.crewListBox);
        this.Name = "Form1";
        this.Text = "Crew Viewer";
        this.Load += new System.EventHandler(this.Form1_Load);
        this.ResumeLayout(false);
    }

    #endregion
}
