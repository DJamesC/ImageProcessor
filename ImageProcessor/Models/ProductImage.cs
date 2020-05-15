using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageProcessor.Models
{
    public class ProductImage : IProductImage
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public string Name { get; set; }
        public string Watermark { get; set; }
        public ImageFormat Format { get; set; }
        public byte[] BArr { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(typeof(ProductImage), Name, Width, Height, Watermark, Format);
        }

        public void ProcessImage()
        {
            AddWatermark();
            Resize();
        }

        public void LoadBasic(string name)
        {
            using (Image img = LoadFromDisk(name))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Position = 0;
                    img.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    BArr = memoryStream.ToArray();
                }
            }
        }

        public Image LoadFromDisk(string name)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "ProductImages", $"{name}.png");
            if (File.Exists(file))
            {
                return Image.FromFile(file);
            }
            else
            {
                throw new FileNotFoundException($"File {name} does not exist");
            }
        }

        public void Resize()
        {
            using (var ms = new MemoryStream(BArr))
            {
                using (var newImage = new Bitmap(Width, Height))
                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(Image.FromStream(ms), new Rectangle(0, 0, Width, Height));
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        memoryStream.Position = 0;
                        newImage.Save(memoryStream, ImageFormat.Png);
                        BArr = memoryStream.ToArray();
                    }
                }
            }
        }

        public void AddWatermark()
        {
            using (Image img = LoadFromDisk(Name))
            {
                using (Graphics grp = Graphics.FromImage(img))
                {
                    if (Watermark != "NULL")
                    {
                        Brush brush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
                        Font goodFont = FindFont(grp, Watermark, img.Size, new System.Drawing.Font("Arial", 200, FontStyle.Bold, GraphicsUnit.Pixel));
                        SizeF textSize = new SizeF();
                        textSize = grp.MeasureString(Watermark, goodFont);
                        grp.DrawString(Watermark, goodFont, brush, (img.Width / 2) - (textSize.Width / 2), (img.Height / 2) - (textSize.Height / 2));
                    }
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        memoryStream.Position = 0;
                        img.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        BArr = memoryStream.ToArray();
                    }
                }
            }
        }

        private Font FindFont(System.Drawing.Graphics g, string longString, Size Room, Font PreferedFont)
        {
            SizeF RealSize = g.MeasureString(longString, PreferedFont);
            float HeightScaleRatio = Room.Height / RealSize.Height;
            float WidthScaleRatio = Room.Width / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio)
               ? ScaleRatio = HeightScaleRatio
               : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = PreferedFont.Size * ScaleRatio;
            return new Font(PreferedFont.FontFamily, ScaleFontSize, PreferedFont.Style, PreferedFont.Unit);
        }
    }
}
