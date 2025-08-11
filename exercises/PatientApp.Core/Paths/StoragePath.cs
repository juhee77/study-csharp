namespace PatientApp.Core.Paths;

public static class StoragePath
{
    public static string ResolveDefault()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dir = Path.Combine(appData, "PatientApp");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return Path.Combine(dir, "patients.json");
    }
}


