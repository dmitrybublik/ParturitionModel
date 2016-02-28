using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
        const string FileName = "simulation.csv";

        private readonly ObservableCollection<DataViewModel> _items =
            new ObservableCollection<DataViewModel>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Year;\tCount;\tChild Death");
            File.AppendAllText(FileName, sb.ToString());

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

        private string BornInfosToString(IEnumerable<BornInfo> bornInfos)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var info in bornInfos)
            {
                sb.AppendFormat("#{0} Age: {1}; Death: {2:P2}", info.Order, info.MotherAge, info.Factor);
            }

            return sb.ToString();
        }

        private void SaveDataToFile(DataViewModel data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0};\t{1};\t{2}", data.Year, data.Population, BornInfosToString(data.BornInfos));
            sb.AppendLine();

            File.AppendAllText(FileName, sb.ToString());
        }

        public IEnumerable<DataViewModel> Items

        {
            get { return _items; }
        }

        private async void Simulation_SimulationEvent(object sender, SimulationEventArgs e)
        {
            var data = new DataViewModel
            {
                Year = e.CurrentYear,
                Population = ((Simulator) sender).CurrentPopuation,
                BornInfos = e.BornInfos.Values.OrderBy(x => x.Order)
            };

            SaveDataToFile(data);

            await Dispatcher.InvokeAsync(() =>
            {
                _items.Add(data);
                DataGrid.ScrollIntoView(data);
            });

        }
    }
}
