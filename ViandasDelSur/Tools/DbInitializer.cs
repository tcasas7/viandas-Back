﻿using Microsoft.AspNetCore.Mvc;
using ViandasDelSur.Models;
using ViandasDelSur.Models.Enums;

namespace ViandasDelSur.Tools
{
    public class DbInitializer
    {
        public static void Initialize(VDSContext context)
        {
            bool imageContent = context.Images.Any();
            bool userContent = context.Users.Any();
            bool menuContent = context.Menus.Any();
            bool productContent = context.Products.Any();

            if (!imageContent)
            {
                Image image = new Image();
                image.route = "Default.png";
                image.name = "Default";

                context.Images.Add(image);
                context.SaveChanges();
            }

            if (!userContent)
            {
                Encrypter encrypter = new Encrypter();

                encrypter.EncryptString("@Fran4897685", out byte[] hash, out byte[] salt);

                var user = new User
                {
                    role = Role.ADMIN,
                    firstName = "Francisco",
                    lastName = "Fernandez",
                    email = "cejita678@gmail.com",
                    phone = "+542235834483",
                    hash = hash,
                    salt = salt
                };

                context.Users.Add(user);
                context.SaveChanges();
            }

            if (!menuContent)
            {
                Menu menuEstandar = new Menu();
                menuEstandar.category = "Estandar";
                menuEstandar.validDate = DatesTool.GetNextDay(DayOfWeek.Monday);

                Menu menuLight = new Menu();
                menuLight.category = "Light";
                menuLight.validDate = DatesTool.GetNextDay(DayOfWeek.Monday);

                Menu menuProteico = new Menu();
                menuProteico.category = "Proteico";
                menuProteico.validDate = DatesTool.GetNextDay(DayOfWeek.Monday);

                context.Menus.Add(menuEstandar);
                context.Menus.Add(menuLight);
                context.Menus.Add(menuProteico);
                context.SaveChanges();
            }

            if (!productContent)
            {

                var mE = context.Menus.Where(m => m.category == "Estandar").FirstOrDefault();

                if(mE == null) 
                    return;

                var mL = context.Menus.Where(m => m.category == "Light").FirstOrDefault();

                if (mL == null)
                    return;

                var mP = context.Menus.Where(m => m.category == "Proteico").FirstOrDefault();

                if (mP == null)
                    return;

                var image = context.Images.Where(i => i.name == "Default").FirstOrDefault();

                if (image == null) 
                    return;

                Product e1 = new Product();
                Product l1 = new Product();
                Product p1 = new Product();

                Product e2 = new Product();
                Product l2 = new Product();
                Product p2 = new Product();

                Product e3 = new Product();
                Product l3 = new Product();
                Product p3 = new Product();

                Product e4 = new Product();
                Product l4 = new Product();
                Product p4 = new Product();

                Product e5 = new Product();
                Product l5 = new Product();
                Product p5 = new Product();

                e1.menuId = mE.Id;
                e1.name = "Albondigas con arroz";
                e1.day = DayOfWeek.Monday;
                e1.Image = image;
                context.Products.Add(e1);

                l1.menuId = mL.Id;
                l1.name = "Omelette de zanahoria y zuccini";
                l1.day = DayOfWeek.Monday;
                l1.Image = image;
                context.Products.Add(l1);

                p1.menuId = mP.Id;
                p1.name = "Lasagna proteica";
                p1.day = DayOfWeek.Monday;
                p1.Image = image;
                context.Products.Add(p1);

                ///////////////////////////////////////////

                e2.menuId = mE.Id;
                e2.name = "Tallarines";
                e2.day = DayOfWeek.Tuesday;
                e2.Image = image;
                context.Products.Add(e2);

                l2.menuId = mL.Id;
                l2.name = "Milanesa de calabaza";
                l2.day = DayOfWeek.Tuesday;
                l2.Image = image;
                context.Products.Add(l2);

                p2.menuId = mP.Id;
                p2.name = "Carbonada de pollo";
                p2.day = DayOfWeek.Tuesday;
                p2.Image = image;
                context.Products.Add(p2);

                //////////////////////////////////////////

                e3.menuId = mE.Id;
                e3.name = "Carbonada de pollo";
                e3.day = DayOfWeek.Wednesday;
                e3.Image = image;
                context.Products.Add(e3);

                l3.menuId = mL.Id;
                l3.name = "Cazuela veggie";
                l3.day = DayOfWeek.Wednesday;
                l3.Image = image;
                context.Products.Add(l3);

                p3.menuId = mP.Id;
                p3.name = "Pastel de cerdo braseado";
                p3.day = DayOfWeek.Wednesday;
                p3.Image = image;
                context.Products.Add(p3);

                //////////////////////////////////////////

                e4.menuId = mE.Id;
                e4.name = "Brochette de cerdo";
                e4.day = DayOfWeek.Thursday;
                e4.Image = image;
                context.Products.Add(e4);

                l4.menuId = mL.Id;
                l4.name = "Wok de fideos de arroz";
                l4.day = DayOfWeek.Thursday;
                l4.Image = image;
                context.Products.Add(l4);

                p4.menuId = mP.Id;
                p4.name = "Ternera con zapallitos";
                p4.day = DayOfWeek.Thursday;
                p4.Image = image;
                context.Products.Add(p4);

                //////////////////////////////////////////

                e5.menuId = mE.Id;
                e5.name = "Ternera";
                e5.day = DayOfWeek.Thursday;
                e5.Image = image;
                context.Products.Add(e5);

                l5.menuId = mL.Id;
                l5.name = "Tarta capresse";
                l5.day = DayOfWeek.Thursday;
                l5.Image = image;
                context.Products.Add(l5);

                p5.menuId = mP.Id;
                p5.name = "Planchita de pollo";
                p5.day = DayOfWeek.Thursday;
                p5.Image = image;
                context.Products.Add(p5);

                context.SaveChanges();
            }
        }
    }
    
}
