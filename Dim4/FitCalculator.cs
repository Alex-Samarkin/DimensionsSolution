using System;
using System.Collections.Generic;
using System.Linq;

namespace Dim4
{
    public class FitCalculator
    {
        public enum FitSystem
        {
            ShaftBasis, // Система вала
            HoleBasis   // Система отверстия
        }

        /// <summary>
        /// Рассчитывает посадку с учетом заданной системы (вала или отверстия) и опциональной точности валов.
        /// Возвращает 1-2 ближайших результата.
        /// </summary>
        public static List<FitWithTolerance> CalculateFit(
            double nominalSize,
            string baseCode, // Код базового элемента (вала или отверстия)
            FitSystem system,
            int shaftQualityOffset = 0, // Смещение квалитета вала относительно отверстия (например, -1 или -2)
            int maxResults = 2 // Максимальное количество предложений
        )
        {
            if (string.IsNullOrWhiteSpace(baseCode))
                throw new ArgumentException("Код базового элемента не может быть пустым");

            if (nominalSize <= 0 || nominalSize > 500)
                throw new ArgumentException("Номинальный размер должен быть в диапазоне >0 до 500 мм");

            if (shaftQualityOffset < -5 || shaftQualityOffset > 5)
                throw new ArgumentOutOfRangeException(nameof(shaftQualityOffset), "Смещение квалитета должно быть в диапазоне от -5 до 5");

            // Разбираем базовый элемент
            var baseSize = new SizeWithTolerance(nominalSize, baseCode);

            // Подбираем второй элемент (вал или отверстие)
            var fits = FindMatchingSizes(nominalSize, baseSize, system, shaftQualityOffset);

            // Возвращаем 1-2 ближайших результата
            return fits.Take(maxResults).ToList();
        }

        /// <summary>
        /// Подбирает подходящие размеры (валы или отверстия) с учетом заданного квалитета и смещения.
        /// </summary>
        private static IEnumerable<FitWithTolerance> FindMatchingSizes(
            double nominalSize,
            SizeWithTolerance baseSize,
            FitSystem system,
            int qualityOffset
        )
        {
            // Определяем, подбираем ли отверстие или вал
            bool isHole = system == FitSystem.HoleBasis;

            // Получаем данные отклонений из таблицы
            var deviations = isHole
                ? ToleranceDataReader.GetHoleDeviations(ToleranceCalculator.GetIntervalKey(nominalSize))
                : ToleranceDataReader.GetShaftDeviations(ToleranceCalculator.GetIntervalKey(nominalSize));

            foreach (var deviation in deviations)
            {
                string code = deviation.Key; // Код допуска (например, H7, h6)

                // Валидация формата кода допуска
                if (string.IsNullOrWhiteSpace(code) || code.Length < 2 || !char.IsLetter(code[0]) || !int.TryParse(code.Substring(1), out _))
                {
                    Console.WriteLine($"Пропущен некорректный код допуска: {code}");
                    continue;
                }

                SizeWithTolerance candidate;
                try
                {
                    candidate = new SizeWithTolerance(nominalSize, code);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при создании SizeWithTolerance для кода {code}: {ex.Message}");
                    continue;
                }

                // Проверяем смещение квалитета
                int baseQuality;
                int candidateQuality;
                try
                {
                    baseQuality = (int)baseSize.GetToleranceData().ToleranceValue;
                    candidateQuality = (int)candidate.GetToleranceData().ToleranceValue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при получении данных допуска: {ex.Message}");
                    continue;
                }

                if (Math.Abs(candidateQuality - baseQuality) <= Math.Abs(qualityOffset))
                {
                    // Создаем объект FitWithTolerance
                    if (isHole)
                        yield return new FitWithTolerance(baseSize, candidate); // Система отверстия
                    else
                        yield return new FitWithTolerance(candidate, baseSize); // Система вала
                }
            }
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