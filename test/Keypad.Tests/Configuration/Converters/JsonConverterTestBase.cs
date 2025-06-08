using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TUnit.Assertions.Exceptions;

namespace Keypad.Tests.Configuration.Converters;

public abstract class JsonConverterTestBase<T>
{
    public abstract JsonConverter<T> Converter { get; }

    protected async Task AssertConversion(string text, T expected)
    {
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes($"\"{text}\""));
        reader.Read();

        var parsed = Converter.Read(ref reader, typeof(T), JsonSerializerOptions.Default);

        await Assert.That(parsed).IsEqualTo(expected).Because($"Parsing '{text}' should yield correct result");
    }
    
    protected void AssertConversionFails(string text)
    {
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes($"\"{text}\""));
        reader.Read();

        try
        {
            _ = Converter.Read(ref reader, typeof(T), JsonSerializerOptions.Default);
            Assert.Fail($"Expected conversion of '{text}' to fail");
        }
        catch (Exception e) when (e is not AssertionException)
        {
            // All good
        }
    }
}
