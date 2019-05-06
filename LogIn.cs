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
{
   /// <summary>
   /// Programmerare:Ted Olsson, Edgar Bonnevie, Jonathan Halvarsson
   /// Denna klass är inloggningsfönstret till programmet, Ifall användaren skriver in rätt uppgifter som finns på databasen så kommer man till startsidan.
   /// </summary>
   
    public partial class LogIn : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();     
    public LogIn()
        {
            InitializeComponent();
            { NlogConfig.ConfigureNLog(); }         
        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Hämtar värden från textboxarna.
                string username = usernameTextBox.Text;
                string password = passwordTextBox.Text;
            
                
                string query = "SELECT * FROM h18edgbo_UserLogIn WHERE name=@nameVar AND password=@passVar;";

                MySqlParameter namePar = new MySqlParameter("@nameVar", username);
                MySqlParameter pwPar = new MySqlParameter("@passVar", password);
                MySqlParameter[] parameters = new MySqlParameter[] { namePar, pwPar };

                //Kollar om det användare skriver stämmer överens med inloggningsuppgifter på databasen.
                if (SQLHelp.Login(username, password, query, parameters))
                {
                    Hide();
                    StartSida startSida = new StartSida();
                    //hänvisas till startsidan.
                    startSida.ShowDialog();
                    Close();
                    logger.Info("Inloggning Lyckades\nAnvändarnamn: " + username);
                    logger.Info("Lösenord: " + password);
                }
                else
                {
                    MessageBox.Show("Oops, wrong input!\nTry again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    usernameTextBox.Text = "";
                    passwordTextBox.Clear();
                    usernameTextBox.Focus();
                    logger.Info("Användaren Skrev in Fel användarnamn eller lösenord.");
                    logger.Info("username: " + username + ", password: " + password);
                }
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
