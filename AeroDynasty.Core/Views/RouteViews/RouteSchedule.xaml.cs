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

namespace AeroDynasty.Core.Views.RouteViews
{
    /// <summary>
    /// Interaction logic for RouteSchedule.xaml
    /// </summary>
    public partial class RouteSchedule : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty RouteScheduleItemProperty =
            DependencyProperty.Register(
                "RouteScheduleItem",
                typeof(Models.RouteModels.RouteSchedule),
                typeof(RouteSchedule),
                new PropertyMetadata(null, OnFlightScheduleChanged));

        public static readonly DependencyProperty ActualWidthProperty =
            DependencyProperty.Register(
                "ActualWidth",
                typeof(double),
                typeof(RouteSchedule),
                new PropertyMetadata(0.0, OnFlightScheduleChanged));

        public Models.RouteModels.RouteSchedule RouteScheduleItem
        {
            get { return (Models.RouteModels.RouteSchedule)GetValue(RouteScheduleItemProperty); }
            set { SetValue(RouteScheduleItemProperty, value); }
        }

        public double ActualWidth
        {
            get { return (double)GetValue(ActualWidthProperty); }
            set
            {
                SetValue(ActualWidthProperty, value);
                OnPropertyChanged(nameof(ActualWidth));
                UpdateProperties();
                this.UpdateLayout();
            }
        }

        private double _outboundGridWidth;
        public double OutboundGridWidth
        {
            get => _outboundGridWidth;
            private set
            {
                _outboundGridWidth = value;
                OnPropertyChanged(nameof(OutboundGridWidth));
            }
        }

        private double _inboundGridWidth;
        public double InboundGridWidth
        {
            get => _inboundGridWidth;
            private set
            {
                _inboundGridWidth = value;
                OnPropertyChanged(nameof(InboundGridWidth));
            }
        }

        private double _turnAroundWidth;
        public double TurnAroundWidth
        {
            get => _turnAroundWidth;
            private set
            {
                _turnAroundWidth = value;
                OnPropertyChanged(nameof(TurnAroundWidth));
            }
        }

        private double _cooldownWidth;
        public double CooldownWidth
        {
            get => _cooldownWidth;
            private set
            {
                _cooldownWidth = value;
                OnPropertyChanged(nameof(CooldownWidth));
            }
        }

        private double _leadTimeWidth;
        public double LeadTimeWidth
        {
            get => _leadTimeWidth;
            private set
            {
                _leadTimeWidth = value;
                OnPropertyChanged(nameof(LeadTimeWidth));
            }
        }

        public RouteSchedule()
        {
            InboundGridWidth = 0;
            InitializeComponent();

            // Subscribe to the loaded event to access the parent container's size
            Loaded += RouteSchedule_Loaded;
        }

        private void RouteSchedule_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if Parent is null
            if (Parent == null)
            {
                // Handle case when Parent is not set yet (e.g., defer the work)
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Parent is FrameworkElement parentElement)
                    {
                        // Subscribe to SizeChanged event
                        parentElement.SizeChanged += ParentElement_SizeChanged;

                        // Initialize sizes based on the current parent size
                        ActualWidth = parentElement.ActualWidth;
                    }
                }));
            }
            else
            {
                // Parent is already set, proceed with normal logic
                if (Parent is FrameworkElement parentElement)
                {
                    // Subscribe to SizeChanged event
                    parentElement.SizeChanged += ParentElement_SizeChanged;

                    // Initialize sizes based on the current parent size
                    ActualWidth = parentElement.ActualWidth;
                }
            }
        }

        private void ParentElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update properties whenever the parent size changes
            if (sender is FrameworkElement parentElement)
            {
                ActualWidth = parentElement.ActualWidth;
            }
        }

        private static void OnFlightScheduleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RouteSchedule control)
            {
                control.UpdateProperties();
            }
        }

        private void UpdateProperties()
        {
            if ((double)GetValue(ActualWidthProperty) <= 0 || RouteScheduleItem == null)
                return;

            // Total width minus the first column width
            double ListViewWidth = ActualWidth;

            // Calculate the pixel width of one minute
            var minuteToPixelRatio = ListViewWidth / 1440;


            OutboundGridWidth = Math.Floor(RouteScheduleItem.Outbound.FlightDuration.TotalMinutes * minuteToPixelRatio);
            InboundGridWidth = Math.Floor(RouteScheduleItem.Inbound.FlightDuration.TotalMinutes * minuteToPixelRatio);
            TurnAroundWidth = Math.Floor(RouteScheduleItem.TurnaroundTime.TotalMinutes * minuteToPixelRatio);
            CooldownWidth = Math.Floor(RouteScheduleItem.TurnaroundTime.TotalMinutes * minuteToPixelRatio);

            LeadTimeWidth = Math.Floor(RouteScheduleItem.Outbound.DepartureTime.TotalMinutes * minuteToPixelRatio);
            
            // Check if total value exceeds maxWidth
            if(LeadTimeWidth + OutboundGridWidth + TurnAroundWidth + InboundGridWidth > ListViewWidth)
            {
                if(LeadTimeWidth > ListViewWidth)
                {
                    LeadTimeWidth = ListViewWidth;
                    OutboundGridWidth = TurnAroundWidth = InboundGridWidth = CooldownWidth = 0;
                }
                else if (LeadTimeWidth + OutboundGridWidth > ListViewWidth)
                {
                    OutboundGridWidth = (ListViewWidth - LeadTimeWidth);
                    TurnAroundWidth = InboundGridWidth = CooldownWidth = 0;
                }
                else if (LeadTimeWidth + OutboundGridWidth + TurnAroundWidth > ListViewWidth)
                {
                    TurnAroundWidth = (ListViewWidth - LeadTimeWidth - OutboundGridWidth);
                    InboundGridWidth = CooldownWidth = 0;
                }
                else if (LeadTimeWidth + OutboundGridWidth + TurnAroundWidth + InboundGridWidth > ListViewWidth)
                {
                    InboundGridWidth = (ListViewWidth - LeadTimeWidth - OutboundGridWidth - TurnAroundWidth);
                    CooldownWidth = 0;
                }
                else if (LeadTimeWidth + OutboundGridWidth + TurnAroundWidth + InboundGridWidth + CooldownWidth > ListViewWidth)
                {
                    CooldownWidth = (ListViewWidth - LeadTimeWidth - OutboundGridWidth - TurnAroundWidth - InboundGridWidth);
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}