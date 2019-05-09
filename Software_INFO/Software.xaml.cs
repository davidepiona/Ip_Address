using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections;

namespace IP_Address
{
    /// <summary>
    /// Logica di interazione per Software.xaml
    /// ORGANIZZAZIONE:
    /// - COSTRUTTORI
    /// - FUNZIONI PRINCIPALI
    /// - CONTROLLI GENERALI
    /// - BOTTONI
    /// - INTERAZIONE CON L'UTENTE
    /// - MENU DI IMPOSTAZIONI
    /// </summary>
    public partial class Software : Page
    {
        private int ProgSelezionato;
        private bool newProgram = false;
        private bool? obs = null;
        private bool ricercaCompleta = false;
        private bool abilitation = false;
        List<string> tuttiFile = new List<string>();
        List<Ip> indirizziTxt = new List<Ip>();

        /// <summary>
        /// Il costruttore inizializza il componenti. Legge i programmi da file e li scrive nella DataGrid.
        /// </summary>
        public Software()
        {
            InitializeComponent();
            PreviewKeyDown += new KeyEventHandler(PreviewKeyDown2);
            Loaded += Software_Loaded;
            readAddress();
            createList();
            
            //SetVisibility();
            abilitation = true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                            FUNZIONI PRINCIPALI                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void coloraLista()
        {
            dataGrid = this.FindName("dataGrid") as DataGrid;
            for (int i = 0; i < Globals.INDIRIZZI.Count; i++)
            {
                if (Globals.INDIRIZZI[i].presenza)
                {
                    var row = dataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                    row.Background = Brushes.Green;
                }
            }
        }
        /// <summary>
        /// Legge i programmi da file .csv e li salva nella lista programmi.
        /// </summary>
        private void readAddress()
        {
            int j = 0;
            try
            {
                if (Globals.INDIRIZZI == null)
                {
                    Globals.INDIRIZZI = new List<Ip>();
                    Globals.log.Info("Lettura PROGRAMMI.csv");
                    var file = File.OpenRead(Globals.DATI + @"INDIRIZZI.csv");
                    var reader = new StreamReader(file);
                    while (!reader.EndOfStream)
                    {
                        string[] line = reader.ReadLine().Split(';');

                        if(line.Length == 3)
                        {
                            int a = int.Parse(line[0].Substring(0, 3));
                            int b = int.Parse(line[0].Substring(4, 3));
                            int c = int.Parse(line[0].Substring(8, 3));
                            int d = int.Parse(line[0].Substring(12, line[0].Length-12));
                            bool presente = line[2].Equals("True") ? true : false;
                            Globals.INDIRIZZI.Add(new Ip(a, b, c, d, line[1], null, null, presente));
                        }
                        //if (line.Length == 9)
                        //{
                        //    Globals.INDIRIZZI.Add(new Programma(Int32.Parse(line[0]), line[1], line[2], line[3], line[4].Equals("True"), line[5], line[6], line[7], line[8].Equals("True")));
                        //}
                        //j++;
                        //Console.WriteLine("LETTO"+j + "   "+ Int32.Parse(line[0]));
                    }
                    file.Close();
                    foreach (Ip ip in Globals.INDIRIZZI)
                    {
                        Console.WriteLine(ip.ipCompleto + " --- " + ip.descrizione);
                    }
                }
            }
            catch (IOException)
            {
                string msg = "E01 - Il file " + Globals.DATI + @"PROGRAMMI.csv non esiste o è aperto da un altro programma. \n";
                MessageBox.Show(msg, "E01"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Fatal(msg);
            }
            catch (FormatException)
            {
                string msg = "E02 - Il file " + Globals.DATI + @"PROGRAMMI.csv" +
                    " è in un formato non corretto. \nProblema riscontrato all'informazione numero: " + j;
                MessageBox.Show(msg, "E02", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        /// <summary>
        /// Aggiunge la lista di programmi alla DataGrid dopo averla svuotata.
        /// </summary>
        private void createList()
        {
            Console.WriteLine("Create List");
            Globals.log.Info("Create List");
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            dataGrid.EnableRowVirtualization = false;
            dataGrid.SelectionChanged += new SelectionChangedEventHandler(ChangePreview);
            dataGrid.Items.Clear();
            int i = 0;
            foreach (Ip p in Globals.INDIRIZZI)
            {
                dataGrid.Items.Add(p);
                i++;
            }
            
            
            //var itemsSource = dataGrid.ItemsSource as IEnumerable;
            ////if (null == itemsSource) yield return null;
            //foreach (var item in itemsSource)
            //{
            //    dataGrid.EnableRowVirtualization = false;
            //    var row = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
            //    //if (null != row) yield return row;
            //}

            //for (int j = 0; j < dataGrid.Items.Count; j++)
            //{
            //    Console.WriteLine("a");
            //    DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(j);
            //    row.Background = Brushes.Red;
            //}
        }

        /// <summary>
        /// Aggiorna gli elementi nella DataGrid:
        /// - aggiorna le ultime modifiche in programmi
        /// - aggiunge tutti i programmi presenti e FILTRATI dopo aver svuotato la DataGrid
        /// - mentre scorre i programmi restituisce il primo visualizzato per permettere di selezionarlo durante la ricerca
        /// </summary>
        private void updateList(string filter, bool? obsoleti)
        {
            Console.WriteLine("Update list1");
            Globals.log.Info("Update list1");
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.Items.Clear();
                int i = 0;
                foreach (Ip p in Globals.INDIRIZZI)
                {
                    if (p.toName().IndexOf(filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        dataGrid.Items.Add(p);
                    }
                }
                    //if (obsoleti == null || p.obsoleto == obsoleti)
                    //{
                    //    if (p.toName().IndexOf(filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    //    {
                    //        if (i == 0)
                    //        {
                    //            primo = p;
                    //        }
                    //        dataGrid.Items.Add(p);
                    //        i++;
                    //    }
                    //}
                }
        }

        /// <summary>
        /// Aggiorna gli elementi nella DataGrid DOPO AVER CREATO NUOVI PROGRAMMI:
        /// - LEGGE I PROGRAMMI DA FILE (unica cosa in più del precedente)
        /// - aggiorna le ultime modifiche in programmi
        /// - aggiunge tutti i programmi presenti dopo aver svuotato la DataGrid
        /// - seleziona l'ultima informazione della lista (quello appena creato in teoria)
        /// </summary>
        private void readAgainListPrograms(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Console.WriteLine("UpdateList2");
            Globals.log.Info("UpdateList2");
            Globals.INDIRIZZI = null;
            readAddress();
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.Items.Clear();
                int i = 0;
                foreach (Ip p in Globals.INDIRIZZI)
                {
                    dataGrid.Items.Add(p);
                    i++;
                }
                if (newProgram)
                {
                    newProgram = false;
                    dataGrid.SelectedIndex = Globals.INDIRIZZI.Last().id - 1;
                    dataGrid.ScrollIntoView(Globals.INDIRIZZI.Last());
                }
            }
        }

        /// <summary>
        /// Metodo per la riscrittura di Globals.INDIRIZZI nel file PROGRAMMI.csv
        /// </summary>
        private void scriviCSV()
        {
            List<string> lines = new List<string>();
            int i = 0;
            foreach (Ip p in Globals.INDIRIZZI)
            {
                lines.Add(p.ipCompleto + ";" + p.descrizione + ";" + p.presenza.ToString());
                i++;
            }
            try
            {
                File.WriteAllLines(Globals.DATI + "INDIRIZZI.csv", lines);
            }
            catch (IOException)
            {
                string msg = "E03 - errore nella scrittura del file";
                MessageBox.Show(msg, "E03", MessageBoxButton.OK,
                                MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                             CONTROLLI GENERALI                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Dopo aver caricato la pagina da il focus alla textbox
        /// </summary>
        public void Software_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
            coloraLista();
        }

        /// <summary>
        /// Impostazione iniziale della visibilità dei bottoni secondo le impostazioni
        /// </summary>
        private void SetVisibility()
        {
            Console.WriteLine("Set visibility");
            Globals.log.Info("Set visibility");
            //MenuItem ma = this.FindName("Menu_anteprima_check") as MenuItem;
            //ma.IsChecked = Globals.ANTEPRIME;
            if (!Globals.ANTEPRIME)
            {
                RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
                Button button = this.FindName("buttonOpenDocx") as Button;
                richTextBox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
            }
            DataGrid dataGridFiles = this.FindName("dataGridFiles") as DataGrid;
            dataGridFiles.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Metodo per l'aggiornamento del selettore di indice
        /// </summary>
        private void changePreviewForm(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            ChangePreview(dataGrid, null);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                                BOTTONI                                     ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Controlla se esiste un file programma.docx nel programma attualmente visualizzato 
        /// - se esiste: apre il file docx
        /// - se non esiste : crea un file docx con tutte le informazioni del programma
        /// </summary>
        private void Load_Txt_Address(object sender, RoutedEventArgs e)
        {
            Globals.log.Info("Lettura txt");
            var file = File.OpenRead(Globals.DATI + @"export.txt");
            var reader = new StreamReader(file);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split('\t');
                int a = int.Parse(line[0].Substring(0, 3));
                int b = int.Parse(line[0].Substring(4, 3));
                int c = int.Parse(line[0].Substring(8, 3));
                int d = int.Parse(line[0].Substring(12, line[0].Length - 12));
                string MAC = "";
                bool presente = false;
                if (!line[1].Equals(""))
                {
                    MAC = line[1];
                    presente = true;
                }
                indirizziTxt.Add(new Ip(a, b, c, d, null, line[1], null, presente));
            }
            foreach(Ip ip in indirizziTxt)
            {
                Ip ricercaIp = Globals.INDIRIZZI.Find(r => r.id == ip.id);
                if (ricercaIp != null)
                {
                    ricercaIp.confronto = ip.presenza;
                }
            }
            updateList("", null);
        }

        /// <summary>
        /// Bottone che attiva la scrittura del file PROGRAMMI.csv
        /// </summary>
        private void Button_Save(object sender, RoutedEventArgs e)
        {
            scriviCSV();
            Console.WriteLine("Salvato PROGRAMMI.csv");
            Globals.log.Info("Salvato PROGRAMMI.csv");
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                        INTERAZIONE CON L'UTENTE                            ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Al doppio click sulla riga apre la cartella del filesystem.
        /// </summary>
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Funzioni per consentire di navigare la DataGrid con le freccie su e giù mentre si effettua una ricerca.
        /// Con invio si apre la cartella del filesystem dell'informazione selezionata
        /// </summary>
        private void PreviewKeyDown2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                DataGrid dataGrid;
                    dataGrid = this.FindName("dataGrid") as DataGrid;
                dataGrid.Focus();
                if (dataGrid.SelectedIndex > 0)
                {
                    dataGrid.SelectedIndex = dataGrid.SelectedIndex - 1;
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                }
            }
            if (e.Key == Key.Down)
            {
                DataGrid dataGrid;
                    dataGrid = this.FindName("dataGrid") as DataGrid;
                dataGrid.SelectedIndex = dataGrid.SelectedIndex + 1;
                if (dataGrid.SelectedItem != null)
                {
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                }
            }
        }

        /// <summary>
        /// Richiamato ogni volta che cambia la selezione nella DataGrid.
        /// - aggiorna ProgSelezionato.
        /// - carica docx di anteprima (se disponibile)
        /// NullReferenceException spesso sollevata perchè quando si cerca si ricarica la DataGrid e 
        /// per un istante non c'è nessuna informazione selezionata. 
        /// </summary>
        private void ChangePreview(object sender, EventArgs e)
        {
            Console.WriteLine("Change Preview");
            Globals.log.Info("Change Preview");
            //RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
            //Button button = this.FindName("buttonOpenDocx") as Button;
            try
            {
                ProgSelezionato = (((Ip)((DataGrid)sender).SelectedValue).id);
                if (Globals.ANTEPRIME)
                {
                    //richTextBox.Visibility = Visibility.Visible;
                    //button.Visibility = Visibility.Visible;
                    string file = Globals.INDIRIZZIpath + "Id" + ProgSelezionato + @"\programma.docx";
                    if (File.Exists(file))
                    {
                        try
                        {
                            var doc = Xceed.Words.NET.DocX.Load(file);
                            //richTextBox.Document.Blocks.Clear();
                            //richTextBox.AppendText(doc.Text);
                        }catch(IOException)
                        {
                            string msg = "E52 - Il file " + file + " non è accessibile";
                            Globals.log.Error(msg);
                        }
                    }
                    else
                    {
                        //richTextBox.Document.Blocks.Clear();
                        //richTextBox.Visibility = Visibility.Hidden;
                        //button.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (NullReferenceException nre)
            {
                //richTextBox.Visibility = Visibility.Hidden;
                //button.Visibility = Visibility.Visible;
                Globals.log.Warn("Eccezione in changePreview: " + nre);
            }
            if (!Globals.ANTEPRIME)
            {
                //richTextBox.Visibility = Visibility.Hidden;
                //button.Visibility = Visibility.Hidden;
            }

        }

        /// <summary>
        /// Richiamato ogni volta che viene modificato il testo nella TextBox di ricerca.
        /// Aggiorna e filtra la lista e poi seleziona la prima riga visualizzata.
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
                updateList(((TextBox)sender).Text, obs);
                //DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
                //dataGrid.SelectedIndex = 0;
                //dataGrid.ScrollIntoView(p);
        }

        private void updateFileList(string filter)
        {
            Console.WriteLine("Update File list");
            Globals.log.Info("Update File list");
            DataGrid dataGridFiles = this.FindName("dataGridFiles") as DataGrid;
            if (dataGridFiles != null)
            {
                dataGridFiles.Items.Clear();
                //int i = 0;
                foreach (string s in tuttiFile)
                {
                        if (s.IndexOf(filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                        {
                            //if (i == 0)
                            //{
                            //    primo = p;
                            //}
                            dataGridFiles.Items.Add(s);
                            //i++;
                        }
                }
            }
        }

        ///// <summary>
        ///// Controllo cambiamento valore della checkbox che permette di filtrare i programmi per obsoleti e non. 
        ///// Checkbox Checked
        ///// </summary>
        //private void CheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    Globals.log.Info("Checkbox Checked");
        //    obs = true;
        //    if (abilitation)
        //    {
        //        updateList("", obs);
        //    }
        //}

        ///// <summary>
        ///// Controllo cambiamento valore della checkbox che permette di filtrare i programmi per obsoleti e non. 
        ///// Checkbox Unchecked
        ///// </summary>
        //private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    Globals.log.Info("Checkbox Unchecked");
        //    obs = false;
        //    if (abilitation)
        //    {
        //        updateList("", obs);
        //    }
        //}

        /// <summary>
        /// Controllo cambiamento valore della checkbox che permette di filtrare i programmi per obsoleti e non. 
        /// Checkbox Checked
        /// </summary>
        private void CheckBox_Ricerca_Checked(object sender, RoutedEventArgs e)
        {
            Globals.log.Info("Checkbox Ricerca Checked");
            ricercaCompleta = true;
            //foreach(string s in tuttiFile) { 
            //    Console.WriteLine(s);
            //}
            DataGrid dataGridFiles = this.FindName("dataGridFiles") as DataGrid;
            dataGridFiles.Visibility = Visibility.Visible;
            Button BottNuovo = this.FindName("BottNuovo") as Button;
            BottNuovo.Visibility = Visibility.Hidden;
            Button BottModifiche = this.FindName("BottModifiche") as Button;
            BottModifiche.Visibility = Visibility.Hidden;
            Button BottElimina = this.FindName("BottElimina") as Button;
            BottElimina.Visibility = Visibility.Hidden;
            if (dataGridFiles != null)
            {
                dataGridFiles.Items.Clear();
                int i = 0;
                foreach (string f in tuttiFile)
                {
                    dataGridFiles.Items.Add(f);
                    i++;
                }
            }
        }

        /// <summary>
        /// Controllo cambiamento valore della checkbox che permette di filtrare i programmi per obsoleti e non. 
        /// Checkbox Unchecked
        /// </summary>
        private void CheckBox_Ricerca_Unchecked(object sender, RoutedEventArgs e)
        {
            Globals.log.Info("Checkbox Ricerca Unchecked");
            ricercaCompleta = false;
            DataGrid dataGridFiles = this.FindName("dataGridFiles") as DataGrid;
            dataGridFiles.Visibility = Visibility.Hidden;
            Button BottNuovo = this.FindName("BottNuovo") as Button;
            BottNuovo.Visibility = Visibility.Visible;
            Button BottModifiche = this.FindName("BottModifiche") as Button;
            BottModifiche.Visibility = Visibility.Visible;
            Button BottElimina = this.FindName("BottElimina") as Button;
            BottElimina.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Controllo cambiamento valore della checkbox che permette di filtrare i programmi per obsoleti e non. 
        /// Checkbox Indeterminate
        /// </summary>
        private void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            Globals.log.Info("Checkbox Indeterminate");
            obs = null;
            if (abilitation)
            {
                updateList("", obs);
            }
        }
        //ESPERIMENTO PER RICONOSCERE QUANDO CAMBIA LO STATO DELLE CHECKBOX - NON MOLTO FUNZIONANTE
        //private void CheckBoxChanged(object sender, RoutedEventArgs e)
        //{
        //    //bool value = ((DataGridCell)sender).Content.ToString().Split(':').Last().Equals("True") ? true : false;
        //    //Globals.INDIRIZZI[ProgSelezionato].obsoleto = value;
        //    MessageBox.Show("scrivo");
        //    scriviCSV();
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                          MENU DI IMPOSTAZIONI                              ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Apre il FORM Form_percorsi per modificare i path in cui il programma cerca i file.
        /// </summary>
        private void Menu_percorsi(object sender, RoutedEventArgs e)
        {
            Form_percorsi form = new Form_percorsi();
            form.FormClosed
                    += new System.Windows.Forms.FormClosedEventHandler(this.readAgainListPrograms);
            form.ShowDialog();
        }

        /// <summary>
        /// Imposta la visibilità dell'anteprima del docx.
        /// Aggiorna la variabile Globals.ANTEPRIME e scrive sul .csv.
        /// </summary>
        private void Menu_anteprima(object sender, RoutedEventArgs e)
        {
            bool value = ((MenuItem)sender).IsChecked;
            RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
            //Button button = this.FindName("buttonOpenDocx") as Button;
            if (value != Globals.ANTEPRIME)
            {
                Globals.ANTEPRIME = value;
                MainWindow m = new MainWindow();
                m.scriviSETTINGS();
            }
            if (value)
            {
                //button.Visibility = Visibility.Visible;
                richTextBox.Visibility = Visibility.Visible;
                DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
                ChangePreview(dataGrid, null);
            }
            else
            {
                richTextBox.Visibility = Visibility.Hidden;
                //button.Visibility = Visibility.Hidden;
            }

        }

        /// <summary>
        /// Se l'utente conferma crea un file programma.docx per ogni programma con i dati dell'informazione.
        /// </summary>
        private void Menu_Settings(object sender, RoutedEventArgs e)
        {
            Form_Generali form = new Form_Generali(ProgSelezionato);
            form.FormClosed
                    += new System.Windows.Forms.FormClosedEventHandler(this.changePreviewForm);
            form.ShowDialog();
        }
    }
}