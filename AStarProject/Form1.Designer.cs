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
            tiles_container = new Panel();
            navigation = new Panel();
            btn_zoom_in = new Button();
            btn_zoom_out = new Button();
            btn_reset = new Button();
            btn_start = new Button();
            navigation.SuspendLayout();
            SuspendLayout();
            // 
            // tiles_container
            // 
            tiles_container.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tiles_container.AutoScroll = true;
            tiles_container.Location = new Point(12, 102);
            tiles_container.Name = "tiles_container";
            tiles_container.Size = new Size(1055, 797);
            tiles_container.TabIndex = 0;
            // 
            // navigation
            // 
            navigation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            navigation.Controls.Add(btn_zoom_in);
            navigation.Controls.Add(btn_zoom_out);
            navigation.Controls.Add(btn_reset);
            navigation.Controls.Add(btn_start);
            navigation.Location = new Point(12, 12);
            navigation.Name = "navigation";
            navigation.Size = new Size(1055, 82);
            navigation.TabIndex = 1;
            // 
            // btn_zoom_in
            // 
            btn_zoom_in.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_zoom_in.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            btn_zoom_in.Location = new Point(936, 22);
            btn_zoom_in.Name = "btn_zoom_in";
            btn_zoom_in.Size = new Size(50, 44);
            btn_zoom_in.TabIndex = 3;
            btn_zoom_in.Text = "+";
            btn_zoom_in.UseVisualStyleBackColor = true;
            // 
            // btn_zoom_out
            // 
            btn_zoom_out.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_zoom_out.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            btn_zoom_out.Location = new Point(992, 22);
            btn_zoom_out.Name = "btn_zoom_out";
            btn_zoom_out.Size = new Size(50, 44);
            btn_zoom_out.TabIndex = 2;
            btn_zoom_out.Text = "-";
            btn_zoom_out.UseVisualStyleBackColor = true;
            // 
            // btn_reset
            // 
            btn_reset.Location = new Point(193, 22);
            btn_reset.Name = "btn_reset";
            btn_reset.Size = new Size(157, 44);
            btn_reset.TabIndex = 1;
            btn_reset.Text = "Reset";
            btn_reset.UseVisualStyleBackColor = true;
            // 
            // btn_start
            // 
            btn_start.Location = new Point(21, 22);
            btn_start.Name = "btn_start";
            btn_start.Size = new Size(157, 44);
            btn_start.TabIndex = 0;
            btn_start.Text = "Start";
            btn_start.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1079, 911);
            Controls.Add(navigation);
            Controls.Add(tiles_container);
            Name = "Form1";
            Text = "Form1";
            navigation.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel tiles_container;
        private Panel navigation;
        private Button btn_start;
        private Button btn_reset;
        private Button btn_zoom_in;
        private Button btn_zoom_out;
    }
}