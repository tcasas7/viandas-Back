using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class ImageRepository : RepositoryBase<Image>, IImageRepository
    {
        public ImageRepository(VDSContext repositoryContext) : base(repositoryContext)
        {
        }

        public void Save(Image image)
        {
            if (image.Id == 0)
            {
                Create(image);
            }
            else
            {
                Update(image);
            }
            SaveChanges();
        }
        public Image GetById(long id)
        {
            return FindByCondition(i => i.Id == id).FirstOrDefault();
        }

        public Image GetByName(string name)
        {
            return FindByCondition(i => i.name.ToUpper() == name.ToUpper()).FirstOrDefault();
        }
        public void Remove(Image image)
        {
            Delete(image);
            SaveChanges();
        }
    }
}

