using Newtonsoft.Json;
using Stepan.Song;

namespace Stepan.Service;

public class LayoutService
{
    private readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto
    };

    public string ReadLayout(string name)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanPattern", name);
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "Stepan Player");
        }

        string header = File.ReadAllText(path);
        return header;
    }

    public void SaveLayout(string name)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanPattern", name);
        File.WriteAllText(path, "Stepan Player");
    }
}