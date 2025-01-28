using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models
{
    public abstract class _PeriodModel
    {
        private bool _isActive;
        public bool IsActive { get => _isActive; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public void SetPeriod(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate = end;
        }

        /// <summary>
        /// Check if the object is within its period
        /// </summary>
        /// <param name="currentDate"></param>
        public void CheckIsActive(DateTime currentDate)
        {
            if(currentDate >= StartDate && currentDate < EndDate)
            {
                _isActive = true;
            }
            else
            {
                _isActive = false;
            }
        }
    }
    public static class PeriodModelExtensions
    {
        // Async method to check if all items in a collection are active
        public static async Task CheckIsActiveForAllAsync<T>(this ObservableCollection<T> collection, DateTime currentDate)
            where T : _PeriodModel
        {
            // Iterate over each item and check if it's active
            await Task.WhenAll(collection.Select(async item =>
            {
                item.CheckIsActive(currentDate);
                await Task.CompletedTask; // placeholder for potential async operations
            }));
        }
    }
}
