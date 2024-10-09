using ViandasDelSur.Models;
using System.IO;

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
            var mediaRoute = "media"; // Carpeta donde se guardan las imágenes
            var imageRoute = Path.Combine(mediaRoute);

            if (!Directory.Exists(imageRoute))
            {
                Directory.CreateDirectory(imageRoute);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Solo el nombre del archivo
            var fullRoute = Path.Combine(imageRoute, fileName); // Ruta completa para guardar la imagen

            // Guardar el archivo físicamente en la carpeta media
            using (var stream = new FileStream(fullRoute, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var newImage = new Image
            {
                name = fileName, // Guardar solo el nombre de la imagen en la base de datos
                route = fileName // Guardar solo el nombre de la imagen, sin 'media/' en la ruta
            };

            return newImage;
        }


        /*public Image CreateImage(IFormFile file)
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
        }*/

        public bool DeleteImage(string imagePath, string imageName)
        {
            if (System.IO.File.Exists(imagePath) && imagePath.ToUpper() != "DEFAULT.PNG".ToUpper())
            {
                System.IO.File.Delete(imagePath);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Image CreateForNew(string productName)
        {
            var newImage = new Image();
            newImage.name = productName + ".png";
            newImage.route = "Media\\Default.png";
            return newImage;
        }

        public Image SaveImage(Stream imageStream, string fileName)
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Media");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                imageStream.CopyTo(fileStream);
            }

            // Crear y devolver un nuevo objeto de imagen con la información correcta
            return new Image
            {
                name = fileName,
                route = fileName
            };
        }
    }
}
