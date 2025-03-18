using AeroDynasty.Core.Models.Core;
using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.RouteModels
{
    public class RouteStatistics
    {
        // Private vars
        private readonly object _monthLock = new object(); // lock for adjusting months

        // Public vars
        public ObservableCollection<RouteStatisticsPeriod> Years { get; set; }
        public ObservableCollection<RouteStatisticsPeriod> Months { get; set; }
        public RouteStatisticsPeriod CurrentMonth
        {
            get => GetCurrentMonth();
        }
        public RouteStatisticsPeriod PreviousMonth
        {
            get => GetPreviousMonth();
        }
        public double LifetimePassengers
        {
            get
            {
                double lastYears = Years.Sum(y => y.Passengers);
                double lastMonths = Months.Where(m => m.Date.Year == GameState.Instance.CurrentDate.Year).Sum(m => m.Passengers);
                return lastYears + lastMonths;
            }
        }
        public Price LifetimeBalance
        {
            get
            {
                double lastYears = Years.Sum(y => y.Balance.Amount);
                double lastMonths = Months.Where(m => m.Date.Year == GameState.Instance.CurrentDate.Year).Sum(m => m.Balance.Amount);
                return new Price (lastYears + lastMonths);
            }
        }
        public double LifetimeFillingPercentage
        {
            get
            {
                try
                {
                    var validMonths = Months
                        .Where(m => m.Date.Year == GameState.Instance.CurrentDate.Year && !double.IsNaN(m.FillingPercentage))
                        .ToList();

                    if (validMonths.Count == 0)
                        return 0.0; // Return 0 if no valid months exist to avoid division by zero

                    return validMonths.Sum(m => m.FillingPercentage) / validMonths.Count;
                }
                catch (Exception ex)
                {
                    throw; // Avoid rethrowing 'ex' directly (preserves stack trace)
                }
            }
        }


        public RouteStatistics()
        {
            Years = new ObservableCollection<RouteStatisticsPeriod>();
            Months = new ObservableCollection<RouteStatisticsPeriod>();

            // Initiate the Months
            DateTime Date = GameState.Instance.CurrentDate.AddYears(-1);
            Date = Date.AddDays(-1 * (Date.Day - 1));

            for (int i = 0; i <= 12; i++)
            {
                RouteStatisticsPeriod period = new RouteStatisticsPeriod(Date.AddMonths(i));
                Months.Add(period);
            }
        }

        public RouteStatisticsPeriod GetCurrentMonth()
        {
            return Months.FirstOrDefault(m => m.Date.Year == GameState.Instance.CurrentDate.Year && m.Date.Month == GameState.Instance.CurrentDate.Month);
        }

        public RouteStatisticsPeriod GetPreviousMonth()
        {
            return Months.FirstOrDefault(m => m.Date.Year == GameState.Instance.CurrentDate.AddMonths(-1).Year && m.Date.Month == GameState.Instance.CurrentDate.AddMonths(-1).Month);
        }

        public void ChangeCurrentMonth(DateTime currentDate)
        {
            lock (_monthLock) // Ensure only one thread enters this block at a time
            {
                // Check if the month already exists to avoid duplicates
                if (Months.Any(m => m.Date.Year == currentDate.Year && m.Date.Month == currentDate.Month))
                {
                    return; // The month already exists, so exit
                }

                // Check if it's a new year
                if (currentDate.Month == 1)
                {
                    // Sum up last year
                    RouteStatisticsPeriod year = new RouteStatisticsPeriod(new DateTime(currentDate.AddYears(-1).Year, 1, 1));

                    // Get data
                    List<RouteStatisticsPeriod> yearData = Months.Where(m => m.Date.Year == year.Date.Year).ToList();

                    // Sum financial data
                    year.AirlinerCosts.Amount = yearData.Sum(m => m.AirlinerCosts.Amount);
                    year.FuelCosts.Amount = yearData.Sum(m => m.FuelCosts.Amount);
                    year.TicketIncome.Amount = yearData.Sum(m => m.TicketIncome.Amount);

                    // Sum passenger data
                    year.AvailableSeats = yearData.Sum(m => m.AvailableSeats);
                    year.Passengers = yearData.Sum(m => m.Passengers);

                    Years.Add(year);
                }

                // Create the new month
                RouteStatisticsPeriod newMonth = new RouteStatisticsPeriod(currentDate);
                Months.Add(newMonth);

                // Check if total amount of months is exceeded, remove unnecessary data
                if (Months.Count > 13) // Keep 13 months, so one year plus the same current month last year
                {
                    Months.RemoveAt(0); // Remove the oldest entry
                }
            }
        }

        public void UpdateCurrentMonth(DateTime currentDate, Price ticketIncome, Price airlinerCosts, Price fuelCosts, int passengers, int availableSeats)
        {
            // Check if the month exists in the list
            if(Months.FirstOrDefault(m => m.Date.Year == currentDate.Year && m.Date.Month == currentDate.Month) == null)
            {
                ChangeCurrentMonth(currentDate);
            }

            RouteStatisticsPeriod month = Months.FirstOrDefault(m => m.Date.Year == currentDate.Year && m.Date.Month == currentDate.Month);

            // Update financial properties
            month.TicketIncome += ticketIncome;
            month.AirlinerCosts += airlinerCosts;
            month.FuelCosts += fuelCosts;

            // Update passenger properties
            month.Passengers += passengers;
            month.AvailableSeats += availableSeats;

        }
    }
}
