namespace Tervisipaevik_Daria_Valeria
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var exception = e.ExceptionObject as Exception;
                WriteLog($"Unhandled exception: {exception?.Message}\nStackTrace: {exception?.StackTrace}");
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Current.MainPage.DisplayAlert("Необработанная ошибка",
                        $"Сообщение: {exception?.Message}\nStackTrace: {exception?.StackTrace}", "OK");
                });
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                var exception = e.Exception;
                WriteLog($"Unobserved task exception: {exception?.Message}\nStackTrace: {exception?.StackTrace}");
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Current.MainPage.DisplayAlert("Асинхронная ошибка",
                        $"Сообщение: {exception?.Message}\nStackTrace: {exception?.StackTrace}", "OK");
                });
                e.SetObserved();
            };
        }

        private void WriteLog(string message)
        {
            string logPath = Path.Combine(FileSystem.CacheDirectory, "app_log.txt");
            File.AppendAllText(logPath, $"{DateTime.Now}: {message}\n");
        }
    }
}
