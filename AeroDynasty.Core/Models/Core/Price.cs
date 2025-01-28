using System;
using System.ComponentModel;

namespace AeroDynasty.Core.Models.Core
{
    public class Price : _BaseModel
    {
        private double _amount;
        public double Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = Math.Round(value, 2);
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        public Price(double amount)
        {
            Amount = amount;
        }

        public void calcInflation(double percentage)
        {
            Amount = Amount * ((100 + percentage) / 100.0);
        }

        #region Operator Overloads
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

        public override string ToString()
        {
            return Amount.ToString("C2");
        }
        #endregion
    }
}
