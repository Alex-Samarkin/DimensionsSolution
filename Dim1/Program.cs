using Dim1;

class Program
{
    static void Main()
    {
        double nominalSize = 25; // мм
        int itGrade = 7;

        double tolerance = ToleranceCalculator.CalculateTolerance(nominalSize, itGrade);

        Console.WriteLine($"Допуск для размера {nominalSize} мм по квалитету IT{itGrade}: {tolerance:F2} мкм");
        // Допуск для размера 25 мм по квалитету IT7: 16.00 мкм
        tolerance = ToleranceCalculator.CalculateTolerance(nominalSize, itGrade+2);
        Console.WriteLine($"Допуск для размера {nominalSize} мм по квалитету IT{itGrade+2}: {tolerance:F2} мкм");

        Console.WriteLine("Нажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}