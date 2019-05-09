using IP_Address;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IP_Address
{
    /// <summary>
    /// Form che permette di eseguire due operazioni:
    /// - creazione di file docx in ogni cartella di informazioni in cui manca
    /// - eliminazione delle cartelle vuote presenti nelle cartelle dei programmi
    /// </summary>
    public partial class Form_Generali : Form
    {
        private int ProgSelezionato;
        private int countFolder;
        private int countDocx;

        /// <summary>
        /// Costruttore che inizializza gli attributi 
        /// </summary>
        public Form_Generali(int ProgSelezionato)
        {
            this.ProgSelezionato = ProgSelezionato;
            this.countFolder = 0;
            this.countDocx = 0;
            InitializeComponent();
        }

        /// <summary>
        /// Metodo per la creazione dei file docx per ogni informazione
        /// - chiede all'utente se vuole procedere
        /// - disabilita tutti i pulsanti
        /// - per ogni informazione, se il file non esiste, lo crea
        /// - informa del numero di file creati
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.MessageBoxResult dialogResult = System.Windows.MessageBox.Show("Sei sicuro di voler CREARE un file programma.docx in ogni programma?",
                "Creare TUTTI i DOCX?", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
            if (dialogResult == System.Windows.MessageBoxResult.Yes)
            {
                //button1.Enabled = false;
                button2.Enabled = false;
                button4.Enabled = false;
                //pictureBox1.Enabled = false;
                pictureBox2.Enabled = false;
                countDocx = 0;
                foreach (Ip p in Globals.INDIRIZZI)
                {
                    if (p != null)
                    {
                    }
                }
                //button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;
                //pictureBox1.Enabled = true;
                pictureBox2.Enabled = true;
                string msg2 = "Terminata la creazione dei file docx. File creati: " + countDocx;
                Globals.log.Info(msg2);
                System.Windows.MessageBox.Show(msg2, "File creati", System.Windows.MessageBoxButton.OK,
                       System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
            }
        }

        /// <summary>
        /// Metodo per l'eliminazione delle cartelle vuote
        /// - chiede all'utente se vuole procedere
        /// - disabilita tutti i pulsanti
        /// - per ogni cartella, ricorsivamente elimina le cartelle vuote interne presenti
        /// - informa del numero di cartelle eliminate
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            System.Windows.MessageBoxResult dialogResult = System.Windows.MessageBox.Show("Sei sicuro di voler ELIMINARE tutte le cartelle e sottocartelle vuote presenti nel percorso " + Globals.INDIRIZZIpath + " ?",
                "Eliminare tutte le cartelle vuote?", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
            if (dialogResult == System.Windows.MessageBoxResult.Yes)
            {
                //button1.Enabled = false;
                button2.Enabled = false;
                button4.Enabled = false;
                //pictureBox1.Enabled = false;
                pictureBox2.Enabled = false;
                countFolder = 0;
                try
                {
                    processDirectory(Globals.INDIRIZZIpath);
                }
                catch (IOException ioe)
                {
                    string msg = "E30 - errore nella eliminazione delle cartelle vuote" + ioe;
                    System.Windows.MessageBox.Show(msg, "E30", System.Windows.MessageBoxButton.OK,
                                    System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg);
                }
                //button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;
                //pictureBox1.Enabled = true;
                pictureBox2.Enabled = true;
                string msg2 = "Cartelle vuote eliminate: " + countFolder;
                Globals.log.Info(msg2);
                System.Windows.MessageBox.Show(msg2, "Cartelle eliminate", System.Windows.MessageBoxButton.OK,
                       System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
            }
        }

        /// <summary>
        /// Funzione ausiliaria per la ricorsione nelle cartelle
        /// </summary>
        private void processDirectory(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                processDirectory(directory);
                var folder = new DirectoryInfo(directory);
                if (folder.GetFileSystemInfos().Length == 0)
                {
                    try
                    {
                        Directory.Delete(directory, false);
                        countFolder++;
                    }
                    catch (Exception e)
                    {
                        Globals.log.Error("Errore nell'eliminazione della cartella " + directory + "\n " + e);
                    }
                }
            }
        }

        /// <summary>
        /// Bottone per la chiusura del form
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
