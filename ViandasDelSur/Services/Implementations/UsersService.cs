using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Enums;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Implementations;
using ViandasDelSur.Repositories.Interfaces;
using ViandasDelSur.Services.Interfaces;
using ViandasDelSur.Tools;

namespace ViandasDelSur.Services.Implementations
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IVerificationService _verificationService;
        private readonly IContactRepository _contactRepository;
        private readonly Encrypter _encrypter;

        public UsersService(
            IUserRepository userRepository,
            ILocationRepository locationRepository,
            IVerificationService verificationService,
            IContactRepository contactRepository
            )
        {
            _userRepository = userRepository;
            _locationRepository = locationRepository;
            _verificationService = verificationService;
            _contactRepository = contactRepository;
            _encrypter = new Encrypter();
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
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            // Llamada a la API de Google para obtener coordenadas (latitud y longitud)
            var coordinates = await GetCoordinatesFromGoogleMaps(model.dir);

            Location newLocation = new Location
            {
                Id = model.Id,
                dir = model.dir,
                userId = user.Id,
                Latitude = coordinates.Latitude,
                Longitude = coordinates.Longitude // Asignamos las coordenadas obtenidas
            };

            _locationRepository.Save(newLocation);

            response.statusCode = 200;
            response.message = "Ok";

            return response;
        }

        // Método para llamar a la API de Google Maps y obtener latitud y longitud
        private async Task<(double Latitude, double Longitude)> GetCoordinatesFromGoogleMaps(string address)
        {
            var client = new HttpClient();
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key=TU_API_KEY";
            var response = await client.GetStringAsync(url);

            // Procesar la respuesta JSON y obtener latitud y longitud
            dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
            var lat = (double)jsonResponse.results[0].geometry.location.lat;
            var lng = (double)jsonResponse.results[0].geometry.location.lng;

            return (lat, lng);
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
                response.message = "Sesión invalida";
                return response;
            }

            response = _verificationService.VerifyAdmin(user);

            if (response.statusCode != 200)
                return response;

            var activeContact = _contactRepository.GetActive();

            if (activeContact == null)
            {
                response.statusCode = 404;
                response.message = "Contacto activo no encontrado";
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
    }
}
