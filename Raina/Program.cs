using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Raina
{
    public static class Program
    {
        // ReSharper disable once InconsistentNaming
        [STAThread]
        public static void Main(string[] args)
        {
            NativeWindowSettings nativeWindowSettings = new()
            {
                Size = new Vector2i(1152, 720),
                Title = "Raina",
                Flags = ContextFlags.ForwardCompatible
            };

            using Raina window = new(GameWindowSettings.Default, nativeWindowSettings);
            window.Run();
        }
    }
}