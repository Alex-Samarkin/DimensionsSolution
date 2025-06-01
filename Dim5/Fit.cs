using System;

namespace Dim5
{
    public class Fit
    {
        public enum FitType
        {
            Clearance, // Гарантированный зазор
            Interference, // Гарантированный натяг
            Transition // Переходная посадка
        }

        public SizeWithTolerance Shaft { get; }
        public SizeWithTolerance Hole { get; }
        public FitCalculator.FitSystem System { get; }

        public Fit(double nominalSize, string shaftCode, string holeCode, FitCalculator.FitSystem system)
        {
            // Создаем вал и отверстие с одинаковым номинальным размером
            Shaft = new SizeWithTolerance(nominalSize, shaftCode);
            Hole = new SizeWithTolerance(nominalSize, holeCode);
            System = system;
        }

        /// <summary>
        /// Рассчитывает максимальный зазор или натяг.
        /// </summary>
        public double GetMaxClearance()
        {
            return Hole.GetMaxSize() - Shaft.GetMinSize();
        }

        /// <summary>
        /// Рассчитывает минимальный зазор или натяг.
        /// </summary>
        public double GetMinClearance()
        {
            return Hole.GetMinSize() - Shaft.GetMaxSize();
        }

        /// <summary>
        /// Рассчитывает средний зазор или натяг.
        /// </summary>
        public double GetMeanClearance()
        {
            double meanHole = (Hole.GetMaxSize() + Hole.GetMinSize()) / 2;
            double meanShaft = (Shaft.GetMaxSize() + Shaft.GetMinSize()) / 2;
            return meanHole - meanShaft;
        }

        /// <summary>
        /// Определяет тип посадки: зазор, натяг или переходная посадка.
        /// </summary>
        public FitType GetFitType()
        {
            double minClearance = GetMinClearance();
            double maxClearance = GetMaxClearance();

            if (minClearance > 0)
                return FitType.Clearance; // Гарантированный зазор
            if (maxClearance < 0)
                return FitType.Interference; // Гарантированный натяг
            return FitType.Transition; // Переходная посадка
        }

        /// <summary>
        /// Возвращает информацию о посадке в читаемом виде.
        /// </summary>
        public override string ToString()
        {
            return $"Система: {System}\n" +
                   $"Номинальный размер: {Shaft.NominalSize} мм\n" +
                   $"Код вала: {Shaft.Code}, Мин. размер: {Shaft.GetMinSize():F3} мм, Макс. размер: {Shaft.GetMaxSize():F3} мм\n" +
                   $"Код отверстия: {Hole.Code}, Мин. размер: {Hole.GetMinSize():F3} мм, Макс. размер: {Hole.GetMaxSize():F3} мм\n" +
                   $"Тип посадки: {GetFitType()}\n" +
                   $"Минимальный зазор/натяг: {GetMinClearance():F3} мм\n" +
                   $"Средний зазор/натяг: {GetMeanClearance():F3} мм\n" +
                   $"Максимальный зазор/натяг: {GetMaxClearance():F3} мм";
        }
    }
}