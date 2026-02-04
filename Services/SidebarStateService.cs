namespace S7Dashboard.Services;

public class SidebarStateService
{
    public bool IsCollapsed { get; private set; } = false;

    public event Action? OnChange;

    public void Toggle()
    {
        IsCollapsed = !IsCollapsed;
        OnChange?.Invoke();
    }
}
