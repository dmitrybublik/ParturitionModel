using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ParturitionModel.Core;

namespace ParturitionModel.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ObservableCollection<SimulationEventArgs> _items =
            new ObservableCollection<SimulationEventArgs>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ((Button) sender).IsEnabled = false;

            try
            {
                _items.Clear();

                var simulation = new Simulator();
                simulation.SimulationEvent += Simulation_SimulationEvent;
                await simulation.StartSimulationAsync();
                simulation.SimulationEvent -= Simulation_SimulationEvent;
            }
            finally
            {
                ((Button)sender).IsEnabled = true;
            }
        }

        public IEnumerable<SimulationEventArgs> Items

        {
            get { return _items; }
        }

        private async void Simulation_SimulationEvent(object sender, SimulationEventArgs e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                _items.Add(e);
            });

        }
    }
}
