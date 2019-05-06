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
    /// Denna klass är startsidan i programmet, här har man tre alternativ att klicka sig vidare till.
    /// </summary>
    public partial class StartSida : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public StartSida()
        {
            InitializeComponent();
        }

        private void seLagerButton_Click(object sender, EventArgs e)
        {
            Lager lagerForm = new Lager();
            lagerForm.Show();
        }

        private void LeveransButton_Click(object sender, EventArgs e)
        {
            inkommandeLeverans leveransForm = new inkommandeLeverans();
            leveransForm.Show();
        }

        private void FärdigbyggdButton_Click(object sender, EventArgs e)
        {
            ProduktlistaMonterade produktlistaMonterade = new ProduktlistaMonterade();
            produktlistaMonterade.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }     
    }
}
