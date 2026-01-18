namespace TowerDefenseOyunuProje
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            this.oyunAlani = new System.Windows.Forms.PictureBox();
            this.dusmanTayicisi = new System.Windows.Forms.Timer(this.components);
            this.kuleTayicisi = new System.Windows.Forms.Timer(this.components);
            this.dusmanUreticisi = new System.Windows.Forms.Timer(this.components);
            this.bilgiLabel = new System.Windows.Forms.Label();
            this.sifirlaButto = new System.Windows.Forms.Button();
            this.siradakiDalgaButt = new System.Windows.Forms.Button();
            this.pesEtButt = new System.Windows.Forms.Button();
            this.insaMenusu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.okcuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topcuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            
            ((System.ComponentModel.ISupportInitialize)(this.oyunAlani)).BeginInit();
            this.SuspendLayout();
            
            // oyunAlani
            this.oyunAlani.BackColor = System.Drawing.Color.ForestGreen;
            this.oyunAlani.Location = new System.Drawing.Point(12, 12);
            this.oyunAlani.Name = "oyunAlani";
            this.oyunAlani.Size = new System.Drawing.Size(1000, 500);
            this.oyunAlani.TabIndex = 0;
            this.oyunAlani.TabStop = false;
            this.oyunAlani.Click += new System.EventHandler(this.OyunAlani_Click);
            this.oyunAlani.Paint += new System.Windows.Forms.PaintEventHandler(this.OyunAlani_Paint);
            
            // dusmanTayicisi
            this.dusmanTayicisi.Interval = 20;
            this.dusmanTayicisi.Tick += new System.EventHandler(this.DusmanTayicisi_Tick);
            
            // kuleTayicisi
            this.kuleTayicisi.Interval = 50;
            this.kuleTayicisi.Tick += new System.EventHandler(this.KuleTayicisi_Tick);
            
            // dusmanUreticisi
            this.dusmanUreticisi.Interval = 1000;
            this.dusmanUreticisi.Tick += new System.EventHandler(this.DusmanUreticisi_Tick);
            
            // bilgiLabel
            this.bilgiLabel.AutoSize = true;
            this.bilgiLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.bilgiLabel.Location = new System.Drawing.Point(12, 515);
            this.bilgiLabel.Name = "bilgiLabel";
            this.bilgiLabel.Size = new System.Drawing.Size(400, 16);
            this.bilgiLabel.TabIndex = 1;
            this.bilgiLabel.Text = "Altın: 200 | Kule: 0 | Ana Üs Canı: 20 | Dalga: 1";
            
            // sifirlaButto
            this.sifirlaButto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.sifirlaButto.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.sifirlaButto.ForeColor = System.Drawing.Color.White;
            this.sifirlaButto.Location = new System.Drawing.Point(850, 515);
            this.sifirlaButto.Name = "sifirlaButto";
            this.sifirlaButto.Size = new System.Drawing.Size(75, 35);
            this.sifirlaButto.TabIndex = 2;
            this.sifirlaButto.Text = "Sıfırla";
            this.sifirlaButto.UseVisualStyleBackColor = false;
            this.sifirlaButto.Click += new System.EventHandler(this.SifirlaButton_Click);
            
            // siradakiDalgaButt
            this.siradakiDalgaButt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(150)))), ((int)(((byte)(220)))));
            this.siradakiDalgaButt.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.siradakiDalgaButt.ForeColor = System.Drawing.Color.White;
            this.siradakiDalgaButt.Location = new System.Drawing.Point(937, 515);
            this.siradakiDalgaButt.Name = "siradakiDalgaButt";
            this.siradakiDalgaButt.Size = new System.Drawing.Size(75, 35);
            this.siradakiDalgaButt.TabIndex = 3;
            this.siradakiDalgaButt.Text = "Dalga >>>";
            this.siradakiDalgaButt.UseVisualStyleBackColor = false;
            this.siradakiDalgaButt.Click += new System.EventHandler(this.SiradakiDalgaButton_Click);
            
            // pesEtButt
            this.pesEtButt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(140)))), ((int)(((byte)(50)))));
            this.pesEtButt.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.pesEtButt.ForeColor = System.Drawing.Color.White;
            this.pesEtButt.Location = new System.Drawing.Point(763, 515);
            this.pesEtButt.Name = "pesEtButt";
            this.pesEtButt.Size = new System.Drawing.Size(75, 35);
            this.pesEtButt.TabIndex = 4;
            this.pesEtButt.Text = "Pes Et";
            this.pesEtButt.UseVisualStyleBackColor = false;
            this.pesEtButt.Click += new System.EventHandler(this.PesEtButton_Click);
            
            // insaMenusu
            this.insaMenusu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.okcuToolStripMenuItem,
            this.topcuToolStripMenuItem});
            this.insaMenusu.Name = "insaMenusu";
            this.insaMenusu.Size = new System.Drawing.Size(230, 48);
            
            // okcuToolStripMenuItem
            this.okcuToolStripMenuItem.Name = "okcuToolStripMenuItem";
            this.okcuToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.okcuToolStripMenuItem.Text = "Okçu Kulesi (50 Altın)";
            this.okcuToolStripMenuItem.ToolTipText = "Hızlı ateş eder, tek hedefe vurur. Hızlı düşmanlar için ideal.";
            this.okcuToolStripMenuItem.Click += new System.EventHandler(this.OkcuKuleKur_Click);
            
            // topcuToolStripMenuItem
            this.topcuToolStripMenuItem.Name = "topcuToolStripMenuItem";
            this.topcuToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.topcuToolStripMenuItem.Text = "Topçu Kulesi (150 Altın)";
            this.topcuToolStripMenuItem.ToolTipText = "Yavaş ateş eder, ALAN hasarı verir (Zırhlı için ideal).\nNormal, Hızlı, Zırhlı düşmanları öldürebilir.";
            this.topcuToolStripMenuItem.Click += new System.EventHandler(this.TopcuKuleKur_Click);
            
            // Form1
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 560);
            this.Controls.Add(this.siradakiDalgaButt);
            this.Controls.Add(this.pesEtButt);
            this.Controls.Add(this.sifirlaButto);
            this.Controls.Add(this.bilgiLabel);
            this.Controls.Add(this.oyunAlani);
            this.Name = "Form1";
            this.Text = "Tower Defense - Orta Çağ Kalesi Savunması";
            this.Load += new System.EventHandler(this.Form1_Load);
            
            // Sabit boyut ayarları
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.Black;
            
            ((System.ComponentModel.ISupportInitialize)(this.oyunAlani)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox oyunAlani;
        private System.Windows.Forms.Timer dusmanTayicisi;
        private System.Windows.Forms.Timer kuleTayicisi;
        private System.Windows.Forms.Timer dusmanUreticisi;
        private System.Windows.Forms.Label bilgiLabel;
        private System.Windows.Forms.Button sifirlaButto;
        private System.Windows.Forms.Button siradakiDalgaButt;
        private System.Windows.Forms.Button pesEtButt;
        private System.Windows.Forms.ContextMenuStrip insaMenusu;
        private System.Windows.Forms.ToolStripMenuItem okcuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topcuToolStripMenuItem;
    }
}

