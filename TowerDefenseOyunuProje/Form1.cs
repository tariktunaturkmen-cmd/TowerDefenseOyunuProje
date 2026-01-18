using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TowerDefenseOyunuProje
{
    public partial class Form1 : Form
    {
        #region Sabitler ve Deðiþkenler

        // ===== OYUN NESNELERÝ =====
        private List<Kule> kuleler = new List<Kule>();
        private List<Dusman> dusmanlari = new List<Dusman>();
        private List<InsaAlani> insaAlanlari = new List<InsaAlani>();
        
        // ===== OYUN PARAMETRELERÝ =====
        private int altin = 200;
        private int anaBaseCan = 20;
        private int mevcutDalga = 1;
        private int kalanDusmanSayisi = 0;
        private bool dalgaAktifMi = false;

        // ===== SABÝT DEÐERLER (Magic Numbers Düzeltmesi) =====
        private const int BASLANGIC_ALTIN = 200;
        private const int BASLANGIC_CAN = 20;
        private const int OKCU_FIYATI = 50;
        private const int TOPCU_FIYATI = 150;
        private const int KALE_X = 900;
        private const int KALE_Y = 450;
        private const int KALE_GENISLIK = 80;
        private const int KALE_YUKSEKLIGI = 80;
        private const int KULE_GENISLIK = 40;
        private const int KULE_YUKSEKLIGI = 40;
        private const int DUSMAN_GENISLIGI = 16;
        private const int DUSMAN_YUKSEKLIGI = 16;
        private const int DUSMAN_CAN_BARSI_GENISLIGI = 20;
        private const int DUSMAN_CAN_BARSI_YUKSEKLIGI = 3;
        private const int KALE_CAN_BARSI_GENISLIGI = 100;
        private const int KALE_CAN_BARSI_YUKSEKLIGI = 8;
        private const int HUD_YUKSEKLIGI = 30;
        private const int MAKSIMUM_CAN = 20;
        private const int GRID_SIZE = 150;

        // ===== OTOMATIK DALGA SÝSTEMÝ =====
        private Timer dalgaGeriSayimTayicisi = null;
        private int dalgaGeriSayimSaniye = 0;
        private bool dalgaGeriSayimAktifMi = false;

        // ===== ROTA VE ÝNÞA ALANLARI =====
        private List<Point> dusmanRotasi = new List<Point>();
        private InsaAlani tikaliInsaAlani = null;

        // ===== RANDOM ÜRETECI =====
        private Random rastgele = new Random();

        // ===== MOUSE TAKIBI =====
        private Kule _fareAltindakiKule = null;

        // ===== GÖRSEL YÖNETÝMÝ (Bellek Optimizasyonu) =====
        private Image kaleGorseli = null;
        private Image okcuGorseli = null;
        private Image topcuGorseli = null;

        // ===== OPTIMIZATION CACHE =====
        private Dictionary<int, List<Dusman>> _dusmanGrid = new Dictionary<int, List<Dusman>>();
        private string _cachedHudText = "";
        private int _lastAltinCache = -1;
        private int _lastCanCache = -1;
        private int _lastDalgaCache = -1;
        private int _lastDusmanCache = -1;

        // ===== VERÝTABANI =====
        private VeriTabaniYoneticisi veriTabani = null;
        private string mevcutOyuncuAdi = "Oyuncu";

        #endregion

        #region GDI+ Nesneleri (Kalemler, Fýrçalar, Fontlar)

        // ===== GDÝ+ NESNELERÝ (readonly - Bellek Yönetimi) =====
        // Yol
        private readonly Pen yolKalemi = new Pen(Color.SaddleBrown, 70);
        
        // Ýnþa alanlarý
        private readonly Pen insaAlaniCercevesi = new Pen(Color.White, 2);
        private readonly Brush insaAlaniIci = new SolidBrush(Color.FromArgb(30, 255, 255, 0));
        
        // Kuleler
        private readonly Pen kuleCercevesi = new Pen(Color.Black, 2);
        private readonly Pen kuleMenzili = new Pen(Color.LightBlue, 1);
        private readonly Brush okcuKuleBrush = new SolidBrush(Color.Tan);
        private readonly Brush topcuKuleBrush = new SolidBrush(Color.DarkGray);
        
        // Düþmanlar
        private readonly Pen dusmanCercevesi = new Pen(Color.Black, 1);
        private readonly Brush dusmanCanBarsiArkaplan = Brushes.Red;
        private readonly Brush dusmanCanBarsýYesil = Brushes.LimeGreen;
        
        // Kale
        private readonly Pen kaleCercevesi = new Pen(Color.Black, 3);
        private readonly Brush kaleCanBarsiArkaplan = Brushes.Red;
        private readonly Brush kaleCanBarsýYesil = Brushes.LimeGreen;
        
        // HUD ve Legend
        private readonly Brush hudArkaplan = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
        private readonly Brush hudYaziBrush = Brushes.White;
        private readonly Brush legendArkaplan = new SolidBrush(Color.FromArgb(180, 255, 255, 255));
        private readonly Pen legendCercevesi = new Pen(Color.Black, 1);
        private readonly Brush legendYaziBrush = Brushes.Black;
        
        // Fontlar
        private readonly Font hudFont = new Font("Arial", 11, FontStyle.Bold);
        private readonly Font kuleSeviyelyFont = new Font("Arial", 8, FontStyle.Bold);
        private readonly Font kaleCanFont = new Font("Arial", 10, FontStyle.Bold);
        private readonly Font legendFont = new Font("Arial", 7);
        private readonly Font buyukFont = new Font("Arial", 32, FontStyle.Bold);

        // ===== LAZER PEN CACHE =====
        private readonly Pen lazerOkcuPen = new Pen(Color.Yellow, 3);
        private readonly Pen lazerTopcuPen = new Pen(Color.OrangeRed, 3);

        // ===== DÜÞMAN BRUSH CACHE =====
        private readonly Dictionary<DusmanTuru, Brush> _dusmanBrushCache = 
            new Dictionary<DusmanTuru, Brush>()
            {
                { DusmanTuru.Normal, new SolidBrush(Color.Red) },
                { DusmanTuru.Hizli, new SolidBrush(Color.Yellow) },
                { DusmanTuru.Zirhli, new SolidBrush(Color.DarkBlue) }
            };

        #endregion

        #region Form ve Baþlangýç Ayarlarý (Constructor, Load)

        public Form1()
        {
            InitializeComponent();
            
            this.DoubleBuffered = true;
            oyunAlani.BackgroundImageLayout = ImageLayout.None;

            dalgaGeriSayimTayicisi = new Timer();
            dalgaGeriSayimTayicisi.Interval = 1000;
            dalgaGeriSayimTayicisi.Tick += DalgaGeriSayimTayicisi_Tick;

            insaAlaniCercevesi.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            // Veritabanýný baþlat
            veriTabani = new VeriTabaniYoneticisi();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OlusturDusmanRotasi();
            OlusturInsaAlanlari();

            OlusturGirisMenusu();

            siradakiDalgaButt.Enabled = false;
            siradakiDalgaButt.BackColor = Color.Gray;

            siradakiDalgaButt.BringToFront();
            sifirlaButto.BringToFront();
            oyunAlani.SendToBack();

            oyunAlani.MouseMove += OyunAlani_MouseMove;

            BilgiGuncelleMe();
            oyunAlani.Invalidate();
        }

        #endregion

        #region Arayüz Oluþturma (Menüler, Butonlar)

        // Form1.cs içine yapýþtýr (Eski MouseMove yerine)
        private void OyunAlani_MouseMove(object sender, MouseEventArgs e)
        {
            int mouseX = e.X;
            int mouseY = e.Y;

            // Önceki durumu kaydet
            Kule oncekiKule = _fareAltindakiKule;
            Kule yeniKule = null;

            // Hangi inþa alanýnýn üzerinde olduðumuzu kontrol et
            foreach (InsaAlani alan in insaAlanlari)
            {
                if (alan.TiklanaYildimi(mouseX, mouseY) && alan.DoluMu)
                {
                    yeniKule = alan.YerlesenKule;
                    break;
                }
            }

            // SADECE deðiþiklik varsa çizim yap (Performans Fix)
            if (oncekiKule != yeniKule)
            {
                _fareAltindakiKule = yeniKule;
                oyunAlani.Invalidate();
            }
        }

        private void OlusturGirisMenusu()
        {
            Panel pnlMenu = new Panel();
            pnlMenu.Name = "pnlGirisMenusu";
            pnlMenu.Dock = DockStyle.Fill;
            pnlMenu.BackColor = Color.Black;

            Label lblBaslik = new Label();
            lblBaslik.Text = "AMASYA SAVUNMASI";
            lblBaslik.Font = new Font("Arial", 48, FontStyle.Bold);
            lblBaslik.ForeColor = Color.Yellow;
            lblBaslik.TextAlign = ContentAlignment.MiddleCenter;
            lblBaslik.Dock = DockStyle.Top;
            lblBaslik.Height = 120;
            pnlMenu.Controls.Add(lblBaslik);

            Button btnBasla = new Button();
            btnBasla.Text = "OYUNA BAÞLA";
            btnBasla.Font = new Font("Arial", 18, FontStyle.Bold);
            btnBasla.BackColor = Color.LimeGreen;
            btnBasla.ForeColor = Color.Black;
            btnBasla.Size = new System.Drawing.Size(300, 60);
            btnBasla.Location = new System.Drawing.Point(362, 280);
            btnBasla.Click += (sender, e) =>
            {
                pnlMenu.Visible = false;

                siradakiDalgaButt.BringToFront();
                sifirlaButto.BringToFront();
                oyunAlani.SendToBack();

                siradakiDalgaButt.Enabled = true;
                sifirlaButto.Enabled = true;

                dusmanTayicisi.Start();
                kuleTayicisi.Start();

                BilgiGuncelleMe();
                oyunAlani.Invalidate();
            };
            pnlMenu.Controls.Add(btnBasla);

            Button btnKurallar = new Button();
            btnKurallar.Text = "NASIL OYNANIR?";
            btnKurallar.Font = new Font("Arial", 18, FontStyle.Bold);
            btnKurallar.BackColor = Color.DeepSkyBlue;
            btnKurallar.ForeColor = Color.White;
            btnKurallar.Size = new System.Drawing.Size(300, 60);
            btnKurallar.Location = new System.Drawing.Point(362, 360);
            btnKurallar.Click += (sender, e) =>
            {
                GosterKurallar();
            };
            pnlMenu.Controls.Add(btnKurallar);

            this.Controls.Add(pnlMenu);
            pnlMenu.BringToFront();
        }

        private void GosterKurallar()
        {
            string kurallar =
                "GÖREVÝN: Düþmanlarýn kaleye ulaþmasýný engelle!\n\n" +

                "Kalenin 20 caný vardýr. Her düþman kaleye ulaþýrsa 1 can kaybedersin.\n" +
                "Can 0 olursa oyun biter!\n\n" +
              
                "KULE TÜRLERÝ:\n" +
                "?? OKCU KULESÝ (50 Altýn)\n" +
                "?? TOPCU KULESÝ (150 Altýn)\n" +
              
                "DÜÞMAN TÜRLERÝ:\n" +
                "?? NORMAL (Kýrmýzý): Dengeli düþman.\n" +
                "?? HIZLI (Sarý): Hýzlý ama zayýf. Dalga 3'ten baþlar.\n" +
                "?? ZIRHLI (Koyu Mavi): Yavaþ ama güçlü. Dalga 5'ten baþlar.\n\n" +
                "Ýyi Oyunlar! ????";
            MessageBox.Show(kurallar, "Nasýl Oynanýr?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OyunAlani_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            int x = me.X;
            int y = me.Y;

            tikaliInsaAlani = null;
            foreach (InsaAlani alan in insaAlanlari)
            {
                if (alan.TiklanaYildimi(x, y))
                {
                    tikaliInsaAlani = alan;
                    break;
                }
            }

            if (tikaliInsaAlani != null)
            {
                if (!tikaliInsaAlani.DoluMu)
                {
                    insaMenusu.Show(oyunAlani, x, y);
                }
                else
                {
                    Kule mevcutKule = tikaliInsaAlani.YerlesenKule;
                    if (mevcutKule != null)
                    {
                        GosterYukseltmeMenusu(mevcutKule, x, y);
                    }
                }
            }
        }

        private void GosterYukseltmeMenusu(Kule kule, int x, int y)
        {
            ContextMenuStrip yukseltMenusu = new ContextMenuStrip();
            
            int bedel = kule.YukseltmeBedeli();
            string menuText = $"Seviye Yükselt (Bedel: {bedel} Altýn)";
            
            ToolStripMenuItem yukseltItem = new ToolStripMenuItem(menuText);
            yukseltItem.Click += (sender, e) =>
            {
                if (altin >= bedel)
                {
                    altin -= bedel;
                    kule.SeviyeAtla();
                    BilgiGuncelleMe();
                    oyunAlani.Invalidate();
                }
                else
                {
                    MessageBox.Show("Yetersiz Altýn!", "Uyarý");
                }
            };
            
            yukseltMenusu.Items.Add(yukseltItem);
            yukseltMenusu.Show(oyunAlani, x, y);
        }

        private void OkcuKuleKur_Click(object sender, EventArgs e)
        {
            if (tikaliInsaAlani != null && altin >= OKCU_FIYATI)
            {
                Point orta = tikaliInsaAlani.OrtaNokta();
                Kule yeniKule = new OkcuKulesi(orta.X, orta.Y);
                
                altin -= yeniKule.Fiyat;
                tikaliInsaAlani.DoluMu = true;
                tikaliInsaAlani.YerlesenKule = yeniKule;
                kuleler.Add(yeniKule);

                BilgiGuncelleMe();
                oyunAlani.Invalidate();
            }
            else if (altin < OKCU_FIYATI)
            {
                MessageBox.Show("Yetersiz Altýn!", "Uyarý");
            }
        }

        private void TopcuKuleKur_Click(object sender, EventArgs e)
        {
            if (tikaliInsaAlani != null && altin >= TOPCU_FIYATI)
            {
                Point orta = tikaliInsaAlani.OrtaNokta();
                Kule yeniKule = new TopcuKulesi(orta.X, orta.Y);
                
                altin -= yeniKule.Fiyat;
                tikaliInsaAlani.DoluMu = true;
                tikaliInsaAlani.YerlesenKule = yeniKule;
                kuleler.Add(yeniKule);

                BilgiGuncelleMe();
                oyunAlani.Invalidate();
            }
            else if (altin < TOPCU_FIYATI)
            {
                MessageBox.Show("Yetersiz Altýn!", "Uyarý");
            }
        }

        #endregion

        #region Oyun Mantýðý ve Timerlar (Tick Olaylarý)

        private void DusmanTayicisi_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < dusmanlari.Count; i++)
            {
                dusmanlari[i].Ilerle();
            }

            for (int i = dusmanlari.Count - 1; i >= 0; i--)
            {
                Dusman dusman = dusmanlari[i];

                // KONTROL 1: Düþman kuleye çarptý mý?
                bool kuleVurusuVar = false;
                Kule vurduguKule = null;
                
                for (int k = kuleler.Count - 1; k >= 0; k--)
                {
                    Kule kule = kuleler[k];
                    if (kule.OluMu())
                        continue;

                    float dx = dusman.X - kule.X;
                    float dy = dusman.Y - kule.Y;
                    float mesafeKaresi = dx * dx + dy * dy;
                    float carpismaMenziliKaresi = 40 * 40;

                    if (mesafeKaresi <= carpismaMenziliKaresi)
                    {
                        kule.HasarAl(dusman.Can / 2);
                        System.Console.WriteLine($"ÇARPIÞMA! Düþman kuleye çarptý! Kule caný: {kule.Can}/{kule.MaxCan}");
                        kuleVurusuVar = true;
                        vurduguKule = kule;
                        break;
                    }
                }

                if (kuleVurusuVar)
                {
                    dusmanlari.RemoveAt(i);
                    kalanDusmanSayisi--;
                    
                    if (vurduguKule != null && vurduguKule.OluMu())
                    {
                        kuleler.Remove(vurduguKule);
                        System.Console.WriteLine("?? KULE YIKILDIII!");
                    }
                    continue;
                }

                // KONTROL 2: Düþman kaleye ulaþtý mý?
                if (dusman.KaleyeleMiUlasti())
                {
                    anaBaseCan--;
                    dusmanlari.RemoveAt(i);
                    dusman.Can = 0;
                    kalanDusmanSayisi--;
                    System.Console.WriteLine($"KALE VURUÞU! Kale caný: {anaBaseCan}");
                }
                // KONTROL 3: Düþman ölü mü?
                else if (dusman.OluMu())
                {
                    altin += 10;
                    dusmanlari.RemoveAt(i);
                    kalanDusmanSayisi--;
                }
            }

            if (dalgaAktifMi && dusmanlari.Count == 0 && kalanDusmanSayisi <= 0)
            {
                DalgaBitti();
            }

            if (anaBaseCan <= 0)
            {
                // ÖNCE timerlarý durdur
                dusmanTayicisi.Stop();
                kuleTayicisi.Stop();
                dusmanUreticisi.Stop();
                dalgaGeriSayimTayicisi.Stop();

                int basariliDalgalar = mevcutDalga - 1;
                
                // Veritabanýna kaydet
                veriTabani.OyunSonuKaydet(mevcutOyuncuAdi, basariliDalgalar, altin, 0);
                
                // SONRA mesaj göster
                GosterOyunBittiDialog(basariliDalgalar);
                
                // EN SON sýfýrla
                SifirlaButton_Click(null, null);
                return;
            }

            GuncelleDusmanGrid();
            oyunAlani.Invalidate();
            BilgiGuncelleMe();
        }

        private void KuleTayicisi_Tick(object sender, EventArgs e)
        {
            GuncelleDusmanGrid();

            foreach (Kule kule in kuleler)
            {
                if (!kule.AtesiAcabilirMi())
                {
                    if (kule.SaldirildiTikSayisi > 0)
                        kule.SaldirildiTikSayisi--;
                    continue;
                }

                int kuleGridKey = GetGridKey(kule.X, kule.Y);
                bool hedefBulundu = false;

                for (int dx = -1; dx <= 1 && !hedefBulundu; dx++)
                {
                    for (int dy = -1; dy <= 1 && !hedefBulundu; dy++)
                    {
                        int neighborKey = kuleGridKey + dx * 10000 + dy;
                        
                        if (_dusmanGrid.ContainsKey(neighborKey))
                        {
                            foreach (Dusman dusman in _dusmanGrid[neighborKey])
                            {
                                if (kule.MenzilIcindeMi(dusman) && !dusman.OluMu())
                                {
                                    kule.Saldir(dusman, dusmanlari);
                                    hedefBulundu = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (kule.SaldirildiTikSayisi > 0)
                    kule.SaldirildiTikSayisi--;
            }

            oyunAlani.Invalidate();
        }

        private void DalgaBitti()
        {
            dalgaAktifMi = false;
            dalgaGeriSayimAktifMi = false;
            dalgaGeriSayimSaniye = 0;

            siradakiDalgaButt.Enabled = true;
            siradakiDalgaButt.BackColor = Color.LimeGreen;
            siradakiDalgaButt.Text = "Sýradaki Dalga ?";

            System.Console.WriteLine($"Dalga {mevcutDalga} Tamamlandý! Butona basarak sonraki dalgaya geç.");
        }

        private void DalgaGeriSayimTayicisi_Tick(object sender, EventArgs e)
        {
            dalgaGeriSayimSaniye--;

            if (dalgaGeriSayimSaniye <= 0)
            {
                dalgaGeriSayimTayicisi.Stop();
                dalgaGeriSayimAktifMi = false;

                dalgaAktifMi = true;
                DalgeBaslat();
            }

            BilgiGuncelleMe();
            oyunAlani.Invalidate();
        }

        private void SiradakiDalgaButton_Click(object sender, EventArgs e)
        {
            if (dalgaAktifMi || dusmanlari.Count > 0)
            {
                MessageBox.Show("Lütfen mevcut dalgayý tamamlayýnýz!", "Uyarý");
                return;
            }

            dalgaAktifMi = true;
            
            siradakiDalgaButt.Enabled = false;
            siradakiDalgaButt.BackColor = Color.Gray;
            siradakiDalgaButt.Text = "Dalga Devam Ediyor...";

            DalgeBaslat();
        }

        private void DalgeBaslat()
        {
            int temelDusmanCani = 30;
            int dusmanCani = temelDusmanCani + (mevcutDalga * 20);
            int dusmanSayisi = 10 + (mevcutDalga - 1) * 5;
            int dusmanHizi = 2 + (mevcutDalga - 1) / 5;

            kalanDusmanSayisi = dusmanSayisi;

            System.Console.WriteLine($"Dalga {mevcutDalga} Baþladý!");
            System.Console.WriteLine($"  -> Düþman Sayýsý: {dusmanSayisi}");
            System.Console.WriteLine($"  -> Düþman Caný: {dusmanCani}");
            System.Console.WriteLine($"  -> Düþman Hýzý: {dusmanHizi}");

            dusmanUreticisi.Tag = new int[] { dusmanSayisi, dusmanHizi, dusmanCani };
            dusmanUreticisi.Start();

            BilgiGuncelleMe();
        }

        private void DusmanUreticisi_Tick(object sender, EventArgs e)
        {
            if (dusmanRotasi.Count > 0)
            {
                object tag = dusmanUreticisi.Tag;
                int[] parametreler = tag as int[];

                if (parametreler != null && parametreler[0] > 0)
                {
                    int dusmanHizi = parametreler[1];
                    int dusmanCani = parametreler[2];

                    DusmanTuru secilenTur = DusmanTuru.Normal;
                    int dusmanHiziFinal = dusmanHizi;
                    int dusmanCaniFinal = dusmanCani;

                    if (mevcutDalga >= 3 && mevcutDalga < 5)
                    {
                        if (rastgele.Next(0, 100) < 20)
                        {
                            secilenTur = DusmanTuru.Hizli;
                            dusmanCaniFinal = (int)(dusmanCani * 0.6f);
                            dusmanHiziFinal = dusmanHizi * 2;
                        }
                    }

                    if (mevcutDalga >= 5)
                    {
                        if (rastgele.Next(0, 100) < 10)
                        {
                            secilenTur = DusmanTuru.Zirhli;
                            dusmanCaniFinal = dusmanCani * 3;
                            dusmanHiziFinal = (int)(dusmanHizi * 0.5f);
                        }
                        else if (rastgele.Next(0, 100) < 20)
                        {
                            secilenTur = DusmanTuru.Hizli;
                            dusmanCaniFinal = (int)(dusmanCani * 0.6f);
                            dusmanHiziFinal = dusmanHizi * 2;
                        }
                    }

                    Dusman yeniDusman = new Dusman(
                        can: dusmanCaniFinal, 
                        hiz: dusmanHiziFinal, 
                        rota: dusmanRotasi, 
                        maxCan: dusmanCaniFinal,
                        tur: secilenTur
                    );
                    dusmanlari.Add(yeniDusman);

                    parametreler[0]--;

                    if (parametreler[0] <= 0)
                    {
                        dusmanUreticisi.Stop();
                        mevcutDalga++;
                    }
                }
            }
        }

        #endregion

        #region Yardýmcý Metotlar (Rota, Ýnþa Alanlarý, Sýfýrlama, vb.)

        private void OlusturDusmanRotasi()
        {
            dusmanRotasi.Clear();

            dusmanRotasi.Add(new Point(0, 100));

            for (int x = 0; x <= 200; x += 20)
                dusmanRotasi.Add(new Point(x, 100));

            for (int y = 100; y <= 350; y += 20)
                dusmanRotasi.Add(new Point(200, y));

            for (int x = 200; x <= 600; x += 20)
                dusmanRotasi.Add(new Point(x, 350));

            for (int y = 350; y >= 100; y -= 20)
                dusmanRotasi.Add(new Point(600, y));

            for (int x = 600; x <= 900; x += 20)
                dusmanRotasi.Add(new Point(x, 100));

            for (int y = 100; y <= 450; y += 20)
                dusmanRotasi.Add(new Point(900, y));

            dusmanRotasi.Add(new Point(900, 450));
        }

        private void OlusturInsaAlanlari()
        {
            insaAlanlari.Clear();

            insaAlanlari.Add(new InsaAlani(90, 30));
            insaAlanlari.Add(new InsaAlani(270, 150));
            insaAlanlari.Add(new InsaAlani(130, 280));
            insaAlanlari.Add(new InsaAlani(400, 420));
            insaAlanlari.Add(new InsaAlani(530, 200));
            insaAlanlari.Add(new InsaAlani(750, 30));
        }

        private void SifirlaButton_Click(object sender, EventArgs e)
        {
            kuleler.Clear();
            dusmanlari.Clear();
            insaAlanlari.Clear();
            altin = BASLANGIC_ALTIN;
            anaBaseCan = BASLANGIC_CAN;
            mevcutDalga = 1;
            kalanDusmanSayisi = 0;
            dalgaAktifMi = false;
            dalgaGeriSayimAktifMi = false;

            dusmanTayicisi.Stop();
            kuleTayicisi.Stop();
            dusmanUreticisi.Stop();
            dalgaGeriSayimTayicisi.Stop();

            OlusturDusmanRotasi();
            OlusturInsaAlanlari();

            Panel pnlMenu = this.Controls["pnlGirisMenusu"] as Panel;
            if (pnlMenu != null)
            {
                pnlMenu.Visible = true;
                pnlMenu.BringToFront();
            }

            siradakiDalgaButt.Enabled = false;
            siradakiDalgaButt.BackColor = Color.Gray;
            siradakiDalgaButt.Text = "Sýradaki Dalga ?";
            
            siradakiDalgaButt.BringToFront();
            sifirlaButto.BringToFront();
            oyunAlani.SendToBack();

            oyunAlani.Invalidate();
            BilgiGuncelleMe();
        }

        private void BilgiGuncelleMe()
        {
            string dusmaniYazisi = dalgaAktifMi ? $"Kalan: {kalanDusmanSayisi}" : "Bekleme";
            
            if (dalgaGeriSayimAktifMi)
                dusmaniYazisi = $"Geri Sayým: {dalgaGeriSayimSaniye} sn";

            bilgiLabel.Text = $"Altýn: {altin} | Kule: {kuleler.Count} | Ana Üs Caný: {anaBaseCan} | Dalga: {mevcutDalga} | {dusmaniYazisi}";
        }

        private System.Drawing.Drawing2D.GraphicsPath OlusturYolPath()
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            
            for (int i = 0; i < dusmanRotasi.Count - 1; i++)
                path.AddLine(dusmanRotasi[i], dusmanRotasi[i + 1]);

            return path;
        }

        private int GetGridKey(float x, float y)
        {
            int gridX = (int)x / GRID_SIZE;
            int gridY = (int)y / GRID_SIZE;
            return gridX * 10000 + gridY;
        }

        private void GuncelleDusmanGrid()
        {
            _dusmanGrid.Clear();
            foreach (Dusman dusman in dusmanlari)
            {
                int key = GetGridKey(dusman.X, dusman.Y);
                if (!_dusmanGrid.ContainsKey(key))
                    _dusmanGrid[key] = new List<Dusman>();
                _dusmanGrid[key].Add(dusman);
            }
        }

        private string GetCachedHudText()
        {
            if (_lastAltinCache != altin || _lastCanCache != anaBaseCan || 
                _lastDalgaCache != mevcutDalga || _lastDusmanCache != kalanDusmanSayisi)
            {
                _cachedHudText = $"ALTIN: {altin}  |  CAN: {anaBaseCan}  |  DALGA: {mevcutDalga}  |  DÜÞMAN: {kalanDusmanSayisi}";
                _lastAltinCache = altin;
                _lastCanCache = anaBaseCan;
                _lastDalgaCache = mevcutDalga;
                _lastDusmanCache = kalanDusmanSayisi;
            }
            return _cachedHudText;
        }
        private bool oyunBittiGosterildi = false;
        private void GosterOyunBittiDialog(int basariliDalgalar)
        {
            if (oyunBittiGosterildi == false)
            {

                



                string mesaj = $"Oyun Bitti! Kale Yýkýldý!\n\n" +
                    $"Baþarýlý Dalgalar: {basariliDalgalar}\n" +
                    $"Kazanýlan Altýn: {altin}\n\n" +
                    $"En Yüksek Skorlarý göstermek ister misiniz?";

                DialogResult sonuc = MessageBox.Show(mesaj, "Oyun Bitti",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                oyunBittiGosterildi = true;
                if (sonuc == DialogResult.Yes)
                {
                    GosterEnYuksekSkorlar();
                    oyunBittiGosterildi = false;
                }
                else if (sonuc == DialogResult.No)
                {
                    oyunBittiGosterildi = false;
                }
            }
        }

        private void GosterEnYuksekSkorlar()
        {
            List<OyunSkoru> skorlar = veriTabani.EnYuksekSkorlariGetir();
            
            if (skorlar.Count == 0)
            {
                MessageBox.Show("Henüz kayýtlý skor yok.", "En Yüksek Skorlar");
                return;
            }
            
            string skorListesi = "=== EN YÜKSEK SKORLAR ===\n\n";
            
            for (int i = 0; i < skorlar.Count; i++)
            {
                OyunSkoru skoru = skorlar[i];
                skorListesi += $"{i + 1}. {skoru.OyuncuAdi}\n" +
                    $"   Dalga: {skoru.BasariliDalgalar} | Altýn: {skoru.KazanýlanAltin}\n" +
                    $"   Kale Caný: {skoru.KaleTCaný}/20 | {skoru.TarihSaat:dd.MM.yyyy HH:mm}\n\n";
            }
            
            MessageBox.Show(skorListesi, "En Yüksek Skorlar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PesEtButton_Click(object sender, EventArgs e)
        {
            if (!dalgaAktifMi && dusmanlari.Count == 0)
            {
                MessageBox.Show("Oyun baþlamadý! Lütfen 'Dalga >>>' butonuna basýnýz.", "Uyarý");
                return;
            }

            DialogResult sonuc = MessageBox.Show(
                "Oyundan pes etmek istediðinize emin misiniz?",
                "Oyundan Çýk",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (sonuc == DialogResult.Yes)
            {
                // ÖNCE timerlarý durdur
                dusmanTayicisi.Stop();
                kuleTayicisi.Stop();
                dusmanUreticisi.Stop();
                dalgaGeriSayimTayicisi.Stop();

                int basariliDalgalar = mevcutDalga - 1;
                
                // Veritabanýna kaydet
                veriTabani.OyunSonuKaydet(mevcutOyuncuAdi, basariliDalgalar, altin, anaBaseCan);
                
                // Normal kaybetme ekranýný göster
                GosterOyunBittiDialog(basariliDalgalar);
                
                // EN SON sýfýrla
                SifirlaButton_Click(null, null);
            }
        }

        #endregion

        #region Çizim Ýþlemleri (Paint Metodu)

        private void OyunAlani_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.ForestGreen);

            e.Graphics.DrawPath(yolKalemi, OlusturYolPath());

            foreach (InsaAlani alan in insaAlanlari)
            {
                e.Graphics.FillRectangle(insaAlaniIci, 
                    alan.X, alan.Y, alan.Genislik, alan.Yukseklik);
                
                e.Graphics.DrawRectangle(insaAlaniCercevesi, 
                    alan.X, alan.Y, alan.Genislik, alan.Yukseklik);
            }

            foreach (Kule kule in kuleler)
            {
                int kuleX = (int)kule.X - KULE_GENISLIK / 2;
                int kuleY = (int)kule.Y - KULE_YUKSEKLIGI / 2;

                bool gorselCizildi = false;

                try
                {
                    if (kule is OkcuKulesi && okcuGorseli != null)
                    {
                        e.Graphics.DrawImage(okcuGorseli, kuleX, kuleY, KULE_GENISLIK, KULE_YUKSEKLIGI);
                        gorselCizildi = true;
                    }
                    else if (kule is TopcuKulesi && topcuGorseli != null)
                    {
                        e.Graphics.DrawImage(topcuGorseli, kuleX, kuleY, KULE_GENISLIK, KULE_YUKSEKLIGI);
                        gorselCizildi = true;
                    }
                }
                catch
                {
                    gorselCizildi = false;
                }

                if (!gorselCizildi)
                {
                    Brush kuleRengi = kule is OkcuKulesi ? okcuKuleBrush : topcuKuleBrush;
                    
                    e.Graphics.FillEllipse(kuleRengi, 
                        kule.X - KULE_GENISLIK / 2, kule.Y - KULE_YUKSEKLIGI / 2, 
                        KULE_GENISLIK, KULE_YUKSEKLIGI);
                    e.Graphics.DrawEllipse(kuleCercevesi, 
                        kule.X - KULE_GENISLIK / 2, kule.Y - KULE_YUKSEKLIGI / 2, 
                        KULE_GENISLIK, KULE_YUKSEKLIGI);
                }

                if (kule == _fareAltindakiKule)
                {
                    e.Graphics.DrawEllipse(kuleMenzili, 
                        kule.X - kule.Menzil, kule.Y - kule.Menzil, 
                        kule.Menzil * 2, kule.Menzil * 2);
                }

                string seviyeYazisi = $"Lv.{kule.Seviye}";
                e.Graphics.DrawString(seviyeYazisi, kuleSeviyelyFont, Brushes.White, 
                    kule.X - 10, kule.Y + 10);

                if (kule.SonHedefDusman != null && kule.SaldirildiTikSayisi > 0)
                {
                    Pen lazerKalemi = kule is OkcuKulesi ? lazerOkcuPen : lazerTopcuPen;
                    e.Graphics.DrawLine(lazerKalemi, 
                        (int)kule.X, (int)kule.Y, 
                        (int)kule.SonHedefDusman.X, (int)kule.SonHedefDusman.Y);
                }
            }

            foreach (Dusman dusman in dusmanlari)
            {
                Brush dusmanBrush = _dusmanBrushCache[dusman.Tur];
                
                e.Graphics.FillRectangle(dusmanBrush, 
                    (int)dusman.X - DUSMAN_GENISLIGI / 2, 
                    (int)dusman.Y - DUSMAN_YUKSEKLIGI / 2, 
                    DUSMAN_GENISLIGI, DUSMAN_YUKSEKLIGI);
                e.Graphics.DrawRectangle(dusmanCercevesi, 
                    (int)dusman.X - DUSMAN_GENISLIGI / 2, 
                    (int)dusman.Y - DUSMAN_YUKSEKLIGI / 2, 
                    DUSMAN_GENISLIGI, DUSMAN_YUKSEKLIGI);

                int barX = (int)dusman.X - DUSMAN_CAN_BARSI_GENISLIGI / 2;
                int barY = (int)dusman.Y - DUSMAN_YUKSEKLIGI / 2 - 8;

                e.Graphics.FillRectangle(dusmanCanBarsiArkaplan, 
                    barX, barY, DUSMAN_CAN_BARSI_GENISLIGI, DUSMAN_CAN_BARSI_YUKSEKLIGI);

                float canYuzdesi = dusman.CanYuzdesi();
                int yesilGenisligi = (int)(DUSMAN_CAN_BARSI_GENISLIGI * canYuzdesi);
                e.Graphics.FillRectangle(dusmanCanBarsýYesil, 
                    barX, barY, yesilGenisligi, DUSMAN_CAN_BARSI_YUKSEKLIGI);

                e.Graphics.DrawRectangle(dusmanCercevesi, 
                    barX, barY, DUSMAN_CAN_BARSI_GENISLIGI, DUSMAN_CAN_BARSI_YUKSEKLIGI);
            }

            int kaleX = KALE_X - KALE_GENISLIK / 2;
            int kaleY = KALE_Y - KALE_YUKSEKLIGI / 2;

            try
            {
                if (kaleGorseli != null)
                {
                    e.Graphics.DrawImage(kaleGorseli, kaleX, kaleY, KALE_GENISLIK, KALE_YUKSEKLIGI);
                }
                else
                {
                    e.Graphics.FillRectangle(topcuKuleBrush, kaleX, kaleY, KALE_GENISLIK, KALE_YUKSEKLIGI);
                    e.Graphics.DrawRectangle(kaleCercevesi, kaleX, kaleY, KALE_GENISLIK, KALE_YUKSEKLIGI);
                }
            }
            catch
            {
                e.Graphics.FillRectangle(topcuKuleBrush, kaleX, kaleY, KALE_GENISLIK, KALE_YUKSEKLIGI);
                e.Graphics.DrawRectangle(kaleCercevesi, kaleX, kaleY, KALE_GENISLIK, KALE_YUKSEKLIGI);
            }

            int barX2 = KALE_X - KALE_CAN_BARSI_GENISLIGI / 2;
            int barY2 = KALE_Y + KALE_YUKSEKLIGI / 2 + 10;

            e.Graphics.FillRectangle(kaleCanBarsiArkaplan, 
                barX2, barY2, KALE_CAN_BARSI_GENISLIGI, KALE_CAN_BARSI_YUKSEKLIGI);

            float canYuzde = (float)anaBaseCan / MAKSIMUM_CAN;
            int yesilGenislik2 = (int)(KALE_CAN_BARSI_GENISLIGI * canYuzde);
            e.Graphics.FillRectangle(kaleCanBarsýYesil, 
                barX2, barY2, yesilGenislik2, KALE_CAN_BARSI_YUKSEKLIGI);

            e.Graphics.DrawRectangle(kaleCercevesi, 
                barX2, barY2, KALE_CAN_BARSI_GENISLIGI, KALE_CAN_BARSI_YUKSEKLIGI);

            string kaleCanYazisi = $"KALECan: {anaBaseCan}/{MAKSIMUM_CAN}";
            SizeF yaziBoyutu = e.Graphics.MeasureString(kaleCanYazisi, kaleCanFont);
            float yazýX = KALE_X - yaziBoyutu.Width / 2;
            float yazýY = barY2 - 20;
            
            e.Graphics.DrawString(kaleCanYazisi, kaleCanFont, hudYaziBrush, yazýX, yazýY);

            CizLegend(e);

            if (dalgaGeriSayimAktifMi)
            {
                string geriSayimYazisi = $"Sonraki Dalga: {dalgaGeriSayimSaniye}";
                SizeF yaziBoyutu2 = e.Graphics.MeasureString(geriSayimYazisi, buyukFont);
                
                float yaziX2 = (oyunAlani.Width - yaziBoyutu2.Width) / 2;
                float yaziY2 = (oyunAlani.Height - yaziBoyutu2.Height) / 2;

                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 0, 0)), 
                    yaziX2 - 20, yaziY2 - 20, yaziBoyutu2.Width + 40, yaziBoyutu2.Height + 40);

                e.Graphics.DrawString(geriSayimYazisi, buyukFont, hudYaziBrush, yaziX2, yaziY2);
            }

            CizHUD(e);
        }

        private void CizHUD(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(hudArkaplan, 0, 0, oyunAlani.Width, HUD_YUKSEKLIGI);

            string hudBilgisi = GetCachedHudText();
            
            SizeF textSize = e.Graphics.MeasureString(hudBilgisi, hudFont);
            float textY = (HUD_YUKSEKLIGI - textSize.Height) / 2;

            e.Graphics.DrawString(hudBilgisi, hudFont, hudYaziBrush, 15, textY);
        }

        private void CizLegend(PaintEventArgs e)
        {
            int legendX = 10;
            int legendY = 450;
            int legendGenisligi = 150;
            int legendYuksekligi = 45;

            e.Graphics.FillRectangle(legendArkaplan, legendX, legendY, legendGenisligi, legendYuksekligi);
            e.Graphics.DrawRectangle(legendCercevesi, legendX, legendY, legendGenisligi, legendYuksekligi);

            e.Graphics.FillRectangle(Brushes.Red, legendX + 5, legendY + 3, 10, 10);
            e.Graphics.DrawRectangle(legendCercevesi, legendX + 5, legendY + 3, 10, 10);
            e.Graphics.DrawString("Normal", legendFont, legendYaziBrush, legendX + 18, legendY + 3);

            e.Graphics.FillRectangle(Brushes.Yellow, legendX + 5, legendY + 16, 10, 10);
            e.Graphics.DrawRectangle(legendCercevesi, legendX + 5, legendY + 16, 10, 10);
            e.Graphics.DrawString("Hýzlý", legendFont, legendYaziBrush, legendX + 18, legendY + 16);

            e.Graphics.FillRectangle(Brushes.DarkBlue, legendX + 5, legendY + 29, 10, 10);
            e.Graphics.DrawRectangle(legendCercevesi, legendX + 5, legendY + 29, 10, 10);
            e.Graphics.DrawString("Zýrhlý", legendFont, legendYaziBrush, legendX + 18, legendY + 29);
        }

        #endregion
    }
}
