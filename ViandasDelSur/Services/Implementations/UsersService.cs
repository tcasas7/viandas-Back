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


        public Response AddLocation(LocationDTO model, string email)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            Location newLocation = new Location();

            newLocation.Id = model.Id;
            newLocation.dir = model.dir;
            newLocation.userId = user.Id;

            _locationRepository.Save(newLocation);

            response.statusCode = 200;
            response.message = "Ok";

            return response;
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

            if (model.Id != 0)
            {
                response.statusCode = 400;
                response.message = "Datos invalidos";
                return response;
            }

            Contact newContact = new Contact(model);
            newContact.isActive = false;

            _contactRepository.Save(newContact);

            response.statusCode = 200;
            response.message = "Ok";

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
            contact.isActive = true;

            _contactRepository.Save(contact);


            if (activeContact != null)
            {
                if (activeContact.Id != contact.Id)
                {
                    activeContact.isActive = false;
                    _contactRepository.Save(activeContact);
                }
            }

            response.statusCode = 200;
            response.message = "Ok";

            return response;
        }
    }
}
