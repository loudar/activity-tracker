namespace LifeMonitor;

public class ActiveWindowModel
{
    public IntPtr WindowHandle { get; private set; }
    public string WindowTitle { get; private set; }

    private ActiveWindowModel(IntPtr windowHandle, string windowTitle)
    {
        this.WindowHandle = windowHandle;
        this.WindowTitle = windowTitle;
    }

    public void IsDifferentThan(ActiveWindowModel otherActiveWindow, Action onDifferent)
    {
        if (otherActiveWindow.WindowHandle == this.WindowHandle) return;
        onDifferent();
    }

    public static ActiveWindowModel Create(IntPtr windowHandle, string windowTitle) => new(windowHandle, windowTitle);

    public static ActiveWindowModel CreateFrom(ActiveWindowModel activeWindow) =>
        new(activeWindow.WindowHandle, activeWindow.WindowTitle);

    public static ActiveWindowModel CreateEmpty() => new(IntPtr.Zero, string.Empty);
}