namespace CityNexus.Modulith.Domain.Modules.Shared.VO;

public enum DocumentType
{
    Cpf = 1,
}

public sealed record Document(string Value)
{
    public static Document Create(string value)
    {
        if (!IsAValidCpf(value))
        {
            throw new Exception("The CPF is invalid");
        }
        var document = new Document(value.Trim().Replace(".", "").Replace("-", ""));
        return document;
    }

    private static bool IsAValidCpf(string? cpf)
    {
        if (string.IsNullOrEmpty(cpf))
            return false;
        int[] multiplier = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] secondMultiplier = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];
        cpf = cpf.Trim().Replace(".", "").Replace("-", "");
        if (cpf.Length != 11)
            return false;
        var tempCpf = cpf[..9];
        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += int.Parse(tempCpf[i].ToString()) * multiplier[i];
        var rest = sum % 11;
        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;
        var digit = rest.ToString();
        tempCpf += digit;
        sum = 0;
        for (var i = 0; i < 10; i++)
            sum += int.Parse(tempCpf[i].ToString()) * secondMultiplier[i];
        rest = sum % 11;
        if (rest < 2)
        {
            rest = 0;
        }
        else
        {
            rest = 11 - rest;
        }
        digit += rest;
        return cpf.EndsWith(digit);
    }
};
