using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;

namespace SendEmailScheduleJob
{
    class Program
    {
        static void Main(string[] args)
        {
            sendMail();
        }

        public static void sendMail()
        {
            try
            {
                DataSet Ds = new DataSet();

                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conn"]))
                {
                    SqlCommand cmd = new SqlCommand("spGetEmailIds", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter Sda = new SqlDataAdapter();
                    con.Open();
                    Sda.Fill(Ds, "EmailIds");
                    con.Close();
                }

                if (Ds != null && Ds.Tables["EmailIds"].Rows.Count > 0)
                {
                    for (int i = 0; i < Ds.Tables["EmailIds"].Rows.Count; i++)
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                        mail.From = new MailAddress("customercare@gmail.com");
                        mail.To.Add(Ds.Tables["EmailIds"].Rows[0][i].ToString());
                        mail.Subject = "Exclusive Offer!!!";
                        mail.IsBodyHtml = true;
                        mail.Body = "This is an exclusive offer for you, please check the below link to know more <html> <br/> <a href='http://www.yoursite.com'></a> </html>";
                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
                        SmtpServer.EnableSsl = true;
                        SmtpServer.Send(mail);

                        UpdateDatabase();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
            }
        }

        public static void UpdateDatabase()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conn"]))
            {
                SqlCommand cmd = new SqlCommand("spStoreEmailRecord", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@moduleCode", moduleCode);
                cmd.Parameters.AddWithValue("@moduleType", moduleType);
                cmd.Parameters.AddWithValue("@clientId", clientid);

                int noOfRowsAffected = cmd.ExecuteNonQuery();
            }
        }
        public static void WriteErrorLog(Exception ex)
        {
            //write code to store the exception log in database
        }
    }
}
