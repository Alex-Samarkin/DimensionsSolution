namespace Dim3
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await ToleranceDataReader.LoadDataAsync();

            var size1 = new SizeWithTolerance(25, "H7");
            var size2 = new SizeWithTolerance(25, "h6");
            var size3 = new SizeWithTolerance(25, "F8");
            var size4 = new SizeWithTolerance(25, "f8");

            Console.WriteLine("Ø25 H7:");
            Console.WriteLine(size1.GetToleranceData());
            Console.WriteLine($"Мин. размер: {size1.GetMinSize():F3} мм");
            Console.WriteLine($"Макс. размер: {size1.GetMaxSize():F3} мм");

            Console.WriteLine("\nØ25 h6:");
            Console.WriteLine(size2.GetToleranceData());
            Console.WriteLine($"Мин. размер: {size2.GetMinSize():F3} мм");
            Console.WriteLine($"Макс. размер: {size2.GetMaxSize():F3} мм");

            Console.WriteLine("Ø25 F8:");
            Console.WriteLine(size3.GetToleranceData());
            Console.WriteLine($"Мин. размер: {size3.GetMinSize():F3} мм");
            Console.WriteLine($"Макс. размер: {size3.GetMaxSize():F3} мм");

            Console.WriteLine("\nØ25 f8:");
            Console.WriteLine(size4.GetToleranceData());
            Console.WriteLine($"Мин. размер: {size4.GetMinSize():F3} мм");
            Console.WriteLine($"Макс. размер: {size4.GetMaxSize():F3} мм");

        }
    }
}
