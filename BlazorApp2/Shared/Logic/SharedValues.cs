using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApp2.Shared.Logic;

public static class SharedValues
{
    public static readonly string[] IndexTableHeaders =
        {"Id", "FirstName", "LastName", "Info", "Delete"};
    
    public static JsonSerializerOptions SerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
}