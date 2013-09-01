using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseRecreater
{
    public class Program
    {
        private const string ConnectionString = "Server=localhost; Database=master; Trusted_Connection=True;";

        static void Main(string[] args)
        {
            var dbName = "Taskever";

            if (IsDatabaseExists(dbName))
            {
                DeleteDatabase(dbName);
            }

            CreateDatabase(dbName);

            RunMigrations();
        }

        private static void CreateDatabase(string dbName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"CREATE DATABASE " + dbName + ";";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void DeleteDatabase(string dbName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"ALTER DATABASE " + dbName + @" SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [" + dbName + "]";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static bool IsDatabaseExists(string dbName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var query = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", dbName);
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        return reader.Read();
                    }
                }
            }
        }

        private static void RunMigrations()
        {
            RunMigration(@"..\..\..\..\Abp\Core\Abp.Core.Data\");
            RunMigration(@"..\..\..\..\Taskever\Taskever.Data\");
        }

        private static void RunMigration(string directory)
        {
            var process = new Process()
                              {
                                  StartInfo = new ProcessStartInfo()
                                      {
                                          WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), directory),
                                          FileName = "RunMigrations.bat"
                                      }
                              };
            process.Start();
            process.WaitForExit();
        }
    }
}
