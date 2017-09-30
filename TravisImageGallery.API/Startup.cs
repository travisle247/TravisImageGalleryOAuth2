using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Model = TraivsImageGallery.Model;
using TravisImageGallery.API.Entities;
using NLog.Extensions.Logging;
using TravisImageGallery.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;


namespace TravisImageGallery.API
{
    public class Startup
    {
        private bool isWindows = false;
        private bool isMac = false;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            if (isWindows == true)
            {
                var sqlConnectionString = Configuration["ConnectionStrings:DefaultWindowConnectionn"];

                services.AddDbContext<GalleryContext>(options =>
                    options.UseSqlServer(sqlConnectionString)
                );
            }
            else if (isMac == true)
            {
                var sqlConnectionString = Configuration["ConnectionStrings:DefaultMacConnectionn"];

                services.AddDbContext<GalleryContext>(options =>
                    options.UseSqlServer(sqlConnectionString)
                );
            }
          

            services.AddScoped<IGalleryRepository, GalleryRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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
            var scope = app.ApplicationServices.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<GalleryContext>();
            initializer.Initialize();
           
            app.UseMvc();
        }
    }
}
