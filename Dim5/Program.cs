using Dim5;

namespace Dim4
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await ToleranceDataReader.LoadDataAsync();

            var size1 = new SizeWithTolerance(54, "H8");
            var size2 = new SizeWithTolerance(54, "f7");

            Console.WriteLine("Ø54 H8:");
            Console.WriteLine(size1.GetToleranceData());
            Console.WriteLine($"Мин. размер: {size1.GetMinSize():F3} мм");
            Console.WriteLine($"Макс. размер: {size1.GetMaxSize():F3} мм");

            Console.WriteLine("\nØ54 f7:");
            Console.WriteLine(size2.GetToleranceData());
            Console.WriteLine($"Мин. размер: {size2.GetMinSize():F3} мм");
            Console.WriteLine($"Макс. размер: {size2.GetMaxSize():F3} мм");

            Fit fit = new Fit(54, "t7", "H8", FitCalculator.FitSystem.HoleBasis);

            Console.WriteLine(fit.ToString());

            // FitSelector fitSelector = new FitSelector();
            /// explain parameters
            /// 
            Console.WriteLine(new string('-', 64));
            var fits = FitSelector.CalculateFit(54,                                           // Номинальный размер в мм
                                                "H8",                                         // Код базового элемента (вала или отверстия)
                                                FitCalculator.FitSystem.HoleBasis,            // Система посадки (вал или отверстие)
                                                FitSelector.FitCalculationMode.MeanClearance, // Режим расчета (средний зазор)
                                                -50, // Ожидаемый зазор или натяг в мкм
                                                -1  // Смещение квалитета вала относительно отверстия (например, -1 или -2)
                                                );
            Console.WriteLine(fits.ToString());

            double[] dataX = { 1, 2, 3, 4, 5 };
            double[] dataY = { 1, 4, 9, 16, 25 };

            ScottPlot.Plot myPlot = new();
            myPlot.Add.Scatter(dataX, dataY);
            Console.WriteLine(myPlot.ToString());

        }
    }
}
