using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Reactive.Linq;
using ReactiveUI;
using Predictive.Lsdyna.Mpp.ViewModels;

namespace Predictive.Lsdyna.Mpp.Views
{
    /// <summary>
    /// Interaction logic for AdvancedOptionsView.xaml
    /// </summary>
    public partial class AdvancedOptionsFlyout : Flyout, IViewFor<AdvancedOptionsViewModel>
    {
        public AdvancedOptionsFlyout()
        {

            ViewModel = new AdvancedOptionsViewModel();
            DataContext = ViewModel;
            InitializeComponent();

            this.OutputExpander.Expanded += Toggle_Expander;
            this.InputExpander.Expanded += Toggle_Expander;

            // Data binding
            this.Bind(ViewModel, vm => vm.InterfaceSegment.Value, v => v.InterfaceSegment.Text);
            this.Bind(ViewModel, vm => vm.VdaGeometry.Value, v => v.VdaGeometry.Text);
            this.Bind(ViewModel, vm => vm.Cal3dInput.Value, v => v.CAL3DInput.Text);
            this.Bind(ViewModel, vm => vm.Topaz3dfile.Value, v => v.TOPAZ3DFile.Text);
            this.Bind(ViewModel, vm => vm.StressInitialization.Value, v => v.StressInitialization.Text);
            this.Bind(ViewModel, vm => vm.MappingInputFile.Value, v => v.MappingInputFile.Text);
            this.Bind(ViewModel, vm => vm.Graphics.Value, v => v.Graphics.Text);
            this.Bind(ViewModel, vm => vm.TimeHistories.Value, v => v.TimeHistories.Text);
            this.Bind(ViewModel, vm => vm.InterfaceForce.Value, v => v.InterfaceForce.Text);
            this.Bind(ViewModel, vm => vm.FsiInterfaceForce.Value, v => v.FSIInterfaceForce.Text);
            this.Bind(ViewModel, vm => vm.DynamicRelaxation.Value, v => v.DynamicRelaxation.Text);
            this.Bind(ViewModel, vm => vm.AcousticOuput.Value, v => v.AcousticOutput.Text);
            this.Bind(ViewModel, vm => vm.DemInterfaceForce.Value, v => v.DEMInterfaceForce.Text);
            this.Bind(ViewModel, vm => vm.InputEcho.Value, v => v.InputEcho.Text);
            this.Bind(ViewModel, vm => vm.RestartDump.Value, v => v.RestartDump.Text);
            this.Bind(ViewModel, vm => vm.InterfaceSegmentSave.Value, v => v.InterfaceSegmentSave.Text);
            this.Bind(ViewModel, vm => vm.RemapCrackDatabase.Value, v => v.RemapCrackDatabase.Text);
            this.Bind(ViewModel, vm => vm.RunningRestartDump.Value, v => v.RunningRestartDump.Text);
            this.Bind(ViewModel, vm => vm.PropertyOutput.Value, v => v.PropertyOutput.Text);
            this.Bind(ViewModel, vm => vm.MappingOutputFile.Value, v => v.MappingOutputFile.Text);
        }

        public AdvancedOptionsViewModel ViewModel
        {
            get { return (AdvancedOptionsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(AdvancedOptionsViewModel), typeof(AdvancedOptionsFlyout), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (AdvancedOptionsViewModel)value; }
        }

        private void Toggle_Expander(object sender, RoutedEventArgs e)
        {
            var expander = sender as Expander;
            if (expander == this.InputExpander)
            {
                this.OutputExpander.IsExpanded = false;
            }
            else {
                this.InputExpander.IsExpanded = false;
            }
        }
    }
}

