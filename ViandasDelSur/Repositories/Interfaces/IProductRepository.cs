﻿using ViandasDelSur.Models;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface IProductRepository
    {
        ICollection<Product> GetAll();
        Product GetById(int id);
        void Save(Product product);
    }
}