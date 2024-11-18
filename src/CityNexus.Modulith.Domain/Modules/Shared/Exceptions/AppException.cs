namespace CityNexus.Modulith.Domain.Modules.Shared.Exceptions;

public class AppException(string message, int statusCode = 400) : Exception(message)
{
    public int StatusCode { get; set; } = statusCode;
}