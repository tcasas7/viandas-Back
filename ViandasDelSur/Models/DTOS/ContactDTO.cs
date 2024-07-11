namespace ViandasDelSur.Models.DTOS
{
    public class ContactDTO
    {
        public int Id { get; set; }
        public string phone { get; set; }
        public string cbu { get; set; }
        public string alias { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
        public string wppMessage { get; set; }
        public string accountName { get; set; }

        public ContactDTO()
        {

        }
        public ContactDTO(Contact contact)
        {
            Id = contact.Id;
            phone = contact.phone;
            cbu = contact.cbu;
            alias = contact.alias;
            name = contact.name;
            isActive = contact.isActive;
            wppMessage = contact.wppMessage;
            accountName = contact.accountName;
        }
    }
}
