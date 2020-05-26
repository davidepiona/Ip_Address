using IP_Address;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IP_ADDRESS
{
    /// <summary>
    /// Logica di interazione per lista_nomi_excel.xaml
    /// </summary>
    public partial class lista_nomi_excel : Window
    {
        private Excel excel;
        List<Sheet> listaFogli = new List<Sheet>();
        public lista_nomi_excel(Excel excel, String[] lista)
        {
            this.excel = excel;
            InitializeComponent();

            Console.WriteLine("aperta pagina");
            foreach (String s in lista)
            {
                Console.WriteLine(s);
            }

            DataGrid dataGrid = this.FindName("datagrid_excel") as DataGrid;
            //Sheet[] listaFogli = new Sheet[lista.Length];
            
            int i = 0;
            foreach (string s in lista)
            {
                listaFogli.Add(new Sheet(i, lista[i], false));
                i++;
            }
            dataGrid.ItemsSource = listaFogli;           

        }

        public void Select_and_Close(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("bottone premuto - lettura excel");
            var fogliDaLeggere = new List<int>();
            foreach (Sheet s in listaFogli)
            {
                if (s.Seleziona)
                    fogliDaLeggere.Add(s.Numero + 1);
            }
            
            foreach (int foglio in fogliDaLeggere)
            {
                Console.WriteLine("LETTURA FOGLIO " + foglio);
                for (int i = 1; i < 255; i++)
                {
                    string indirizzo = excel.ReadCell(foglio, i, 1);
                    Console.WriteLine(i + ") " + indirizzo);
                    int a = int.Parse(indirizzo.Substring(0, 3));
                    int b = int.Parse(indirizzo.Substring(4, 3));
                    int c = int.Parse(indirizzo.Substring(8, 3));
                    int d = int.Parse(indirizzo.Substring(12, indirizzo.Length - 12));
                    int presente = int.Parse(excel.ReadCell(foglio, i, 3));
                    var ip = new Ip(a, b, c, d, excel.ReadCell(foglio, i, 2), null, null, presente);
                    Console.WriteLine(ip.descrizione + " " + ip.a, ip.b, ip.c, ip.d);
                    Globals.INDIRIZZI.Add(ip);
                }
            }
            excel.Close();
            this.Close();
        }
    }
}
