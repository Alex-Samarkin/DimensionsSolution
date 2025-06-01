using System;
using System.Collections.Generic;
using System.Linq;

namespace Dim4
{
    public class FitCalculator
    {
        public enum FitSystem
        {
            ShaftBasis, // ������� ����
            HoleBasis   // ������� ���������
        }

        /// <summary>
        /// ������������ ������� � ������ �������� ������� (���� ��� ���������) � ������������ �������� �����.
        /// ���������� 1-2 ��������� ����������.
        /// </summary>
        public static List<FitWithTolerance> CalculateFit(
            double nominalSize,
            string baseCode, // ��� �������� �������� (���� ��� ���������)
            FitSystem system,
            int shaftQualityOffset = 0, // �������� ��������� ���� ������������ ��������� (��������, -1 ��� -2)
            int maxResults = 2 // ������������ ���������� �����������
        )
        {
            if (string.IsNullOrWhiteSpace(baseCode))
                throw new ArgumentException("��� �������� �������� �� ����� ���� ������");

            if (nominalSize <= 0 || nominalSize > 500)
                throw new ArgumentException("����������� ������ ������ ���� � ��������� >0 �� 500 ��");

            if (shaftQualityOffset < -5 || shaftQualityOffset > 5)
                throw new ArgumentOutOfRangeException(nameof(shaftQualityOffset), "�������� ��������� ������ ���� � ��������� �� -5 �� 5");

            // ��������� ������� �������
            var baseSize = new SizeWithTolerance(nominalSize, baseCode);

            // ��������� ������ ������� (��� ��� ���������)
            var fits = FindMatchingSizes(nominalSize, baseSize, system, shaftQualityOffset);

            // ���������� 1-2 ��������� ����������
            return fits.Take(maxResults).ToList();
        }

        /// <summary>
        /// ��������� ���������� ������� (���� ��� ���������) � ������ ��������� ��������� � ��������.
        /// </summary>
        private static IEnumerable<FitWithTolerance> FindMatchingSizes(
            double nominalSize,
            SizeWithTolerance baseSize,
            FitSystem system,
            int qualityOffset
        )
        {
            // ����������, ��������� �� ��������� ��� ���
            bool isHole = system == FitSystem.HoleBasis;

            // �������� ������ ���������� �� �������
            var deviations = isHole
                ? ToleranceDataReader.GetHoleDeviations(ToleranceCalculator.GetIntervalKey(nominalSize))
                : ToleranceDataReader.GetShaftDeviations(ToleranceCalculator.GetIntervalKey(nominalSize));

            foreach (var deviation in deviations)
            {
                string code = deviation.Key; // ��� ������� (��������, H7, h6)

                // ��������� ������� ���� �������
                if (string.IsNullOrWhiteSpace(code) || code.Length < 2 || !char.IsLetter(code[0]) || !int.TryParse(code.Substring(1), out _))
                {
                    Console.WriteLine($"�������� ������������ ��� �������: {code}");
                    continue;
                }

                SizeWithTolerance candidate;
                try
                {
                    candidate = new SizeWithTolerance(nominalSize, code);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"������ ��� �������� SizeWithTolerance ��� ���� {code}: {ex.Message}");
                    continue;
                }

                // ��������� �������� ���������
                int baseQuality;
                int candidateQuality;
                try
                {
                    baseQuality = (int)baseSize.GetToleranceData().ToleranceValue;
                    candidateQuality = (int)candidate.GetToleranceData().ToleranceValue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"������ ��� ��������� ������ �������: {ex.Message}");
                    continue;
                }

                if (Math.Abs(candidateQuality - baseQuality) <= Math.Abs(qualityOffset))
                {
                    // ������� ������ FitWithTolerance
                    if (isHole)
                        yield return new FitWithTolerance(baseSize, candidate); // ������� ���������
                    else
                        yield return new FitWithTolerance(candidate, baseSize); // ������� ����
                }
            }
        }

        /// <summary>
        /// ���������� ������ ��������� ����� �������� ��� ��������� ��� �����.
        /// </summary>
        private static IEnumerable<string> GeneratePossibleCodes(bool isHole)
        {
            // ������� ���� ��� ��������� � �����
            var baseCodes = isHole
                ? new[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "M", "N", "P", "R", "S", "T", "U", "V", "X", "Z" } // ���������
                : new[] { "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "m", "n", "p", "r", "s", "t", "u", "v", "x", "z" }; // ����

            // ��������� ����� � ����������� �� IT5 �� IT16
            foreach (var baseCode in baseCodes)
            {
                for (int quality = 5; quality <= 16; quality++) // ��������� IT5-IT16
                {
                    yield return $"{baseCode}{quality}";
                }
            }
        }
    }
}