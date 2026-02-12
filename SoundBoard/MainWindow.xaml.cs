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
            Width = 1600;
            Height = 900;
            string sizeStr = Properties.Settings.Default.WindowSize;
            if(sizeStr != "" && sizeStr != null)
            {
                double[] size = sizeStr.Split("x").Select(double.Parse).ToArray();
                Width = size[0];
                Height = size[1];
            }
            else
            {
                Window_SizeChanged(this, null);
            }
            


            DirectoryPath = Properties.Settings.Default.DirectoryPath;
            SoundOutDevices = new();
            if (DirectoryPath == "" || DirectoryPath == null)
            {
                _manager = new SoundManager();
            }
            else
            {
                try
                {
                    _manager = new SoundManager(DirectoryPath);
                }
                catch (Exception ex)
                {
                    _manager = new SoundManager();
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            string outDeviceGuid = Properties.Settings.Default.FirstOutDevice;
            if (outDeviceGuid != "" && outDeviceGuid != null)
            {
                try
                {
                    _manager.ChangeDevice(Guid.Parse(outDeviceGuid));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            outDeviceGuid = Properties.Settings.Default.SecondOutDevice;
            if (outDeviceGuid != "" && outDeviceGuid != null)
            {
                try
                {
                    _manager.ChangeDevice(Guid.Parse(outDeviceGuid), true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            InitializeComponent();
            LoadDevices();
            LoadSounds();
            DataContext = this;

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
            ControlTemplate btnTemplate = SoundBtnTemplate();
            for (int i = 0; i < sounds.Count; i++)
            {

                int icopy = i;
                var button = new Button
                {
                    Width = 150,
                    Height = 150,
                    Margin = new Thickness(15),
                    BorderBrush = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(2),
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
                    string sname = System.IO.Path.GetFileNameWithoutExtension(sounds[i][0]);
                    button.Content = new TextBlock
                    {
                        Text = sname,
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Center,
                    };
                }

                button.Click += (object sender, RoutedEventArgs arg) => { _manager.PlaySound(icopy); };
                button.Template = btnTemplate;
                button.Cursor = Cursors.Hand;
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
                OnPropertyChanged("DirectoryPath");
                LoadSounds();
                Properties.Settings.Default.DirectoryPath = DirectoryPath;
                Properties.Settings.Default.Save();

            }

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
                _manager.StopAll();
                Guid deviceGuid = ((DirectSoundDeviceInfo)FirstDeviceCbox.SelectedItem).Guid;
                _manager.ChangeDevice(deviceGuid);
                Properties.Settings.Default.FirstOutDevice = deviceGuid.ToString();
                Properties.Settings.Default.Save();
            }


        }

        private void SecondDeviceCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SecondDeviceCbox.SelectedItem != null)
            {
                _manager.StopAll();
                Guid deviceGuid = ((DirectSoundDeviceInfo)SecondDeviceCbox.SelectedItem).Guid;
                _manager.ChangeDevice(deviceGuid, true);
                Properties.Settings.Default.SecondOutDevice = deviceGuid.ToString();
                Properties.Settings.Default.Save();
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private ControlTemplate SoundBtnTemplate()
        {
            var template = new ControlTemplate(typeof(Button));

            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
            border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));

            var content = new FrameworkElementFactory(typeof(ContentPresenter));
            content.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            content.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(content);
            template.VisualTree = border;

            return template;

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            string size = $"{Width}x{Height}";
            Properties.Settings.Default.WindowSize = size;
            Properties.Settings.Default.Save();
        }
    }
}