using MathNet.Numerics.Distributions;

namespace Dim5.Distributions
{
    public class NormalDistribution : BaseDistribution
    {
        private double Mean { get; set; }
        private double StdDev { get; set; }

        public NormalDistribution(double mean = 0, double stdDev = 1)
        {
            Mean = mean;
            StdDev = stdDev;
        }

        public override double Generate()
        {
            return Normal.Sample(Random, Mean, StdDev);
        }

        public override void FitParameters(double min, double max)
        {
            Mean = (min + max) / 2;
            StdDev = (max - min) / 6; // 99.7% данных лежат в пределах ±3σ
        }

        public override void FitParametersWithOutliers(double min, double max, double leftOutlierPercentage, double rightOutlierPercentage)
        {
            double leftBound = leftOutlierPercentage / 100.0;
            double rightBound = 1 - (rightOutlierPercentage / 100.0);

            Mean = (min + max) / 2;
            StdDev = (max - min) / 6;

            // Итеративный подбор параметров
            for (int i = 0; i < 1000; i++)
            {
                double leftCdf = Normal.CDF(Mean, StdDev, min);
                double rightCdf = 1 - Normal.CDF(Mean, StdDev, max);

                if (Math.Abs(leftCdf - leftBound) < 0.001 && Math.Abs(rightCdf - rightBound) < 0.001)
                    break;

                if (leftCdf < leftBound)
                    StdDev *= 1.1;
                else
                    StdDev *= 0.9;

                if (rightCdf < rightBound)
                    StdDev *= 1.1;
                else
                    StdDev *= 0.9;
            }
        }
    }
}