using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

IConfigurationBuilder? configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");

IConfigurationRoot config = configuration.Build();
string? connectionString = config.GetConnectionString("Database");

if (connectionString == null)
{
    throw new ArgumentNullException("ConnectionString is null");
}

Console.WriteLine("Enter script folder path :");
string? scriptsPath = Console.ReadLine();

if (!Directory.Exists(scriptsPath))
{
    throw new InvalidOperationException("Path not found");
}

DirectoryInfo d = new DirectoryInfo(scriptsPath);

FileInfo[] sqlfiles = d.GetFiles("*.sql");

SqlCommand cm;

using (SqlConnection cnn = new SqlConnection(connectionString))

    try
    {
        cnn.Open();

        Console.WriteLine("Connection Open ! ");
        foreach (FileInfo file in sqlfiles)
        {
            Console.WriteLine("Executing : " + file.FullName);
            string script = file.OpenText().ReadToEnd();

            using (cm = cnn.CreateCommand())
                cm.CommandText = script;

            cm.ExecuteNonQuery();

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine("Executing : " + file.FullName);
            Console.ResetColor();
        }

        cnn.Close();
        Console.WriteLine("All script have been successfully executed ! ");
        Console.WriteLine("Connection Closed ! ");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine("Can not open connection ! ", ex);
    }

