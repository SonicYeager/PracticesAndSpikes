using System.Text.RegularExpressions;

namespace Codewars.Training.RegexPinValidate;

public static class Kata
{
    public static bool ValidatePin(string pin)
    {
        if (pin.Length != 4 && pin.Length != 6)
        {
            return false;
        }

        return Regex.IsMatch(pin, @"^\d{4}$|^\d{6}$");
    }
}