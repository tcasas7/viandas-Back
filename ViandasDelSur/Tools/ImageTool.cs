using ViandasDelSur.Models;

namespace ViandasDelSur.Tools
{
    public class ImageTool
    {
        public byte[] GetImageFromPath(string path, string placeholderPath)
        {
            byte[] fileContent;

            if (System.IO.File.Exists(path))
            {
                fileContent = System.IO.File.ReadAllBytes(path);
            }
            else
            {
                fileContent = System.IO.File.ReadAllBytes(placeholderPath);
            }
            return fileContent;
        }

        public Image CreateImage(IFormFile file)
        {
            var mediaRoute = "media";
            var imageRoute = Path.Combine(mediaRoute);

            if (!Directory.Exists(imageRoute))
            {
                Directory.CreateDirectory(imageRoute);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fullRoute = Path.Combine(imageRoute, fileName);

            using (var stream = new FileStream(fullRoute, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var newImage = new Image();

            newImage.name = fileName;
            newImage.route = fullRoute;

            return newImage;
        }

        public bool DeleteImage(string imagePath, string imageName)
        {
            if (System.IO.File.Exists(imagePath) && imageName.ToUpper() != "DEFAULT".ToUpper())
            {
                System.IO.File.Delete(imagePath);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
