using System.Runtime.InteropServices;
using System.Text;

namespace LifeMonitor;

public class WindowApi
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    
    public static ActiveWindowModel? GetActiveWindowTitle()
    {
        const int nChars = 256;
        StringBuilder buff = new(nChars);
        IntPtr handle = GetForegroundWindow();
        Console.WriteLine(handle);

        if (GetWindowText(handle, buff, nChars) > 0)
        {
            return ActiveWindowModel.Create(handle,buff.ToString());
        }

        return null;
    }
}