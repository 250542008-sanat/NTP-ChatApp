# NTP-ChatApp
## Kullanılan Teknolojiler
- C# / .NET Windows Forms
## Kurulum ve Çalıştırma Adımları
## Kurulum ve Çalıştırma Adımları

1. Bu depoyu yerel bilgisayarınıza klonlayın:
   `git clone https://github.com/250542008-sanat/NTP-ChatApp.git`
2. Proje klasörüne girin ve `NTP-ChatApp.csproj` dosyasını Visual Studio ile açın.
3. Üst menüden "Başlat" (Start) butonuna basarak veya `F5` tuşu ile uygulamayı derleyip çalıştırın.
4. İlk pencerede boş bir port (ör. `5555`) girip **Sunucuyu Başlat (Server)** butonuna basın. Ardından **+ Pencere** ile ikinci pencereyi açıp aynı porttan **Sunucuya Bağlan (Client)** diyerek mesajlaşmayı test edin.

## Uygulamanın Temel Özellikleri

* **Sunucu ve İstemci Modu:** Tek form üzerinden hem TCP sunucusu (`TcpListener`) başlatabilme hem de istemci (`TcpClient`) olarak bağlanabilme.
* **Asenkron Anlık İletişim:** `NetworkStream` ve `async/await` yapısıyla arayüzü dondurmadan arka planda sürekli mesaj dinleme ve gönderme.
* **Kolay Yerel Test:** `+ Pencere` butonu ile `127.0.0.1` (localhost) üzerinde anında ikinci sohbet penceresini açabilme.
* **Zaman Damgası ve Durum Bildirimi:** Mesajları saat ve kullanıcı adıyla listeleme, bağlantı durumunu renkli durum etiketiyle anlık takip etme.
## Uygulamanın Temel Özellikleri
(Daha sonra eklenecek)
## Ekran Görüntüleri
<img width="1345" height="748" alt="Ekran Resmi 2026-11-19 22 15 34" src="https://github.com/user-attachments/assets/9f61e84d-22e4-4cc1-8ecd-171c7fd64c9c" />
<img width="1341" height="751" alt="Ekran Resmi 2026-11-19 22 14 09" src="https://github.com/user-attachments/assets/462ae2cc-232b-4487-8319-4a36e594589e" />



## Geliştirici
Mustafa Şahingöz - 250542008
