using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using MovieApp.Models.Database;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MovieApp.Models.Identity;

namespace MovieApp.Data
{
    public class MovieAppContext :  IdentityDbContext<ApplicationUser>
    {
        public MovieAppContext(DbContextOptions<MovieAppContext> options) : base(options)
        {
        }

        public DbSet<MovieReview> MovieReviews { get; set; } = null!;
        public DbSet<UserFavoriteMovie> UserFavoriteMovies { get; set; } = null!;   
        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
            builder.Entity<MovieReview>(entity => 
            {
                entity.HasIndex(r => r.MovieId, "IX_MovieReviews_MovieId"); 
            });

            builder.Entity<UserFavoriteMovie>(entity => {
                entity.HasKey(ufm => new { ufm.ApplicationUserId, ufm.MovieId });

                entity.HasOne(ufm => ufm.ApplicationUser)
                      .WithMany() 
                      .HasForeignKey(ufm => ufm.ApplicationUserId)
                      .IsRequired(); 

                entity.HasIndex(ufm => ufm.MovieId, "IX_UserFavoriteMovies_MovieId");
            });
        }
    }
}
