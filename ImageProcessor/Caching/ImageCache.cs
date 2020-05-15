using ImageProcessor.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace ImageProcessor.Caching
{
    public class ImageCache : IImageCache
    {
        private IMemoryCache Cache;
        private IConfiguration Configuration;
        private ILogger Logger;

        public ImageCache(IMemoryCache memoryCache, IConfiguration config, ILogger<ImageCache> logger)
        {
            Cache = memoryCache;
            Configuration = config;
            Logger = logger;
        }

        public ProductImage TryGetImage(ProductImage image)
        {
            var code = image.GetHashCode();
            if (!Cache.TryGetValue(code, out ProductImage cachedImage))
            {
                Logger.LogInformation($"Image not Cached - Caching image - Name:{image.Name} Height:{image.Height} Width:{image.Width} Format:{image.Format.ToString()} with Watermark:'{image.Watermark}' Code:{code}");
                image.ProcessImage();
                cachedImage = image;
                var cacheDuration = double.Parse(Configuration.GetSection("CacheSettings:Duration").Value);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetSlidingExpiration(TimeSpan.FromSeconds(cacheDuration));
                Cache.Set(code, cachedImage, cacheEntryOptions);
            } else
            {
                Logger.LogInformation($"Returning Cached image - Name:{image.Name} Height:{image.Height} Width:{image.Width} Format:{image.Format.ToString()} with Watermark:'{image.Watermark}' Code:{code}");
            }
            return cachedImage;
        }
    }
}
