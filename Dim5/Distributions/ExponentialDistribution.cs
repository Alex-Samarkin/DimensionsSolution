using MathNet.Numerics.Distributions;

namespace Dim5.Distributions
{
    public class ExponentialDistribution : BaseDistribution
    {
        private double Rate { get; set; }

        public ExponentialDistribution(double rate = 1)
        {
            Rate = rate;
        }

        public override double Generate()
        {
            return Exponential.Sample(Random, Rate);
        }

        public override void FitParameters(double min, double max)
        {
            Rate = 1 / ((max - min) / 2); // Среднее значение
        }

        public override void FitParametersWithOutliers(double min, double max, double leftOutlierPercentage, double rightOutlierPercentage)
        {
            FitParameters(min, max); // Для экспоненциального распределения параметры не зависят от брака
        }
    }
}