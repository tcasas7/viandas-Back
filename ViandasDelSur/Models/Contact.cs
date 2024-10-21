using ViandasDelSur.Models.DTOS;

namespace ViandasDelSur.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string phone { get; set; }
        public string cbu { get; set; }
        public string alias { get; set; }
        public string name { get; set; }
        public bool IsActive { get; set; }
        public string wppMessage { get; set; }
        public string accountName { get; set; }
       
        public Contact()
        {

        }
        public Contact(ContactDTO contactDTO)
        {
            Id = contactDTO.Id;
            phone = contactDTO.phone;
            cbu = contactDTO.cbu;
            alias = contactDTO.alias;
            name = contactDTO.name;
            IsActive = contactDTO.IsActive;
            wppMessage = contactDTO.wppMessage;
            accountName = contactDTO.accountName;
            
        }
    }
}
