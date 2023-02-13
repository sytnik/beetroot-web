using AutoMapper;
using BlazorServerMud.DAO;

// ReSharper disable UnusedType.Global

namespace BlazorServerMud.Logic;

public static class Values
{
    public static readonly string ConnectionString = new SqlConnectionStringBuilder
    {
        DataSource = "127.0.0.1", InitialCatalog = "newdb",
        IntegratedSecurity = true, TrustServerCertificate = true
    }.ConnectionString;
    
    public static readonly string[] IndexTableHeaders =
        {"Id", "FirstName", "LastName", "Info", "Delete"};
}

public sealed class AppProfile : Profile
{
    public AppProfile()
    {
        CreateProjection<Department, DepartmentDto>();
        CreateProjection<User, UserDto>();
    }
}