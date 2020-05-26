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
using System.Security;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using IP_ADDRESS;

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
        List<Ip> filterList = new List<Ip>();
        private bool cambiamento = true;
        private int pageNumber=1;
        private bool EnableColori = false;

        /// <summary>
        /// Il costruttore inizializza il componenti. Legge i programmi da file e li scrive nella DataGrid.
        /// </summary>
        public Software()
        {
            Console.WriteLine("Inizializzazione");
            InitializeComponent();
            dataGrid = this.FindName("dataGrid") as DataGrid;
            ComboBox PageNumber = this.FindName("PageNumberComboBox") as ComboBox;
            PageNumber.SelectedIndex = 0;
            //PreviewKeyDown += new KeyEventHandler(PreviewKeyDown2);
            Loaded += Software_Loaded;
            Task.Factory.StartNew(() =>
            {
                readAddressCSV("");
            }).ContinueWith(task =>
            {
                createList();
            dataGrid.LayoutUpdated += new EventHandler(coloraLista);
            abilitation = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                            FUNZIONI PRINCIPALI                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void coloraLista(object sender, EventArgs e)
        {
            //Console.WriteLine("ColoraLista");
            if (cambiamento && EnableColori)
            {
                Console.WriteLine("COLORALISTA");
                dataGrid = this.FindName("dataGrid") as DataGrid;
                var row = dataGrid.ItemContainerGenerator.ContainerFromIndex(filterList.Count - 1) as DataGridRow;
                if (row == null)
                {
                    return;
                }

                for (int i = 0; i < filterList.Count; i++)
                {
                    row = dataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                    if (row != null)
                    {
                        //Console.WriteLine(filterList[i].confronto);
                        //if (filterList[i].presenza == 1 && (filterList[i].confronto == 1 || filterList[i].confronto == 0))
                        //{
                        //    row.Background = Brushes.LightGreen;
                        //}
                        //else if (filterList[i].presenza == 1 && filterList[i].confronto == 2)
                        //{
                        //    row.Background = Brushes.Yellow;
                        //}
                        //else if (filterList[i].presenza == 0 && filterList[i].confronto == 1)
                        //{
                        //    row.Background = Brushes.Red;
                        //}
                        //else
                        //{
                        //    row.Background = Brushes.White;
                        //}
                        if (filterList[i].presenza == 1)
                        {
                            row.Background = Brushes.Yellow;
                        }
                        else if (filterList[i].presenza == 2)
                        {
                            row.Background = Brushes.LightGreen;
                        }
                        else if (filterList[i].presenza == 3)
                        {
                            row.Background = Brushes.Red;
                        }
                        else
                        {
                            row.Background = Brushes.White;
                        }
                    }
                }
                cambiamento = false;
            }
        }
        /// <summary>
        /// Legge i programmi da file .csv e li salva nella lista programmi.
        /// </summary>
        private void readAddressCSV(string path)
        {
            Console.WriteLine("ReadAddressCSV");
            try
            {
                if (Globals.INDIRIZZI == null)
                {
                    Globals.INDIRIZZI = new List<Ip>();
                }
                Globals.INDIRIZZI.Clear();
                if (path.Equals(""))
                {
                    path = Globals.DATI + @"INDIRIZZI.csv";
                }
                Globals.log.Info("Lettura di: " + path);
                var file = File.OpenRead(path);
                var reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(';');

                    if (line.Length == 3)
                    {
                        int a = int.Parse(line[0].Substring(0, 3));
                        int b = int.Parse(line[0].Substring(4, 3));
                        int c = int.Parse(line[0].Substring(8, 3));
                        int d = int.Parse(line[0].Substring(12, line[0].Length - 12));
                        int presente = int.Parse(line[2]);
                        Globals.INDIRIZZI.Add(new Ip(a, b, c, d, line[1], null, null, presente));
                    }
                }
                file.Close();
                confronto();
            }
            catch (IOException)
            {
                string msg = "E01 - Il file " + path + " non esiste o è aperto da un altro programma. \n";
                MessageBox.Show(msg, "E01"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Fatal(msg);
            }
            catch (FormatException)
            {
                string msg = "E02 - Il file " + path + " è in un formato non corretto.";
                MessageBox.Show(msg, "E02", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }
        private void ReadAddressTxt(string path)
        {
            Globals.log.Info("Lettura txt");
            try
            {
                if (indirizziTxt == null)
                {
                    indirizziTxt = new List<Ip>();
                }
                indirizziTxt.Clear();
                Console.WriteLine("PERCORSO: " + path);
                if (path.Equals(""))
                {
                    path = Globals.DATI + @"export.txt";
                }
                Console.WriteLine(Globals.DATI + @"export.txt");
                Globals.log.Info("Lettura di: " + path);
                var file = File.OpenRead(path);
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
                    int presente = 1;
                    if (!line[1].Equals(""))
                    {
                        MAC = line[1];
                        presente = 2;
                    }
                    indirizziTxt.Add(new Ip(a, b, c, d, null, MAC, null, presente));
                }
                file.Close();
                confronto();
            }
            catch (IOException)
            {
                string msg = "E11 - Il file " + path + " non esiste o è aperto da un altro programma. \n";
                MessageBox.Show(msg, "E01"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Fatal(msg);
            }
            catch (FormatException)
            {
                string msg = "E12 - Il file " + path + " è in un formato non corretto.";
                MessageBox.Show(msg, "E02", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        public void readAddressEXCEL(string path)
        {
            Console.WriteLine("ReadAddressEXCEL");
            try
            {
                if (Globals.INDIRIZZI == null)
                {
                    Globals.INDIRIZZI = new List<Ip>();
                }
                Globals.INDIRIZZI.Clear();
                if (path.Equals(""))
                {
                    Globals.log.Info("Percorso nullo");
                    return;
                }
                
                Globals.log.Info("Lettura di: " + path + " alla pagina: " + pageNumber);
                Console.WriteLine("aperto excel");


                Excel excel = new Excel(path);
                var listaNomi = excel.SheetsName();
                
                
                lista_nomi_excel lista_excel = new lista_nomi_excel(excel, listaNomi);
                lista_excel.ShowDialog();

                
                confronto();
            }
            catch (IOException)
            {
                string msg = "E31 - Il file " + path + " non esiste o è aperto da un altro programma. \n";
                MessageBox.Show(msg, "E31"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Fatal(msg);
            }
            catch (FormatException)
            {
                string msg = "E32 - Il file " + path + " è in un formato non corretto.";
                MessageBox.Show(msg, "E32", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
            
        }

        private void confronto()
        {
            foreach (Ip ip in indirizziTxt)
            {
                Ip ricercaIp = Globals.INDIRIZZI.Find(r => r.id == ip.id);
                if (ricercaIp != null)
                {
                    if (ricercaIp.presenza == 2 && ip.presenza == 2)
                    {
                        ricercaIp.confronto = 2;    //OK, ci dovrebbe essere e c'è
                    }
                    else if ((ricercaIp.presenza == 0 || ricercaIp.presenza == 1) && ip.presenza == 2)
                    {
                        ricercaIp.confronto = 1;    //MALE, non ci dovrebbe essere e c'è
                    }
                    else if (ricercaIp.presenza == 2 && ip.presenza == 1)
                    {
                        ricercaIp.confronto = 3;    //INCOGNITA, ci dovrebbe essere e ora non c'è
                    }
                }
            }
        }
        /// <summary>
        /// Aggiunge la lista di programmi alla DataGrid dopo averla svuotata.
        /// </summary>
        private void createList()
        {
            cambiamento = false;
            Console.WriteLine("Create List");
            Globals.log.Info("Create List");
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            dataGrid.EnableRowVirtualization = true;
            dataGrid.SelectionChanged += new SelectionChangedEventHandler(ChangePreview);
            dataGrid.Items.Clear();
            int i = 0;
            foreach (Ip p in Globals.INDIRIZZI)
            {
                //dataGrid.Items.Add(p);
                filterList.Add(p);
                i++;
            }
            dataGrid.ItemsSource = filterList;
            cambiamento = true;
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
                //dataGrid.Items.Clear();
                filterList.Clear();
                foreach (Ip p in Globals.INDIRIZZI)
                {
                    if (p.toName().IndexOf(filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        //dataGrid.Items.Add(p);
                        filterList.Add(p);
                    }
                }
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = filterList;
                cambiamento = true;
            }
            
        }


        /// <summary>
        /// Aggiorna gli elementi nella DataGrid DOPO AVER CREATO NUOVI PROGRAMMI:
        /// - LEGGE I PROGRAMMI DA FILE (unica cosa in più del precedente)
        /// - aggiorna le ultime modifiche in programmi
        /// - aggiunge tutti i programmi presenti dopo aver svuotato la DataGrid
        /// - seleziona l'ultima informazione della lista (quello appena creato in teoria)
        /// </summary>
        //private void readAgainListPrograms(object sender, System.Windows.Forms.FormClosedEventArgs e)
        //{
        //    Console.WriteLine("UpdateList2");
        //    Globals.log.Info("UpdateList2");
        //    Globals.INDIRIZZI = null;
        //    readAddressCSV("");
        //    DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
        //    if (dataGrid != null)
        //    {
        //        //dataGrid.Items.Clear();
        //        int i = 0;
        //        foreach (Ip p in Globals.INDIRIZZI)
        //        {
        //            dataGrid.Items.Add(p);
        //            i++;
        //        }
        //        if (newProgram)
        //        {
        //            newProgram = false;
        //            dataGrid.SelectedIndex = Globals.INDIRIZZI.Last().id - 1;
        //            dataGrid.ScrollIntoView(Globals.INDIRIZZI.Last());
        //        }
        //    }
        //}

        /// <summary>
        /// Metodo per la riscrittura di Globals.INDIRIZZI nel file PROGRAMMI.csv
        /// </summary>
        private void scriviCSV()
        {
            Console.WriteLine("ScriviCsv");

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
            Console.WriteLine("SoftwareLoaded");
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
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
        /// Bottone che attiva la scrittura del file PROGRAMMI.csv
        /// </summary>
        private void Button_Save(object sender, RoutedEventArgs e)
        {
            scriviCSV();
            cambiamento = true;
            updateList("", null);
            Console.WriteLine("Salvato PROGRAMMI.csv");
            Globals.log.Info("Salvato PROGRAMMI.csv");
        }

        private void Open_Excel(object sender, RoutedEventArgs e)
        {
            ComboBox PageNumber = this.FindName("PageNumberComboBox") as ComboBox;
            pageNumber = PageNumber.SelectedIndex + 1;
            //System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            //if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    try
            //    {
            //        Task.Factory.StartNew(() =>
            //        {
            //            readAddressEXCEL(openFileDialog1.FileName);
            //        }).ContinueWith(task =>
            //        {
            //            updateList("", null);
            //        }, TaskScheduler.FromCurrentSynchronizationContext()); 

            //    }
            //    catch (SecurityException ex)
            //    {
            //        MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
            //        $"Details:\n\n{ex.StackTrace}");
            //    }
            //}
            var path="";
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    path = openFileDialog1.FileName;
                    if (Globals.INDIRIZZI == null)
                    {
                        Globals.INDIRIZZI = new List<Ip>();
                    }
                    Globals.INDIRIZZI.Clear();
                    if (path.Equals(""))
                    {
                        Globals.log.Info("Percorso nullo");
                        return;
                    }

                    Globals.log.Info("Lettura di: " + path + " alla pagina: " + pageNumber);
                    Console.WriteLine("aperto excel");


                    Excel excel = new Excel(path);
                    var listaNomi = excel.SheetsName();

                    lista_nomi_excel lista_excel = new lista_nomi_excel(excel, listaNomi);
                    lista_excel.ShowDialog();
                    updateList("", null);
                    confronto();
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
                catch (IOException)
                {
                    string msg = "E31 - Il file " + path + " non esiste o è aperto da un altro programma. \n";
                    MessageBox.Show(msg, "E31"
                                         , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Fatal(msg);
                }
                catch (FormatException)
                {
                    string msg = "E32 - Il file " + path + " è in un formato non corretto.";
                    MessageBox.Show(msg, "E32", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg);
                }
        }

        }

        private void Open_Csv(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Task.Factory.StartNew(() =>
                    {
                        readAddressCSV(openFileDialog1.FileName);
                    }).ContinueWith(task =>
                    {
                        updateList("", null);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void Open_Txt(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Task.Factory.StartNew(() =>
                    {
                        ReadAddressTxt(openFileDialog1.FileName);
                    }).ContinueWith(task =>
                    {
                        updateList("", null);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
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
        /// Al doppio click sulla riga apre la cartella del filesystem.
        /// </summary>
        private void LostFocusCell(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("LostFocus");
            cambiamento = true;
            confronto();
        }

        /// <summary>
        /// Funzioni per consentire di navigare la DataGrid con le freccie su e giù mentre si effettua una ricerca.
        /// Con invio si apre la cartella del filesystem dell'informazione selezionata
        /// </summary>
        private void PreviewKeyDown2(object sender, KeyEventArgs e)
        {
            Console.WriteLine("PreviewKeyDown");

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
            if (EnableColori)
            {
                dataGrid.EnableRowVirtualization = false;
            }
            else
            {
                dataGrid.EnableRowVirtualization = true;
            }
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
                        }
                        catch (IOException)
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
                cambiamento = false;
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
            Console.WriteLine("TextChanged");
            cambiamento = true;
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
        /// Controllo colorazione lista
        /// Checkbox Checked
        /// </summary>
        private void CheckBox_Colori_Checked(object sender, RoutedEventArgs e)
        {
            Globals.log.Info("Checkbox Colori Checked");
            EnableColori = true;
            DataGrid dataGrid;
            dataGrid = this.FindName("dataGrid") as DataGrid;
            ChangePreview(dataGrid, null);

        }

        /// <summary>
        /// Controllo colorazione lista
        /// Checkbox Unchecked
        /// </summary>
        private void CheckBox_Colori_Unchecked(object sender, RoutedEventArgs e)
        {
            Globals.log.Info("Checkbox Colori Unchecked");
            EnableColori = false;
            DataGrid dataGrid;
            dataGrid = this.FindName("dataGrid") as DataGrid;
            ChangePreview(dataGrid, null);
        }

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
            //form.FormClosed
            //        += new System.Windows.Forms.FormClosedEventHandler(this.readAgainListPrograms);
            //form.ShowDialog();
            Globals.log.Error("avrei dovuto leggere ancora la lista degli ip ma non l'ho fatto - private void Menu_percorsi");
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