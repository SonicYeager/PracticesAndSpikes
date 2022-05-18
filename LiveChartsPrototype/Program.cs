using Avalonia;
using Avalonia.Controls;

namespace Prototype
{
    static class Program
    {
        public static MainUI mainUI;
        public static void Main(string[] args)
        {
            PrototypeApp.Init(args, AppInit);
        }

        private static Window AppInit()
        {
            //init controller here
            mainUI = new MainUI();
            return mainUI;
        }
    }
}