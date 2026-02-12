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

            // --- CONFIGURAZIONE SPECIFICA PER WINDOWS ---
#if WINDOWS
            // Diamo una dimensione dignitosa da applicazione Desktop
            window.Width = 1200;
            window.Height = 800;

            // Opzionale: Possiamo anche impostare un minimo per evitare che la stringano troppo
            window.MinimumWidth = 800;
            window.MinimumHeight = 600;
#endif
            // ---------------------------------------------
            return window;
        }
    }
}