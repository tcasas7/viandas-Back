using ViandasDelSur.Models;
using ViandasDelSur.Models.Enums;

namespace ViandasDelSur.Tools
{
    public class DbInitializer
    {
        public static void Initialize(VDSContext context)
        {
            bool userContent = context.Users.Any();
            bool imageContent = context.Images.Any();

            if (!imageContent)
            {
                Image image = new Image();
                image.route = "Default.png";
                image.name = "Default";

                context.Images.Add(image);
                context.SaveChanges();
            }

            if (!userContent)
            {
                Encrypter encrypter = new Encrypter();

                encrypter.EncryptString("@Fran4897685", out byte[] hash, out byte[] salt);

                var user = new User
                {
                    role = Role.ADMIN,
                    firstName = "Francisco",
                    lastName = "Fernandez",
                    email = "cejita678@gmail.com",
                    hash = hash,
                    salt = salt
                };

                context.Users.Add(user);
                context.SaveChanges();
            }
        }
    }
    
}
