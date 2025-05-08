using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Authorization; 
using Microsoft.EntityFrameworkCore; 
using MovieApp.Data;
using MovieApp.Models.Database; 
using MovieApp.Models.Identity; 
using MovieApp.Services;
using System.Security.Claims; 
using System.Linq;
using System.Threading.Tasks;
using System;
using MovieApp.Models.Details; 
using System.Collections.Generic; 

namespace MovieApp.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly MovieAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TMDbService _tmdbService;
        private readonly ILogger<FavoritesController> _logger;

        public FavoritesController(
            MovieAppContext context,
            UserManager<ApplicationUser> userManager,
            TMDbService tmdbService,
            ILogger<FavoritesController> logger)
        {
            _context = context;
            _userManager = userManager;
            _tmdbService = tmdbService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                _logger.LogWarning("Favoriler sayfası için kullanıcı ID'si alınamadı.");
                return Challenge();
            }

            _logger.LogInformation("Kullanıcı {UserId} favori listesini görüntülüyor.", userId);

            List<int> favoriteMovieIds;
            try
            {
                favoriteMovieIds = await _context.UserFavoriteMovies
                    .Where(f => f.ApplicationUserId == userId)
                    .OrderByDescending(f => f.AddedDate) 
                    .Select(f => f.MovieId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı {UserId} için favori ID'leri çekilirken veritabanı hatası.", userId);
                TempData["ErrorMessage"] = "Favoriler yüklenirken bir hata oluştu.";
                return View(new List<MovieDetail>()); 
            }


            var favoriteMoviesDetails = new List<MovieDetail>();

            if (favoriteMovieIds.Any())
            {
                _logger.LogInformation("Kullanıcı {UserId} için {Count} adet favori film ID'si bulundu. Detaylar çekiliyor...", userId, favoriteMovieIds.Count);

                foreach (var movieId in favoriteMovieIds)
                {
                    var movieDetail = await _tmdbService.GetMovieDetailsAsync(movieId);
                    if (movieDetail != null)
                    {
                        favoriteMoviesDetails.Add(movieDetail);
                    }
                    else
                    {
                        _logger.LogWarning("Favori listesi için film detayı çekilemedi veya film bulunamadı. MovieId: {MovieId}, UserId: {UserId}", movieId, userId);
                    }
                }
                _logger.LogInformation("Kullanıcı {UserId} için {DetailCount} adet favori film detayı başarıyla çekildi.", userId, favoriteMoviesDetails.Count);
            }
            else
            {
                _logger.LogInformation("Kullanıcı {UserId} için favori film bulunamadı.", userId);
            }

            return View(favoriteMoviesDetails);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Add(int movieId) 
        {
            if (movieId <= 0)
            {
                TempData["ErrorMessage"] = "Geçersiz Film ID.";
                return RedirectToAction("Index", "Home");
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge(); 

            _logger.LogInformation("Kullanıcı {UserId}, Film ID {MovieId}'yi favorilere eklemeye çalışıyor.", userId, movieId);

            bool alreadyExists = await _context.UserFavoriteMovies
                                     .AnyAsync(f => f.ApplicationUserId == userId && f.MovieId == movieId);

            if (alreadyExists)
            {
                _logger.LogWarning("Film ID {MovieId} zaten Kullanıcı {UserId}'nin favorilerinde.", movieId, userId);
                TempData["WarningMessage"] = "Bu film zaten favorilerinizde.";
            }
            else
            {
                var favorite = new UserFavoriteMovie
                {
                    ApplicationUserId = userId,
                    MovieId = movieId,
                    AddedDate = DateTime.UtcNow 
                };
                _context.UserFavoriteMovies.Add(favorite);
                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Film ID {MovieId}, Kullanıcı {UserId} tarafından favorilere eklendi.", movieId, userId);
                    TempData["SuccessMessage"] = "Film favorilerinize eklendi!";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Favori eklenirken veritabanı hatası oluştu. UserId: {UserId}, MovieId: {MovieId}", userId, movieId);
                    TempData["ErrorMessage"] = "Film favorilere eklenirken bir veritabanı hatası oluştu.";
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, "Favori eklenirken genel hata oluştu. UserId: {UserId}, MovieId: {MovieId}", userId, movieId);
                    TempData["ErrorMessage"] = "Film favorilere eklenirken beklenmedik bir hata oluştu.";
                }
            }

            return RedirectToAction("Details", "Movies", new { id = movieId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int movieId)
        {
            if (movieId <= 0)
            {
                TempData["ErrorMessage"] = "Geçersiz Film ID.";
                return RedirectToAction("Index"); 
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            _logger.LogInformation("Kullanıcı {UserId}, Film ID {MovieId}'yi favorilerden çıkarmaya çalışıyor.", userId, movieId);

            var favoriteToRemove = await _context.UserFavoriteMovies
                                        .FirstOrDefaultAsync(f => f.ApplicationUserId == userId && f.MovieId == movieId);

            if (favoriteToRemove != null) 
            {
                _context.UserFavoriteMovies.Remove(favoriteToRemove);
                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Film ID {MovieId}, Kullanıcı {UserId} tarafından favorilerden çıkarıldı.", movieId, userId);
                    TempData["SuccessMessage"] = "Film favorilerinizden çıkarıldı.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Favori çıkarılırken veritabanı hatası oluştu. UserId: {UserId}, MovieId: {MovieId}", userId, movieId);
                    TempData["ErrorMessage"] = "Film favorilerden çıkarılırken bir veritabanı hatası oluştu.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Favori çıkarılırken genel hata oluştu. UserId: {UserId}, MovieId: {MovieId}", userId, movieId);
                    TempData["ErrorMessage"] = "Film favorilerden çıkarılırken beklenmedik bir hata oluştu.";
                }
            }
            else // Kayıt bulunamadıysa
            {
                _logger.LogWarning("Kullanıcı {UserId} favorilerinde olmayan Film ID {MovieId}'yi silmeye çalıştı.", userId, movieId);
                TempData["WarningMessage"] = "Bu film zaten favorilerinizde değildi.";
            }

            string? referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) && (referer.Contains($"/Movies/Details/{movieId}") || referer.Contains("/Favorites")))
            {
                return Redirect(referer);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}