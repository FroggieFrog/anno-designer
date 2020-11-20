﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using ColorPresetsDesigner.ViewModels;
using NLog;
using NLog.Targets;

namespace ColorPresetsDesigner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static App()
        {
            Trace.Listeners.Add(new NLogTraceListener());
        }

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");

            TaskScheduler.UnobservedTaskException += (s, e) =>
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");

            logger.Info($"program version: {Assembly.GetExecutingAssembly().GetName().Version}");
        }

        private void LogUnhandledException(Exception ex, string @event)
        {
            logger.Error(ex, @event);

            var message = "An unhandled exception occurred.";

            //find loaction of log file
            var fileTarget = LogManager.Configuration.FindTargetByName("MainLogger") as FileTarget;
            var logFile = fileTarget?.FileName.Render(new LogEventInfo());
            if (!string.IsNullOrWhiteSpace(logFile))
            {
                logFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), logFile);
                if (File.Exists(logFile))
                {
                    logFile = logFile.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                    message += $"{Environment.NewLine}{Environment.NewLine}Details in \"{logFile}\".";
                }
            }

            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            Environment.Exit(-1);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (ColorPresetsDesigner.Properties.Settings.Default.SettingsUpgradeNeeded)
            {
                logger.Trace("upgrade settings");

                ColorPresetsDesigner.Properties.Settings.Default.Upgrade();
                ColorPresetsDesigner.Properties.Settings.Default.SettingsUpgradeNeeded = false;
                ColorPresetsDesigner.Properties.Settings.Default.Save();
            }

            var mainVM = new MainWindowViewModel();

            MainWindow = new MainWindow();
            MainWindow.DataContext = mainVM;

            MainWindow.ShowDialog();
        }
    }
}
