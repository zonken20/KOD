using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using NLog;

namespace Produktionslager
{ /// <summary>
  /// Programmerare:Ted Olsson, Edgar Bonnevie, Jonathan Halvarsson
  /// Denna Klass visar en Lagerstatusen på komponenter, samt att man ska kunna öka antalet på komponenter vid leverans
  /// och kunna ta bort en komponent.
  /// </summary>
    public partial class inkommandeLeverans : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string _query = "SELECT * FROM db30.h18edgbo_KomponentLager";
        public inkommandeLeverans()
        {
            InitializeComponent();            
            SQLHelp.LäsaData(_query, ref leveransDataGridView);
        }

        private void LäggTillMängdButton_Click(object sender, EventArgs e)
        {
            // Hänvisar till Klassen antalForm för att kunna öka antalet på en komponent.
            antalForm antalForm = new antalForm(leveransDataGridView, "h18edgbo_KomponentLager");
            antalForm.Show(); 
        }

        private void avbrytButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void taBortButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Är du säker på att du vill ta bort?", "Fråga", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SQLHelp.Delete(leveransDataGridView, "h18edgbo_KomponentLager");
                SQLHelp.LäsaData(_query, ref leveransDataGridView);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
