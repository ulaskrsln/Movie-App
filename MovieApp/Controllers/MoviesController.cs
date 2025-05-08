using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieApp.Models;
using MovieApp.Models.Search;
using MovieApp.Models.Details;
using MovieApp.Services;
using MovieApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Identity;
using MovieApp.Models.Identity;
using System.Security.Claims;

namespace MovieApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly TMDbService _tmdbService;
        private readonly ILogger<MoviesController> _logger;
        private readonly MovieAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MoviesController(TMDbService tmdbService, ILogger<MoviesController> logger, MovieAppContext context, UserManager<ApplicationUser> userManager)
        {
            _tmdbService = tmdbService;
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Search(string query, int page = 1)
        {
            if (page < 1) page = 1;

            var viewModel = new MovieSearchViewModel { Query = query };

            if (string.IsNullOrWhiteSpace(query))
            {
                ViewBag.ErrorMessage = "Lütfen aramak için bir film veya kişi adı girin.";
                return View(viewModel);
            }

            _logger.LogInformation("Arama başlatıldı. Sorgu: '{Query}', Film Sayfası: {Page}", query, page);

            var movieSearchTask = _tmdbService.SearchMoviesAsync(query, page);
            var personSearchTask = _tmdbService.SearchPeopleAsync(query, 1);

            try
            {
                await Task.WhenAll(movieSearchTask, personSearchTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arama görevleri beklenirken bir hata oluştu. Sorgu: {Query}", query);
                ViewBag.ErrorMessage = "Arama sırasında beklenmedik bir sunucu hatası oluştu.";
                return View(viewModel);
            }


            var movieResult = movieSearchTask.Result;
            var personResult = personSearchTask.Result;

            if (movieResult != null)
            {
                const int maxAllowedPages = 500;

                viewModel.MovieResults = movieResult.Results ?? new List<MovieResult>();
                viewModel.MovieCurrentPage = movieResult.Page;
                viewModel.MovieTotalPages = Math.Min(movieResult.TotalPages, maxAllowedPages);
                viewModel.MovieTotalResults = movieResult.TotalResults;
                _logger.LogInformation("{Count} film sonucu bulundu. Toplam Sayfa: {TotalPages}", viewModel.MovieResults.Count, viewModel.MovieTotalPages);
            }
            else
            {
                _logger.LogWarning("Film arama servisi null sonuç döndürdü. Sorgu: {Query}, Sayfa: {Page}", query, page);
            }
            if (personResult != null)
            {
                viewModel.PersonResults = personResult.Results ?? new List<PersonResult>();
                viewModel.PersonCurrentPage = personResult.Page;
                viewModel.PersonTotalPages = personResult.TotalPages;
                viewModel.PersonTotalResults = personResult.TotalResults;
                _logger.LogInformation("{Count} kişi sonucu bulundu. Toplam Sayfa: {TotalPages}", viewModel.PersonResults.Count, viewModel.PersonTotalPages);
            }
            else
            {
                _logger.LogWarning("Kişi arama servisi null sonuç döndürdü. Sorgu: {Query}, Sayfa: 1", query);
            }

            if (!viewModel.MovieResults.Any() && !viewModel.PersonResults.Any())
            {
                ViewBag.InfoMessage = $"'{query}' için film veya kişi bulunamadı.";
                _logger.LogInformation("'{Query}' için hiç sonuç bulunamadı.", query);
            }

            return View(viewModel);
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Geçersiz Film ID.");
            }

            _logger.LogInformation("Film detayı isteniyor. ID: {MovieId}", id);
            var movieDetail = await _tmdbService.GetMovieDetailsAsync(id);

            if (movieDetail == null)
            {
                _logger.LogWarning("Film detayı bulunamadı veya getirilemedi. ID: {MovieId}", id);
                return NotFound($"'{id}' ID'li film bulunamadı.");
            }

            var reviews = await _context.MovieReviews
                .Where(r => r.MovieId == id)
                                    .OrderByDescending(r => r.CreatedAt)
                                    .ToListAsync();
            _logger.LogInformation("{ReviewCount} adet yorum bulundu. Film ID: {MovieId}", reviews.Count, id);

            bool isFavorited = false;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                if (!string.IsNullOrEmpty(userId))
                {
                    try
                    {
                        isFavorited = await _context.UserFavoriteMovies
                                            .AnyAsync(f => f.ApplicationUserId == userId && f.MovieId == id);
                        _logger.LogInformation("Film ID {MovieId} için favori durumu kontrol edildi. Kullanıcı: {UserId}, Favori mi: {IsFavorited}", id, userId, isFavorited);
                    }
                    catch (Exception ex) 
                    {
                        _logger.LogError(ex, "Favori durumu kontrol edilirken veritabanı hatası. UserId: {UserId}, MovieId: {MovieId}", userId, id);
                    }
                }
                else { _logger.LogWarning("Favori kontrolü için User ID alınamadı."); }
            }

            var viewModel = new MovieDetailViewModel
            {
                Movie = movieDetail,
                Reviews = reviews,
                NewReview = new AddReviewViewModel { MovieId = id },
                IsFavorited = isFavorited
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(AddReviewViewModel NewReview) 
        {

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Geçerli yorum verisi alındı. Film ID: {MovieId}, Kullanıcı: {Username}", NewReview.MovieId, NewReview.UserName);

                var newReviewEntity = new MovieReview 
                {
                    MovieId = NewReview.MovieId,       
                    Username = NewReview.UserName,   
                    Rating = NewReview.Rating,       
                    CommentText = NewReview.CommentText, 
                    CreatedAt = DateTime.UtcNow
                };

                _context.MovieReviews.Add(newReviewEntity);

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Yorum başarıyla veritabanına kaydedildi. Review ID: {ReviewId}", newReviewEntity.Id);

                    return RedirectToAction("Details", new { id = NewReview.MovieId });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Yorum kaydedilirken veritabanı hatası oluştu. Film ID: {MovieId}", NewReview.MovieId);
                    TempData["ErrorMessage"] = "Yorum kaydedilemedi. Lütfen tekrar deneyin.";
                    return RedirectToAction("Details", new { id = NewReview.MovieId });
                }
            }

            _logger.LogWarning("Geçersiz yorum verisi gönderildi. Hatalar: {ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

            var movieDetailForError = await _tmdbService.GetMovieDetailsAsync(NewReview.MovieId); 
            if (movieDetailForError == null) return RedirectToAction("Index", "Home");
            var reviewsForError = await _context.MovieReviews
                                        .Where(r => r.MovieId == NewReview.MovieId) 
                                        .OrderByDescending(r => r.CreatedAt)
                                        .ToListAsync();

            var viewModelForError = new MovieDetailViewModel
            {
                Movie = movieDetailForError,
                Reviews = reviewsForError,
                NewReview = NewReview 
            };

            return View("Details", viewModelForError);
        }
    }
}