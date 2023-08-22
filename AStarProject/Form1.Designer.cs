namespace AStarProject
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
            this.tiles_container = new System.Windows.Forms.Panel();
            this.navigation = new System.Windows.Forms.Panel();
            this.btn_start = new System.Windows.Forms.Button();
            this.btn_reset = new System.Windows.Forms.Button();
            this.navigation.SuspendLayout();
            this.SuspendLayout();
            // 
            // tiles_container
            // 
            this.tiles_container.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tiles_container.AutoScroll = true;
            this.tiles_container.Location = new System.Drawing.Point(12, 102);
            this.tiles_container.Name = "tiles_container";
            this.tiles_container.Size = new System.Drawing.Size(1055, 797);
            this.tiles_container.TabIndex = 0;
            // 
            // navigation
            // 
            this.navigation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.navigation.Controls.Add(this.btn_reset);
            this.navigation.Controls.Add(this.btn_start);
            this.navigation.Location = new System.Drawing.Point(12, 12);
            this.navigation.Name = "navigation";
            this.navigation.Size = new System.Drawing.Size(1055, 82);
            this.navigation.TabIndex = 1;
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(21, 22);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(157, 44);
            this.btn_start.TabIndex = 0;
            this.btn_start.Text = "Start";
            this.btn_start.UseVisualStyleBackColor = true;
            // 
            // btn_reset
            // 
            this.btn_reset.Location = new System.Drawing.Point(193, 22);
            this.btn_reset.Name = "btn_reset";
            this.btn_reset.Size = new System.Drawing.Size(157, 44);
            this.btn_reset.TabIndex = 1;
            this.btn_reset.Text = "Reset";
            this.btn_reset.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1079, 911);
            this.Controls.Add(this.navigation);
            this.Controls.Add(this.tiles_container);
            this.Name = "Form1";
            this.Text = "Form1";
            this.navigation.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel tiles_container;
        private Panel navigation;
        private Button btn_start;
        private Button btn_reset;
    }
}