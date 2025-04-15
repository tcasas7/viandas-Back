using Microsoft.CodeAnalysis.Scripting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Enums;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Implementations;
using ViandasDelSur.Repositories.Interfaces;
using ViandasDelSur.Services.Interfaces;
using ViandasDelSur.Tools;
using BCrypt.Net;


namespace ViandasDelSur.Services.Implementations
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IVerificationService _verificationService;
        private readonly IContactRepository _contactRepository;
        private readonly Encrypter _encrypter;
        private readonly IConfiguration _configuration;
        private readonly VDSContext _db;



        public UsersService(
            IUserRepository userRepository,
            ILocationRepository locationRepository,
            IVerificationService verificationService,
            IContactRepository contactRepository,
            IConfiguration configuration,
            VDSContext db
            )
        {
            _userRepository = userRepository;
            _locationRepository = locationRepository;
            _verificationService = verificationService;
            _contactRepository = contactRepository;
            _encrypter = new Encrypter();
            _configuration = configuration;
            _db = db;
        }

        public Response GetAll(string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            response = _verificationService.VerifyAdmin(user);

            if (response.statusCode != 200)
                return response;

            var users = _userRepository.GetAllUsers().ToList();

            if (users == null || users.Count <= 0)
            {
                response.statusCode = 404;
                response.message = "Usuarios no encontrados";
                return response;
            }

            List<UserDTO> result = new List<UserDTO>();

            foreach (var item in users)
            {
                UserDTO userDTO = new UserDTO(item);
                result.Add(userDTO);
            }

            response = new ResponseCollection<UserDTO>(200, "Ok", result);

            return response;
        }

        public Response Register(RegisterDTO model)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(model.email);

            if (user != null)
            {
                response.statusCode = 401;
                response.message = "El email ya esta en uso";
                return response;
            }

            _encrypter.EncryptString(model.password, out byte[] hash, out byte[] salt);

            User newUser = new User();

            newUser.email = model.email;
            newUser.firstName = model.firstName;
            newUser.lastName = model.lastName;
            newUser.phone = model.phone;
            newUser.hash = hash;
            newUser.salt = salt;
            newUser.role = Models.Enums.Role.CLIENT;

            _userRepository.Save(newUser);

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }

        public string GenerateResetToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public void UpdatePasswordByEmail(string email, string newPassword)
        {
            var user = _db.Users.FirstOrDefault(u => u.email == email);
            if (user != null)
            {
                // Usamos Encrypter
                byte[] newHash, newSalt;
                _encrypter.EncryptString(newPassword, out newHash, out newSalt);

                user.hash = newHash;
                user.salt = newSalt;

                _db.SaveChanges();
            }
        }

        public User GetUserByEmail(string email)
        {
            return _db.Users.FirstOrDefault(u => u.email == email);
        }


        public Response ChangePassword(ChangePasswordDTO model)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(model.email);

            if (user == null)
            {
                response.statusCode = 404;
                response.message = "Usuario no encontrado";
                return response;
            }

            if (user.phone != model.phone)
            {
                response.statusCode = 404;
                response.message = "Usuario no encontrado";
                return response;
            }

            _encrypter.EncryptString(model.password, out byte[] hash, out byte[] salt);

            user.hash = hash;
            user.salt = salt;

            _userRepository.Save(user);

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }

        public Response ChangeRole(ChangeRoleDTO model, string adminEmail)
        {
            Response response = new Response();

            var admin = _userRepository.FindByEmail(adminEmail);

            response = _verificationService.VerifyAdmin(admin);

            if (response.statusCode != 200)
            {
                response.statusCode = 403;
                response.message = "Sesión invalida";
                return response;
            }

            var user = _userRepository.FindByEmail(model.email);

            if (user == null)
            {
                response.statusCode = 404;
                response.message = "Usuario no encontrado";
                return response;
            }

            user.role = (Role)Enum.Parse(typeof(Role), model.Role, true);

            _userRepository.Save(user);

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }

        public Response ChangePhone(ChangePhoneDTO model, string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 404;
                response.message = "Usuario no encontrado";
                return response;
            }

            user.phone = model.phone;

            _userRepository.Save(user);

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }

        public Response Data(string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            // Asegurarse de que 'user' tiene todas las propiedades necesarias
            UserDTO userDTO = new UserDTO(user);

            // Devolver el UserDTO dentro de un ResponseModel
            response = new ResponseModel<UserDTO>(200, "Ok", userDTO);

            return response;
        }

        public async Task<Response> AddLocation(LocationDTO model, string email)
        {
            var response = new Response();

            try
            {
                // Validar el usuario
                var user = _userRepository.FindByEmail(email);
                if (user == null)
                {
                    response.statusCode = 401;
                    response.message = "Sesión inválida. Usuario no encontrado.";
                    return response;
                }

                // Validar que la dirección no esté vacía
                if (string.IsNullOrEmpty(model.dir))
                {
                    response.statusCode = 400;
                    response.message = "La dirección no puede estar vacía.";
                    return response;
                }

                // Verificar si la dirección ya existe para este usuario
                var existingLocation = _locationRepository.FindByUserIdAndDir(user.Id, model.dir);
                if (existingLocation != null)
                {
                    response.statusCode = 400;
                    response.message = "La dirección ya está registrada para este usuario.";
                    return response;
                }

                // Obtener las coordenadas usando la API de Google Maps
                var coordinates = await GetCoordinatesFromGoogleMaps(model.dir);
                if (coordinates == null || coordinates.Latitude == 0 || coordinates.Longitude == 0)
                {
                    response.statusCode = 400;
                    response.message = "No se pudieron obtener coordenadas válidas para la dirección proporcionada.";
                    return response;
                }

                // Crear la nueva ubicación
                var newLocation = new Location
                {
                    Id = 0, // Puede ser generado automáticamente por la base de datos
                    dir = model.dir,
                    userId = user.Id,
                    Latitude = coordinates.Latitude,
                    Longitude = coordinates.Longitude
                };

                // Guardar la ubicación
                _locationRepository.Save(newLocation);

                response.statusCode = 200;
                response.message = "Dirección agregada con éxito.";
                response.data = newLocation;

                return response;
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = $"Error interno del servidor: {ex.Message}";
                return response;
            }
        }
        private async Task<Location?> GetCoordinatesFromGoogleMaps(string address)
        {
            try
            {
                // Configurar cliente HTTP con timeout
                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };

                // Generar URL para la API de Google Maps
                string apiKey = "AIzaSyDggzKaDsmmqRH0dr_SXWg37tyJax0U0eo";
                string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={apiKey}";

                // Realizar solicitud HTTP
                var response = await client.GetStringAsync(url);

                Console.WriteLine("Google Maps Response: " + response);

                // Procesar la respuesta JSON
                dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

                // Validar si se encontraron resultados
                if (jsonResponse.results == null || jsonResponse.results.Count == 0)
                {
                    Console.WriteLine("No se encontraron resultados para la dirección proporcionada.");
                    return null;
                }

                // Extraer coordenadas de latitud y longitud
                var lat = (double?)jsonResponse.results[0].geometry.location.lat;
                var lng = (double?)jsonResponse.results[0].geometry.location.lng;

                if (lat == null || lng == null)
                {
                    Console.WriteLine("No se pudieron extraer coordenadas de la respuesta.");
                    return null;
                }

                // Validar que la dirección pertenezca a Mar del Plata y General Pueyrredon
                var addressComponents = jsonResponse.results[0].address_components;
                bool isValidLocation = false;

                foreach (var component in addressComponents)
                {
                    var types = component.types.ToObject<List<string>>();

                    // Verificar que el componente corresponda a Mar del Plata
                    if (types.Contains("locality") && component.long_name == "Mar del Plata")
                    {
                        isValidLocation = true;
                    }

                    // Verificar que el componente corresponda a General Pueyrredon
                    if (types.Contains("administrative_area_level_2") && component.long_name == "General Pueyrredon")
                    {
                        isValidLocation = true;
                    }
                }

                if (!isValidLocation)
                {
                    Console.WriteLine("La dirección no pertenece a Mar del Plata o General Pueyrredon.");
                    return null;
                }

                // Retornar objeto Location con las coordenadas obtenidas
                return new Location
                {
                    Latitude = lat.Value,
                    Longitude = lng.Value,
                    dir = address
                };
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error de red al contactar la API de Google Maps: {httpEx.Message}");
                throw new Exception("Timeout al contactar la API de Google Maps.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetCoordinatesFromGoogleMaps: {ex.Message}");
                throw;
            }
        }


        public Response MakeDefault(LocationDTO model, string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            var newLocation = _locationRepository.FindById(model.Id);

            if (newLocation == null || newLocation.userId != user.Id)
            {
                response.statusCode = 404;
                response.message = "Dirección no encontrada";
                return response;
            }

            var oldLocation = _locationRepository.GetDefault(email);

            if (oldLocation != null)
            {
                oldLocation.isDefault = false;
                _locationRepository.Save(oldLocation);
            }

            newLocation.isDefault = true;
            _locationRepository.Save(newLocation);

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }

        public Response RemoveLocation(LocationDTO model, string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            var location = _locationRepository.FindById(model.Id);

            if (location == null || location.userId != user.Id)
            {
                response.statusCode = 404;
                response.message = "Dirección no encontrada";
                return response;
            }

            _locationRepository.Remove(location);

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }
        public Response GetAllContacts(string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            response = _verificationService.VerifyAdmin(user);

            if (response.statusCode != 200)
                return response;

            var contacts = _contactRepository.GetAll().ToList();

            if (contacts == null)
            {
                response.statusCode = 404;
                response.message = "Contactos no encontrados";
                return response;
            }

            List<ContactDTO> result = new List<ContactDTO>();

            foreach (var contact in contacts)
            {
                ContactDTO contactDTO = new ContactDTO(contact);
                result.Add(contactDTO);
            }

            response = new ResponseCollection<ContactDTO>(200, "Ok", result);

            return response;
        }
        public Response GetActiveContact(string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión inválida";
                return response;
            }

            var activeContact = _contactRepository.GetActive();

            if (activeContact == null)
            {
                response.statusCode = 404;
                response.message = "No hay información de pago activa.";
                return response;
            }

            ContactDTO contactDTO = new ContactDTO(activeContact);

            response = new ResponseModel<ContactDTO>(200, "Ok", contactDTO);

            return response;
        }

        public Response AddContact(ContactDTO model, string email)
        {
            var response = new Response();

            try
            {
                // Verificar si el usuario existe
                var user = _userRepository.FindByEmail(email);
                if (user == null)
                {
                    response.statusCode = 401;
                    response.message = "Sesión inválida";
                    return response;
                }

                // Verificar permisos de administrador
                response = _verificationService.VerifyAdmin(user);
                if (response.statusCode != 200)
                    return response;

                Console.WriteLine($"Datos a guardar: Phone={model.phone}, CBU={model.cbu}, Alias={model.alias}, Name={model.name}, AccountName={model.accountName}, WppMessage={model.wppMessage}");

                // Crear un nuevo contacto
                var newContact = new Contact
                {
                    phone = model.phone,
                    cbu = model.cbu,
                    alias = model.alias,
                    name = model.name,
                    accountName = model.accountName,
                    wppMessage = model.wppMessage,
                    IsActive = false
                };

                // Guardar en la base de datos
                _contactRepository.Save(newContact);

                response.statusCode = 200;
                response.message = "Contacto agregado con éxito";
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = $"Error al guardar el contacto: {ex.Message}";
            }

            return response;
        }

        public Response UpdateContact(ContactDTO model, string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            response = _verificationService.VerifyAdmin(user);

            if (response.statusCode != 200)
                return response;

            if (model.Id == 0)
            {
                response.statusCode = 400;
                response.message = "Datos invalidos";
                return response;
            }

            var contact = _contactRepository.GetById(model.Id);

            if (contact == null)
            {
                response.statusCode = 404;
                response.message = "Contacto no encontrado";
                return response;
            }
            contact = new Contact(model);

            _contactRepository.Save(contact);

            response.statusCode = 200;
            response.message = "Ok";

            return response;
        }

        public Response RemoveContact(ContactDTO model, string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            response = _verificationService.VerifyAdmin(user);

            if (response.statusCode != 200)
                return response;

            var contact = _contactRepository.GetById(model.Id);

            if (contact == null)
            {
                response.statusCode = 404;
                response.message = "Contacto no encontrado";
                return response;
            }

            _contactRepository.Remove(contact);

            response.statusCode = 200;
            response.message = "Ok";

            return response;
        }

        public Response MakeActive(ContactDTO model, string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            response = _verificationService.VerifyAdmin(user);

            if (response.statusCode != 200)
                return response;

            var contact = _contactRepository.GetById(model.Id);

            if (contact == null)
            {
                response.statusCode = 404;
                response.message = "Contacto no encontrado";
                return response;
            }

            var activeContact = _contactRepository.GetActive();
            contact.IsActive = true;

            _contactRepository.Save(contact);


            if (activeContact != null)
            {
                if (activeContact.Id != contact.Id)
                {
                    activeContact.IsActive = false;
                    _contactRepository.Save(activeContact);
                }
            }

            response.statusCode = 200;
            response.message = "Ok";

            return response;
        }

        public Response GetPendingUsers()
        {
            // Filtrar usuarios no verificados desde el repositorio
            var pendingUsers = _userRepository.GetAllUsers()
                .Where(u => !u.IsVerified) // Solo usuarios no verificados
                .Select(u => new UserDTO(u)) // Mapear al DTO
                .ToList();

            if (!pendingUsers.Any())
            {
                return new Response
                {
                    statusCode = 404,
                    message = "No hay usuarios pendientes de aprobación."
                };
            }

            // Devolver usuarios en una respuesta estándar
            return new ResponseCollection<UserDTO>(200, "Usuarios pendientes obtenidos", pendingUsers);
        }

        public Response ApproveUser(int userId)
        {
            // Buscar el usuario por ID
            var user = _userRepository.FindById(userId);

            // Si el usuario no existe
            if (user == null)
            {
                return new Response
                {
                    statusCode = 404,
                    message = "Usuario no encontrado."
                };
            }

           
            if (user.IsVerified)
            {
                return new Response
                {
                    statusCode = 400,
                    message = "El usuario ya está aprobado."
                };
            }

            
            user.IsVerified = true;

            
            _userRepository.Save(user);

            
            return new Response
            {
                statusCode = 200,
                message = "Usuario aprobado exitosamente."
            };
        }

        public void RejectUser(int userId)
        {
            var user = _userRepository.FindById(userId);

            if (user != null)
            {
                _userRepository.Remove(user); 
                _userRepository.SaveChanges();
            }
        }

        public User FindById(int userId)
        {
            return _userRepository.FindById(userId); 
        }

    }
}
