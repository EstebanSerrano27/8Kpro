
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
            this.comboBoxConnection = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxDisplayModes = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Port1 = new System.Windows.Forms.Label();
            this.preview1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.preview1)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxConnection
            // 
            this.comboBoxConnection.FormattingEnabled = true;
            this.comboBoxConnection.Location = new System.Drawing.Point(106, 48);
            this.comboBoxConnection.Name = "comboBoxConnection";
            this.comboBoxConnection.Size = new System.Drawing.Size(279, 21);
            this.comboBoxConnection.TabIndex = 3;
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
            // comboBoxDisplayModes
            // 
            this.comboBoxDisplayModes.FormattingEnabled = true;
            this.comboBoxDisplayModes.Location = new System.Drawing.Point(106, 96);
            this.comboBoxDisplayModes.Name = "comboBoxDisplayModes";
            this.comboBoxDisplayModes.Size = new System.Drawing.Size(279, 21);
            this.comboBoxDisplayModes.TabIndex = 5;
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
            // preview1
            // 
            this.preview1.Location = new System.Drawing.Point(15, 177);
            this.preview1.Name = "preview1";
            this.preview1.Size = new System.Drawing.Size(910, 400);
            this.preview1.TabIndex = 10;
            this.preview1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 652);
            this.Controls.Add(this.preview1);
            this.Controls.Add(this.Port1);
            this.Controls.Add(this.comboBoxDisplayModes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxConnection);
            this.Controls.Add(this.label2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.preview1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxConnection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxDisplayModes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label Port1;
        private System.Windows.Forms.PictureBox preview1;
    }
}

