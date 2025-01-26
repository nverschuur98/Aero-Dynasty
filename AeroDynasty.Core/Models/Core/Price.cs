using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.Core
{
    public class Price
    {
        private double _amount { get; set; }
        public double Amount
        {
            get => _amount;
            set
            {
                _amount = Math.Round(value, 2);
            }
        }

        public Price(double amount)
        {
            Amount = amount;
        }

        /// <summary>
        /// Calculate the inflation on the price
        /// </summary>
        /// <param name="percentage">Enter the percentage as 5.0%</param>
        public void calcInflation(double percentage)
        {
            Amount = Amount * ((100 + percentage) / 100.0);
        }

        #region operator overloads
        public static Price operator +(Price p1, Price p2)
        {
            return new Price(p1.Amount + p2.Amount);
        }
        public static Price operator +(Price p, double d)
        {
            return new Price(p.Amount + d);
        }
        public static Price operator +(double d, Price p)
        {
            return new Price(p.Amount + d);
        }
        public static Price operator -(Price p1, Price p2)
        {
            return new Price(p1.Amount - p2.Amount);
        }
        public static Price operator *(Price p1, Price p2)
        {
            return new Price(p1.Amount * p2.Amount);
        }
        public static Price operator *(Price p, double d)
        {
            return new Price(p.Amount * d);
        }
        public static Price operator *(double d, Price p)
        {
            return new Price(p.Amount * d);
        }
        public static Price operator *(Price p, int i)
        {
            return new Price(p.Amount * i);
        }
        public static Price operator *(int i, Price p)
        {
            return new Price(p.Amount * i);
        }
        public static bool operator <=(Price p1, Price p2)
        {
            return p1.Amount <= p2.Amount;
        }
        public static bool operator >=(Price p1, Price p2)
        {
            return p1.Amount >= p2.Amount;
        }
        public static bool operator ==(Price p1, Price p2)
        {
            if (p1 is null && p2 is null) return true;
            if (p1 is null || p2 is null) return false;
            return p1.Amount == p2.Amount;
        }
        public static bool operator !=(Price p1, Price p2)
        {
            return !(p1 == p2);
        }

        // ToString override for printing
        public override string ToString()
        {
            return Amount.ToString("C2"); // Currency formatting
        }

        #endregion
    }
}
