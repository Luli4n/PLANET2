using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
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
using TravelingSalesmanSolver;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel;

namespace UIApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CanvasDrawer _CanvasDrawer;
        private const double _margin = 300;
        

        public MainWindow()
        {
            InitializeComponent();
            _CanvasDrawer = new CanvasDrawer(this,this.Width - _margin, this.Height * 2.0/3.0);
            this.SizeChanged += OnWindowSizeChanged;
            DataContext = _CanvasDrawer;
        }

        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWindowHeight = e.NewSize.Height;
            double newWindowWidth = e.NewSize.Width;
            _CanvasDrawer.Width = newWindowWidth - _margin;
            _CanvasDrawer.Height = newWindowHeight * 2.0/3.0;
        }

        protected void File_Button(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".tsp";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                FileTextBox.Text = filename;
            }
            this._CanvasDrawer.Points = PointsLoader.LoadPoints(FileTextBox.Text);
        }

        protected void RunCalculations_Button(object sender, RoutedEventArgs e)
        {
            RunCalculations();
        }

        private void RunCalculations()
        {
            StartServer();
            if ((bool)TasksRadioButton.IsChecked)
            {
                Process.Start("../net6.0/TaskPoolProcess.exe", $"\"{FileTextBox.Text}\" {AmountBox.Text} {Phase1TextBox.Text} {Phase2TextBox.Text}");
            }
            if ((bool)ThreadsRadioButton.IsChecked)
            {
                Process.Start("../net6.0/ThreadPoolProcess.exe", $"\"{FileTextBox.Text}\" {AmountBox.Text} {Phase1TextBox.Text} {Phase2TextBox.Text}");
            }
        }

        private void StartServer()
        {
            Task.Factory.StartNew(() =>
            {
                _CanvasDrawer.SolutionsCount = 0;
                var server = new NamedPipeServerStream("PipeOfCalculations");
                server.WaitForConnection();
                StreamReader reader = new StreamReader(server);
                StreamWriter writer = new StreamWriter(server);
                while (true)
                {
                    var line = reader.ReadLine();
                    if(line == null)
                    {
                        break;
                    }
                    _CanvasDrawer.Points = JsonConvert.DeserializeObject<GraphPoint[]>(line);
                    _CanvasDrawer.SolutionsCount++;
                    writer.Flush();
                }
                server.Close();
            });
        }

    }
}
