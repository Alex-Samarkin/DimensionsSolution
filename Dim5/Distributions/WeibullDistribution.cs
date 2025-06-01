using MathNet.Numerics.Distributions;

namespace Dim5.Distributions
{
    public class WeibullDistribution : BaseDistribution
    {
        private double Shape { get; set; }
        private double Scale { get; set; }

        public WeibullDistribution(double shape = 1, double scale = 1)
        {
            Shape = shape;
            Scale = scale;
        }

        public override double Generate()
        {
            return Weibull.Sample(Random, Shape, Scale);
        }

        public override void FitParameters(double min, double max)
        {
            Scale = (max - min) / 2;
            Shape = 2; // Начальное приближение
        }

        public override void FitParametersWithOutliers(double min, double max, double leftOutlierPercentage, double rightOutlierPercentage)
        {
            FitParameters(min, max); // Для распределения Вейбулла параметры не зависят от брака
        }
    }
}