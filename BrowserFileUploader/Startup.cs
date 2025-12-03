using BrowserFileUploader.Data;
using BrowserFileUploader.Interfaces;
using BrowserFileUploader.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BrowserFileUploader
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_config.GetConnectionString(Consts.EnvironmentalVariables.DEFAULT_CONNECTION_KEY)));

            services.AddScoped<IImageStorageService>(sp =>
            {
                var env = sp.GetRequiredService<IWebHostEnvironment>();
                var mode = _config.GetValue<string>("Storage:Mode") ?? Consts.Enums.enumStorageMode.FILE_SYSTEM.ToString();

                //Azure storage mode
                if (string.Equals(mode, Consts.Enums.enumStorageMode.AZURE.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    var connectionString = _config.GetConnectionString(Consts.EnvironmentalVariables.AZURE_BLOB_STORAGE_KEY)
                        ?? throw new InvalidOperationException("BlobStorage connection string is missing.");

                    var containerName = _config["Azure:ContainerName"]
                        ?? throw new InvalidOperationException("Container name is missing.");

                    return new AzureBlobImageStorageService(connectionString, containerName);
                }
                //File storage mode
                else
                {
                    var relativePath = _config["Storage:LocalPath"] ?? Consts.Consts.DEFAULT_FILE_STORAGE_UPLOAD_PATH;

                    var webRoot = env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
                    var fullPath = Path.Combine(webRoot, relativePath);

                    Directory.CreateDirectory(fullPath);

                    return new FileSystemImageStorageService(fullPath);
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=ImageUpload}/{action=Upload}/{id?}");
            });
        }
    }
}
