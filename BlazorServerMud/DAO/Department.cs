using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServerMud.DAO;

// dbo object
public record Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Data { get; set; }
    public string SomeOtherData { get; set; }
    public List<User> Users { get; set; }
    
    [NotMapped] public int UserCounter { get; set; }
}

// dto
public record DepartmentDto(string Name, List<UserDto> Users);