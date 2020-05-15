using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageProcessor.Models.Images
{
    public class ProductViewModel
    {
        public List<string> ProductImages { get; set; }
        public void GetFileNames()
        {
            ProductImages = new List<string>();
            var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "ProductImages");
            if (Directory.Exists(imageDir))
            {
                foreach (var file in Directory.GetFiles(imageDir))
                {
                    ProductImages.Add(Path.GetFileNameWithoutExtension(file));
                }
                ProductImages.Add("XDemoMissingImage");
            }
        }
    }
}
