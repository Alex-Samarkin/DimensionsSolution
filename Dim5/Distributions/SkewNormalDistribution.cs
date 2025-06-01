using MathNet.Numerics.Distributions;

namespace Dim5.Distributions
{
    public class SkewNormalDistribution : BaseDistribution
    {
        private SkewedGeneralizedTDistribution InternalDistribution { get; }

        public SkewNormalDistribution(double location = 0, double scale = 1, double skewness = 0)
        {
            // Initialize the internal SkewedGeneralizedTDistribution
            InternalDistribution = new SkewedGeneralizedTDistribution(location, scale, skewness, shape: 5);
        }

        public override double Generate()
        {
            // Delegate to the internal distribution
            return InternalDistribution.Generate();
        }

        public override void FitParameters(double min, double max)
        {
            // Delegate to the internal distribution
            InternalDistribution.FitParameters(min, max);
        }

        public override void FitParametersWithOutliers(double min, double max, double leftOutlierPercentage, double rightOutlierPercentage)
        {
            // Delegate to the internal distribution
            InternalDistribution.FitParametersWithOutliers(min, max, leftOutlierPercentage, rightOutlierPercentage);
        }

        public override IEnumerable<double> GenerateSequence(int count)
        {
            // Delegate to the internal distribution
            return InternalDistribution.GenerateSequence(count);
        }
    }
}