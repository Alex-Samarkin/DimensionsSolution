using System;
using System.Collections.Generic;
using System.Linq;
using static Dim4.FitCalculator;

namespace Dim4
{
    public class FitSelector
    {
        public enum FitCalculationMode
        {
            MinClearance, // Минимальный гарантированный зазор
            MaxClearance, // Максимальный гарантированный зазор
            MeanClearance, // Средний зазор
            MinInterference, // Минимальный гарантированный натяг
            MaxInterference, // Максимальный гарантированный натяг
            MeanInterference // Средний натяг
        }

        public static Fit CalculateFit(
            double nominalSize,
            string baseCode, // Код базового элемента (вала или отверстия)
            FitSystem system,
            FitCalculationMode mode,
            double targetValueMicrons, // Ожидаемый зазор или натяг в мкм
            int qualityOffset = 0 // Смещение квалитета вала относительно отверстия (например, -1 или -2)
        )
        {
            if (string.IsNullOrWhiteSpace(baseCode))
                throw new ArgumentException("Код базового элемента не может быть пустым");

            if (nominalSize <= 0 || nominalSize > 500)
                throw new ArgumentException("Номинальный размер должен быть в диапазоне >0 до 500 мм");

            // Создаем базовый элемент (вал или отверстие)
            var baseSize = new SizeWithTolerance(nominalSize, baseCode);

            // Генерируем возможные коды допусков для противоположного элемента
            var possibleCodes = GeneratePossibleCodes(system == FitSystem.ShaftBasis);

            // Ищем подходящую вилку
            Fit bestFit = null;
            double bestDifference = double.MaxValue;

            foreach (var candidateCode in possibleCodes)
            {
                // Создаем объект Fit в зависимости от системы
                var fit = system == FitSystem.HoleBasis
                    ? new Fit(nominalSize, candidateCode, baseCode, system) // Подбираем вал
                    : new Fit(nominalSize, baseCode, candidateCode, system); // Подбираем отверстие

                // Рассчитываем значение в зависимости от режима
                double calculatedValue = mode switch
                {
                    FitCalculationMode.MinClearance => fit.GetMinClearance(),
                    FitCalculationMode.MaxClearance => fit.GetMaxClearance(),
                    FitCalculationMode.MeanClearance => fit.GetMeanClearance(),
                    FitCalculationMode.MinInterference => -fit.GetMaxClearance(),
                    FitCalculationMode.MaxInterference => -fit.GetMinClearance(),
                    FitCalculationMode.MeanInterference => -fit.GetMeanClearance(),
                    _ => throw new InvalidOperationException("Неизвестный режим расчета")
                };

                // Сравниваем с целевым значением
                double difference = Math.Abs(calculatedValue - targetValueMicrons / 1000.0);
                if (difference < bestDifference)
                {
                    bestFit = fit;
                    bestDifference = difference;
                }
            }

            if (bestFit == null)
                throw new InvalidOperationException("Не удалось найти подходящую посадку");

            return bestFit;
        }

        /// <summary>
        /// Генерирует список возможных кодов допусков для отверстий или валов.
        /// </summary>
        private static IEnumerable<string> GeneratePossibleCodes(bool isHole)
        {
            // Базовые коды для отверстий и валов
            var baseCodes = isHole
                ? new[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "M", "N", "P", "R", "S", "T", "U", "V", "X", "Z" } // Отверстия
                : new[] { "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "m", "n", "p", "r", "s", "t", "u", "v", "x", "z" }; // Валы

            // Генерация кодов с квалитетами от IT5 до IT16
            foreach (var baseCode in baseCodes)
            {
                for (int quality = 5; quality <= 16; quality++) // Квалитеты IT5-IT16
                {
                    yield return $"{baseCode}{quality}";
                }
            }
        }
    }
}