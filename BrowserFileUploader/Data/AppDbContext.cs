using BrowserFileUploader.Models;
using Microsoft.EntityFrameworkCore;

namespace BrowserFileUploader.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        public DbSet<ImageUploadReference> UploadedImages => Set<ImageUploadReference>();
    }
}
