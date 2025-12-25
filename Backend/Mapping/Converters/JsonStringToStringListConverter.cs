using AutoMapper;
using System.Text.Json;

namespace Backend.Mapping.Converters;
/// <summary>
/// Custom AutoMapper converter for deserializing JSON strings to List&lt;string&gt;.
/// Used when mapping from Entity (JSON string storage) to DTO (List&lt;string&gt;).
/// Handles null or empty source strings gracefully by returning an empty list.
/// </summary>
public class JsonStringToStringListConverter : IValueConverter<string, IList<string>>
{
    /// <summary>
    /// Converts a JSON string to a list of strings.
    /// </summary>
    /// <param name="sourceMember">The source JSON string.</param>
    /// <param name="context">The resolution context.</param>
    /// <returns>A list of strings deserialized from the JSON string, or an empty list if source is null/empty.</returns>
    public IList<string> Convert(string sourceMember, ResolutionContext context)
    {
        return string.IsNullOrEmpty(sourceMember) ? [] : JsonSerializer.Deserialize<IList<string>>(sourceMember) ?? [];
    }
}