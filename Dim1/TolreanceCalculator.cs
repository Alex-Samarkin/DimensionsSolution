using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dim1
{
    public class ToleranceCalculator
    {
        // Квалитет -> коэффициент k (IT = i * k)
        private static readonly Dictionary<int, double> KFactors = new Dictionary<int, double>
    {
        {5, 7}, {6, 10}, {7, 16}, {8, 25}, {9, 40}, {10, 64},
        {11, 100}, {12, 160}, {13, 250}, {14, 400}, {15, 640}, {16, 1000}
    };

        /// <summary>
        /// Возвращает допуск (микрометры) для заданного номинального размера и квалитета
        /// </summary>
        public static double CalculateTolerance(double nominalSize, int itGrade)
        {
            if (!KFactors.ContainsKey(itGrade))
                throw new ArgumentException($"Unsupported IT grade: {itGrade}");

            double D = GetGeometricMean(nominalSize);
            double i = 0.45 * Math.Pow(D, 1.0 / 3) + 0.001 * D;
            double k = KFactors[itGrade];

            return i * k; // допуск в микрометрах
        }

        /// <summary>
        /// Возвращает среднегеометрический размер для интервала
        /// </summary>
        private static double GetGeometricMean(double size)
        {
            if(size<0) size = -size; // Обработка отрицательных значений

            // Диапазоны по ISO 286 для размеров до 500 мм
            var intervals = new List<(double min, double max)>
        {
            (0, 3), (3, 6), (6, 10), (10, 18), (18, 30),
            (30, 50), (50, 80), (80, 120), (120, 180),
            (180, 250), (250, 315), (315, 400), (400, 500)
        };

            foreach (var (min, max) in intervals)
            {
                if (size > min && size <= max)
                    return Math.Sqrt(min * max);
            }

            if (size <= 0 || size > 500)
                throw new ArgumentException("Nominal size must be between >0 and ≤500 mm.");

            return Math.Sqrt(400 * 500); // если точно 500
        }
    }
}