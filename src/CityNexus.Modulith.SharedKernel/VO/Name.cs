using CityNexus.Modulith.SharedKernel.Exceptions;

namespace CityNexus.Modulith.SharedKernel.VO;

public sealed record Name(string Value)
{
    public override string ToString() => this.Value;

    public static Name From(string value)
    {
        var results = value.Split(" ").Select(n => $"{n[0].ToString().ToUpper()}{n[1..]}").ToList();
        if (results.Count < 2)
        {
            throw new AppException($"The name must have at least 2 words.");
        }
        return new Name(string.Join(' ', results));
    }
};
