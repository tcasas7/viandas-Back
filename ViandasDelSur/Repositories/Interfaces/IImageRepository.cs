using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface IImageRepository
    {
        void Save(Image image);
        Image GetById(long id);
        Image GetByName(string name);
        void Remove(Image image);
        ICollection<Image> GetAll();
    }
}
