using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using NLog;


namespace Produktionslager
{
    /// <summary>
    /// Programmerare:Ted Olsson
    /// Denna Klass blir hänvisad till när man ska öka antalet komponenter eller antalet produkter.
    /// </summary>
    public partial class antalForm : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string _DB;

        private DataGridView _selectedRows;
        
        public antalForm(DataGridView selectedRows, string DB)
        {
            InitializeComponent();           
            _DB = DB;
            _selectedRows = selectedRows;
        }

        private void AvbrytButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                int antal = int.Parse(InputTextBox.Text);
                if (antal > 0)
                {
                    SubtraheraKomponentLager(antal);
                    SQLHelp.UpdatePlus(_selectedRows, _DB, antal);
                    Close();
                }
                else
                {
                    MessageBox.Show("Antalet får inte vara negativt!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.Info(ex.Message);
            }
            
        }

        public void SubtraheraKomponentLager(int antal)
        {
            //om _DB är "h18edgbo_ProduktLager" ska man ta bort det receptet innehåller från komponentlagret
            //Beroende på vad selectedRows innehåller ska den ta bort olika typer av komponenter från lagret
            try
            {
                if (_DB == "h18edgbo_ProduktLager")
                {
                    foreach (DataGridViewRow temp in _selectedRows.SelectedRows)
                    {
                        SQLHelp.UpdateMinus(temp.Cells[1].Value.ToString(), antal);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Info(ex.Message);
            }
        }
    }
}
