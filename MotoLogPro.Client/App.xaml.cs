namespace MotoLogPro.Client
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        // Override CreateWindow only once and add window resizing logic here
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

#if WINDOWS
            const int newWidth = 400;
            const int newHeight = 700;
            window.Width = newWidth;
            window.Height = newHeight;
#endif

            return window;
        }
    }
}