using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using DesktopContainer.Services;

namespace DesktopContainer.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainContainer.xaml
    /// </summary>
    public partial class MainContainer : Window
    {
        private readonly ContainerWindowPositionService _positionService; 

        public MainContainer()
        {
            InitializeComponent();

            _positionService = new ContainerWindowPositionService(this);
        }

        private void MainContainer_OnActivated(object sender, EventArgs e)
        {
            _positionService.SetWindowUndermost();
        }
    }
}
