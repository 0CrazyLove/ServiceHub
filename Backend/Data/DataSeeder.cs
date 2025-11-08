using Backend.Models;

namespace Backend.Data;
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Products.Any())
            {
                var products = new Product[]
                {
                    new Product { Name = "Laptop Gamer", Description = "Laptop de alta gama para juegos", Price = 1500.00m, ImageUrl = "images/laptop.jpg", Stock = 10, Category = "Electrónica" },
                    new Product { Name = "Smartphone", Description = "Teléfono inteligente de última generación", Price = 800.00m, ImageUrl = "images/smartphone.jpg", Stock = 25, Category = "Electrónica" },
                    new Product { Name = "Auriculares Bluetooth", Description = "Auriculares con cancelación de ruido", Price = 150.00m, ImageUrl = "images/headphones.jpg", Stock = 50, Category = "Accesorios" },
                    new Product { Name = "Teclado Mecánico", Description = "Teclado para gaming y programación", Price = 120.00m, ImageUrl = "images/keyboard.jpg", Stock = 30, Category = "Accesorios" },
                    new Product { Name = "Monitor 4K", Description = "Monitor de 27 pulgadas con resolución 4K", Price = 450.00m, ImageUrl = "images/monitor.jpg", Stock = 15, Category = "Electrónica" },
                    new Product { Name = "Silla Gamer", Description = "Silla ergonómica para largas sesiones de juego", Price = 250.00m, ImageUrl = "images/chair.jpg", Stock = 20, Category = "Muebles" },
                    new Product { Name = "Mouse Inalámbrico", Description = "Mouse óptico de alta precisión", Price = 60.00m, ImageUrl = "images/mouse.jpg", Stock = 40, Category = "Accesorios" },
                    new Product { Name = "Webcam HD", Description = "Cámara web para streaming y videoconferencias", Price = 90.00m, ImageUrl = "images/webcam.jpg", Stock = 35, Category = "Electrónica" },
                    new Product { Name = "Mochila para Laptop", Description = "Mochila resistente al agua para laptops de hasta 15 pulgadas", Price = 70.00m, ImageUrl = "images/backpack.jpg", Stock = 22, Category = "Accesorios" },
                    new Product { Name = "Disco Duro Externo 1TB", Description = "Almacenamiento externo portátil de 1TB", Price = 100.00m, ImageUrl = "images/hdd.jpg", Stock = 18, Category = "Almacenamiento" }
                };
                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
