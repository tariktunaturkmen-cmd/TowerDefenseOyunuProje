using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace TowerDefenseOyunuProje
{
    public class VeriTabaniYoneticisi
    {
        private const string VT_DOSYA = "TowerDefense.db";
        private const string BAGLANTI_YAZISI = "Data Source=TowerDefense.db;Version=3;";

        public VeriTabaniYoneticisi()
        {
            BaglantiyiBaslat();
        }

        private void BaglantiyiBaslat()
        {
            try
            {
                if (!File.Exists(VT_DOSYA))
                {
                    SQLiteConnection.CreateFile(VT_DOSYA);
                }

                using (SQLiteConnection baglanti = new SQLiteConnection(BAGLANTI_YAZISI))
                {
                    baglanti.Open();

                    string tablaSorgusu = @"
                        CREATE TABLE IF NOT EXISTS OyunSkorlari (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            OyuncuAdi TEXT NOT NULL,
                            BasariliDalgalar INTEGER NOT NULL,
                            KazanýlanAltin INTEGER NOT NULL,
                            KaleTCaný INTEGER NOT NULL,
                            TarihSaat DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";

                    using (SQLiteCommand komut = new SQLiteCommand(tablaSorgusu, baglanti))
                    {
                        komut.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Veritabaný baþlatma hatasý: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Oyun sonunu veritabanýna kaydeder
        /// </summary>
        public void OyunSonuKaydet(string oyuncuAdi, int basariliDalgalar, int kazanilanAltin, int kalanCan)
        {
            try
            {
                using (SQLiteConnection baglanti = new SQLiteConnection(BAGLANTI_YAZISI))
                {
                    baglanti.Open();

                    string eklemeSorgusu = @"
                        INSERT INTO OyunSkorlari (OyuncuAdi, BasariliDalgalar, KazanýlanAltin, KaleTCaný)
                        VALUES (@oyuncu, @dalga, @altin, @can)";

                    using (SQLiteCommand komut = new SQLiteCommand(eklemeSorgusu, baglanti))
                    {
                        komut.Parameters.AddWithValue("@oyuncu", oyuncuAdi ?? "Oyuncu");
                        komut.Parameters.AddWithValue("@dalga", basariliDalgalar);
                        komut.Parameters.AddWithValue("@altin", kazanilanAltin);
                        komut.Parameters.AddWithValue("@can", kalanCan);

                        komut.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Veritabanýna kaydetme hatasý: {ex.Message}");
            }
        }

        /// <summary>
        /// En yüksek 10 skoru getirir
        /// </summary>
        public List<OyunSkoru> EnYuksekSkorlariGetir()
        {
            List<OyunSkoru> skorlar = new List<OyunSkoru>();

            try
            {
                using (SQLiteConnection baglanti = new SQLiteConnection(BAGLANTI_YAZISI))
                {
                    baglanti.Open();

                    string sorgulama = @"
                        SELECT OyuncuAdi, BasariliDalgalar, KazanýlanAltin, KaleTCaný, TarihSaat
                        FROM OyunSkorlari
                        ORDER BY BasariliDalgalar DESC, KazanýlanAltin DESC
                        LIMIT 10";

                    using (SQLiteCommand komut = new SQLiteCommand(sorgulama, baglanti))
                    {
                        using (SQLiteDataReader okuyucu = komut.ExecuteReader())
                        {
                            while (okuyucu.Read())
                            {
                                skorlar.Add(new OyunSkoru
                                {
                                    OyuncuAdi = okuyucu["OyuncuAdi"].ToString(),
                                    BasariliDalgalar = Convert.ToInt32(okuyucu["BasariliDalgalar"]),
                                    KazanýlanAltin = Convert.ToInt32(okuyucu["KazanýlanAltin"]),
                                    KaleTCaný = Convert.ToInt32(okuyucu["KaleTCaný"]),
                                    TarihSaat = Convert.ToDateTime(okuyucu["TarihSaat"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Veritabanýndan okuma hatasý: {ex.Message}");
            }

            return skorlar;
        }
    }

    /// <summary>
    /// Oyun skoru veri sýnýfý
    /// </summary>
    public class OyunSkoru
    {
        public string OyuncuAdi { get; set; }
        public int BasariliDalgalar { get; set; }
        public int KazanýlanAltin { get; set; }
        public int KaleTCaný { get; set; }
        public DateTime TarihSaat { get; set; }
    }
}
