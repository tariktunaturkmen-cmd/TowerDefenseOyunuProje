using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TowerDefenseOyunuProje
{
    // KALITIM (INHERITANCE): TopcuKulesi, Kule sýnýfýndan türüyor
    // TopcuKulesi bir Kule'dir (IS-A iliþkisi)
    public class TopcuKulesi : Kule
    {
        // Constructor - base sýnýfýn constructor'ýný çaðýrýyoruz
        // YENÝ: Topçu Kulesi => Yavaþ Ateþ (2000ms), Orta Menzil (130)
        public TopcuKulesi(float x, float y)
            : base(hasar: 25, menzil: 120, fiyat: 100, x: x, y: y, atesHizi: 2000, maxCan: 120)
        {
        }

        // ÇOK BÝÇÝMLÝLÝK (POLYMORPHISM): Saldir metodunu override ediyoruz
        // Bu metot TopcuKulesi için özel davranýþ tanýmlýyor
        // YENÝ: tumDusmanlar parametresi - AoE hasar hesabý için
        public override void Saldir(Dusman hedef, List<Dusman> tumDusmanlar)
        {
            // Cooldown kontrolü: Yeterli zaman geçtiyse ateþ et
            if (!this.AtesiAcabilirMi())
                return;

            if (hedef == null || hedef.Can <= 0)
                return;

            // Son ateþ zamanýný güncelle
            this.SonAtesZamaniniGuncelle();

            // ALAN HASARI (AoE - Area of Effect): Topçu kulesi çevre hasarý verir
            int aoeMenzil = 100; // AoE menzili: 100 birim
            int aoeFarkliHasar = this.Hasar / 2; // Çevre düþmanlar daha az hasar alýr

            // Birincil hedef - Full hasar
            hedef.HasarAl(this.Hasar);
            System.Console.WriteLine($"Topçu Kulesi {hedef} ana hedef vuruþ! Hasar: {this.Hasar}");

            // YENÝ: Çevre düþmanlarýna da hasar ver
            if (tumDusmanlar != null)
            {
                foreach (Dusman diger in tumDusmanlar)
                {
                    // Kendisine hasar verme ve ölü düþmanlarý atla
                    if (diger == hedef || diger.OluMu())
                        continue;

                    // AoE menzili içinde mi kontrol et
                    float dx = diger.X - hedef.X;
                    float dy = diger.Y - hedef.Y;
                    float mesafe = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (mesafe <= aoeMenzil)
                    {
                        diger.HasarAl(aoeFarkliHasar); // Çevre hasarý ver
                        System.Console.WriteLine($"  -> Çevre hasar: {diger} ek hasar aldý: {aoeFarkliHasar}");
                    }
                }
            }

            // Son hedefi ve saldýrý bilgisini depolayýp lazer çizimi yapýlacak
            this.SonHedefDusman = hedef;
            this.SaldirildiTikSayisi = 3; // YENÝ: Topçu biraz daha uzun (3 frame) - daha etkileyici

            // Topçu Kulesi yavaþ ateþ eder, çok hasar verir
            System.Console.WriteLine($"Topçu Kulesi saldýrý! Ana Hasar: {this.Hasar}, Düþmanýn kalan can: {hedef.Can}");
        }
    }
}
