using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface IImageRepository
    {
        Image GetById(long id);
        void Save(Image image); // Aseguramos que el método Save esté definido
        void Remove(Image image);
        Image GetByName(string name); // Aseguramos que GetByName esté definido
        ICollection<Image> GetAll();  // Aseguramos que GetAll esté definido
        void Update(Image image); // Aseguramos que Update esté definido
    }
}
