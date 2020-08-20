using System;
using System.Reflection;

using Microsoft.Office.Interop.Excel;

using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelComInterop
{
    class Program
    {
        const string fileName = "C:\\SinjulMSBH\\MyExcel\\MyTrader.xlsx";
        const string topLeft = "A1";
        const string bottomRight = "A4";
        const string graphTitle = "SinjulMSBH MyTrader Result .. !!!!";
        const string xAxis = "Time";
        const string yAxis = "Value";

        private static Excel.Range GetRange(Excel.Worksheet sh, int row1, int col1, int row2, int col2)
        {
            Excel.Range rng1 = (Excel.Range)sh.Cells[row1, col1];
            Excel.Range rng2 = (Excel.Range)sh.Cells[row2, col2];
            return sh.Range[rng1, rng2];
        }

        public static void Main(string[] args)
        {
            try
            {
                Excel.Application excel;
                Excel.Workbook workbook;
                Excel.Worksheet worksheet;

                excel = new Excel.Application
                {
                    Visible = true,
                    UserControl = false,
                };

                Console.WriteLine("\n\tHello from SinjulMSBH and Application version: ", excel.Version);

                workbook = excel.Workbooks.Add(Missing.Value);
                worksheet = (Excel.Worksheet)workbook.ActiveSheet;

                ChartObjects charts = worksheet.ChartObjects() as ChartObjects;
                ChartObject chartObject = charts.Add(8, 103, 350, 310);
                Chart chart = chartObject.Chart;

                Excel.Range range = worksheet.get_Range(topLeft, bottomRight);
                chart.SetSourceData(range);

                chart.ChartType = XlChartType.xlLine;
                chart.ChartWizard(Source: range,
                    Title: graphTitle,
                    CategoryTitle: xAxis,
                    ValueTitle: yAxis);


                worksheet.Cells[1, 1] = "Number";
                worksheet.Cells[1, 2] = "English";
                worksheet.Cells[1, 3] = "فارسی";

                worksheet.Cells[1, 4] = "Formula | فرمول";

                Excel.Range header = GetRange(worksheet, 1, 1, 1, 4);
                header.Font.Bold = true;

                header.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                header.Interior.Color = System.Drawing.Color.FromArgb(220, 220, 220);

                object[,] values = new object[,] {
                    { 1 , "one"   , "یک" },
                    { 2 , "two"   , "دو" },
                    { 3 , "three" , "سه" },
                    { 4 , "four"  , "چهار" },
                };

                int valuesHeight = values.GetLength(0);
                int valuesWidth = values.GetLength(1);

                GetRange(worksheet,
                  2, 1,
                  2 + valuesHeight - 1, 1 + valuesWidth - 1
                ).Value2 = values;

                Excel.Range formula = GetRange(worksheet, 2, 1 + valuesWidth,
                                                      2 + valuesHeight - 1, 1 + valuesWidth);

                formula.FormulaR1C1 = "=rc[-3] & \": \" & rc[-2] & \"-\" & rc[-1]";

                Excel.Range columns = GetRange(worksheet, 1, 1, 1, 4);
                columns.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                columns.EntireColumn.Font.Color = System.Drawing.Color.DarkSlateGray;
                columns.Font.Color = System.Drawing.Color.DarkSlateBlue;
                columns.EntireColumn.AutoFit();

                Console.WriteLine("Press Enter to close Excel .. !!!!");
                Console.ReadLine();

                excel.Quit();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message} Line: {e.Source}");
            }
        }
    }
}
