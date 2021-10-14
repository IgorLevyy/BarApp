using System;
using System.Collections.Generic;
using System.Globalization;

namespace TradeBarApp
{
    //ABBV,NYSE,02.01.2020,08:01:00,89.090,89.090,88.950,88.950,1325
    public class Bar
    {
        public string Symbol { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public int TotalVolume { get; set; }

        public override string ToString()
        {
            return $"{Symbol},{Description},{Date:dd.MM.yyyy},{Date:HH:mm:ss},{Open.ToString("0.000", CultureInfo.InvariantCulture)},{High.ToString("0.000", CultureInfo.InvariantCulture)},{Low.ToString("0.000", CultureInfo.InvariantCulture)},{Close.ToString("0.000", CultureInfo.InvariantCulture)},{TotalVolume}";
        }
    }

    class BarComparer : IEqualityComparer<Bar>
    {
        public bool Equals(Bar x, Bar y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.ToString() == y.ToString();
        }

        public int GetHashCode(Bar bar)
        {
            if (Object.ReferenceEquals(bar, null)) return 0;

            int hashBar = bar.ToString() == null ? 0 : bar.ToString().GetHashCode();
          
            return hashBar;
        }
    }
}
