using System;
using System.Drawing.Imaging;
using System.IO;
using ImageProcessor.Caching;
using ImageProcessor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ImageProcessor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageProcessorController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        ILogger<ImageProcessorController> Logger;
        IImageCache Cache { get; set; }
        public ImageProcessorController(IConfiguration configuration, ILogger<ImageProcessorController> logger, IImageCache cache)
        {
            Configuration = configuration;
            Logger = logger;
            Cache = cache;
        }

        [HttpGet("{name}/{height}/{width}/{waterMark}/{format}")]
        public IActionResult Get(string name, int height, int width, string watermark, string format)
        {
            ProductImage img;
            Logger.LogInformation($"Request for image Name:{name} Height:{height} Width:{width} Format:{format} with WaterMark:{watermark}");
            try
            {
                img = Cache.TryGetImage(new ProductImage()
                {
                    Name = name,
                    Width = width,
                    Height = height,
                    Watermark = watermark,
                    Format = SetFormat(format)
                });
            } catch (FileNotFoundException e)
            {
                Logger.LogError($"Unable to locate file {name} - {e.Message}");
                img = new ProductImage();
                img.LoadBasic("Error/ImageNotFound");
            } catch (ArgumentException e)
            {
                Logger.LogError($"Invalid Argument - {e.Message}");
                img = new ProductImage();
                img.LoadBasic("Error/InvalidParametersBottle");
            } catch (Exception e)
            {
                Logger.LogError(e.Message);
                img = new ProductImage();
                img.LoadBasic("Error/SomethingWentWrongBottle");
            }
            return new FileContentResult(img.BArr, $"image/{format.ToLowerInvariant()}") { FileDownloadName = $"{name}.{format.ToLowerInvariant()}" };
        }

        private ImageFormat SetFormat(string format)
        {
            switch (format.ToUpperInvariant())
            {
                case "PNG":
                    return ImageFormat.Png;
                case "JPG":
                    return ImageFormat.Png;
                case "BMP":
                    return ImageFormat.Png;
                default:
                    throw new ArgumentException($"Format {format} is not supported");
            }
        }
    }
}



