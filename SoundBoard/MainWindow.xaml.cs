using System.Collections.ObjectModel;
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
using NAudio.Wave;
using SBClassLib;

namespace SoundBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SoundManager _manager;
        public string DirectoryPath { get; set; }
        public ObservableCollection<DirectSoundDeviceInfo> SoundOutDevices { get; set; }

        public MainWindow()
        {

            _manager = new SoundManager();
            InitializeComponent();
            this.DataContext = this;
            LoadDevices();
        }



        private void LoadDevices()
        {
            SoundOutDevices = new ObservableCollection<DirectSoundDeviceInfo>(SoundManager.GetOutDevices());
            FirstDeviceCbox.ItemsSource = SoundOutDevices;
            SecondDeviceCbox.ItemsSource = SoundOutDevices;



        }
    }
}