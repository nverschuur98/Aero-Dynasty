using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using AeroDynasty.Core.Models.Core;

namespace AeroDynasty.WPF.Views.Map
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl, INotifyPropertyChanged
    {
        // Private vars
        private bool isPanning = false;
        private Point panStart;
        private const double MapHeight = 398.9;
        private const double MapWidth = 798;
        private const double ZoomFactor = 1.1;
        private const double MinZoom = 1;
        private const double MaxZoom = 10.0;

        // Public vars
        public static readonly DependencyProperty IsControllableProperty =
            DependencyProperty.Register(
                "IsControllable",
                typeof(bool),
                typeof(MapView),
                new PropertyMetadata(false));

        public static readonly DependencyProperty SetSecondaryAirportsProperty =
            DependencyProperty.Register(
                "SetSecondaryAirports",
                typeof(bool),
                typeof(MapView),
                new PropertyMetadata(false));

        public static readonly DependencyProperty AirportProperty =
            DependencyProperty.Register(
                "Airport",
                typeof(Core.Models.AirportModels.Airport),
                typeof(MapView),
                new PropertyMetadata(null, OnDataChanged));

        public static readonly DependencyProperty AirportsProperty =
            DependencyProperty.Register(
                "Airports",
                typeof(ICollectionView),
                typeof(MapView),
                new PropertyMetadata(null, OnDataChanged));

        public bool IsControllable
        {
            get { return (bool)GetValue(IsControllableProperty); }
            set { SetValue(IsControllableProperty, value); }
        }

        public bool SetSecondaryAirports
        {
            get { return (bool)GetValue(SetSecondaryAirportsProperty); }
            set { SetValue(SetSecondaryAirportsProperty, value); }
        }

        public Core.Models.AirportModels.Airport Airport
        {
            get { return (Core.Models.AirportModels.Airport)GetValue(AirportProperty); }
            set { SetValue(AirportProperty, value); }
        }

        public ICollectionView Airports
        {
            get { return (ICollectionView)GetValue(AirportsProperty); }
            set { SetValue(AirportsProperty, value); }
        }

        // Constructor
        public MapView()
        {
            InitializeComponent();
            MapCanvas.MouseWheel += MapCanvas_MouseWheel;
            MapCanvas.MouseDown += MapCanvas_MouseDown;
            MapCanvas.MouseMove += MapCanvas_MouseMove;
            MapCanvas.MouseUp += MapCanvas_MouseUp;

        }

        // Private funcs
        private void MapViewLoaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            //Auto zoom in to the max height
            if (MapCanvas.ActualHeight == 0)
                return;

            ClearChildren();

            // If an airport is provided, add a marker and focus
            if (Airport != null)
            {
                AddPoint(Airport.Coordinates);
            }

            // If multiple airports are provided, add them
            if (Airports != null)
            {
                foreach (Core.Models.AirportModels.Airport airport in Airports)
                {
                    AddPoint(airport.Coordinates, SetSecondaryAirports ? true : false);
                }
            }
        }

        #region MouseTracking
        private void MapCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Check if user is allowed to control the map
            if (!IsControllable)
                return;

            double zoom = e.Delta > 0 ? ZoomFactor : 1 / ZoomFactor;
            double newScaleX = MapScaleTransform.ScaleX * zoom;
            double newScaleY = MapScaleTransform.ScaleY * zoom;

            if (newScaleX < MinZoom || newScaleX > MaxZoom) return;

            MapScaleTransform.ScaleX = newScaleX;
            MapScaleTransform.ScaleY = newScaleY;

            UpdatePointSizes();
        }

        private void MapCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if user is allowed to control the map
            if (!IsControllable)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isPanning = true;
                panStart = e.GetPosition(this);
                MapCanvas.CaptureMouse();
            }
        }

        private void MapCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Check if user is allowed to control the map
            if (!IsControllable)
                return;

            if (isPanning)
            {
                Point current = e.GetPosition(this);
                double offsetX = current.X - panStart.X;
                double offsetY = current.Y - panStart.Y;

                MapTranslateTransform.X += offsetX;
                MapTranslateTransform.Y += offsetY;

                /*
                if (MapTranslateTransform.X > 0)
                {
                    MapTranslateTransform.X = 0;
                }
                else if (Math.Abs(MapTranslateTransform.X) > (MapWidth - 300))
                {
                    MapTranslateTransform.X = -(MapWidth - 300);
                }
                if (MapTranslateTransform.Y > 0)
                {
                    MapTranslateTransform.Y = 0;
                }
                else if (Math.Abs(MapTranslateTransform.Y) > (MapHeight - 300))
                {
                    MapTranslateTransform.Y = -(MapHeight - 300);
                }*/

                panStart = current;
            }

            // Console.WriteLine($"{e.GetPosition(this).X},{e.GetPosition(this).Y}");
        }

        private void MapCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Check if user is allowed to control the map
            if (!IsControllable)
                return;

            if (isPanning)
            {
                isPanning = false;
                MapCanvas.ReleaseMouseCapture();
            }
        }
        #endregion

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MapView control)
            {
                control.LoadData();

                if(control.Airport != null)
                {
                    control.FocusOnCoordinate(control.Airport.Coordinates, 7);
                }
            }
        }

        private void UpdatePointSizes()
        {
            // Normalize ellipse sizes to avoid extreme scaling
            double markerSize = 2; // Keep size consistent in screen space

            foreach (UIElement child in MapCanvas.Children)
            {
                if (child is Ellipse ellipse)
                {
                    ellipse.Width = markerSize;
                    ellipse.Height = markerSize;

                    // Keep markers centered after resizing
                    Tuple<double, double> coords = (Tuple<double, double>)ellipse.Tag;
                    double x = (coords.Item2 + 180) * (MapWidth / 360);
                    double y = (90 - coords.Item1) * (MapHeight / 180);

                    Canvas.SetLeft(ellipse, x - markerSize / 2);
                    Canvas.SetTop(ellipse, y - markerSize / 2);
                }
            }
        }

        private static double CalculateAngle(Coordinates start, Coordinates end)
        {
            double deltaLatitude = end.Latitude - start.Latitude;

            // Compute Delta Longitude and normalize it to be within -180 to 180 degrees
            double deltaLongitude = (end.Longitude - start.Longitude + 180) % 360 - 180;

            // Use Math.Atan2 to calculate the angle
            return Math.Atan2(deltaLatitude, deltaLongitude);
        }

        private static double CalculateLatitudeAtLongitude(Coordinates start, Coordinates end)
        {
            // Calculate the change in longitude and latitude (delta values)
            double deltaLongitude = end.Longitude - start.Longitude;

            // Calculate the angle θ
            double angle = CalculateAngle(start, end);

            // Use the formula to calculate the latitude at the target longitude
            double deltaPhi = Math.Tan(angle) * deltaLongitude;  // Latitude change
            double targetLatitude = start.Latitude + deltaPhi;  // Latitude at target longitude

            return targetLatitude;
        }

        // Public funcs
        public void ClearChildren()
        {
            // Remove all Ellipse elements from the MapCanvas children
            var ellipses = MapCanvas.Children.OfType<Ellipse>().ToList();

            foreach (var ellipse in ellipses)
            {
                MapCanvas.Children.Remove(ellipse);
            }
        }

        public void AddPoint(Coordinates coordinates)
        {
            AddPoint(coordinates, false);
        }

        public void AddPoint(Coordinates coordinates, bool isSecondary)
        {
            if (MapCanvas.ActualWidth == 0 || MapCanvas.ActualHeight == 0)
                return;

            // Console.WriteLine($"{MapCanvas.ActualWidth}, {MapCanvas.ActualHeight}");

            // Scale factors based on the canvas size and map size
            double scaleX = MapWidth / 360;
            double scaleY = MapHeight / 180;

            // Convert lat/lon to map coordinates (scaled by the map dimensions)
            double x = (coordinates.Longitude + 180) * scaleX;
            double y = (90- coordinates.Latitude) * scaleY;

            // Create a marker
            double baseSize = 2;  // Base size of the ellipse

            // Check if IsSecondary to reflect color
            Brush fill = isSecondary ? (Brush)FindResource("SecondaryBrush") : (Brush)FindResource("AccentBrush");
            Brush stroke = isSecondary ? (Brush)FindResource("BlueTransparentBackgroundBrush") : (Brush)FindResource("RedTransparentBackgroundBrush");

            Ellipse point = new Ellipse
            {
                Width = baseSize,  // Adjust size based on zoom
                Height = baseSize,
                Fill = fill,
                Stroke = stroke,
                StrokeThickness = 1
            };

            // Add to Canvas and position it
            MapCanvas.Children.Add(point);
            Canvas.SetLeft(point, x - point.Width / 2);
            Canvas.SetTop(point, y - point.Height / 2);

            // Set Z-Index to ensure markers are on top
            Panel.SetZIndex(point, 10);

            // Store the ellipse in a list for later updates on zoom
            point.Tag = new Tuple<double, double>(coordinates.Latitude, coordinates.Longitude);
        }

        public void AddLine(Coordinates start, Coordinates end)
        {
            if (MapCanvas.ActualWidth == 0 || MapCanvas.ActualHeight == 0)
                return;

            double scaleX = MapWidth / 360;
            double scaleY = MapHeight / 180;

            double x1 = (start.Longitude + 180) * scaleX;
            double y1 = (90 - start.Latitude) * scaleY;
            double x2 = (end.Longitude + 180) * scaleX;
            double y2 = (90 - end.Latitude) * scaleY;

            // Check if the line crosses the -180/180 longitude boundary
            if (Math.Abs(start.Longitude - end.Longitude) > 180)
            {
                // Split the line into two parts to wrap around
                double midLongitude = (start.Longitude < end.Longitude) ? -180 : 180;

                double x;
                if(start.Longitude < end.Longitude)
                {
                    x = 0.0;
                }
                else
                {
                    x = (360 + Math.Abs(180 + end.Longitude)) * scaleX;
                }

                double crossLatitude = (90 - CalculateLatitudeAtLongitude(start, end)) * scaleY;

                // First segment: Start to wrap boundary
                Line part1 = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = (start.Longitude < end.Longitude) ? 0 : MapWidth, // Left or Right edge
                    Y2 = crossLatitude,
                    Stroke = (Brush)FindResource("RedTransparentBackgroundBrush"),
                    StrokeThickness = 0.5
                };
                MapCanvas.Children.Add(part1);
                Panel.SetZIndex(part1, 5);  // Lower value than ellipses

                // Second segment: Wrap boundary to end
                Line part2 = new Line
                {
                    X1 = (start.Longitude < end.Longitude) ? MapWidth : 0, // Right or Left edge
                    Y1 = crossLatitude,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = (Brush)FindResource("RedTransparentBackgroundBrush"),
                    StrokeThickness = 0.5
                };
                MapCanvas.Children.Add(part2);
                Panel.SetZIndex(part2, 5);  // Lower value than ellipses
            }
            else
            {
                // Regular case: No wrapping needed
                Line line = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = (Brush)FindResource("RedTransparentBackgroundBrush"),
                    StrokeThickness = 0.5
                };
                MapCanvas.Children.Add(line);
                Panel.SetZIndex(line, 5);  // Lower value than ellipses
            }
        }

        public void FocusOnCoordinate(Coordinates target)
        {
            // Use the current zoomlevel
            FocusOnCoordinate(target, MapScaleTransform.ScaleX);
        }

        public void FocusOnCoordinate(Coordinates target, double zoomlevel)
        {
            if (MapCanvas.ActualWidth == 0 || MapCanvas.ActualHeight == 0)
                return;

            double scaleX = MapWidth / 360;
            double scaleY = MapHeight / 180;

            // Convert coordinates to map positions
            double x = (target.Longitude + 180) * scaleX;
            double y = (90 - target.Latitude) * scaleY;

            MapScaleTransform.ScaleX = zoomlevel;
            MapScaleTransform.ScaleY = zoomlevel;

            // Get the size of the visible map area
            double viewWidth = ActualWidth > 0 ? ActualWidth : 300;   // Default to 300 if not set
            double viewHeight = ActualHeight > 0 ? ActualHeight : 300; // Default to 300 if not set

            // Calculate new translation to center the target
            MapTranslateTransform.X = (viewWidth / 2) - (x * zoomlevel);
            MapTranslateTransform.Y = (viewHeight / 2) - (y * zoomlevel);
            //MapTranslateTransform.X = -(viewWidth - MapWidth) / 2;// - (x * zoomlevel);
            //MapTranslateTransform.Y = -(viewHeight - MapHeight) / 2;// - (y * zoomlevel);

            // Make sure to update the marker sizes
            UpdatePointSizes();
        }

        /// <summary>
        /// Raised when a property changes to notify bindings.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Triggers the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
