using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using TravelingSalesmanSolver;

namespace UIApp
{
    public class CanvasDrawer 
    {
        public double Width { get; set; }
        public double Height { get; set; }
        private Window _Window;

        private GraphPoint[] _Points;

        public GraphPoint[] Points { get => _Points; set =>
                _Points = value;}


        public int SolutionsCount { get; set; } = 0;
        private DispatcherTimer DispatcherTimer;
        
        public CanvasDrawer(Window window, double width, double height)
        {
            _Window = window;
            this.Width = width;
            this.Height = height;
            DispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            DispatcherTimer.Tick += new EventHandler(Draw);
            DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 32);
            DispatcherTimer.Start();
        }

        public void Draw(object sender, EventArgs e)
        {

            if (Points == null)
            {
                return;
            }
            Canvas _Canvas = (Canvas)_Window.FindName("PointCanvas");
            TextBlock sc = (TextBlock)this._Window.FindName("SolutionCount");
            sc.Text = SolutionsCount.ToString();
            
            TextBlock br = (TextBlock)this._Window.FindName("BestResult");
            br.Text = PointExtension.DistanceSum(Points).ToString();

            List<GraphPoint> points = Points.Select(p => p.Clone()).ToList();
            
            _Canvas.Children.Clear();
            double minX = points.Min(p => p.X);
            double minY = points.Min(p => p.Y);
            points.ForEach(p => { p.X = p.X - minX; p.Y = p.Y - minY; } );

            double maxX = points.Max(p => p.X);
            double maxY = points.Max(p => p.Y);

            double scaleX = maxX / (Width - 100.0);
            double scaleY =  maxY / (Height - 100.0);

            points.ForEach(p => { p.X = p.X / scaleX + 50.0 ; p.Y = p.Y / scaleY + 50.0; });

            for(int i = 0; i < points.Count() - 1; i++) 
            {
                Line line = new Line();
                line.Visibility = System.Windows.Visibility.Visible;
                line.StrokeThickness = 1;
                line.Stroke = System.Windows.Media.Brushes.Black;
                line.X1 = points[i].X;
                line.X2 = points[i+1].X;
                line.Y1 = points[i].Y;
                line.Y2 = points[i+1].Y;
                _Canvas.Children.Add(line);
            }
            Line lastLine = new Line();
            lastLine.Visibility = System.Windows.Visibility.Visible;
            lastLine.StrokeThickness = 1;
            lastLine.Stroke = System.Windows.Media.Brushes.Black;
            lastLine.X1 = points[0].X;
            lastLine.X2 = points[points.Count()-1].X;
            lastLine.Y1 = points[0].Y;
            lastLine.Y2 = points[points.Count()-1].Y;
            _Canvas.Children.Add(lastLine);

            for (int i = 0; i < points.Count(); i++)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 5;
                ellipse.Height = 5;
                ellipse.Fill = System.Windows.Media.Brushes.Red;
                Canvas.SetLeft(ellipse, points[i].X -2.5);
                Canvas.SetTop(ellipse, points[i].Y-2.5);
                _Canvas.Children.Add(ellipse);
            }

        }

    }
}
