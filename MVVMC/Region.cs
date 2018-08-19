using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            _navigationService = MVVMCNavigationService.GetInstance();
            this.Loaded += OnRegionLoaded;
            this.Unloaded += OnRegionUnloaded;
        }

        private void OnRegionUnloaded(object sender, RoutedEventArgs e)
        {
            bool shouldCancel = _navigationService.GetController(ControllerID).CallOnLeavingNavigation(false);
            if (shouldCancel)
            {
                throw new InvalidOperationException("Can't cancel navigation from 'OnLeave' when higher-level controller is the one navigating. " +
                                                    "Check value 'CancellingNavigationAllowed' property in LeavingPageEventArgs.");
            }
            _navigationService.RemoveController(ControllerID);
            _navigationService.RemoveRegion(this);
        }

        private void OnRegionLoaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ControllerID))
            {
                throw new NullReferenceException($"A loaded Region doesn't have 'ControllerID' set to anything");
            }

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
