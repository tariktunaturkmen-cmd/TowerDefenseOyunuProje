using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TowerDefenseOyunuProje
{
    // KALITIM (INHERITANCE): OkcuKulesi, Kule sýnýfýndan türüyor
    // OkcuKulesi bir Kule'dir (IS-A iliþkisi)
    public class OkcuKulesi : Kule
    {
        // Constructor - base sýnýfýn constructor'ýný çaðýrýyoruz
        // YENÝ: Okçu Kulesi => Hýzlý Ateþ (500ms), Yüksek Menzil (200)
        public OkcuKulesi(float x, float y) 
            : base(hasar: 15, menzil: 175, fiyat: 50, x: x, y: y, atesHizi: 500, maxCan: 80)
        {
        }

        // ÇOK BÝÇÝMLÝLÝK (POLYMORPHISM): Saldir metodunu override ediyoruz
        // Bu metot OkcuKulesi için özel davranýþ tanýmlýyor
        // YENÝ: tumDusmanlar parametresi eklendi (ileride uyumluluk için)
        public override void Saldir(Dusman hedef, List<Dusman> tumDusmanlar)
        {
            // Cooldown kontrolü: Yeterli zaman geçtiyse ateþ et
            if (!this.AtesiAcabilirMi())
                return;

            if (hedef == null || hedef.Can <= 0)
                return;

            // Son ateþ zamanýný güncelle
            this.SonAtesZamaniniGuncelle();

            // Düþmana hasar veriyoruz
            hedef.HasarAl(this.Hasar);

            // Son hedefi ve saldýrý bilgisini depolayýp lazer çizimi yapýlacak
            this.SonHedefDusman = hedef;
            this.SaldirildiTikSayisi = 2; // YENÝ: Sadece 2 frame boyunca lazer göster (Flash efekti)

            // Okçu Kulesi hýzlý ateþ eder, az hasar verir
            System.Console.WriteLine($"Okçu Kulesi saldýrý! Hasar: {this.Hasar}, Düþmanýn kalan can: {hedef.Can}");
        }
    }
}
