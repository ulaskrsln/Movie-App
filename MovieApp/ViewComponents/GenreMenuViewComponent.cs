using Microsoft.AspNetCore.Mvc; 
using Microsoft.Extensions.Caching.Memory; 
using MovieApp.Models.Details; 
using MovieApp.Services; 
using System.Collections.Generic; 
using System.Threading.Tasks; 
using System.Linq; 

namespace MovieApp.ViewComponents 
{
    public class GenreMenuViewComponent : ViewComponent
    {
        private readonly TMDbService _tmdbService;
        private readonly ILogger<GenreMenuViewComponent> _logger; 

        public GenreMenuViewComponent(TMDbService tmdbService, ILogger<GenreMenuViewComponent> logger)
        {
            _tmdbService = tmdbService;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            _logger.LogInformation("GenreMenuViewComponent çağrıldı.");
            List<Genre> genres = new List<Genre>(); 

            try
            {
                genres = await _tmdbService.GetGenresAsync();

                if (genres != null && genres.Any())
                {
                    genres = genres.OrderBy(g => g.Name).ToList();
                    _logger.LogInformation("{GenreCount} adet tür bulundu ve sıralandı.", genres.Count);
                }
                else
                {
                    _logger.LogWarning("GetGenresAsync boş veya null liste döndürdü.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GenreMenuViewComponent içinde türler alınırken hata oluştu.");
            }

            return View(genres);
        }
    }
}