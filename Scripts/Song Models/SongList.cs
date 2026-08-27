namespace Stepan.Song
{
    public class SongList
    {
        public string SongListName = "Default";
        //public List<string> songName = new List<string>();
        public List<string> filePath = new List<string>();

        public List<string> SongNames()
        {
            return filePath.Select(x => Path.GetFileName(x)).ToList();
        }
        
        public SongList()
        {
            
        }

        public SongList(SongList songList)
        {
            SongListName = songList.SongListName;
            filePath = new List<string>(songList.filePath);
        }
    }
}