namespace BlazorApp2.Server.Logic;

public class NewDbContext : DbContext
{
    public DbSet<Department> Department { get; set; }
    public DbSet<DetailsInfo> DetailsInfo { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<Purchase> Purchase { get; set; }
    public DbSet<PurchaseProduct> PurchaseProduct { get; set; }
    public DbSet<Manager> Manager { get; set; }
    public DbSet<User> User { get; set; }

    public NewDbContext(DbContextOptions<NewDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(user => user.Details)
            .WithOne(detailsInfo => detailsInfo.User)
            .HasForeignKey<DetailsInfo>(detailsInfo => detailsInfo.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(user => user.Department)
            .WithMany(department => department.Users)
            .HasForeignKey(user => user.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Purchase>()
            .HasOne(purchase => purchase.Manager)
            .WithMany(manager => manager.Purchases)
            .HasForeignKey(purchase => purchase.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Product>()
            .HasMany(product => product.Purchases)
            .WithMany(purchase => purchase.Products)
            .UsingEntity<PurchaseProduct>(
                entityTypeBuilder => entityTypeBuilder
                    .HasOne(purchaseProduct => purchaseProduct.Purchase)
                    .WithMany(purchase => purchase.PurchaseProducts)
                    .HasForeignKey(bookAuthor => bookAuthor.PurchaseId)
                    .OnDelete(DeleteBehavior.Restrict),
                entityTypeBuilder => entityTypeBuilder
                    .HasOne(purchaseProduct => purchaseProduct.Product)
                    .WithMany(product => product.PurchaseProducts)
                    .HasForeignKey(product => product.ProductId)
                    .OnDelete(DeleteBehavior.Restrict),
                entityTypeBuilder => entityTypeBuilder
                    .HasKey(bookAuthor => new { bookAuthor.PurchaseId, bookAuthor.ProductId }));
    }
}