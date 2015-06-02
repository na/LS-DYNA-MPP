using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ReactiveUI;
using Predictive.Lsdyna.Mpp.ViewModels;

namespace Predictive.Lsdyna.Mpp.Views
{
    /// <summary>
    /// Interaction logic for AdvancedOptionsView.xaml
    /// </summary>
    public partial class AdvancedOptionsFlyout : Flyout
    {
        public AdvancedOptionsFlyout()
        {
            InitializeComponent();
            AdvancedOptionsStackPanel.DataContext = new AdvancedOptionsViewModel();
        }
    }

    public class ExpanderToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (System.Convert.ToBoolean(value)) return parameter;
            return null;
        }
    }
}
