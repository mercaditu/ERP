using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace entity.Brillo
{
    public class Byte2FileConverter
    {


        public BitmapImage ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null || byteArrayIn.Length == 0) return null;
            using (var ms = new System.IO.MemoryStream(byteArrayIn))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
        public byte[] ResizeImage(string path)
        {
            Image image;
            using (var inputStream = new MemoryStream(File.ReadAllBytes(path)))
            {
                string extension = Path.GetExtension(path);
                image = Image.FromStream(inputStream);
                var encoderParameters = new EncoderParameters(1);

                if (extension.ToLower().Contains("jpeg") || extension.Contains("jpg"))
                {
                    var jpegEncoder = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                    var jpegQuality = 50;
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);
                    using (var outputStream = new MemoryStream())
                    {
                        image.Save(outputStream, jpegEncoder, encoderParameters);
                        return outputStream.ToArray();
                    }

                }
                else if (extension.ToLower().Contains("png"))
                {
                    var pngEncoder = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == ImageFormat.Png.Guid);
                    var pngQuality = 50;
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, pngQuality);

                    using (var outputStream = new MemoryStream())
                    {
                        image.Save(outputStream, pngEncoder, encoderParameters);
                        return outputStream.ToArray();
                    }
                }
            }

            return null;
        }

    }
}
