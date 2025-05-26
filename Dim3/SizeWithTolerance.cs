using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dim3
{
    public class SizeWithTolerance
    {
        public double NominalSize { get; }
        public string Code { get; }

        private readonly char _deviation;
        private readonly int _itGrade;

        public SizeWithTolerance(double nominalSize, string code)
        {
            if (nominalSize <= 0 || nominalSize > 500)
                throw new ArgumentException("Номинальный размер должен быть в диапазоне >0 до 500 мм");

            NominalSize = nominalSize;
            Code = code; // Убрано ToUpper()

            var parsed = ParseCode(Code);
            _deviation = parsed.deviation;
            _itGrade = parsed.itGrade;
        }

        private (char deviation, int itGrade) ParseCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length < 2)
                throw new ArgumentException("Неверный формат кода допуска");

            char deviation = code[0];
            string gradeStr = code.Substring(1);

            if (!int.TryParse(gradeStr, out int itGrade))
                throw new ArgumentException("Не удалось извлечь квалитет");

            return (deviation, itGrade);
        }

        public ToleranceInfo GetToleranceData()
        {
            string intervalKey = ToleranceCalculator.GetIntervalKey(NominalSize);

            // Console.WriteLine($"Interval Key: {intervalKey}");

            bool isHole = char.IsUpper(_deviation);

            // Console.WriteLine($"Deviation: {_deviation}, IsHole: {isHole}");

            double basicTol = ToleranceCalculator.CalculateTolerance(NominalSize, _itGrade, ToleranceCalculator.CalculationMode.Table);

            double fundamental = 0;

            if (isHole)
            {
                if (!ToleranceDataReader.GetHoleDeviations(intervalKey).TryGetValue(_deviation.ToString(), out fundamental))
                    throw new KeyNotFoundException($"Отклонение {_deviation} не найдено для отверстия");
            }
            else
            {
                var devKey = _deviation.ToString().ToLower();
                if (!ToleranceDataReader.GetShaftDeviations(intervalKey).TryGetValue(devKey, out fundamental))
                    throw new KeyNotFoundException($"Отклонение {_deviation} не найдено для вала");
            }

            double upper = isHole ? fundamental + basicTol : fundamental;
            double lower = isHole ? fundamental : fundamental - Math.Abs(basicTol);

            // Логирование для отладки
            // Console.WriteLine($"Interval: {intervalKey}, Fundamental: {fundamental}, BasicTol: {basicTol}, Upper: {upper}, Lower: {lower}");

            return new ToleranceInfo
            {
                UpperDeviation = upper,
                LowerDeviation = lower
            };
        }

        public double GetMaxSize() => NominalSize + GetToleranceData().UpperDeviation / 1000;
        public double GetMinSize() => NominalSize + GetToleranceData().LowerDeviation / 1000;
    }
}
