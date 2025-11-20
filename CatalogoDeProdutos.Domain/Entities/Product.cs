


namespace CatalogoDeProdutos.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public string Category { get; private set; }
        public string? ImageUrl { get; private set; }
        public ProductStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Construtor para o EF Core
        protected Product() { }

        // Construtor para criar um NOVO produto
        public Product(string name, string description, decimal price, string category, string? imageUrl)
        {
            ValidateDomain(name, description, price, category);

            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            ImageUrl = imageUrl;
            Status = ProductStatus.Active;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string description, decimal price, string category)
        {
            ValidateDomain(name, description, price, category);
            
            Name = name;
            Description = description;
            Price = price;
            Category = category;
        }

        public void UpdateImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL cannot be empty.");

            ImageUrl = imageUrl;
        }

        // Alterar status
        public void Activate() => Status = ProductStatus.Active;
        public void Deactivate() => Status = ProductStatus.Inactive;

        // Validação centralizada
        private static void ValidateDomain(string name, string description, decimal price, string category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");

            if (name.Length < 3)
                throw new ArgumentException("Name must be at least 3 characters long.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.");

            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category is required.");
        }
    }
}