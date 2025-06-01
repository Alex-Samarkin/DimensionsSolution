using MathNet.Numerics.Distributions;

namespace Dim5.Distributions
{
    public class UniformDistribution : BaseDistribution
    {
        private double Min { get; set; }
        private double Max { get; set; }

        public UniformDistribution(double min = 0, double max = 1)
        {
            Min = min;
            Max = max;
        }

        public override double Generate()
        {
            return ContinuousUniform.Sample(Random, Min, Max);
        }

        public override void FitParameters(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public override void FitParametersWithOutliers(double min, double max, double leftOutlierPercentage, double rightOutlierPercentage)
        {
            // Для равномерного распределения параметры не зависят от брака
            FitParameters(min, max);
        }
    }
}