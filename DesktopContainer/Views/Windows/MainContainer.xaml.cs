using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DesktopContainer.Services;
using DesktopContainer.Utils;
using DesktopContainer.Utils.StaticHelpers;

namespace DesktopContainer.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainContainer.xaml
    /// </summary>
    public partial class MainContainer : Window
    {
        private readonly WindowPositionService _positionService; 
        private readonly AcrylicBackgroundService _acrylicBackgroundServiceService;

        public MainContainer()
        {
            InitializeComponent();
            var handle = WindowHandleWorker.GetWindowHandler(this);

            _positionService = new WindowPositionService(handle);
            _acrylicBackgroundServiceService = new AcrylicBackgroundService(handle);
        }

        private void MainContainer_OnActivated(object sender, EventArgs e)
        {
            _positionService.SetWindowUndermost();
        }

        private void MainContainer_OnLoaded(object sender, RoutedEventArgs e)
        {
            _acrylicBackgroundServiceService.Windows10EnableBlurBehind();
        }

        private void IconsContainer_OnDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = ((string[]) e.Data.GetData(DataFormats.FileDrop));
                for (var i = 0; i < files.Length; i++)
                {
                    IconsContainer.Items.Add(new Image {Source = FileIconWorker.GetThumbnail(files[i]), MaxWidth = 64});
                }
            }
        }
    }
}
