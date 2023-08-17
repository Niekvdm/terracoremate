using System.Text;

namespace TerracoreMate.Extensions;

/// <summary>
/// This static class provides extensions for string type.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Transforms a string to snake case format.
    /// </summary>
    /// <param name="input">The string to be transformed.</param>
    /// <returns>Snake case formatted string.</returns>
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var snakeCase = new StringBuilder();
        snakeCase.Append(char.ToLower(input[0]));

        for (var i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                snakeCase.Append('_');
                snakeCase.Append(char.ToLower(input[i]));
            }
            else
            {
                snakeCase.Append(input[i]);
            }
        }

        return snakeCase.ToString();
    }
}