# 🎬 MovieBlogApp

MovieBlogApp, ASP.NET Core MVC (.NET 8) kullanılarak geliştirilmiş, kullanıcıların filmleri keşfetmesine, detaylarını incelemesine, yorum ve puan eklemesine, favorilerini yönetmesine olanak tanıyan dinamik bir film blog platformudur. Proje, TMDb (The Movie Database) API'sinden film verilerini çekmekte ve kullanıcı etkileşimlerini yerel bir SQL Server veritabanında saklamaktadır.

![image](https://github.com/user-attachments/assets/ff30885e-9a65-47c2-81df-003eb0fabb1a)

## 🚀 Proje Amacı

Bu projenin temel amaçları şunlardır:

*   ASP.NET Core MVC, Entity Framework Core, ASP.NET Core Identity gibi modern .NET teknolojileriyle tam teşekküllü bir web uygulaması geliştirmek.
*   Harici bir RESTful API (TMDb) ile etkileşim kurma pratiği kazanmak.
*   Kullanıcı kimlik doğrulama ve yetkilendirme mekanizmalarını uygulamak.
*   Veritabanı tasarımı, ORM kullanımı ve migration süreçlerini deneyimlemek.
*   Dinamik ve kullanıcı dostu arayüzler oluşturmak için Razor Views ve Bootstrap kullanmak.
*   Temiz kod, katmanlı mimari ve ViewModel deseni gibi iyi yazılım geliştirme pratiklerini uygulamak.

## ✨ Temel Özellikler

*   **Film Keşfi:**
    *   Popüler Filmler listesi (TMDb API `/popular`)
    *   En Yüksek Puanlı (Top 250 benzeri) Filmler listesi (TMDb API `/top_rated`, ilk 250 ile sınırlı)
      ![image](https://github.com/user-attachments/assets/9753e4c7-a85e-4568-a079-2972ad7ccb85)
    *   Listelerde Sayfalama (Pagination)
*   **Arama:**
    *   Film adına göre arama (TMDb API `/search/movie`)
    *   Kişi (Aktör/Yönetmen) adına göre arama (TMDb API `/search/person`)
    *   Birleşik arama sonuçları sayfası (eşleşen filmler ve kişiler ayrı listelenir)
*   **Detay Sayfaları:**
    *   **Film Detayları:** Afiş, başlık, yıl, puan, oy sayısı, vizyon tarihi, yönetmen (tıklanabilir), başrol oyuncuları (tıklanabilir), özet, süre, tür etiketleri.
      ![image](https://github.com/user-attachments/assets/14dc0229-d08d-42af-b9bd-6851688d9c4c)

    *   **Kişi Detayları (Aktör/Yönetmen):** Profil resmi, isim, departman, biyografi, doğum/yaş/yer bilgileri, yönettiği filmler (slider ile), oynadığı filmler (slider ile).
      ![image](https://github.com/user-attachments/assets/cd724c22-6859-4e8b-ba4f-88dbe11fd4d7)

*   **Kullanıcı Etkileşimi:**
    *   **Yorum ve Puanlama:** Film detay sayfalarında kullanıcıların rumuz belirterek 1-10 arası puan vermesi ve yorum yazması (Yerel veritabanında saklanır).
    *   **Favori Filmler:** Giriş yapmış kullanıcıların filmleri favorilerine eklemesi/çıkarması. Favori filmlerin ayrı bir sayfada listelenmesi.
*   **Kullanıcı Yönetimi (ASP.NET Core Identity):**
    *   Kullanıcı Kaydı (Username, E-posta, Parola)
    *   Kullanıcı Girişi (Username ve Parola ile)
    *   Kullanıcı Çıkışı
    *   Temel Hesap Yönetimi (Profil, Parola Değiştirme - Özelleştirilmiş UI)
*   **Türlere Göre Filtreleme:**
    *   Yandan açılır menü (Offcanvas) ile tüm film türlerini listeleme (API'den alınır ve cache'lenir).
       ![image](https://github.com/user-attachments/assets/bce0fc70-05c7-43bf-b2bf-7389ffa3a70a)
    *   Film detay sayfasındaki tür etiketleri.
    *   Bir türe tıklandığında o türe ait filmleri listeleyen ayrı sayfa (TMDb API `/discover/movie`, sayfalamalı).
      ![image](https://github.com/user-attachments/assets/be9d2678-45e1-4765-9774-5422855918b3)


## 🛠️ Kullanılan Teknolojiler

*   **Framework:** ASP.NET Core MVC (.NET 8)
*   **Dil:** C#
*   **Veritabanı:** SQL Server (LocalDB / Express)
*   **ORM:** Entity Framework Core 8
*   **API:** TMDb (The Movie Database) API
*   **Kimlik Doğrulama:** ASP.NET Core Identity
*   **Frontend:** Razor Views, HTML5, CSS3, JavaScript
*   **UI Framework:** Bootstrap 5
*   **Önbellekleme:** ASP.NET Core In-Memory Cache (Tür listesi için)
*   **Mimari:** Model-View-Controller (MVC), Servis Katmanı, ViewModels

## ⚙️ Kurulum ve Çalıştırma

1.  **Projeyi Klonlayın:**
    ```bash
    git clone (https://github.com/ulaskrsln/Movie-App)
    cd MovieBlogApp
    ```
2.  **.NET 8 SDK:** Sisteminizde .NET 8 SDK'nın kurulu olduğundan emin olun.
3.  **API Anahtarı:**
    *   TMDb API'sini kullanabilmek için bir API anahtarına ihtiyacınız vardır. [TMDb web sitesinden](https://www.themoviedb.org/settings/api) ücretsiz bir anahtar alabilirsiniz.
    *   Aldığınız API anahtarını projenin kök dizinindeki `appsettings.Development.json` dosyasına (veya User Secrets kullanarak) ekleyin:
      ```json
      {
        "ConnectionStrings": {
          // ... bağlantı dizginiz ...
        },
        "TMDbApiKey": "SIZIN_TMDB_API_ANAHTARINIZ", // Bu satırı ekleyin
        "Logging": {
          // ...
        }
        // ...
      }
      ```
    *   **Not:** `appsettings.json` dosyasına doğrudan API anahtarını eklemeyin ve bu dosyayı public repolara göndermeyin. Geliştirme için `appsettings.Development.json` veya User Secrets kullanın.
4.  **Veritabanı Kurulumu:**
    *   `appsettings.json` dosyasındaki `ConnectionStrings` bölümünde yer alan `DefaultConnection` bağlantı dizesinin, sisteminizdeki SQL Server (LocalDB veya Express) örneğine uygun olduğundan emin olun.
    *   Veritabanını ve tabloları oluşturmak için Package Manager Console (Visual Studio'da) veya Terminal üzerinden aşağıdaki EF Core komutunu çalıştırın:
      *   **PMC:** `Update-Database -Context MovieAppContext`
      *   **.NET CLI:** `dotnet ef database update --context MovieAppContext`
      *(Bu komutlar için EF Core Tools'un kurulu olması gerekir.)*
5.  **Uygulamayı Çalıştırma:**
    *   **Visual Studio:** Projeyi açın ve F5 tuşuna basın (veya yeşil 'Oynat' butonu).
    *   **.NET CLI:** Proje klasöründeyken terminalde `dotnet run` komutunu çalıştırın.
    *   Uygulama genellikle `https://localhost:[PORT_NUMARASI]` adresinde açılacaktır.

## 🔮 Gelecek Planları

*   Admin paneli ile yorum/kullanıcı yönetimi.
*   AJAX ile daha akıcı favori ekleme/çıkarma ve yorum gönderme.
*   Film detay sayfasına "Benzer Filmler" bölümü ekleme.
*   Daha gelişmiş filtreleme seçenekleri (yıl, puan vb.).
*   Kullanıcı profillerine daha fazla detay ekleme (profil resmi vb.).

## irtibat

*   **Geliştirici:** Ulaş Karaaslan
*   **GitHub:** [[GitHub Profil]](https://github.com/ulaskrsln)
*   **E-posta:** ulas.karaaslan@icloud.com

