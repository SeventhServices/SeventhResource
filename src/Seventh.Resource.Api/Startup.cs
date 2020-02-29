using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seventh.Core.Services;
using Seventh.Resource.Services;
using System;
using Seventh.Resource.Common.Options;
using Mapster;
using Seventh.Resource.Common.Entities;
using Seventh.Core.Dto.Response.Resource;
using System.Security.Policy;
using Seventh.Core.Extend;
using Seventh.Core.Utilities;

namespace Seventh.Resource.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSeventhResourceServices(option =>
            {
                option.PathOption = new PathOption(_environment.WebRootPath);
            });
        }

        public void Configure(IApplicationBuilder app, IServiceProvider services)
        {
            InitialApplication(services);

            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void InitialApplication(IServiceProvider services)
        {
            var statusService = services.GetService<SevenStatusService>();
            var location = services.GetService<ResourceLocation>();

            var info = statusService.TryGetVersionInfoAsync().Result;
            var downloadUrl = info.AssetVersion.DownloadUrl;

            if (downloadUrl != null)
            {
                location.DownloadUrl = downloadUrl;
            }

            TypeAdapterConfig<AssetFileInfo, DownloadFileDto>
                .NewConfig()
                .Map(des => des.MirrorUrl,
                    src => UrlUtil.MakeFileUrl(
                        MapContext.Current.Parameters["baseUrl"].ToString()
                        ,src.MirrorSavePath))
                .Map(des => des.SortedUrl,
                    src => UrlUtil.MakeFileUrl(
                        MapContext.Current.Parameters["baseUrl"].ToString()
                        ,src.SortedSavePath));
        }
    }
}
