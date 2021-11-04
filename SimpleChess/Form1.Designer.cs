
namespace SimpleChess
{
    partial class Form1
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
            this.animation_picture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.animation_picture)).BeginInit();
            this.SuspendLayout();
            // 
            // animation_picture
            // 
            this.animation_picture.BackColor = System.Drawing.Color.Transparent;
            this.animation_picture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.animation_picture.Location = new System.Drawing.Point(22, 26);
            this.animation_picture.Margin = new System.Windows.Forms.Padding(6);
            this.animation_picture.Name = "animation_picture";
            this.animation_picture.Size = new System.Drawing.Size(113, 139);
            this.animation_picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.animation_picture.TabIndex = 0;
            this.animation_picture.TabStop = false;
            this.animation_picture.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1274, 1393);
            this.Controls.Add(this.animation_picture);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "Simple Chess";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.animation_picture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox animation_picture;
    }
}

