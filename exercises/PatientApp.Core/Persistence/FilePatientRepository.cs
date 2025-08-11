using System.Text.Json;
using System.Text.Json.Serialization;
using PatientApp.Core.Models;

namespace PatientApp.Core.Persistence;

public sealed class FilePatientRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new DateOnlyJsonConverter() }
    };

    public FilePatientRepository(string filePath)
    {
        _filePath = filePath;
        EnsureStorage();
        Console.WriteLine($"[PatientRepo] Storage file: {_filePath}");
    }

    public List<Patient> GetAll()
    {
        var list = Load();
        Console.WriteLine($"[PatientRepo] GetAll -> {list.Count} record(s) before sort");
        return list
            .OrderBy(p => p.Name)
            .ThenBy(p => p.BirthDate)
            .ToList();
    }

    public Patient? GetById(Guid id)
    {
        return Load().FirstOrDefault(p => p.Id == id);
    }

    public List<Patient> SearchByName(string keyword)
    {
        keyword = keyword.Trim();
        if (keyword.Length == 0) return new List<Patient>();
        var result = Load()
            .Where(p => p.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
            .OrderBy(p => p.Name)
            .ToList();
        Console.WriteLine($"[PatientRepo] SearchByName('{keyword}') -> {result.Count} record(s)");
        return result;
    }

    public void Add(Patient patient)
    {
        var list = Load();
        list.Add(patient);
        Save(list);
        Console.WriteLine($"[PatientRepo] Added: {patient.Id} | {patient.Name} | {patient.BirthDate:yyyy-MM-dd} | {patient.Gender}");
    }

    private void EnsureStorage()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        if (!File.Exists(_filePath))
        {
            Save(new List<Patient>());
            Console.WriteLine($"[PatientRepo] Created new storage at {_filePath}");
        }
    }

    private List<Patient> Load()
    {
        try
        {
            using var stream = File.OpenRead(_filePath);
            var list = JsonSerializer.Deserialize<List<Patient>>(stream, _jsonOptions);
            return list ?? new List<Patient>();
        }
        catch
        {
            return new List<Patient>();
        }
    }

    private void Save(List<Patient> list)
    {
        using var stream = File.Create(_filePath);
        JsonSerializer.Serialize(stream, list, _jsonOptions);
        Console.WriteLine($"[PatientRepo] Saved {list.Count} record(s) to {_filePath}");
    }
}

internal sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (str is null) return default;
        if (DateOnly.TryParseExact(str, Format, out var date))
        {
            return date;
        }
        return default;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}


