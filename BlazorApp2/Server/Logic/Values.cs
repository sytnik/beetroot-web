using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;

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
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
}