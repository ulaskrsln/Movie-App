using MovieApp.Models;              
using MovieApp.Models.Details;      
using MovieApp.Models.Search;      
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using MovieApp.Models.People;
using Microsoft.Extensions.Caching.Memory;

namespace MovieApp.Services
{
    public class TMDbService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private const string BaseUrl = "https://api.themoviedb.org/3/";
        private readonly IMemoryCache _cache;
        private readonly ILogger<TMDbService> _logger;

        public TMDbService(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache, ILogger<TMDbService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["TMDbApiKey"];
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("HATA: TMDb API Anahtarı appsettings.json dosyasında bulunamadı veya boş.");
            }

            _cache = cache;
            _logger = logger;
        }
        public async Task<PopularMoviesResponse?> GetPopularMoviesAsync(int page = 1)
        {
            if (string.IsNullOrEmpty(_apiKey)) return null;

            var requestUri = $"{BaseUrl}movie/popular?api_key={_apiKey}&language=tr-TR&page={page}";

            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Popüler film API isteği başarısız: {response.StatusCode}");
                    return null;
                }
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<PopularMoviesResponse>(content, options);
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Popüler film alınırken hata: {ex.Message}");
                return null;
            }
        }

        public async Task<PopularMoviesResponse?> SearchMoviesAsync(string query, int page = 1)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrWhiteSpace(query)) return null;

            var encodedQuery = Uri.EscapeDataString(query);
            var requestUri = $"{BaseUrl}search/movie?api_key={_apiKey}&query={encodedQuery}&language=tr-TR&page={page}&include_adult=false";

            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Film arama API isteği başarısız: {response.StatusCode}");
                    return null;
                }
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<PopularMoviesResponse>(content, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Film aranırken hata: {ex.Message}");
                return null;
            }
        }

        public async Task<MovieDetail?> GetMovieDetailsAsync(int movieId)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("GetMovieDetailsAsync: API Anahtarı eksik.");
                return null;
            }
            if (movieId <= 0)
            {
                Console.WriteLine("GetMovieDetailsAsync: Geçersiz Film ID.");
                return null; 
            }


            // append_to_response=credits parametresi ile oyuncu ve ekip bilgisini de tek seferde istiyoruz
            var requestUri = $"{BaseUrl}movie/{movieId}?api_key={_apiKey}&language=tr-TR&append_to_response=credits";
            string? content = null; 

            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                content = await response.Content.ReadAsStringAsync(); 

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Film bulunamadı (ID: {movieId}). Yanıt: {content}");
                    return null; 
                }

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Film detay API isteği başarısız: {response.StatusCode} (ID: {movieId}). Yanıt: {content}");
                    return null;
                }


                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var movieDetail = JsonSerializer.Deserialize<MovieDetail>(content, options);

                if (movieDetail == null)
                {
                    Console.WriteLine($"MovieDetail null döndü. (ID: {movieId}). JSON: {content}");
                }

                return movieDetail;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP İstek Hatası (Detay - ID: {movieId}): {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Dönüştürme Hatası (Detay - ID: {movieId}): {ex.Message}");
                Console.WriteLine($"Gelen Hatalı JSON: {content ?? "İçerik okunamadı."}");
                return null;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Film detayı alınırken genel hata (ID: {movieId}): {ex.Message}");
                return null;
            }
        }

        public async Task<PersonSearchResponse?> SearchPeopleAsync(string query, int page = 1)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("SearchPeopleAsync: API Anahtarı eksik.");
                return null;
            }
            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("SearchPeopleAsync: Arama sorgusu boş.");
                return null;
            }
            if (page < 1) page = 1; 


            var encodedQuery = Uri.EscapeDataString(query);
            var requestUri = $"{BaseUrl}search/person?api_key={_apiKey}&query={encodedQuery}&language=tr-TR&page={page}&include_adult=false";

            string? content = null;

            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                content = await response.Content.ReadAsStringAsync(); 

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                    {
                        Console.WriteLine($"Kişi bulunamadı veya geçersiz sorgu (Query: {query}, Page: {page}). Status: {response.StatusCode}. Yanıt: {content}");
                        return null;
                    }
                    else
                    {
                        Console.WriteLine($"Kişi arama API isteği başarısız: {response.StatusCode}. Query: {query}, Page: {page}. Yanıt: {content}");
                        return null;
                    }
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var searchResult = JsonSerializer.Deserialize<PersonSearchResponse>(content, options);

                if (searchResult == null)
                {
                    Console.WriteLine($"JSON Deserialization sonrası PersonSearchResponse null döndü. Query: {query}, Page: {page}. JSON: {content}");
                    return null; 
                }

                return searchResult;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP İstek Hatası (Kişi Arama - Query: {query}, Page: {page}): {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Dönüştürme Hatası (Kişi Arama - Query: {query}, Page: {page}): {ex.Message}");
                Console.WriteLine($"Gelen Hatalı JSON: {content ?? "İçerik okunamadı."}");
                return null;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Kişi aranırken genel hata (Query: {query}, Page: {page}): {ex.Message}");
                return null;
            }
        }

        public async Task<PersonDetail?> GetPersonDetailsAsync(int personId) 
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("GetPersonDetailsAsync: API Anahtarı eksik.");
                return null;
            }
            if (personId <= 0) 
            {
                Console.WriteLine("GetPersonDetailsAsync: Geçersiz Kişi ID sağlandı."); 
                return null;
            }

            var requestUri = $"{BaseUrl}person/{personId}?api_key={_apiKey}&language=tr-TR&append_to_response=movie_credits"; 
            string? responseContent = null;

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
                responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Kişi bulunamadı (ID: {personId}). TMDb Yanıtı: {responseContent}"); 
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Kişi detay API isteği başarısız. Status: {response.StatusCode} (ID: {personId}). Yanıt: {responseContent}");
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var personDetail = JsonSerializer.Deserialize<PersonDetail>(responseContent, options);

                if (personDetail == null)
                {
                    Console.WriteLine($"JSON Deserialization sonrası PersonDetail null döndü (ID: {personId}). JSON: {responseContent}"); 
                    return null; 
                }

                if (personDetail.MovieCredits == null)
                {
                    personDetail.MovieCredits = new MovieCreditsResponse(); 
                    Console.WriteLine($"Uyarı: Kişi ID {personId} için 'movie_credits' alanı JSON'da bulunamadı veya null geldi.");
                }
                else
                {
                    if (personDetail.MovieCredits.Cast == null)
                    {
                        personDetail.MovieCredits.Cast = new List<MovieCastCredit>();
                        Console.WriteLine($"Uyarı: Kişi ID {personId} için 'movie_credits.cast' alanı JSON'da bulunamadı veya null geldi.");
                    }
                    if (personDetail.MovieCredits.Crew == null)
                    {
                        personDetail.MovieCredits.Crew = new List<MovieCrewCredit>();
                        Console.WriteLine($"Uyarı: Kişi ID {personId} için 'movie_credits.crew' alanı JSON'da bulunamadı veya null geldi.");
                    }
                }
                return personDetail;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP İstek Hatası (Kişi Detay - ID: {personId}): {ex.Message}"); 
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Dönüştürme Hatası (Kişi Detay - ID: {personId}): {ex.Message}"); 
                Console.WriteLine($"Gelen Hatalı JSON (Kişi Detay): {responseContent ?? "İçerik okunamadı."}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kişi detayı alınırken genel hata (ID: {personId}): {ex.Message}");
                return null;
            }
        }

        public async Task<PopularMoviesResponse?> GetTopRatedMoviesAsync(int page = 1)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("GetTopRatedMoviesAsync: API Anahtarı eksik.");
                return null;
            }
            if (page < 1)
            {
                Console.WriteLine($"GetTopRatedMoviesAsync: Geçersiz sayfa numarası ({page}) istendi, 1 olarak ayarlandı.");
                page = 1;
            }
            const int maxPageLimit = 500;
            if (page > maxPageLimit)
            {
                Console.WriteLine($"GetTopRatedMoviesAsync: İstenen sayfa ({page}) limit ({maxPageLimit}) üzerinde, son limite ayarlandı.");
                page = maxPageLimit;
            }


            var requestUri = $"{BaseUrl}movie/top_rated?api_key={_apiKey}&language=tr-TR&page={page}";

            string? responseContent = null; 

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
                responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Top Rated API isteği başarısız. Status: {response.StatusCode} (Page: {page}). Yanıt: {responseContent}");
                    return null; 
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true 
                };
                var topRatedMovies = JsonSerializer.Deserialize<PopularMoviesResponse>(responseContent, options);

                if (topRatedMovies == null)
                {
                    Console.WriteLine($"JSON Deserialization sonrası PopularMoviesResponse (TopRated için) null döndü (Page: {page}). JSON: {responseContent}");
                }

                return topRatedMovies;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP İstek Hatası (TopRated - Page: {page}): {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Dönüştürme Hatası (TopRated - Page: {page}): {ex.Message}");
                Console.WriteLine($"Gelen Hatalı JSON (TopRated): {responseContent ?? "İçerik okunamadı."}");
                return null;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"En yüksek puanlı filmler alınırken genel hata (Page: {page}): {ex.Message}");
                return null;
            }
        }

        public async Task<PersonSearchResponse?> GetPopularPeopleAsync(int page = 1)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("GetPopularPeopleAsync: API Anahtarı eksik.");
                return null;
            }
            if (page < 1) page = 1;
            const int maxPageLimit = 500;
            if (page > maxPageLimit) page = maxPageLimit;

            var requestUri = $"{BaseUrl}person/popular?api_key={_apiKey}&language=tr-TR&page={page}";

            string? responseContent = null;

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
                responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Popüler Kişiler API isteği başarısız. Status: {response.StatusCode} (Page: {page}). Yanıt: {responseContent}");
                    return null;
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var popularPeople = JsonSerializer.Deserialize<PersonSearchResponse>(responseContent, options);

                if (popularPeople == null)
                {
                    Console.WriteLine($"JSON Deserialization sonrası PersonSearchResponse (PopularPeople için) null döndü (Page: {page}). JSON: {responseContent}");
                }

                return popularPeople;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Popüler kişiler alınırken hata (Page: {page}): {ex.Message}");
                if (ex is JsonException) { Console.WriteLine($"Hatalı JSON (PopularPeople): {responseContent ?? "İçerik okunamadı."}"); }
                return null;
            }
        }

        public async Task<List<Genre>> GetGenresAsync()
        {
            const string cacheKey = "MovieListGenres";
            _logger.LogInformation("Film türleri isteniyor. Cache kontrol ediliyor (Key: {CacheKey}).", cacheKey);

            if (!_cache.TryGetValue(cacheKey, out List<Genre>? genres))
            {
                _logger.LogInformation("Cache'de tür listesi bulunamadı. API'den çekilecek.");
                if (string.IsNullOrEmpty(_apiKey))
                {
                    _logger.LogError("GetGenresAsync: API Anahtarı eksik.");
                    return new List<Genre>(); 
                }

                var requestUri = $"{BaseUrl}genre/movie/list?api_key={_apiKey}&language=tr-TR";
                string? responseContent = null;

                try
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
                    responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Tür listesi API isteği başarısız. Status: {StatusCode}. Yanıt: {Response}", response.StatusCode, responseContent);
                        return new List<Genre>(); 
                    }

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var genreResponse = JsonSerializer.Deserialize<GenreListResponse>(responseContent, options);

                    genres = genreResponse?.Genres ?? new List<Genre>(); 

                    if (genres.Any()) 
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromHours(12)) // 12 saat erişilmezse sil
                            .SetAbsoluteExpiration(TimeSpan.FromHours(24)); // En fazla 24 saat tut

                        _logger.LogInformation("Tür listesi API'den çekildi ve cache'e ekleniyor (Key: {CacheKey}, Süre: 24 saat).", cacheKey);
                        _cache.Set(cacheKey, genres, cacheEntryOptions);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Tür listesi alınırken hata oluştu. Yanıt: {Response}", responseContent ?? "N/A");
                    genres = new List<Genre>(); 
                }
            }
            else 
            {
                _logger.LogInformation("Tür listesi cache'den başarıyla alındı (Key: {CacheKey}).", cacheKey);
            }

            return genres ?? new List<Genre>(); 
        }

        public async Task<PopularMoviesResponse?> DiscoverMoviesByGenreAsync(int genreId, int page = 1)
        {
            if (string.IsNullOrEmpty(_apiKey)) { _logger.LogError("DiscoverMoviesByGenreAsync: API Anahtarı eksik."); return null; }
            if (genreId <= 0) { _logger.LogWarning("DiscoverMoviesByGenreAsync: Geçersiz Tür ID istendi: {GenreId}", genreId); return null; }
            if (page < 1) page = 1;
            const int maxPageLimit = 500;
            if (page > maxPageLimit) page = maxPageLimit;

            var requestUri = $"{BaseUrl}discover/movie?api_key={_apiKey}&language=tr-TR&sort_by=popularity.desc&include_adult=false&include_video=false&page={page}&with_genres={genreId}";

            string? responseContent = null;
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
                responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Türe göre film keşfetme API isteği başarısız. Status: {StatusCode}, TürId: {GenreId}, Sayfa: {Page}. Yanıt: {Response}", response.StatusCode, genreId, page, responseContent);
                    return null;
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var discoveredMovies = JsonSerializer.Deserialize<PopularMoviesResponse>(responseContent, options);

                if (discoveredMovies == null)
                {
                    _logger.LogWarning("JSON Deserialization sonrası PopularMoviesResponse (Discover için) null döndü. TürId: {GenreId}, Sayfa: {Page}. JSON: {Response}", genreId, page, responseContent);
                }

                return discoveredMovies;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Türe göre film keşfederken hata oluştu. TürId: {GenreId}, Sayfa: {Page}. Yanıt: {Response}", genreId, page, responseContent ?? "N/A");
                return null;
            }
        }
    }
}