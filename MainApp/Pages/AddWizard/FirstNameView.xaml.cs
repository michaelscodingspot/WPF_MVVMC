﻿using System;
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

namespace MainApp.Pages.AddWizard
{
    /// <summary>
    /// Interaction logic for Step1_FirstNameView.xaml
    /// </summary>
    public partial class FirstNameView : UserControl
    {
        public FirstNameView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            FirstName.Focus();
        }
    }
}
