using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace IP_Address
{
    public class Excel
    {
        string path = "";
        int Sheet = 1;
        _Application excel = new _Excel.Application();
        Workbook wb;
        Worksheet ws;

        public Excel(string path)
        {
            this.path = path;
            wb = excel.Workbooks.Open(path);
        }

        public void Open(int Sheet)
        {
            ws = wb.Worksheets[Sheet];
        }

        public string ReadCell(int sheet, int i, int j)
        {
            Worksheet foglio = wb.Worksheets[sheet];
            i++;
            j++;
            if (foglio.Cells[i, j].Value2 != null)
            {
                return foglio.Cells[i, j].Value2.ToString();
            }
            else
            {
                return "";
            }
        }

        public void Close()
        {
            wb.Close();
        }

        public String[] SheetsName()
        {
            String[] excelSheets = new String[wb.Worksheets.Count];
            int i = 0;
            foreach (Worksheet wSheet in wb.Worksheets)
            {
                excelSheets[i] = wSheet.Name;
                Console.WriteLine("Foglio numero: " + i + "  " + excelSheets[i] );
                i++;
            }
            return excelSheets;
        }
    }
}