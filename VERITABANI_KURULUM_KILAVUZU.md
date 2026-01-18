# Tower Defense - Veritabanı Entegrasyonu Kurulum Kılavuzu

## Adım 1: NuGet Paketini Yükleyin

Projenizde System.Data.SQLite paketini kurmanız gerekmektedir.

### Visual Studio aracılığıyla:

1. **Araçlar   NuGet Paket Yöneticisi   Paket Yöneticisi Konsolu** açın
2. Aşağıdaki komutu yazın ve Enter'a basın:

```
Install-Package System.Data.SQLite
```

Paket başarıyla yüklendiğinde, konsol şu mesajı gösterecektir:
```
Successfully installed 'System.Data.SQLite'
```

## Adım 2: Using Statement Ekleyin

Form1.cs dosyasının en başında aşağıdaki using statement'i etkinleştirin:

```csharp
using System.Data.SQLite;
```

Bu satır şu anda yorum yapılmıştır, aktifleştirmeniz gerekmektedir.

## Adım 3: VeritabaniYoneticisi.cs Kodunu Aktifleştirin

`VeritabaniYoneticisi.cs` dosyasında SQLite ile ilgili tüm `/* ... */` yorum bloklarını silin ve kodu aktifleştirin.

## Özellikleri

  **Oyun Skorları Kaydedilir:**
- Oyuncu adı
- Başarılı dalgalar sayısı
- Kazanılan altın miktarı
- Kale canı durumu
- Oynanış tarihi ve saati

  **En Yüksek Skorlar Görüntülenebilir:**
- En iyi 10 skor listelenir
- Dalgalara göre sıralanır
- Tarih/saat bilgisi ile gösterilir

## Veritabanı Dosyası

Oyun ilk çalıştırıldığında `TowerDefense.db` dosyası otomatik olarak oluşturulacaktır.

Bu dosya aşağıdaki konumda yer alacaktır:
```
C:\Users\[KullanıcıAdı]\OneDrive\Desktop\TowerDefenseOyunuProje\TowerDefenseOyunuProje\TowerDefense.db
```

## Kullanım

1. Oyunu başlatın ve oyun sonunda kale yıkıldığında:
2. "En Yüksek Skorları görmek ister misiniz " sorusuna **Evet** diye cevap verin
3. Kayıtlı tüm skorlar listelenecektir

## Sorun Giderme

Eğer hala `CS0234` hatası alıyorsanız:
1. Visual Studio'yu kapatın
2. Proje klasörü içindeki `bin` ve `obj` klasörlerini silin
3. Projeyi yeniden açın ve Build All yapın
