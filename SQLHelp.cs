using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using NLog;

namespace Produktionslager
{

    public static class SQLHelp
    { /// <summary>
      /// Programmerare:Ted Olsson, Edgar Bonnevie, Jonathan Halvarsson
      /// Denna klass blir hänvisad till när en koppling till databasen sker.
      /// </summary>
      /// 
        // skapar upp ett objekt av loggern, för att logga information som händer i programmet.
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //hämtar in connectionstringen som pekar till databasen från appconfig.
        private static readonly string connectionSTR = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        private static MySqlConnection connection = new MySqlConnection(connectionSTR);
        private static DataTable table;
        private static bool status = false;

        //Metod för att Läsa in data från databasen till ett datagridview.
        public static void LäsaData(string query, ref DataGridView datagridView)
        {
            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataSet dataSet = new DataSet();

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                
                adapter.Fill(dataSet, "Table1");
                table = dataSet.Tables["Table1"];
                datagridView.DataSource = table.DefaultView;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.Info(ex.Message);
            }

            finally
            {
                connection.Close();
            }

        }

        //Metod för att lägga till en produkt i produktlagret.
        public static void LäggTill(string nyRecept, string produktNamn)
        {
            bool exist = false;
            Dictionary<string, int> nyaKomponenter = new Dictionary<string, int>();
            List<string> attLäggaTill = new List<string>();
            try
            {
                if (nyRecept != null)
                {
                    //splittar connectionstring och lägger in den i en dictionary.
                    var items = nyRecept.Split(';');

                    foreach (string item in items)
                    {
                        var tokens = item.Split('=');
                        nyaKomponenter.Add(tokens[0], int.Parse(tokens[1]));
                    }

                    string query = "SELECT * FROM h18edgbo_KomponentLager";
                    MySqlCommand commandRead = new MySqlCommand(query, connection);
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    foreach (KeyValuePair<string, int> temp in nyaKomponenter)
                    {
                        exist = false;
                        MySqlDataReader reader = commandRead.ExecuteReader();

                        //letar efter objekt som redan finns innan de läggs till i listan
                        while (reader.Read())
                        {
                            if (temp.Key == reader["Komponenter"].ToString())
                            {
                                exist = true;
                                break;
                            }
                        }
                        if (!exist)
                        {
                            attLäggaTill.Add(temp.Key);
                        }
                        reader.Close();
                    }
                    connection.Close();

                    string addRecept = "INSERT INTO `h18edgbo_ProduktionsRecept`(`recept`, `ProduktNamn`) VALUES ('" + nyRecept + "', '" + produktNamn + "')";
                    string addProdukt = "INSERT INTO `h18edgbo_ProduktLager`(`Namn`) VALUES ('" + produktNamn + "')";
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    MySqlCommand command = new MySqlCommand(addRecept, connection);
                    command.ExecuteNonQuery();
                    MySqlCommand command2 = new MySqlCommand(addProdukt, connection);
                    command2.ExecuteNonQuery();

                    foreach (string temp in attLäggaTill)
                    {
                        string addKomponent = "INSERT INTO `h18edgbo_KomponentLager`(`Komponenter`, `Antal`, `TTO`) VALUES ('" + temp + "','" + 0 + "','YES')";
                        MySqlCommand commandAdd = new MySqlCommand(addKomponent, connection);
                        commandAdd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.Info(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public static void UpdatePlus(DataGridView selectedRows, string DB, int antal)
        {
            try
            {
                if (DB == "h18edgbo_KomponentLager")
                    status = true;
                int gammaltVärde = 0;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                foreach (DataGridViewRow temp in selectedRows.SelectedRows)
                {
                    string query = "SELECT * FROM " + DB;
                    MySqlCommand commandRead = new MySqlCommand(query, connection);
                    MySqlDataReader reader = commandRead.ExecuteReader();

                    //hämta värdet för "antal" från DB till gammaltVärde.
                    while (reader.Read())
                    {
                        // Om ID:et i databasen stämmer överens med de valda ID:et i datagriden så körs if-statementet.
                        if (reader["ID"].ToString() == temp.Cells[0].Value.ToString())
                        {
                            gammaltVärde = (int)reader["Antal"];
                            connection.Close();

                            //uppdaterar antalet för komponent/produktnamn
                            if (connection.State == ConnectionState.Closed)
                            {
                                connection.Open();
                            }
                            if (status == true)
                            {
                                int total = gammaltVärde + antal;
                                string SQLUpdater = "UPDATE " + DB + " SET Antal=" + total.ToString() + " WHERE ID='" + temp.Cells[0].Value.ToString() + "';";
                                MySqlCommand command = new MySqlCommand(SQLUpdater, connection);
                                command.ExecuteNonQuery();

                                if (total > 9 && DB == "h18edgbo_KomponentLager")
                                {
                                    string SQLUpdaterTTO = "UPDATE h18edgbo_KomponentLager SET TTO= 'NO' WHERE ID='" + temp.Cells[0].Value.ToString() + "';";
                                    MySqlCommand commandTTO = new MySqlCommand(SQLUpdaterTTO, connection);
                                    commandTTO.ExecuteNonQuery();
                                }
                            }
                            connection.Close();
                            //Uppdaterar fönstret som antalForm öppnades genom. Eftersom den kan öppnas från två håll uppdateras det fönster som har ändrats.
                            if (DB == "h18edgbo_ProduktLager")
                            {
                                LäsaData(query, ref selectedRows);
                            }
                            else if (DB == "h18edgbo_KomponentLager")
                            {
                                LäsaData(query, ref selectedRows);
                            }
                            break;
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.Info(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            status = false;
        }

        public static void UpdateMinus(string produktnamn, int antal)
        {

            Dictionary<string, int> receptDic = new Dictionary<string, int>();
            Dictionary<string, int> komponentStatus = new Dictionary<string, int>();

            //hämta det recept som passar bäst in produktnamnet
            string queryRecept = "SELECT * FROM h18edgbo_ProduktionsRecept";
            MySqlCommand commandReadRecept = new MySqlCommand(queryRecept, connection);
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            MySqlDataReader receptReader = commandReadRecept.ExecuteReader();
            string recept = "";
            while (receptReader.Read())
            {
                if (produktnamn == receptReader["ProduktNamn"].ToString())
                {
                    recept = receptReader["recept"].ToString();
                    break;
                }
            }
            receptReader.Close();
            connection.Close();
            //splittar connectionstring och lägger in den i en dictionary
            var items = recept.Split(';');

            foreach (string item in items)
            {
                var tokens = item.Split('=');
                receptDic.Add(tokens[0], int.Parse(tokens[1]));
            }

            try
            {
                #region hämta data till komponentStatus dictionary.
                string query = "SELECT * FROM db30.h18edgbo_KomponentLager";
                MySqlCommand commandRead = new MySqlCommand(query, connection);

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                MySqlDataReader reader = commandRead.ExecuteReader();
                while (reader.Read())
                {
                    komponentStatus.Add(reader["Komponenter"].ToString(), (int)reader["Antal"]);
                }
                reader.Close();
                #endregion
                bool check = true;
                //skickar innehållet i dictionaryn och subtraherar från databasen
                for (int i = 0; i < 2; i++)
                {
                    foreach (KeyValuePair<string, int> temp in receptDic)
                    {
                        //hämta värde från komponentStatus och subtrahera
                        foreach (KeyValuePair<string, int> komponent in komponentStatus)
                        {                           
                            if (temp.Key == komponent.Key)
                            {
                                int nyvärde = komponent.Value - (temp.Value * antal);
                                if (nyvärde < 0)
                                {
                                    check = false;
                                    //break;
                                }
                                if (check && i == 1)
                                {
                                    string minus = "UPDATE h18edgbo_KomponentLager SET Antal=" + nyvärde.ToString() + " WHERE Komponenter='" + temp.Key + "';";
                                    MySqlCommand commandUpdate = new MySqlCommand(minus, connection);
                                    commandUpdate.ExecuteNonQuery();
                                    status = true;
                                    if (nyvärde < 10)
                                    {
                                        string SQLUpdaterTTO = "UPDATE h18edgbo_KomponentLager SET TTO= 'YES' WHERE Komponenter='" + temp.Key + "';";
                                        MySqlCommand commandTTO = new MySqlCommand(SQLUpdaterTTO, connection);
                                        commandTTO.ExecuteNonQuery();
                                    }
                                    break;
                                }
                                else if (i == 1)
                                {
                                    MessageBox.Show("Saknas komponenter i lager");
                                    check = false;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.Info(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public static bool Login(string username, string password, string query, MySqlParameter[] ParArray)
        {
            bool returnValue = false;
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddRange(ParArray);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                MySqlDataReader reader = command.ExecuteReader();
                //läser igenom användarlistan i databasen och letar efter ett matchande objekt
                while (reader.Read())
                {
                    if (username == reader["name"].ToString() && password == reader["password"].ToString())
                        returnValue = true;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.Info(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return returnValue;
        }

        public static void Delete(DataGridView selectedRows, string DB)
        {
            try
            {
                foreach (DataGridViewRow temp in selectedRows.SelectedRows)
                {
                    string deleteCommand = "DELETE FROM `" + DB + "` WHERE ID=" + temp.Cells[0].Value.ToString();
                    string deleteCommand2 = "DELETE FROM `h18edgbo_ProduktionsRecept` WHERE ProduktNamn='" + temp.Cells[1].Value.ToString() + "'";
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    MySqlCommand command = new MySqlCommand(deleteCommand, connection);
                    command.ExecuteNonQuery();
                    if (DB == "h18edgbo_ProduktLager")
                    {
                        MySqlCommand command2 = new MySqlCommand(deleteCommand2, connection);
                        command2.ExecuteNonQuery();
                    }
                    connection.Close();
                    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.Info(ex.Message);
            }
        }
    }
}