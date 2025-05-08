using Microsoft.AspNetCore.Mvc;
using MovieApp.Models;
using MovieApp.Models.Details;
using MovieApp.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MovieApp.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TMDbService _tmdbService;
    public HomeController(ILogger<HomeController> logger, TMDbService tmdbService)
    {
        _logger = logger;
        _tmdbService = tmdbService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        if (page < 1)
        {
            page = 1;
        }

        var popularMoviesResponse = await _tmdbService.GetPopularMoviesAsync(page);
        var viewModel = new HomeIndexViewModel();

        if (popularMoviesResponse != null && popularMoviesResponse.Results.Any())
        {
            const int maxAllowedPages = 500;

            viewModel.Movies = popularMoviesResponse.Results;
            viewModel.CurrentPage = popularMoviesResponse.Page;
            viewModel.TotalPages = Math.Min(popularMoviesResponse.TotalPages, maxAllowedPages);
            viewModel.TotalResults = popularMoviesResponse.TotalResults;
        }
        else
        {
            ViewBag.ErrorMessage = "Popüler filmler yüklenemedi veya bulunamadý.";
        }

            return View(viewModel);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
    public async Task<IActionResult> Bests(int page = 1)
    {
        const int targetMovieLimit = 250;
        const int itemsPerPage = 20;      
        int maxPageForLimit = (int)Math.Ceiling((double)targetMovieLimit / itemsPerPage);

        if (page < 1) page = 1;
        if (page > maxPageForLimit)
        {
            _logger.LogWarning("Ýstenen sayfa ({RequestedPage}) Top {Limit} limiti ({MaxPage}) üzerinde. Sayfa {MaxPage}'e ayarlandý.", page, targetMovieLimit, maxPageForLimit, maxPageForLimit);
            page = maxPageForLimit;
        }

        _logger.LogInformation("Top Rated filmler isteniyor (Max {Limit}). Sayfa: {Page}", targetMovieLimit, page);

        var topRatedResponse = await _tmdbService.GetTopRatedMoviesAsync(page);

        var viewModel = new TopRatedMoviesViewModel();

        if (topRatedResponse != null && topRatedResponse.Results.Any())
        {
            int startRank = (topRatedResponse.Page - 1) * itemsPerPage + 1;
            int currentRank = startRank; 
            viewModel.MoviesWithRank = new List<TopRatedMovieItem>();
            foreach (var movie in topRatedResponse.Results)
            {
                if (currentRank <= targetMovieLimit)
                {
                    viewModel.MoviesWithRank.Add(new TopRatedMovieItem
                    {
                        Rank = currentRank,
                        Movie = movie
                    });
                    currentRank++;
                }
                else
                {
                    break;
                }
            }

            viewModel.CurrentPage = topRatedResponse.Page;

            int apiTotalPagesLimited = topRatedResponse.TotalPages > 0 ? Math.Min(topRatedResponse.TotalPages, 500) : maxPageForLimit;
            viewModel.TotalPages = Math.Min(apiTotalPagesLimited, maxPageForLimit);

            viewModel.TotalResults = topRatedResponse.TotalResults > 0 ? Math.Min(topRatedResponse.TotalResults, targetMovieLimit) : viewModel.MoviesWithRank.Count;

            _logger.LogInformation("{Count} adet Top Rated film ViewModel'a eklendi. Sayfa: {Page}, Toplam Sayfa (Limitli): {TotalPages}", viewModel.MoviesWithRank.Count, viewModel.CurrentPage, viewModel.TotalPages);

        }
        else 
        {
            _logger.LogWarning("Top Rated filmler getirilemedi veya sonuç boþ. Sayfa: {Page}", page);
            ViewBag.ErrorMessage = "En yüksek puanlý filmler yüklenirken bir sorun oluþtu veya liste boþ.";
            viewModel.CurrentPage = page;
            viewModel.TotalPages = 0;
            viewModel.TotalResults = 0;
        }

        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
 
}
