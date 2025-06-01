using System;

namespace Dim5.Distributions
{
    public class TrapezoidalDistribution : BaseDistribution
    {
        private double Min { get; set; }
        private double Max { get; set; }
        private double LowerMode { get; set; }
        private double UpperMode { get; set; }

        public TrapezoidalDistribution(double min = 0, double max = 1, double lowerMode = 0.25, double upperMode = 0.75)
        {
            Min = min;
            Max = max;
            LowerMode = lowerMode;
            UpperMode = upperMode;
        }

        public override double Generate()
        {
            double u = Random.NextDouble();
            if (u < (LowerMode - Min) / (Max - Min))
                return Min + Math.Sqrt(u * (LowerMode - Min) * (Max - Min));
            else if (u < (UpperMode - Min) / (Max - Min))
                return Min + (u * (Max - Min));
            else
                return Max - Math.Sqrt((1 - u) * (Max - UpperMode) * (Max - Min));
        }

        public override void FitParameters(double min, double max)
        {
            Min = min;
            Max = max;
            LowerMode = min + (max - min) / 3;
            UpperMode = max - (max - min) / 3;
        }

        public override void FitParametersWithOutliers(double min, double max, double leftOutlierPercentage, double rightOutlierPercentage)
        {
            FitParameters(min, max); // Для трапецеидального распределения параметры не зависят от брака
        }
    }
}