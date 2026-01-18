using System;
using System.Collections.Generic;
using System.Drawing;

namespace TowerDefenseOyunuProje
{
    // YENÝ: Düþman Türü Enum - 3 çeþit düþman
    public enum DusmanTuru
    {
        Normal = 0,    // Kýrmýzý - Dengeli
        Hizli = 1,     // Sarý - Hýzlý ama az can
        Zirhli = 2     // Koyu Mavi - Yavaþ ama çok can
    }

    // INTERFACE UYGULAMASI (IMPLEMENTATION): Dusman, IHareketEdebilir'i implement ediyor
    // Bu, "Düþman hareket edebilir" anlamýna gelir
    public class Dusman : IHareketEdebilir
    {
        private int _can;
        private int _maxCan; // Düþmanýn maksimum caný (Can barý için)
        private int _hiz;
        private float _x;
        private float _y;
        private List<Point> _rota;
        private int _rotaIndex = 0;
        private DusmanTuru _tur; // YENÝ: Düþman türü

        public int Can
        {
            get { return _can; }
            set { _can = value > 0 ? value : 0; }
        }

        // Maksimum Can Property
        public int MaxCan
        {
            get { return _maxCan; }
            set { _maxCan = value > 0 ? value : 1; }
        }

        public int Hiz
        {
            get { return _hiz; }
            set { _hiz = value > 0 ? value : 1; }
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

        public List<Point> Rota
        {
            get { return _rota; }
            set { _rota = value; }
        }

        public int RotaIndex
        {
            get { return _rotaIndex; }
            set { _rotaIndex = value; }
        }

        // YENÝ: Düþman Türü Property
        public DusmanTuru Tur
        {
            get { return _tur; }
            set { _tur = value; }
        }

        // Constructor: Düþmaný rota ile oluþturur
        // YENÝ: Tür parametresi eklendi
        public Dusman(int can, int hiz, List<Point> rota, int maxCan = 0, DusmanTuru tur = DusmanTuru.Normal)
        {
            MaxCan = maxCan > 0 ? maxCan : can;
            Can = can;
            Hiz = hiz;
            Rota = rota;
            Tur = tur;
            
            if (rota != null && rota.Count > 0)
            {
                X = rota[0].X;
                Y = rota[0].Y;
            }
        }

        // INTERFACE ÜYESÝ METOT: Ilerle metodu implement ediyoruz
        // Bu metot, IHareketEdebilir interface'inin sözleþmesini yerine getirir
        // Düþman rotayý takip ederek hareket ediyor
        public void Ilerle()
        {
            if (_rota == null || _rota.Count == 0)
                return;

            // Sonraki rota noktasýna yönel
            if (_rotaIndex < _rota.Count)
            {
                Point hedefNokta = _rota[_rotaIndex];
                float dx = hedefNokta.X - _x;
                float dy = hedefNokta.Y - _y;
                float mesafe = (float)Math.Sqrt(dx * dx + dy * dy);

                if (mesafe < _hiz)
                {
                    // Sonraki rotaya geç
                    _rotaIndex++;
                    if (_rotaIndex >= _rota.Count)
                    {
                        // Rota tamamlandý
                        return;
                    }
                }
                else
                {
                    // Hedef noktaya doðru ilerle
                    float oran = _hiz / mesafe;
                    X += dx * oran;
                    Y += dy * oran;
                }
            }
        }

        // Düþman rotayý tamamladý mý kontrolü (Kaleye ulaþtý mý)
        public bool KaleyeleMiUlasti()
        {
            return _rotaIndex >= (_rota?.Count ?? 0);
        }

        // Düþman rotayý tamamladý mý kontrolü (Eski isim - uyumluluk için)
        public bool RotayiTamamladiMi()
        {
            return KaleyeleMiUlasti();
        }

        // Düþman hasar alsýn
        public void HasarAl(int hasar)
        {
            Can -= hasar;
            if (Can < 0)
                Can = 0;
        }

        // Düþman ölü mü kontrolü
        public bool OluMu()
        {
            return Can <= 0;
        }

        // Can yüzdesi (Can barý için) - 0.0 ile 1.0 arasýnda
        public float CanYuzdesi()
        {
            if (_maxCan <= 0)
                return 0;
            return (float)_can / _maxCan;
        }

        // YENÝ: Düþman türüne göre rengi döndür
        public Color TurRengini()
        {
            switch (_tur)
            {
                case DusmanTuru.Hizli:
                    return Color.Yellow; // Sarý - Hýzlý
                case DusmanTuru.Zirhli:
                    return Color.DarkBlue; // Koyu Mavi - Zýrhlý
                default:
                    return Color.Red; // Kýrmýzý - Normal
            }
        }

        // YENÝ: Düþman türüne göre adý döndür
        public string TurAdi()
        {
            switch (_tur)
            {
                case DusmanTuru.Hizli:
                    return "Hýzlý";
                case DusmanTuru.Zirhli:
                    return "Zýrhlý";
                default:
                    return "Normal";
            }
        }
    }
}
