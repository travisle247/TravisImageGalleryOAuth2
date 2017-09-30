using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model = TraivsImageGallery.Model;
using TravisImageGallery.API.Entities;
using NLog.Extensions.Logging;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;
using TravisImageGallery.API.Services;

namespace TravisImageGallery.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        public IConfiguration Configuration { get; }

        private bool isWindows = false;
        private bool isMac = false;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            if (isWindows == true)
            {
                var sqlConnectionString = Configuration["MySqlConnectionStrings:DataAccessMySqlProviderWindow"];

                services.AddDbContext<GalleryContext>(options =>
                    options.UseMySQL(sqlConnectionString)
                );
            }

            if (isMac == true)
            {
                var sqlConnectionString = Configuration["MySqlConnectionStrings:DataAccessMySqlProviderMac"];

                services.AddDbContext<GalleryContext>(options =>
                    options.UseMySQL(sqlConnectionString)
                );
            }

            services.AddScoped<IGalleryRepository, GalleryRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
             GalleryContext galleryContext)
        {
            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            loggerFactory.AddNLog();
            loggerFactory.ConfigureNLog("nlog.config");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }


            AutoMapper.Mapper.Initialize(cfg =>
            {
                // Map from Image (entity) to Image, and back
                cfg.CreateMap<Image, TraivsImageGallery.Model.Image>().ReverseMap();

                // Map from ImageForCreation to Image
                // Ignore properties that shouldn't be mapped
                cfg.CreateMap<Model.ImageForCreation, Image>()
                 .ForMember(m => m.FileName, options => options.Ignore())
                 .ForMember(m => m.Id, options => options.Ignore())
                 .ForMember(m => m.OwnerId, options => options.Ignore());

                // Map from ImageForUpdate to Image
                // ignore properties that shouldn't be mapped
                cfg.CreateMap<Model.ImageForUpdate, Image>()
                    .ForMember(m => m.FileName, options => options.Ignore())
                    .ForMember(m => m.Id, options => options.Ignore())
                    .ForMember(m => m.OwnerId, options => options.Ignore());
            });

            AutoMapper.Mapper.AssertConfigurationIsValid();

            // seed the DB with data
            galleryContext.EnsureSeedDataForContext();

            app.UseMvc();
        }
    }
}
