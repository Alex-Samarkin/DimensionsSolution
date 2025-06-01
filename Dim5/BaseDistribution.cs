using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace Dim5.Distributions
{
    public abstract class BaseDistribution : IDistribution
    {
        public Random Random { get; } = new Random();

        public abstract double Generate();

        public virtual IEnumerable<double> GenerateSequence(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return Generate();
            }
        }

        public abstract void FitParameters(double min, double max);

        public abstract void FitParametersWithOutliers(double min, double max, double leftOutlierPercentage, double rightOutlierPercentage);

        /// <summary>
        /// Сохраняет последовательность данных в файл CSV, HTML или Excel.
        /// </summary>
        public void ToFile(IEnumerable<double> data, string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".csv":
                    SaveToCsv(data, filePath);
                    break;
                case ".html":
                    SaveToHtml(data, filePath);
                    break;
                case ".xlsx":
                    SaveToExcel(data, filePath);
                    break;
                default:
                    throw new NotSupportedException($"Формат файла {extension} не поддерживается. Используйте .csv, .html или .xlsx.");
            }
        }

        private void SaveToCsv(IEnumerable<double> data, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Index,Value");
                int index = 1;
                foreach (var value in data)
                {
                    writer.WriteLine($"{index},{value:F3}");
                    index++;
                }
            }
        }

        private void SaveToHtml(IEnumerable<double> data, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("<!DOCTYPE html>");
                writer.WriteLine("<html>");
                writer.WriteLine("<head><title>Data</title></head>");
                writer.WriteLine("<body>");
                writer.WriteLine("<table border='1'>");
                writer.WriteLine("<tr><th>Index</th><th>Value</th></tr>");
                int index = 1;
                foreach (var value in data)
                {
                    writer.WriteLine($"<tr><td>{index}</td><td>{value:F3}</td></tr>");
                    index++;
                }
                writer.WriteLine("</table>");
                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
            }
        }

        private void SaveToExcel(IEnumerable<double> data, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Установите лицензию для некоммерческого использования
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Data");
                worksheet.Cells[1, 1].Value = "Index";
                worksheet.Cells[1, 2].Value = "Value";

                int row = 2;
                int index = 1;
                foreach (var value in data)
                {
                    worksheet.Cells[row, 1].Value = index;
                    worksheet.Cells[row, 2].Value = value;
                    row++;
                    index++;
                }

                package.SaveAs(new FileInfo(filePath));
            }
        }
    }
}