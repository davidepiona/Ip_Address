using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP_Address
{
    public class Sheet
    {
        /// <summary>
        /// Classe Ip composta dalle informazioni relative agli indirizzi ip
        /// Getter e Setter classici. 
        /// </summary>
        public int Numero { get; set; }
        public string NomeFoglio { get; set; }
        public bool Seleziona { get; set; }
        
        /// <summary>
        /// Inizializza gli attributi coi valori ricevuto
        /// </summary>
        public Sheet(int Numero, string NomeFoglio, bool Seleziona)
        {
            this.Numero = Numero;
            this.NomeFoglio = NomeFoglio;
            this.Seleziona = Seleziona;
        }

        /// <summary>
        /// Metodo utilizzato per cercare i programmi nella lista. 
        /// Vengono utilizzate le parole contenute in numero, nome e descrizione.
        /// </summary>
        public string toName() {
            return Numero + " " + NomeFoglio + " " + Seleziona;
        }
    }
}
