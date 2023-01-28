using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace Kurs_Project_BD;

public abstract class MainFunctions
{
    private const string PatternLogin = "^[a-zA-Z][a-zA-Z0-9]{3,19}$";
    private const string PatternPassword = "^[^а-яёА-ЯЁ]{8,50}$";
    private const string PatternPassport = "^[0-9]{2} [0-9]{2} [0-9]{6}$";
    private const string PatternFio = "^[А-ЯЁ][а-яё]{3,}$";
    private const string PatternTestName = "^[^a-zA-Z]+$";

    public static bool IsValidLogin(string login) => new Regex(PatternLogin).IsMatch(login);
    public static bool IsValidPassword(string password) => new Regex(PatternPassword).IsMatch(password);
    public static bool IsValidPassport(string passport) => new Regex(PatternPassport).IsMatch(passport);
    public static bool IsValidFio(string fio) => new Regex(PatternFio).IsMatch(fio);
    public static bool IsValidTestName(string name) => new Regex(PatternTestName).IsMatch(name);

    public static SQLiteCommand ConnectToDB(string command, string typeOfCommand)
    {
        string connection = $"Data Source={MainWindow.dbFileName};Version=3";
        SQLiteConnection sqliteConn = new SQLiteConnection(connection);
        sqliteConn.Open();
        var SqliteCmd = new SQLiteCommand();
        SqliteCmd = sqliteConn.CreateCommand();
        SqliteCmd.CommandText = command;
        switch (typeOfCommand)
        {
            case "UPDATE":
                SqliteCmd.ExecuteNonQuery();
                sqliteConn.Close();;
                return null!;
                break;
            case "SELECT":
                return SqliteCmd;
                break;
        }

        return SqliteCmd;
    }


}