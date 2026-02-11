using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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
            LoadSounds();
        }



        private void LoadDevices()
        {
            SoundOutDevices = new ObservableCollection<DirectSoundDeviceInfo>(SoundManager.GetOutDevices());
            DirectSoundDeviceInfo[] devices = _manager.GetCurrentDevices();
            FirstDeviceCbox.SelectedItem = devices[0] != null ? SoundOutDevices.FirstOrDefault(d => d.Guid == devices[0]!.Guid) : null;
            SecondDeviceCbox.SelectedItem = devices[1] != null ? SoundOutDevices.FirstOrDefault(d => d.Guid == devices[1]!.Guid) : null;
        }

        private void LoadSounds()
        {
            SoundsWPanel.Children.Clear();
            List<string[]> sounds = _manager.Sounds;
            for(int i=0; i < sounds.Count; i++)
            {

                int icopy = i;
                var button = new Button
                {
                    Width = 150,
                    Height = 150,
                    Margin = new Thickness(15),
                };
                if (sounds[i][1] != null)
                {
                    Task.Run(() =>
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(sounds[icopy][1]);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.DecodePixelWidth = 200;
                        bitmap.EndInit();
                        bitmap.Freeze();

                        Dispatcher.Invoke(() => 
                        { 
                            var img = new  ImageBrush(bitmap);
                            button.Background = img; 
                        });

                    });
                }
                else
                {
                    button.Content = System.IO.Path.GetFileNameWithoutExtension(sounds[i][0]);
                }

                button.Click += (object sender, RoutedEventArgs arg) => { _manager.PlaySound(icopy); };
                SoundsWPanel.Children.Add(button);
            }
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
            LoadSounds();
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
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private static ControlTemplate RemoveHoverEfect()
        {


            return null;
        }

    }
}