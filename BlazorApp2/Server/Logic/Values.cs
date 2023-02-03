using System.Text.Encodings.Web;
using System.Text.Unicode;
using AutoMapper;

// ReSharper disable UnusedType.Global

namespace BlazorApp2.Server.Logic;

public static class Values
{
    public static readonly string ConnectionString = new SqlConnectionStringBuilder
    {
        DataSource = "127.0.0.1", InitialCatalog = "newdb",
        IntegratedSecurity = true, TrustServerCertificate = true
    }.ConnectionString;

    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    // public static readonly MapperConfiguration MapCfg = new(cfg =>
    // {
    //     cfg.CreateMap<Department, DepartmentDto>();
    //     cfg.CreateMap<User, UserDto>();
    // });
}

public sealed class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateProjection<Department, DepartmentDto>();
        CreateProjection<User, UserDto>();
    }
}