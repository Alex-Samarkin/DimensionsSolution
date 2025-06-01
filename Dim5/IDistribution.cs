using System.Collections.Generic;

namespace Dim5.Distributions
{
    public interface IDistribution
    {
        /// <summary>
        /// Генерация одного случайного числа.
        /// </summary>
        double Generate();

        /// <summary>
        /// Генерация последовательности случайных чисел.
        /// </summary>
        IEnumerable<double> GenerateSequence(int count);

        /// <summary>
        /// Подбор параметров распределения для заданного диапазона.
        /// </summary>
        void FitParameters(double min, double max);

        /// <summary>
        /// Подбор параметров распределения с учетом ожидаемого брака.
        /// </summary>
        void FitParametersWithOutliers(double min, double max, double leftOutlierPercentage, double rightOutlierPercentage);
    }
}