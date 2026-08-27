using Newtonsoft.Json;
using Stepan.Song;

namespace Stepan.Service;

public class LayoutService
{
    string path;
    private readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto
    };

    public string ReadLayout(string name)
    {

        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanPattern", name);
        #if DEBUG
            path = Path.Combine(Directory.GetCurrentDirectory(), "Stepan", "StepanPattern", name);
            #endif

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "Stepan Player");
        }

        string header = File.ReadAllText(path);
        return header;
    }

    public void SaveLayout(string name)
    {
        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanPattern", name);
        #if DEBUG
            path = Path.Combine(Directory.GetCurrentDirectory(), "Stepan", "StepanPattern", name);
            #endif
            
        File.WriteAllText(path, "Stepan Player");
    }
}