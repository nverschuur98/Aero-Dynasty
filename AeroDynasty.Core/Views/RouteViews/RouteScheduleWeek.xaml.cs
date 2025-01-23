using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AeroDynasty.Core.Views.RouteViews
{
    /// <summary>
    /// Represents a control to display and manage route schedules grouped by days of the week.
    /// </summary>
    public partial class RouteScheduleWeek : UserControl, INotifyPropertyChanged
    {
        // Dependency property to bind the collection of route schedules
        public static readonly DependencyProperty RouteScheduleItemProperty =
            DependencyProperty.Register(
                nameof(RouteSchedules),
                typeof(ObservableCollection<Models.RouteModels.RouteSchedule>),
                typeof(RouteScheduleWeek),
                new PropertyMetadata(null, OnFlightScheduleChanged));

        /// <summary>
        /// Gets or sets the collection of route schedules.
        /// Updates bindings and notifies when the collection changes.
        /// </summary>
        public ObservableCollection<Models.RouteModels.RouteSchedule> RouteSchedules
        {
            get => (ObservableCollection<Models.RouteModels.RouteSchedule>)GetValue(RouteScheduleItemProperty);
            set
            {
                SetValue(RouteScheduleItemProperty, value);
                OnPropertyChanged(nameof(RouteSchedules)); // Notify that the property has changed
                NotifyDaySchedules(); // Update the filtered schedules for each day
            }
        }

        // Properties for each day of the week that provide the filtered route schedules
        public ObservableCollection<Models.RouteModels.RouteSchedule> MondaySchedule => GetFlightScheduleForDay(DayOfWeek.Monday);
        public ObservableCollection<Models.RouteModels.RouteSchedule> TuesdaySchedule => GetFlightScheduleForDay(DayOfWeek.Tuesday);
        public ObservableCollection<Models.RouteModels.RouteSchedule> WednesdaySchedule => GetFlightScheduleForDay(DayOfWeek.Wednesday);
        public ObservableCollection<Models.RouteModels.RouteSchedule> ThursdaySchedule => GetFlightScheduleForDay(DayOfWeek.Thursday);
        public ObservableCollection<Models.RouteModels.RouteSchedule> FridaySchedule => GetFlightScheduleForDay(DayOfWeek.Friday);
        public ObservableCollection<Models.RouteModels.RouteSchedule> SaturdaySchedule => GetFlightScheduleForDay(DayOfWeek.Saturday);
        public ObservableCollection<Models.RouteModels.RouteSchedule> SundaySchedule => GetFlightScheduleForDay(DayOfWeek.Sunday);

        public double ListActualWidth
        {
            get => CanvasContainer.ActualWidth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteScheduleWeek"/> control.
        /// </summary>
        public RouteScheduleWeek()
        {
            InitializeComponent();
            this.SizeChanged += RouteScheduleWeek_SizeChanged;
        }

        private void RouteScheduleWeek_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ListActualWidth));
        }

        /// <summary>
        /// Called when the <see cref="RouteSchedules"/> dependency property changes.
        /// Attaches a handler to observe collection changes and updates day schedules.
        /// </summary>
        private static void OnFlightScheduleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RouteScheduleWeek control)
            {
                // Attach a listener to the collection's change event if it's not null
                if (control.RouteSchedules != null)
                {
                    control.RouteSchedules.CollectionChanged += control.RouteSchedules_CollectionChanged;
                }

                // Notify the UI to update day-specific schedules
                control.NotifyDaySchedules();
            }
        }

        /// <summary>
        /// Handles changes in the <see cref="RouteSchedules"/> collection (e.g., add, remove).
        /// Updates the filtered schedules for each day.
        /// </summary>
        private void RouteSchedules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyDaySchedules();
        }

        /// <summary>
        /// Filters the route schedules for a specific day of the week.
        /// </summary>
        /// <param name="dayOfWeek">The day of the week to filter by.</param>
        /// <returns>An observable collection of route schedules for the specified day.</returns>
        private ObservableCollection<Models.RouteModels.RouteSchedule> GetFlightScheduleForDay(DayOfWeek dayOfWeek)
        {
            // Filter schedules by day and return a new collection
            var filteredSchedules = RouteSchedules?.Where(schedule => schedule.Outbound?.DepartureDay == dayOfWeek).ToList();

            return filteredSchedules != null ? new ObservableCollection<Models.RouteModels.RouteSchedule>(filteredSchedules) : new ObservableCollection<Models.RouteModels.RouteSchedule>();
        }

        /// <summary>
        /// Notifies that the properties for day-specific schedules have changed.
        /// </summary>
        private void NotifyDaySchedules()
        {
            OnPropertyChanged(nameof(MondaySchedule));
            OnPropertyChanged(nameof(TuesdaySchedule));
            OnPropertyChanged(nameof(WednesdaySchedule));
            OnPropertyChanged(nameof(ThursdaySchedule));
            OnPropertyChanged(nameof(FridaySchedule));
            OnPropertyChanged(nameof(SaturdaySchedule));
            OnPropertyChanged(nameof(SundaySchedule));
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
