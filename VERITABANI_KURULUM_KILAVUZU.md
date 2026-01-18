# Tower Defense - Veritabaný Entegrasyonu Kurulum Kýlavuzu

## Adým 1: NuGet Paketini Yükleyin

Projenizde System.Data.SQLite paketini kurmanýz gerekmektedir.

### Visual Studio aracýlýðýyla:

1. **Araçlar ? NuGet Paket Yöneticisi ? Paket Yöneticisi Konsolu** açýn
2. Aþaðýdaki komutu yazýn ve Enter'a basýn:

```
Install-Package System.Data.SQLite
```

Paket baþarýyla yüklendiðinde, konsol þu mesajý gösterecektir:
```
Successfully installed 'System.Data.SQLite'
```

## Adým 2: Using Statement Ekleyin

Form1.cs dosyasýnýn en baþýnda aþaðýdaki using statement'i etkinleþtirin:

```csharp
using System.Data.SQLite;
```

Bu satýr þu anda yorum yapýlmýþtýr, aktifleþtirmeniz gerekmektedir.

## Adým 3: VeritabaniYoneticisi.cs Kodunu Aktifleþtirin

`VeritabaniYoneticisi.cs` dosyasýnda SQLite ile ilgili tüm `/* ... */` yorum bloklarýný silin ve kodu aktifleþtirin.

## Özellikleri

? **Oyun Skorlarý Kaydedilir:**
- Oyuncu adý
- Baþarýlý dalgalar sayýsý
- Kazanýlan altýn miktarý
- Kale caný durumu
- Oynanýþ tarihi ve saati

? **En Yüksek Skorlar Görüntülenebilir:**
- En iyi 10 skor listelenir
- Dalgalara göre sýralanýr
- Tarih/saat bilgisi ile gösterilir

## Veritabaný Dosyasý

Oyun ilk çalýþtýrýldýðýnda `TowerDefense.db` dosyasý otomatik olarak oluþturulacaktýr.

Bu dosya aþaðýdaki konumda yer alacaktýr:
```
C:\Users\[KullanýcýAdý]\OneDrive\Desktop\TowerDefenseOyunuProje\TowerDefenseOyunuProje\TowerDefense.db
```

## Kullaným

1. Oyunu baþlatýn ve oyun sonunda kale yýkýldýðýnda:
2. "En Yüksek Skorlarý görmek ister misiniz?" sorusuna **Evet** diye cevap verin
3. Kayýtlý tüm skorlar listelenecektir

## Sorun Giderme

Eðer hala `CS0234` hatasý alýyorsanýz:
1. Visual Studio'yu kapatýn
2. Proje klasörü içindeki `bin` ve `obj` klasörlerini silin
3. Projeyi yeniden açýn ve Build All yapýn
