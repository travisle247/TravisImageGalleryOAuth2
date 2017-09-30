using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.InteropServices;

namespace TravisImageGallery.API.Entities
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<GalleryContext>
    {
        private bool isWindows=false;
        private bool isMac = false;
        public GalleryContext CreateDbContext(string[] args)
        {
            isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            var builder = new DbContextOptionsBuilder<GalleryContext>();

            if(isWindows==true)
            {
                var sqlConnectionString = configuration.GetConnectionString("DefaultWindowConnectionn");
                builder.UseSqlServer(sqlConnectionString);
            }
            else if (isMac == true)
            {
                var sqlConnectionString = configuration.GetConnectionString("DefaultMacConnectionn");
                builder.UseSqlServer(sqlConnectionString);
            }

            return new GalleryContext(builder.Options);
        }
    }
}
