using MathNet.Numerics.Distributions;

namespace Dim5.Distributions
{
    public class TriangularDistribution : BaseDistribution
    {
        private double Min { get; set; }
        private double Max { get; set; }
        private double Mode { get; set; }

        public TriangularDistribution(double min = 0, double max = 1, double mode = 0.5)
        {
            Min = min;
            Max = max;
            Mode = mode;
        }

        public override double Generate()
        {
            return Triangular.Sample(Random, Min, Max, Mode);
        }

        public override void FitParameters(double min, double max)
        {
            Min = min;
            Max = max;
            Mode = (min + max) / 2; // Центр распределения
        }

        public override void FitParametersWithOutliers(double min, double max, double leftOutlierPercentage, double rightOutlierPercentage)
        {
            FitParameters(min, max); // Для треугольного распределения параметры не зависят от брака
        }
    }
}