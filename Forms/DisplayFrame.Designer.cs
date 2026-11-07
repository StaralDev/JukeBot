namespace JukeBot;

partial class DisplayFrame
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayFrame));
        components = new System.ComponentModel.Container();
        this.SuspendLayout();
        // 
        // button1
        // 
        // 
        // DisplayFrame
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(449, 84);
        this.Name = "DisplayFrame";
        Text = "JukeBot Display";
        this.ControlBox = true;
        this.AccessibleRole = System.Windows.Forms.AccessibleRole.Default;
        this.FormScreenCaptureMode = System.Windows.Forms.ScreenCaptureMode.Allow;
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
        this.BackColor = System.Drawing.Color.Black;
        this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
        this.MaximizeBox = false;
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

}
