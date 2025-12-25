using AutoMapper;
using System.Text.Json;

namespace Backend.Mapping.Converters;

/// <summary>
/// Custom AutoMapper converter for serializing List&lt;string&gt; to JSON strings.
/// Used when mapping from DTO (List&lt;string&gt;) to Entity (JSON string storage).
/// Handles null or empty lists by returning an empty string.
/// </summary>
public class StringListToJsonStringConverter : IValueConverter<IList<string>, string>
{
    /// <summary>
    /// Converts a list of strings to a JSON string.
    /// </summary>
    /// <param name="sourceMember">The source list of strings.</param>
    /// <param name="context">The resolution context.</param>
    /// <returns>A JSON string representation of the list, or an empty string if source is null/empty.</returns>
    public string Convert(IList<string> sourceMember, ResolutionContext context)
    {
        return sourceMember is null || !sourceMember.Any() ? string.Empty : JsonSerializer.Serialize(sourceMember);
    }
}