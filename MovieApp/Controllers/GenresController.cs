using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieApp.Models; 
using MovieApp.Models.Details; 
using MovieApp.Services;
using System.Threading.Tasks;
using System;
using System.Linq; 

namespace MovieApp.Controllers
{
    public class GenresController : Controller
    {
        private readonly TMDbService _tmdbService;
        private readonly ILogger<GenresController> _logger;

        public GenresController(TMDbService tmdbService, ILogger<GenresController> logger)
        {
            _tmdbService = tmdbService;
            _logger = logger;
        }

        public async Task<IActionResult> MoviesByGenre(int id, string name, int page = 1)
        {
            if (id <= 0) return BadRequest("Geçersiz Tür ID.");
            if (page < 1) page = 1;
            const int maxPageLimit = 500; 
            if (page > maxPageLimit) page = maxPageLimit;

            _logger.LogInformation("'{GenreName}' (ID: {GenreId}) türüne ait filmler isteniyor. Sayfa: {Page}", name ?? "Bilinmeyen Tür", id, page);

            var moviesResponse = await _tmdbService.DiscoverMoviesByGenreAsync(id, page);

            var viewModel = new MoviesByGenreViewModel
            {
                GenreInfo = new Genre { Id = id, Name = name ?? "Bilinmeyen Tür" }, 
                CurrentPage = page 
            };

            if (moviesResponse != null && moviesResponse.Results.Any())
            {
                viewModel.Movies = moviesResponse.Results;
                viewModel.CurrentPage = moviesResponse.Page; 
                viewModel.TotalPages = Math.Min(moviesResponse.TotalPages, maxPageLimit);
                viewModel.TotalResults = moviesResponse.TotalResults;
                _logger.LogInformation("'{GenreName}' türü için {Count} film bulundu. Sayfa: {Page}", viewModel.GenreInfo.Name, viewModel.Movies.Count, viewModel.CurrentPage);
            }
            else
            {
                _logger.LogWarning("'{GenreName}' türü için film bulunamadı veya API hatası. TürId: {GenreId}, Sayfa: {Page}", viewModel.GenreInfo.Name, id, page);
                ViewBag.ErrorMessage = $"'{viewModel.GenreInfo.Name}' türünde film bulunamadı.";
            }

            return View(viewModel);
        }

    }
}