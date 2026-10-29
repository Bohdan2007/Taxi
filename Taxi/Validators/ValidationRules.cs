using System.Text.RegularExpressions;

namespace Taxi.Validators;

public static class ValidationRules
{
    public const int MinCarYear = 2000;
    
    private static readonly Regex PersonNameRegex =
        new(@"^\p{L}+(?:[ '’ʼ\-]\p{L}+)*$", RegexOptions.Compiled);

    public static bool IsValidPersonName(string? value) => !string.IsNullOrWhiteSpace(value) && PersonNameRegex.IsMatch(value.Trim());
}