using Stepan.Service;

namespace Stepan.Controller;

public class LayoutController
{
    private readonly LayoutFactory _factoryService = new();
    private readonly LayoutService _layoutService = new();
    public string ReadDefaultLayout()
    {
        return _factoryService.ReadLayout(LayoutTypeFactory.Default);
    }

    public string ReadMinimaltLayout()
    {
        return _factoryService.ReadLayout(LayoutTypeFactory.Minimal);
    }

    public string ReadLayout(string name)
    {
        return _layoutService.ReadLayout(name);
    }
}