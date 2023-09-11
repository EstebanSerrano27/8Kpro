
namespace _8Kpro
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxVideoFormat = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Port1 = new System.Windows.Forms.Label();
            this.Port2 = new System.Windows.Forms.Label();
            this.Port4 = new System.Windows.Forms.Label();
            this.Port3 = new System.Windows.Forms.Label();
            this.preview1 = new System.Windows.Forms.PictureBox();
            this.preview2 = new System.Windows.Forms.PictureBox();
            this.preview3 = new System.Windows.Forms.PictureBox();
            this.preview4 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.preview1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview4)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(106, 48);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(279, 21);
            this.comboBox2.TabIndex = 3;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Input Connection";
            // 
            // comboBoxVideoFormat
            // 
            this.comboBoxVideoFormat.FormattingEnabled = true;
            this.comboBoxVideoFormat.Location = new System.Drawing.Point(106, 96);
            this.comboBoxVideoFormat.Name = "comboBoxVideoFormat";
            this.comboBoxVideoFormat.Size = new System.Drawing.Size(279, 21);
            this.comboBoxVideoFormat.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Video Format";
            // 
            // Port1
            // 
            this.Port1.AutoSize = true;
            this.Port1.Location = new System.Drawing.Point(12, 161);
            this.Port1.Name = "Port1";
            this.Port1.Size = new System.Drawing.Size(32, 13);
            this.Port1.TabIndex = 6;
            this.Port1.Text = "Port1";
            // 
            // Port2
            // 
            this.Port2.AutoSize = true;
            this.Port2.Location = new System.Drawing.Point(353, 161);
            this.Port2.Name = "Port2";
            this.Port2.Size = new System.Drawing.Size(32, 13);
            this.Port2.TabIndex = 7;
            this.Port2.Text = "Port2";
            // 
            // Port4
            // 
            this.Port4.AutoSize = true;
            this.Port4.Location = new System.Drawing.Point(353, 371);
            this.Port4.Name = "Port4";
            this.Port4.Size = new System.Drawing.Size(32, 13);
            this.Port4.TabIndex = 9;
            this.Port4.Text = "Port4";
            // 
            // Port3
            // 
            this.Port3.AutoSize = true;
            this.Port3.Location = new System.Drawing.Point(12, 371);
            this.Port3.Name = "Port3";
            this.Port3.Size = new System.Drawing.Size(32, 13);
            this.Port3.TabIndex = 8;
            this.Port3.Text = "Port3";
            // 
            // preview1
            // 
            this.preview1.Location = new System.Drawing.Point(15, 177);
            this.preview1.Name = "preview1";
            this.preview1.Size = new System.Drawing.Size(289, 169);
            this.preview1.TabIndex = 10;
            this.preview1.TabStop = false;
            // 
            // preview2
            // 
            this.preview2.Location = new System.Drawing.Point(356, 177);
            this.preview2.Name = "preview2";
            this.preview2.Size = new System.Drawing.Size(289, 169);
            this.preview2.TabIndex = 11;
            this.preview2.TabStop = false;
            // 
            // preview3
            // 
            this.preview3.Location = new System.Drawing.Point(356, 387);
            this.preview3.Name = "preview3";
            this.preview3.Size = new System.Drawing.Size(289, 169);
            this.preview3.TabIndex = 13;
            this.preview3.TabStop = false;
            // 
            // preview4
            // 
            this.preview4.Location = new System.Drawing.Point(15, 387);
            this.preview4.Name = "preview4";
            this.preview4.Size = new System.Drawing.Size(289, 169);
            this.preview4.TabIndex = 12;
            this.preview4.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 652);
            this.Controls.Add(this.preview3);
            this.Controls.Add(this.preview4);
            this.Controls.Add(this.preview2);
            this.Controls.Add(this.preview1);
            this.Controls.Add(this.Port4);
            this.Controls.Add(this.Port3);
            this.Controls.Add(this.Port2);
            this.Controls.Add(this.Port1);
            this.Controls.Add(this.comboBoxVideoFormat);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.label2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.preview1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxVideoFormat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label Port1;
        private System.Windows.Forms.Label Port2;
        private System.Windows.Forms.Label Port4;
        private System.Windows.Forms.Label Port3;
        private System.Windows.Forms.PictureBox preview1;
        private System.Windows.Forms.PictureBox preview2;
        private System.Windows.Forms.PictureBox preview3;
        private System.Windows.Forms.PictureBox preview4;
    }
}

