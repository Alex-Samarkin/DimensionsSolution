using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dim4
{
    public class SizeWithTolerance
    {
        public double NominalSize { get; }
        public string Code { get; }

        private readonly string _deviation; // Изменено на string для поддержки двухбуквенных кодов
        private readonly int _itGrade;

        public SizeWithTolerance(double nominalSize, string code)
        {
            if (nominalSize <= 0 || nominalSize > 500)
                throw new ArgumentException("Номинальный размер должен быть в диапазоне >0 до 500 мм");

            NominalSize = nominalSize;
            Code = code;

            var parsed = ParseCode(Code);
            _deviation = parsed.deviation;
            _itGrade = parsed.itGrade;
        }

        private (string deviation, int itGrade) ParseCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Неверный формат кода допуска");

            // Определяем длину отклонения (1 или 2 символа)
            int deviationLength = char.IsLetter(code[1]) ? 2 : 1;

            if (code.Length <= deviationLength)
                throw new ArgumentException("Неверный формат кода допуска");

            string deviation = code.Substring(0, deviationLength);
            string gradeStr = code.Substring(deviationLength);

            if (!int.TryParse(gradeStr, out int itGrade))
                throw new ArgumentException("Не удалось извлечь квалитет");

            return (deviation, itGrade);
        }

        public ToleranceInfo GetToleranceData()
        {
            string intervalKey = ToleranceCalculator.GetIntervalKey(NominalSize);

            bool isHole = char.IsUpper(_deviation[0]); // Проверяем первый символ отклонения

            double basicTol = ToleranceCalculator.CalculateTolerance(NominalSize, _itGrade, ToleranceCalculator.CalculationMode.Table);

            double fundamental = 0;

            if (isHole)
            {
                if (!ToleranceDataReader.GetHoleDeviations(intervalKey).TryGetValue(_deviation, out fundamental))
                    throw new KeyNotFoundException($"Отклонение {_deviation} не найдено для отверстия");
            }
            else
            {
                var devKey = _deviation.ToLower();
                if (!ToleranceDataReader.GetShaftDeviations(intervalKey).TryGetValue(devKey, out fundamental))
                    throw new KeyNotFoundException($"Отклонение {_deviation} не найдено для вала");
            }

            double upper = isHole ? fundamental + basicTol : fundamental;
            double lower = isHole ? fundamental : fundamental - Math.Abs(basicTol);

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
