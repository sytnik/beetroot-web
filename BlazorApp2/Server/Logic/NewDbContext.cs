namespace BlazorApp2.Server.Logic;

public class NewDbContext : DbContext
{
    public DbSet<Department> Department { get; set; }
    public DbSet<DetailsInfo> DetailsInfo { get; set; }
    public DbSet<Manager> Manager { get; set; }
    public DbSet<User> Users { get; set; }

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
    }
}