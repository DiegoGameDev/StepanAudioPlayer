namespace Stepan.Song;

internal class SongData
{
    public Dictionary<Guid, string> FilePathByGuid = new();

    public void AddFilePath(string filePath)
    {
        FilePathByGuid.Add(new Guid(), filePath);
    }

    public void AddFilePaths(string[] files)
    {
        if (files == null)
            return;
        if (files.Length == 0)
            return;

        foreach(var i in files)
        {
            FilePathByGuid.TryAdd(new Guid(), i);
        }
    }
}