using MathNet.Numerics.Distributions;

namespace Dim5.Distributions
{
    public class SkewedGeneralizedTDistribution : BaseDistribution
    {
        private double Location { get; set; } // Центр распределения
        private double Scale { get; set; }    // Масштаб (ширина)
        private double Skewness { get; set; } // Асимметрия
        private double Shape { get; set; }    // Форма (параметр хвостов)
        private double Q { get; set; }    // Эксцесс

        // private Random Random { get; } // Добавляем поле Random для генерации случайных чисел

        public SkewedGeneralizedTDistribution(double location = 0, double scale = 1, double skewness = 0, double shape = 5, double q = 1)
        {
            Location = location;
            Scale = scale;
            Skewness = skewness;
            Shape = shape;
            Q = q; // Эксцесс, по умолчанию 1            
            //Random = new Random(); // Инициализируем Random
        }

        public override double Generate()
        {
            // Используем встроенное распределение SkewedGeneralizedT
            return SkewedGeneralizedT.Sample(Random, Location, Scale, Skewness, Shape,Q);
        }

        public override void FitParameters(double min, double max)
        {
            Location = (min + max) / 2; // Центр распределения
            Scale = (max - min) / 6;    // 99.7% данных лежат в пределах ±3σ
            Skewness = 0;               // Начальное приближение
            Shape = 5;                  // Стандартное значение для хвостов
        }

        public override void FitParametersWithOutliers(double min, double max, double leftOutlierPercentage, double rightOutlierPercentage)
        {
            Location = (min + max) / 2;
            Scale = (max - min) / 6;

            // Подбор асимметрии и формы хвостов на основе процентного распределения
            double leftBound = leftOutlierPercentage / 100.0;
            double rightBound = 1 - (rightOutlierPercentage / 100.0);

            Skewness = (rightBound - leftBound) / 2; // Примерный расчет асимметрии
            Shape = 5; // Оставляем стандартное значение для хвостов
        }
    }
}