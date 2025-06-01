using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dim5
{
    public class ToleranceInfo
    {
        public double UpperDeviation { get; set; }
        public double LowerDeviation { get; set; }

        public double ToleranceValue => Math.Abs(UpperDeviation - LowerDeviation);

        public double MeanOfToleranceField=> Math.Min(UpperDeviation, LowerDeviation) + ToleranceValue / 2;

        public override string ToString()
        {
            return $"Верхнее: {UpperDeviation:F2} мкм, Нижнее: {LowerDeviation:F2} мкм, Допуск: {ToleranceValue:F2} мкм";
        }
    }
}
