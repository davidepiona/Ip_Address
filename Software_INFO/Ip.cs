using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP_Address
{
    public class Ip
    {
        /// <summary>
        /// Classe Ip composta dalle informazioni relative agli indirizzi ip
        /// Getter e Setter classici. 
        /// </summary>
        public int id { get; set; }
        public int a { get; set; }
        public int b { get; set; }
        public int c { get; set; }
        public int d { get; set; }
        public string descrizione { get; set; }
        public string ipCompleto { get; set; }
        public string MAC { get; set; }
        public string marca { get; set; }
        public bool presenza { get; set; }
        public bool? confronto { get; set; }

        /// <summary>
        /// Inizializza gli attributi coi valori ricevuto
        /// </summary>
        public Ip(int a, int b, int c, int d, string descrizione, string MAC, string marca, bool presenza)
        {
            this.id = c * 1000 + d;
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.descrizione = descrizione;
            this.ipCompleto = a + "." + b + "." + c + "." + d;
            this.MAC = MAC;
            this.marca = marca;
            this.presenza = presenza;
        }

        /// <summary>
        /// Metodo utilizzato per cercare i programmi nella lista. 
        /// Vengono utilizzate le parole contenute in numero, nome e descrizione.
        /// </summary>
        public string toName() {
            return ipCompleto + MAC + descrizione;
        }

    }
}
