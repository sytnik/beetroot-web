namespace BlazorApp2.Shared.DAO;

public record User : UserModel
{
    public DetailsInfo Details { get; set; }
    public Department Department { get; set; }
    [NotMapped] public string FullName => $"{FirstName} {LastName}";

    public User()
    {
    }

    public User(UserModel model)
    {
        Id = model.Id;
        FirstName = model.FirstName;
        LastName = model.LastName;
        Info = model.Info;
        DepartmentId = model.DepartmentId;
    }
}