using ImageProcessor.Models;

namespace ImageProcessor.Caching
{
    public interface IImageCache
    {
        ProductImage TryGetImage(ProductImage image);
    }
}