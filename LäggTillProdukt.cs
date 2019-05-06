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
    public partial class LäggTillProdukt : Form
    {
        /// <summary>
        /// Programmerare:Ted Olsson, Edgar Bonnevie, Jonathan Halvarsson
        /// Denna klass låter en lägga till en ny produkt, användaren ska skriva in vilka komponenter samt antalet som produkten ska innehålla.
        /// </summary>
        /// 
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Dictionary<string, int> produkterDic = new Dictionary<string, int>();
        public LäggTillProdukt()
        {
            InitializeComponent();
        }

        private void läggTillButton_Click(object sender, EventArgs e)
        {
            try
            {
                // hämtar ut komponent namn och antalet till varibler.
                string komponentNamn = komponentTextBox.Text;
                int.TryParse(antalTextBox.Text, out int antal);

                // lägger till variblerna till ett dictionary.
                produkterDic.Add(komponentNamn, antal);

                antalTextBox.Clear();
                komponentTextBox.Clear();
                komponentTextBox.Focus();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void färdigButton_Click(object sender, EventArgs e)
        {
            string nyRecept = "";
            try
            {
                string produktNamn = produktTextBox.Text;                   
                // går igenom Dictionaryt och tittar på varje key value par, för att lägga till de i en string.
                foreach (KeyValuePair<string, int> temp in produkterDic)
                {
                    nyRecept += temp.Key + "=" + temp.Value + ";";
                }

                // tar bort de sista semicolonet från stringen, samt skickar stringen till LäggTill metoden i SQLHelp.
                nyRecept = nyRecept.Remove(nyRecept.Length - 1);               
                SQLHelp.LäggTill(nyRecept, produktNamn);             
                Close();
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
            }                           
        }

        private void avbrytButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
