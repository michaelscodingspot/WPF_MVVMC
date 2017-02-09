using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MVVMC
{
    public class Region : ContentControl
    {
        static Region()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Region), new FrameworkPropertyMetadata(typeof(Region)));
        }

        private MVVMCNavigationService _navigationService;
        public Region()
        {
            _navigationService = MVVMCNavigationService.GetInstance();
            this.Loaded += OnRegionLoaded;
            this.Unloaded += OnRegionUnloaded;
        }

        private void OnRegionUnloaded(object sender, RoutedEventArgs e)
        {
            _navigationService.RemoveController(ControllerID);
            _navigationService.RemoveRegion(this);
        }

        private void OnRegionLoaded(object sender, RoutedEventArgs e)
        {
            _navigationService.CreateAndAddController(ControllerID);
            _navigationService.AddRegion(this);
            _navigationService.GetController(ControllerID).NavigateToInitial();
        }

        public string ControllerID
        {
            get { return (string)GetValue(ControllerIDProperty); }
            set { SetValue(ControllerIDProperty, value); }
        }
        public static readonly DependencyProperty ControllerIDProperty =
            DependencyProperty.Register("ControllerID", typeof(string), typeof(Region), new PropertyMetadata(null));

    }
}
