/*using Microsoft.EntityFrameworkCore;
using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;
using ViandasDelSur.Tools;

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
            var image = FindByCondition(i => i.name.ToUpper() == name.ToUpper()).FirstOrDefault();
            if (image != null)
            {
                RepositoryContext.Entry(image).State = EntityState.Detached;
            }
            return image;
        }
        public void Remove(Image image)
        {
            Delete(image);
            SaveChanges();
        }

        public ICollection<Image> GetAll()
        {
            return FindByCondition(i => i.name != "Default").ToList();
        }
    }
}*/

using Microsoft.EntityFrameworkCore;
using ViandasDelSur.Models;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Repositories.Implementations
{
    public class ImageRepository : RepositoryBase<Image>, IImageRepository
    {
        public new VDSContext RepositoryContext { get; } // Ocultamos intencionalmente la propiedad de la clase base

        public ImageRepository(VDSContext repositoryContext) : base(repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public Image GetById(long id)
        {
            return FindByCondition(i => i.Id == id).FirstOrDefault();
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
            SaveChanges(); // This persists the changes in the database.
        }

        public void Remove(Image image)
        {
            Delete(image);
            SaveChanges();
        }

        public Image GetByName(string name)
        {
            return FindByCondition(i => i.name == name).FirstOrDefault();
        }

        public ICollection<Image> GetAll()
        {
            return FindAll().ToList();
        }

        public new void Update(Image image)
        {
            RepositoryContext.Images.Update(image);
            SaveChanges();
        }
    }
}
