using CmsShoppingCart.WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.DotNet.Scaffolding.Shared;

namespace CmsShoppingCart.WebApp.Infrastucture.Providers
{
    public interface IMediaImageProvider
    {
        Task<string> AddOrUpdateAsync(string oldImageName, IFormFile image, string catalog);

        Task<string> SaveAsync(IFormFile image, string catalog);
        
        void Delete(string imageName, string catalog);
    }

    public class MediaImageProvider : IMediaImageProvider
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public MediaImageProvider(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveAsync(IFormFile image, string catalog)
        {

            string uploadDir = Path.Combine(webHostEnvironment.WebRootPath, Path.Combine("media", catalog));
            var imageName = Guid.NewGuid().ToString() + "_" + image.FileName;
            string filePAth = Path.Combine(uploadDir, imageName);
            var fs = new FileStream(filePAth, FileMode.Create);
            await image.CopyToAsync(fs);
            fs.Close();

            return imageName;
        }

        public async Task<string> AddOrUpdateAsync(string oldImageName, IFormFile image, string catalog)
        {
            string uploadDir = Path.Combine(webHostEnvironment.WebRootPath, Path.Combine("media", catalog));
            string oldImagePath = Path.Combine(uploadDir, oldImageName);

            if (File.Exists(oldImagePath))
            {
                File.Delete(oldImagePath);
            }

            string imageName = Guid.NewGuid().ToString() + "_" + image.FileName;
            string filePAth = Path.Combine(uploadDir, imageName);
            FileStream fs = new FileStream(filePAth, FileMode.Create);
            await image.CopyToAsync(fs);
            fs.Close();

            return imageName;
        }

        public void Delete(string imageName, string catalog)
        {
            string iconsDir = Path.Combine(webHostEnvironment.WebRootPath, Path.Combine("media", catalog));
            string oldImagePath = Path.Combine(iconsDir, imageName);

            if (File.Exists(oldImagePath))
                File.Delete(oldImagePath);
        }
    }
}
