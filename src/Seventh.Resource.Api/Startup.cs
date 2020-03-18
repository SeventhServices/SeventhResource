using System;
using Seventh.Core.Services;
using Seventh.Resource.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seventh.Resource.Common.Options;
using Seventh.Resource.Common.Entities;
using Seventh.Core.Dto.Response.Resource;
using Seventh.Core.Utilities;
using Seventh.Core;
using Mapster;
using System.Net.Http;
using Microsoft.OpenApi.Models;

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
            services.AddSeventhCore();
            services.AddResponseCaching();
            services.AddSeventhResourceServices(option =>
            {
                option.PathOption = new PathOption(_environment.WebRootPath);
            });

            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("master", new OpenApiInfo
                {
                    Title = "Seventh Resource Service",
                    Version = "master",
                    Contact = new OpenApiContact(),
                    Description = "Manage Resource from Seventh Project."
                });
                setup.CustomOperationIds(description => description.RelativePath);
                setup.CustomSchemaIds(type => type.FullName);
            });
        }

        public void Configure(IApplicationBuilder app, IServiceProvider services)
        {
            InitialApplication(services);

            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });

            app.UseResponseCaching();

            app.UseSwagger(option =>
            {
                option.RouteTemplate = "{documentName}.json";
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseReDoc(options =>
            {
                options.SpecUrl = "/master.json";
                options.RoutePrefix = "doc";
                options.PathInMiddlePanel();
                options.NativeScrollbars();
                options.PathInMiddlePanel();
                options.HideLoading();
                options.DisableSearch();
            });
        }

        private void InitialApplication(IServiceProvider services)
        {
            var statusService = services.GetService<SevenStatusService>();
            var location = services.GetService<ResourceLocation>();

            try
            {
                var info = statusService.TryGetVersionInfoAsync().Result;
                var downloadUrl = info.AssetVersion.DownloadUrl;
                if (downloadUrl != null)
                {
                    location.DownloadUrl = downloadUrl;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            TypeAdapterConfig<AssetFileInfo, AssetFileInfoDto>
                .NewConfig()
                .Map(des => des.Url,
                    src => UrlUtil.MakeFileUrl(
                        MapContext.Current.Parameters["baseUrl"].ToString()
                        , src.Path));

            //var sortedPath = location.PathOption.AssetPath.SortedAssetPath;
            //var sortService = services.GetService<SortService>();

            //var fileInfos = new DirectoryInfo(sortedPath).GetFiles();
            //foreach (var fileInfo in fileInfos)
            //{
            //    try
            //    {
            //        File.Copy(fileInfo.FullName,
            //            sortService.SortAsync(fileInfo.Name,info.AssetVersion.Revision).Result
            //            ,true);
            //        File.Delete(fileInfo.FullName);
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //    }
            //}
        }
    }
}
