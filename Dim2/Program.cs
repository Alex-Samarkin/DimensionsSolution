namespace Dim2
{
    class Program
    {
        static void Main()
        {
            double nominalSize = 25; // мм
            int itGrade = 7;

            double toleranceFormula = ToleranceCalculator.CalculateTolerance(nominalSize, itGrade, ToleranceCalculator.CalculationMode.Formula);
            double toleranceTable = ToleranceCalculator.CalculateTolerance(nominalSize, itGrade, ToleranceCalculator.CalculationMode.Table);

            Console.WriteLine($"Номинальный размер: {nominalSize} мм, IT{itGrade}");
            Console.WriteLine($"По формуле: {toleranceFormula:F2} мкм");
            Console.WriteLine($"По таблице: {toleranceTable:F2} мкм");

            Console.ReadLine();
        }
    }
}
