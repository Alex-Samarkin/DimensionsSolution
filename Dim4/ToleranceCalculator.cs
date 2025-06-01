using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dim4
{
    public static class ToleranceCalculator
    {
        // Формула единицы допуска: i = 0.45 * ∛D + 0.001 * D
        private static readonly Dictionary<int, double> KFactors = new Dictionary<int, double>
    {
        {5, 7}, {6, 10}, {7, 16}, {8, 25}, {9, 40}, {10, 64},
        {11, 100}, {12, 160}, {13, 250}, {14, 400}, {15, 640}, {16, 1000}
    };

        public enum CalculationMode
        {
            Formula,
            Table
        }

        public static double CalculateTolerance(double nominalSize, int itGrade, CalculationMode mode = CalculationMode.Formula)
        {
            if (nominalSize <= 0 || nominalSize > 500)
                throw new ArgumentException("Номинальный размер должен быть в диапазоне >0 до 500 мм.");

            if (mode == CalculationMode.Table)
            {
                string intervalKey = GetIntervalKey(nominalSize);
                var tableTolerances = new Dictionary<string, Dictionary<int, double>>
                {
                    ["0-3"] = new() { { 5, 4 }, { 6, 6 }, { 7, 10 }, { 8, 14 }, { 9, 25 }, { 10, 40 }, { 11, 60 }, { 12, 100 }, { 13, 140 }, { 14, 250 }, { 15, 400 }, { 16, 600 } },
                    ["3-6"] = new() { { 5, 4.5 }, { 6, 7 }, { 7, 12 }, { 8, 18 }, { 9, 30 }, { 10, 48 }, { 11, 75 }, { 12, 120 }, { 13, 180 }, { 14, 300 }, { 15, 480 }, { 16, 750 } },
                    ["6-10"] = new() { { 5, 5.5 }, { 6, 8 }, { 7, 15 }, { 8, 22 }, { 9, 36 }, { 10, 58 }, { 11, 90 }, { 12, 150 }, { 13, 220 }, { 14, 360 }, { 15, 580 }, { 16, 900 } },
                    ["10-18"] = new() { { 5, 6.5 }, { 6, 9 }, { 7, 18 }, { 8, 27 }, { 9, 43 }, { 10, 70 }, { 11, 110 }, { 12, 180 }, { 13, 270 }, { 14, 430 }, { 15, 700 }, { 16, 1100 } },
                    ["18-30"] = new() { { 5, 8 }, { 6, 11 }, { 7, 21 }, { 8, 33 }, { 9, 52 }, { 10, 84 }, { 11, 130 }, { 12, 210 }, { 13, 330 }, { 14, 520 }, { 15, 840 }, { 16, 1300 } },
                    ["30-50"] = new() { { 5, 9.5 }, { 6, 13 }, { 7, 25 }, { 8, 39 }, { 9, 62 }, { 10, 100 }, { 11, 160 }, { 12, 250 }, { 13, 390 }, { 14, 620 }, { 15, 1000 }, { 16, 1600 } },
                    ["50-80"] = new() { { 5, 11 }, { 6, 15 }, { 7, 30 }, { 8, 46 }, { 9, 74 }, { 10, 120 }, { 11, 190 }, { 12, 300 }, { 13, 460 }, { 14, 740 }, { 15, 1200 }, { 16, 1900 } },
                    ["80-120"] = new() { { 5, 13 }, { 6, 18 }, { 7, 35 }, { 8, 54 }, { 9, 87 }, { 10, 140 }, { 11, 220 }, { 12, 350 }, { 13, 540 }, { 14, 870 }, { 15, 1400 }, { 16, 2200 } },
                    ["120-180"] = new() { { 5, 15 }, { 6, 20 }, { 7, 40 }, { 8, 63 }, { 9, 100 }, { 10, 160 }, { 11, 250 }, { 12, 400 }, { 13, 630 }, { 14, 1000 }, { 15, 1600 }, { 16, 2500 } },
                    ["180-250"] = new() { { 5, 17 }, { 6, 23 }, { 7, 46 }, { 8, 72 }, { 9, 115 }, { 10, 185 }, { 11, 290 }, { 12, 460 }, { 13, 720 }, { 14, 1150 }, { 15, 1850 }, { 16, 2900 } },
                    ["250-315"] = new() { { 5, 19 }, { 6, 25 }, { 7, 52 }, { 8, 81 }, { 9, 130 }, { 10, 210 }, { 11, 320 }, { 12, 520 }, { 13, 810 }, { 14, 1300 }, { 15, 2100 }, { 16, 3200 } },
                    ["315-400"] = new() { { 5, 21 }, { 6, 27 }, { 7, 57 }, { 8, 89 }, { 9, 140 }, { 10, 230 }, { 11, 360 }, { 12, 570 }, { 13, 890 }, { 14, 1400 }, { 15, 2300 }, { 16, 3600 } },
                    ["400-500"] = new() { { 5, 23 }, { 6, 32 }, { 7, 63 }, { 8, 97 }, { 9, 155 }, { 10, 250 }, { 11, 400 }, { 12, 630 }, { 13, 970 }, { 14, 1550 }, { 15, 2500 }, { 16, 4000 } }
                };

                if (!tableTolerances.TryGetValue(intervalKey, out var grades) ||
                    !grades.TryGetValue(itGrade, out var tolerance))
                {
                    throw new InvalidOperationException($"Данные для IT{itGrade} в диапазоне {intervalKey} отсутствуют.");
                }

                return tolerance;
            }
            else
            {
                double D = GetGeometricMean(nominalSize);
                double i = 0.45 * Math.Pow(D, 1.0 / 3) + 0.001 * D;

                if (!KFactors.TryGetValue(itGrade, out double k))
                    throw new ArgumentException($"Квалитет {itGrade} не поддерживается.");

                return i * k;
            }
        }

        private static double GetGeometricMean(double size)
        {
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

            return Math.Sqrt(400 * 500); // если точно 500
        }

        internal static string GetIntervalKey(double size)
        {
            if (size > 0 && size <= 3) return "0-3";
            else if (size <= 6) return "3-6";
            else if (size <= 10) return "6-10";
            else if (size <= 18) return "10-18";
            else if (size <= 30) return "18-30";
            else if (size <= 50) return "30-50";
            else if (size <= 80) return "50-80";
            else if (size <= 120) return "80-120";
            else if (size <= 180) return "120-180";
            else if (size <= 250) return "180-250";
            else if (size <= 315) return "250-315";
            else if (size <= 400) return "315-400";
            else if (size <= 500) return "400-500";
            else throw new ArgumentOutOfRangeException();
        }
    }
}
