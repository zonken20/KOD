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
    /// Denna klass visar en lagerstatusen på komponenter. 
    /// </summary>
    public partial class Lager : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Lager()
        {
            InitializeComponent();
           //Kallar på "LäsaData" för att uppdatera fönstret med ny informaton från databasen. 
            string query = "SELECT * FROM h18edgbo_KomponentLager";
            SQLHelp.LäsaData(query, ref lagerDataGridView);

        }

        private void avbrytButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
