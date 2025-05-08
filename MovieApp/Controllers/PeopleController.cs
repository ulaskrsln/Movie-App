using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieApp.Models;        
using MovieApp.Models.People;  
using MovieApp.Models.Search;   
using MovieApp.Services;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MovieApp.Controllers
{
    public class PeopleController : Controller
    {
        private readonly TMDbService _tmdbService;
        private readonly ILogger<PeopleController> _logger;

        public PeopleController(TMDbService tmdbService, ILogger<PeopleController> logger)
        {
            _tmdbService = tmdbService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1) page = 1;
            const int maxPageLimit = 500;
            if (page > maxPageLimit) page = maxPageLimit;

            _logger.LogInformation("Popüler Aktörler listesi isteniyor. Sayfa: {Page}", page);

            var popularPeopleResponse = await _tmdbService.GetPopularPeopleAsync(page);
            var viewModel = new PopularActorsViewModel(); 

            if (popularPeopleResponse != null && popularPeopleResponse.Results.Any())
            {
                var actorsOnly = popularPeopleResponse.Results
                                    .Where(p => p.KnownForDepartment == "Acting")
                                    .ToList();

                if (actorsOnly.Any())
                {
                    viewModel.PopularActors = actorsOnly;
                    viewModel.CurrentPage = popularPeopleResponse.Page;
                    viewModel.TotalPages = Math.Min(popularPeopleResponse.TotalPages, maxPageLimit);
                    viewModel.TotalResults = popularPeopleResponse.TotalResults; 
                    _logger.LogInformation("{Count} adet popüler aktör filtrelendi. Sayfa: {Page}", actorsOnly.Count, viewModel.CurrentPage);
                }
                else
                {
                    _logger.LogWarning("Popüler kişiler listesinde bu sayfada aktör bulunamadı. Sayfa: {Page}", page);
                    ViewBag.InfoMessage = "Bu sayfada gösterilecek popüler aktör bulunamadı."; 
                }
            }
            else
            {
                _logger.LogWarning("Popüler kişiler getirilemedi veya sonuç boş. Sayfa: {Page}", page);
                ViewBag.ErrorMessage = "Popüler aktörler yüklenirken bir sorun oluştu.";
                viewModel.CurrentPage = page; viewModel.TotalPages = 0; viewModel.TotalResults = 0;
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest("Geçersiz Kişi ID.");

            _logger.LogInformation("Kişi detayı isteniyor (PeopleController). ID: {PersonId}", id);

            var personDetail = await _tmdbService.GetPersonDetailsAsync(id);

            if (personDetail == null)
            {
                _logger.LogWarning("Kişi detayı bulunamadı (API). ID: {PersonId}", id);
                return NotFound($"'{id}' ID'li kişi bulunamadı.");
            }

            return View(personDetail);
        }
    }
}