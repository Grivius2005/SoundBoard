using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using NAudio.Wave;
using SBClassLib;

namespace SoundBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private SoundManager _manager;
        public string DirectoryPath { get; set; }
        public ObservableCollection<DirectSoundDeviceInfo> SoundOutDevices { get; set; }

        public MainWindow()
        {
            DirectoryPath = AppContext.BaseDirectory + @"TestDir";
            SoundOutDevices = new();
            _manager = new SoundManager(DirectoryPath);
            InitializeComponent();
            this.DataContext = this;
            LoadDevices();
        }



        private void LoadDevices()
        {
            SoundOutDevices = new ObservableCollection<DirectSoundDeviceInfo>(SoundManager.GetOutDevices());
            DirectSoundDeviceInfo?[] devices = _manager.GetCurrentDevices();
            FirstDeviceCbox.SelectedItem = devices[0] != null ? SoundOutDevices.FirstOrDefault(d => d.Guid == devices[0]!.Guid) : null;
            SecondDeviceCbox.SelectedItem = devices[1] != null ? SoundOutDevices.FirstOrDefault(d => d.Guid == devices[1]!.Guid) : null;
        }
        private void LoadDirBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog ofd = new OpenFolderDialog();
            ofd.InitialDirectory = DirectoryPath;
            if(ofd.ShowDialog() == true)
            {
                DirectoryPath = ofd.FolderName;
                _manager.ChangeDirectory(DirectoryPath);
            }
            OnPropertyChanged("DirectoryPath");
        }

        private void RefreshDevicesBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadDevices();
        }

        private void StopSoundBtn_Click(object sender, RoutedEventArgs e)
        {
            _manager.StopAll();
        }

        private void FirstDeviceCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FirstDeviceCbox.SelectedItem != null)
            {
                _manager.ChangeDevice(((DirectSoundDeviceInfo)FirstDeviceCbox.SelectedItem).Guid);
            }

        }

        private void SecondDeviceCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SecondDeviceCbox.SelectedItem != null)
            {
                _manager.ChangeDevice(((DirectSoundDeviceInfo)SecondDeviceCbox.SelectedItem).Guid, true);
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}