using System.Net.Mail;
using CityNexus.Modulith.SharedKernel.Exceptions;

namespace CityNexus.Modulith.SharedKernel.VO;

public sealed record Email(string Value)
{
    public static implicit operator string(Email email) => email.Value;

    public static Email Create(string value)
    {
        try
        {
            new MailAddress(value);
            var email = new Email(value.Trim().ToLower());
            return email;
        }
        catch
        {
            throw new AppException("Invalid email address.");
        }
    }
};
