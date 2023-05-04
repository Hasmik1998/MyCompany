using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Utilities
{
    public class Utility
    {
        public static string IsActive(HttpContext context, params string[] urls)
        {
            foreach (string url in urls)
            {
                if (url.Equals(context.Request.Path, StringComparison.OrdinalIgnoreCase))
                {
                    return "active";
                }
            }
            return null;
        }
        public static byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
        public static Image ResizeImage(Image image, int width, int height)
        {
            MemoryStream imageStream = new MemoryStream();

            Bitmap result = new Bitmap(width, height);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            result.Save(imageStream, ImageFormat.Png);

            return Image.FromStream(imageStream);
        }
    }
}
