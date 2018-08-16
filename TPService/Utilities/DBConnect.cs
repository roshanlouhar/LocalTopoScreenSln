﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPService.Utilities
{
    public class DBConnect
    {
        private MySqlConnection connection;

        //Constructor
        public DBConnect(string connectionString)
        {
            Initialize(connectionString);
        }

        //Initialize values
        private void Initialize(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        public bool OpenConnection()
        {
            try
            {
                if (connection.State.ToString() == "Open")
                {
                    return true;
                }
                else
                {
                    connection.Open();
                    return true;
                }

            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        //Close connection
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        //Insert statement
        public void InsertLogInfo(DateTime datetime, string macId, int currentState)
        {

            string CmdString = "INSERT INTO tbltoposcreensservicestatus (fldDateTime,fldCurrentState,MachineId)" +
                " VALUES(@fldDateTime, @fldCurrentState, @MachineId) ";

            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(CmdString, connection);


            cmd.Parameters.Add("@fldDateTime", MySqlDbType.DateTime);
            cmd.Parameters.Add("@fldCurrentState", MySqlDbType.VarChar, 5);
            cmd.Parameters.Add("@MachineId", MySqlDbType.VarChar, 45);

            cmd.Parameters["@fldDateTime"].Value = datetime;
            cmd.Parameters["@fldCurrentState"].Value = currentState;
            cmd.Parameters["@MachineId"].Value = Convert.ToString(macId);

            //open connection
            if (this.OpenConnection() == true)
            {
                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        //Insert statement
        public void InsertImage(DateTime datetime, string macId, string ScreenShot, string IsScreenShotProcessed, Byte[] ImgObj)
        {
            string CmdString = "INSERT INTO tbltoposcreens (fldDateTime,fldMacID,fldScreenshot,fldScreenShotProcessedYesNo,fldScreenBlob)" +
                " VALUES(@fldDateTime, @fldMacID, @fldScreenshot, @fldScreenShotProcessedYesNo, @fldScreenBlob) ";

            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(CmdString, connection);

            cmd.Parameters.Add("@fldDateTime", MySqlDbType.DateTime);
            cmd.Parameters.Add("@fldMacID", MySqlDbType.VarChar, 45);
            cmd.Parameters.Add("@fldScreenshot", MySqlDbType.VarChar, 45);
            cmd.Parameters.Add("@fldScreenShotProcessedYesNo", MySqlDbType.VarChar, 45);
            cmd.Parameters.Add("@fldScreenBlob", MySqlDbType.MediumBlob);

            cmd.Parameters["@fldDateTime"].Value = datetime;
            cmd.Parameters["@fldMacID"].Value = macId;
            cmd.Parameters["@fldScreenshot"].Value = ScreenShot;
            cmd.Parameters["@fldScreenShotProcessedYesNo"].Value = IsScreenShotProcessed;
            cmd.Parameters["@fldScreenBlob"].Value = ImgObj;

            //open connection
            if (this.OpenConnection() == true)
            {
                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        //Update statement
        public void Update(string query)
        {
            try
            {
                //Open connection
                if (this.OpenConnection() == true)
                {
                    //create mysql command
                    MySqlCommand cmd = new MySqlCommand();
                    //Assign the query using CommandText
                    cmd.CommandText = query;
                    //Assign the connection using Connection
                    cmd.Connection = connection;

                    //Execute query
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();
                }
            }
            catch
            {

            }
        }

        //Delete statement
        public void Delete(string query)
        {
            try
            {
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                }
            }
            catch
            {

            }
        }

        //Select statement
        public DataTable Select(string query)
        {

            DataTable dt = new DataTable();
            try
            {
                //Open connection
                if (this.OpenConnection() == true)
                {

                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    // this will query your database and return the result to your datatable
                    da.Fill(dt);

                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch
            {
                return dt;
            }
        }

        //Count statement
        public int Count(string query)
        {
            try
            {
                int Count = -1;

                //Open Connection
                if (this.OpenConnection() == true)
                {
                    //Create Mysql Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //ExecuteScalar will return one value
                    Count = int.Parse(cmd.ExecuteScalar() + "");

                    //close Connection
                    this.CloseConnection();

                    return Count;
                }
                else
                {
                    return Count;
                }
            }
            catch
            {
                return 0;
            }
        }

        //Backup
        public void Backup()
        {
            //try
            //{
            //    DateTime Time = DateTime.Now;
            //    int year = Time.Year;
            //    int month = Time.Month;
            //    int day = Time.Day;
            //    int hour = Time.Hour;
            //    int minute = Time.Minute;
            //    int second = Time.Second;
            //    int millisecond = Time.Millisecond;

            //    //Save file to C:\ with the current date as a filename
            //    string path;
            //    path = "C:\\" + year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
            //    StreamWriter file = new StreamWriter(path);


            //    ProcessStartInfo psi = new ProcessStartInfo();
            //    psi.FileName = "mysqldump";
            //    psi.RedirectStandardInput = false;
            //    psi.RedirectStandardOutput = true;
            //    psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", uid, password, server, database);
            //    psi.UseShellExecute = false;

            //    Process process = Process.Start(psi);

            //    string output;
            //    output = process.StandardOutput.ReadToEnd();
            //    file.WriteLine(output);
            //    process.WaitForExit();
            //    file.Close();
            //    process.Close();
            //}
            //catch (IOException ex)
            //{
            //    MessageBox.Show("Error , unable to backup!");
            //}
        }

        //Restore
        public void Restore()
        {
            //try
            //{
            //    //Read file from C:\
            //    string path;
            //    path = "C:\\MySqlBackup.sql";
            //    StreamReader file = new StreamReader(path);
            //    string input = file.ReadToEnd();
            //    file.Close();


            //    ProcessStartInfo psi = new ProcessStartInfo();
            //    psi.FileName = "mysql";
            //    psi.RedirectStandardInput = true;
            //    psi.RedirectStandardOutput = false;
            //    psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", uid, password, server, database);
            //    psi.UseShellExecute = false;


            //    Process process = Process.Start(psi);
            //    process.StandardInput.WriteLine(input);
            //    process.StandardInput.Close();
            //    process.WaitForExit();
            //    process.Close();
            //}
            //catch (IOException ex)
            //{
            //    MessageBox.Show("Error , unable to Restore!");
            //}
        }


    }
}
