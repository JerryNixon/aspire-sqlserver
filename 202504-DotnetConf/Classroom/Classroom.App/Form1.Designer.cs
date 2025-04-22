namespace Classroom.App
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox classPicker;
        private System.Windows.Forms.DataGridView attendanceGrid;
        private Button saveButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            classPicker = new ComboBox();
            attendanceGrid = new DataGridView();
            saveButton = new Button();
            ((System.ComponentModel.ISupportInitialize)attendanceGrid).BeginInit();
            SuspendLayout();
            // 
            // classPicker
            // 
            classPicker.Dock = DockStyle.Top;
            classPicker.DropDownStyle = ComboBoxStyle.DropDownList;
            classPicker.FormattingEnabled = true;
            classPicker.Location = new Point(0, 0);
            classPicker.Name = "classPicker";
            classPicker.Size = new Size(446, 23);
            classPicker.TabIndex = 0;
            // 
            // attendanceGrid
            // 
            attendanceGrid.AllowUserToAddRows = false;
            attendanceGrid.AllowUserToDeleteRows = false;
            attendanceGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            attendanceGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            attendanceGrid.Dock = DockStyle.Fill;
            attendanceGrid.Location = new Point(0, 23);
            attendanceGrid.Name = "attendanceGrid";
            attendanceGrid.Size = new Size(446, 165);
            attendanceGrid.TabIndex = 1;
            // 
            // saveButton
            // 
            saveButton.Dock = DockStyle.Bottom;
            saveButton.Location = new Point(0, 188);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(446, 52);
            saveButton.TabIndex = 2;
            saveButton.Text = "Save";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(446, 240);
            Controls.Add(attendanceGrid);
            Controls.Add(saveButton);
            Controls.Add(classPicker);
            Name = "Form1";
            Text = "Classroom Attendance";
            ((System.ComponentModel.ISupportInitialize)attendanceGrid).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}
