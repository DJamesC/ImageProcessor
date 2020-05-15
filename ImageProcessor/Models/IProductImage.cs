namespace ImageProcessor.Models
{
    public interface IProductImage
    {
        byte[] BArr { get; set; }
        int Height { get; set; }
        string Name { get; set; }
        int Width { get; set; }
    }
}