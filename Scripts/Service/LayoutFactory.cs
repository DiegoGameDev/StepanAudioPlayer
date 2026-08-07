namespace Stepan.Service;

public class LayoutFactory
{
    private  readonly SongListService _songListService = new();
    private readonly LayoutService _layoutService = new();

    public string ReadLayout(LayoutTypeFactory layoutTypeFactory)
    {
        switch (layoutTypeFactory)
        {
            case LayoutTypeFactory.Minimal:
                return _layoutService.ReadLayout("minimalLayout.STP");

            default:
                return _layoutService.ReadLayout("defaultLayout.STP");
        }
    }
}

public enum LayoutTypeFactory {Default, Minimal, SongListEdit}