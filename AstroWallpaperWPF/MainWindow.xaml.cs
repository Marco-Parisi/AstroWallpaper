using AstroWallpaperUtils;
using AstroWallpaperUtils.Parser;
using System;
using System.ServiceProcess;
using System.Windows;

namespace AstroWallpaperWPF
{
    public partial class MainWindow : Window
    {
        private const string ServiceName = "AstroWallpaperService";

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            UpdateServiceStatus();
        }

        private void StartServiceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceController service = new ServiceController(ServiceName);
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    MessageBox.Show("Servizio avviato con successo!", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Il servizio è già in esecuzione.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'avvio del servizio: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            UpdateServiceStatus();
        }

        private void StopServiceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceController service = new ServiceController(ServiceName);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    MessageBox.Show("Servizio arrestato con successo!", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Il servizio è già fermo.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'arresto del servizio: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            UpdateServiceStatus();
        }

        private void UpdateServiceStatus()
        {
            try
            {
                ServiceController service = new ServiceController(ServiceName);
                ServiceStatusLabel.Text = $"Stato del Servizio: {service.Status}";
            }
            catch (Exception ex)
            {
                ServiceStatusLabel.Text = $"Errore nel recupero dello stato: {ex.Message}";
            }
        }

        private void LoadSettings()
        {
            apodEnabledCheckBox.IsChecked = SettingsManager.APODEnabled;
            hubbleEnabledCheckBox.IsChecked = SettingsManager.HubbleEnabled;
            intervalTextBox.Text = SettingsManager.IntervalMinutes.ToString();

            WallpaperService ws = new WallpaperService(100);
            
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.APODEnabled = apodEnabledCheckBox.IsChecked ?? false;
            SettingsManager.HubbleEnabled = hubbleEnabledCheckBox.IsChecked ?? false;
            SettingsManager.IntervalMinutes = int.Parse(intervalTextBox.Text);
            SettingsManager.SaveSettings();
        }
    }
}
