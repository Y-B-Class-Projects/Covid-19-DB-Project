using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SQL
    {
        private static SQL instance;

        private string tabe_connection_string;
        private DirectoryInfo workingDirectory;

        private SQL()
        {
            workingDirectory = directory.getWorkingDirectory();
            tabe_connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"" + workingDirectory + "\\DAL\\Covid.mdf\";Integrated Security=True";
        }

        public static SQL getInstance()
        {
            if (instance == null)
                instance = new SQL();

            return instance;
        }

        public void write(string query)
        {
            using (SqlConnection connection = new SqlConnection(tabe_connection_string))
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public string readScalar(string query)
        {
            string count = "";
            using (SqlConnection myConnection = new SqlConnection(tabe_connection_string))
            {
                SqlCommand oCmd = new SqlCommand(query, myConnection);

                myConnection.Open();

                count = oCmd.ExecuteScalar().ToString();
                
                myConnection.Close();
            }
            return count;
        }

        public List<List<string>> getRows(string query, string[] args)
        {
            List<List<string>> ret = new List<List<string>>();

            using (SqlConnection myConnection = new SqlConnection(tabe_connection_string))
            {
                SqlCommand command = new SqlCommand(query, myConnection);

                try
                {
                    myConnection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var row = new List<string>();

                        foreach (var str in args)
                        {
                            row.Add(reader[str].ToString());
                        }
                        
                        ret.Add(row);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    myConnection.Close();
                }
            }
            return ret;
        }
    }
}
