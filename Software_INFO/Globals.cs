using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP_Address
{
    /// <summary>
    /// Classe statica che contiene oggetti accessibili da tutte le altre classi
    /// </summary>
    public static class Globals
    {
        public static List<Ip> INDIRIZZI;
        public static String INDIRIZZIpath = @"T:\IP_ADDRESS\";
        public static String SETTINGS = @"T:\IP_ADDRESS\IP_ADDRESS\SETTINGS.csv";
        public static String DATI = @"T:\IP_ADDRESS\IP_ADDRESS\DATI\";
        public static String LOG = @"T:\IP_ADDRESS\IP_ADDRESS\";
        public static bool ANTEPRIME = true;
        public static bool DEFAULT = false;
        public static log4net.ILog log;

        //public const Int32 BUFFER_SIZE = 512; // Unmodifiable
        //public static Boolean IS_DATA_DIRTY = false; // Modifiable
        //public static String LAST_PROGRAM; // Modifiable
        //public static List<Clienti> CLIENTI;

        //public static String GITURL = "https://github.com/davidepiona/DATIsync.git";
        //public static String GITPATH = @"C:\Program Files\Git\cmd\git.exe";
        //public static bool SINCRONIZZAZIONE = true;
        //public static String DATIsync = @"C:\Users\attil\source\repos\IP_Address\IP_Address\DATIsync\";
        //public static readonly String DATIsync = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATIsync";// Unmodifiable
    }
}
