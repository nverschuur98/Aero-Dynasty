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
        public ObservableCollection<RouteStatisticsPeriod> Years { get; set; }
        public ObservableCollection<RouteStatisticsPeriod> Months { get; set; }
        public double TotalPassengers
        {
            get
            {
                double lastYears = Years.Sum(y => y.Passengers);
                double lastMonths = Months.Where(m => m.Date.Year == GameState.Instance.CurrentDate.Year).Sum(m => m.Passengers);
                return lastYears + lastMonths;
            }
        }
        public Price TotalBalance
        {
            get
            {
                double lastYears = Years.Sum(y => y.Balance.Amount);
                double lastMonths = Months.Where(m => m.Date.Year == GameState.Instance.CurrentDate.Year).Sum(m => m.Balance.Amount);
                return new Price (lastYears + lastMonths);
            }
        }

        public RouteStatistics()
        {
            Years = new ObservableCollection<RouteStatisticsPeriod>();
            Months = new ObservableCollection<RouteStatisticsPeriod>();
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
            // Check if new year
            if(currentDate.Month == 1)
            {
                // Sum up last year
            }

            // Create the new month
            RouteStatisticsPeriod newMonth = new RouteStatisticsPeriod(currentDate);
            Months.Add(newMonth);

            // Check if total amount of months is exceeded, remove unnecessary data
            if(Months.Count >= 12)
            {
                // Remove oldest entry
                Months.RemoveAt(0);
            }
        }

        public void UpdateCurrentMonth(DateTime currentDate, Price balanceDelta, double passengersDelta)
        {
            // Check if the month exists in the list
            if(Months.FirstOrDefault(m => m.Date.Year == currentDate.Year && m.Date.Month == currentDate.Month) == null)
            {
                ChangeCurrentMonth(currentDate);
            }

            RouteStatisticsPeriod month = Months.FirstOrDefault(m => m.Date.Year == currentDate.Year && m.Date.Month == currentDate.Month);
            month.Balance += balanceDelta;
            month.Passengers += passengersDelta;
        }
    }
}
