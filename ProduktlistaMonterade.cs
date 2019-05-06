using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

namespace Produktionslager
{
    /// <summary>
    /// Programmerare:Ted Olsson, Edgar Bonnevie, Jonathan Halvarsson
    /// Denna klass visar en de olika produkterna lagret har, i denna klass kan man skapa en ny produkt
    /// öka antalet på en produkt samt ta bort en produkt.
    /// </summary>
    public partial class ProduktlistaMonterade : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string _query = "SELECT * FROM db30.h18edgbo_ProduktLager";
        
        public ProduktlistaMonterade()
        {
            InitializeComponent();            
            SQLHelp.LäsaData(_query, ref monteradeProdukterDataGridView);
        }

        private void avbrytButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void läggTillButton_Click(object sender, EventArgs e)
        {            
            antalForm antalForm = new antalForm(monteradeProdukterDataGridView, "h18edgbo_ProduktLager");
            antalForm.ShowDialog();
        }

        private void skapaButton_Click(object sender, EventArgs e)
        { 
            //Hänvisar till klassen "LäggTillProdukt" för att komma till fönstret där man kan skapa en ny produkt.
            LäggTillProdukt läggTillProdukt = new LäggTillProdukt();
            läggTillProdukt.ShowDialog();
            SQLHelp.LäsaData(_query, ref monteradeProdukterDataGridView);
        }

        private void taBortButton_Click(object sender, EventArgs e)
        {          
            if(MessageBox.Show("Är du säker på att du vill ta bort?", "Fråga", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SQLHelp.Delete(monteradeProdukterDataGridView, "h18edgbo_ProduktLager");
                SQLHelp.LäsaData(_query, ref monteradeProdukterDataGridView);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
