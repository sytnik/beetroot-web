using System.ComponentModel.DataAnnotations;

namespace BlazorServerMud.Logic;

public record UserModel
{
    [Key] public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Info { get; set; }
    public int? DepartmentId { get; set; }
}