using System;
using System.Collections.Generic;
using System.Drawing;

namespace TowerDefenseOyunuProje
{
    public abstract class Kule
    {
        private int _hasar;
        private int _menzil;
        private int _fiyat;
        private float _x;
        private float _y;
        private Dusman _sonHedefDusman = null;
        private int _saldirildiTikSayisi = 0;
        
        // ===== KULE CAN SÝSTEMÝ =====
        private int _can = 100;
        private int _maxCan = 100;

        private int _atesHizi;
        private DateTime _sonAtesZamani = DateTime.Now;
        private int _seviye = 1;

        // Property ile hasar kontrol ediliyor
        public int Hasar
        {
            get { return _hasar; }
            set { _hasar = value > 0 ? value : 1; }
        }

        // Property ile menzil kontrol ediliyor
        public int Menzil
        {
            get { return _menzil; }
            set { _menzil = value > 0 ? value : 10; }
        }

        // Property ile fiyat kontrol ediliyor - ÇOK ÖNEMLÝ: Fiyat negatif olamaz!
        public int Fiyat
        {
            get { return _fiyat; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Fiyat negatif olamaz!");
                _fiyat = value;
            }
        }

        public float X
        {
            get { return _x; }
            set { _x = value; }
        }

        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        // YENÝ: Ateþ Hýzý Property (ms)
        public int AtesHizi
        {
            get { return _atesHizi; }
            set { _atesHizi = value > 0 ? value : 100; }
        }

        // YENÝ: Son Ateþ Zamaný Property
        public DateTime SonAtesZamani
        {
            get { return _sonAtesZamani; }
            set { _sonAtesZamani = value; }
        }

        // YENÝ: Seviye Property
        public int Seviye
        {
            get { return _seviye; }
            set { _seviye = value > 0 ? value : 1; }
        }

        // Saldýrý animasyonu için son hedef düþman
        public Dusman SonHedefDusman
        {
            get { return _sonHedefDusman; }
            set { _sonHedefDusman = value; }
        }

        // YENÝ: Lazer çizme için saldýrý tick sayýsý (GÜNCELLENMIÞ: 2 frame boyunca yanýp sönsün)
        public int SaldirildiTikSayisi
        {
            get { return _saldirildiTikSayisi; }
            set { _saldirildiTikSayisi = value; }
        }

        // ===== KULE CAN SÝSTEMÝ =====
        public int Can
        {
            get { return _can; }
            set { _can = value > 0 ? value : 0; }
        }

        public int MaxCan
        {
            get { return _maxCan; }
            set { _maxCan = value > 0 ? value : 1; }
        }

        // Constructor
        protected Kule(int hasar, int menzil, int fiyat, float x, float y, int atesHizi, int maxCan = 100)
        {
            Hasar = hasar;
            Menzil = menzil;
            Fiyat = fiyat;
            X = x;
            Y = y;
            AtesHizi = atesHizi;
            MaxCan = maxCan;
            Can = maxCan;
            Seviye = 1; // Baþlangýçta Seviye 1
        }

        // SOYUTLAMA: Abstract metot - alt sýnýflar bunu implement etmek zorunda
        // YENÝ: tumDusmanlar parametresi eklendi (AoE hesabý için)
        public abstract void Saldir(Dusman hedef, List<Dusman> tumDusmanlar);

        // YENÝ: Seviye Yükseltme Metodu
        public virtual void SeviyeAtla()
        {
            // Hasar 2 katýna çýksýn
            Hasar = Hasar * 2;

            // Menzil 20 birim artsýn
            Menzil = Menzil + 20;

            // Can %20 artsýn (Ama max. deðeri geçmesin)
            Can = Math.Min(Can + (MaxCan / 5), MaxCan);

            // Seviye artsýn
            Seviye++;

            System.Console.WriteLine($"Kule Seviye Yükseltildi! Yeni Seviye: {Seviye}");
            System.Console.WriteLine($"  -> Yeni Hasar: {Hasar}");
            System.Console.WriteLine($"  -> Yeni Menzil: {Menzil}");
            System.Console.WriteLine($"  -> Yeni Can: {Can}/{MaxCan}");
        }

        // YENÝ: Yükseltme Bedeli (Kule Tipine göre orijinal fiyat)
        public int YukseltmeBedeli()
        {
            // Yapýcýda verilen orijinal fiyat
            // Okçu için 50, Topçu için 150
            if (this is OkcuKulesi)
                return 50;
            else if (this is TopcuKulesi)
                return 150;
            else
                return 100; // Varsayýlan
        }

        // YENÝ: Cooldown kontrolü - Yeterli zaman geçti mi?
        public bool AtesiAcabilirMi()
        {
            return (DateTime.Now - _sonAtesZamani).TotalMilliseconds >= _atesHizi;
        }

        // YENÝ: Son ateþ zamanýný güncelle
        public void SonAtesZamaniniGuncelle()
        {
            _sonAtesZamani = DateTime.Now;
        }

        // YENÝ: Menzil kontrolü (OPTÝMÝZE EDÝLMÝÞ - Squared Distance, Karekök yok!)
        public bool MenzilIcindeMi(Dusman dusman)
        {
            // OPTÝMÝZE: Math.Sqrt çaðrýsý yerine squared distance kullan
            // Karekök almak iþlemciyi yorar, bu daha hýzlý
            float dx = dusman.X - this.X;
            float dy = dusman.Y - this.Y;
            
            // Mesafenin karesini karþýlaþtýr: sqrt(dx²+dy²) <= r ? dx²+dy² <= r²
            float mesafeKaresi = dx * dx + dy * dy;
            float menzilKaresi = this.Menzil * this.Menzil;
            
            return mesafeKaresi <= menzilKaresi;
        }

        // YENÝ: Kuleye hasar verme metodu
        public void HasarAl(int hasar)
        {
            Can -= hasar;
            if (Can < 0)
                Can = 0;
            System.Console.WriteLine($"Kule hasar aldý! Kalan Can: {Can}/{MaxCan}");
        }

        // YENÝ: Kule yok mu oldu kontrolu
        public bool OluMu()
        {
            return Can <= 0;
        }

        // YENÝ: Can yüzdesini döndürür (örneðin: %75 için 0.75 döner)
        public float CanYuzdesi()
        {
            if (MaxCan <= 0)
                return 0;
            return (float)Can / MaxCan;
        }
    }
}
