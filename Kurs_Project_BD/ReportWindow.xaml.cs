using System;
using System.Data;
using System.Windows;

namespace Kurs_Project_BD;

public partial class ReportWindow : Window
{
    private string _sqlQuery;
    public ReportWindow(string test, DateTime date1, DateTime date2, Profile prof)
    {
        InitializeComponent();
        TextBlock.Text = $"Отчет по прохождению тестирования <{test.Substring(test.IndexOf(' ') + 1)}> \n за период от {date1:dd.MM.yyyy} до {date2:dd.MM.yyyy}";
        _sqlQuery = $"SELECT lastName, firstName, patronymic, u.passport, date_result, points FROM results " +
                    $"INNER JOIN tests t on t.id_test = results.id_test " +
                    $"INNER JOIN users u on u.passport = results.passport " +
                    $"WHERE t.id_test = '{test.Split()[0]}' AND date_result BETWEEN '{date1.Date:yyyy-MM-dd}' AND '{date2.Date:yyyy-MM-dd}' " +
                    $"ORDER BY lastName,firstName,patronymic, points DESC,date_result;";
        var reader = MainFunctions.ConnectToDB(_sqlQuery, "SELECT").ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                lvReport.Items.Add(new Report()
                {
                    LastName = reader.GetString(0),
                    FirstName = reader.GetString(1),
                    Patronymic = reader.GetString(2),
                    Passport = reader.GetString(3),
                    DateTest = reader.GetDateTime(4).Date.ToString("dd.MM.yyyy"),
                    Points = reader.GetInt32(5)
                });
            }
        }
        else
        {
            MessageBox.Show("Под ваши параметы не найдено записей");
            this.IsEnabled = false;
            this.Close();
            return;
        }
        reader.Close();
        _sqlQuery = $"SELECT MAX(points), AVG(points) FROM results " +
                    $"WHERE id_test = '{test.Split()[0]}' AND date_result BETWEEN '{date1.Date:yyyy-MM-dd}' AND '{date2.Date:yyyy-MM-dd}';";
        reader = MainFunctions.ConnectToDB(_sqlQuery, "SELECT").ExecuteReader();
        reader.Read();
        int MaxPoints = reader.GetInt32(0);
        double AVGPoints = reader.GetDouble(1);
        this.AVGPoints.Text = Math.Round(AVGPoints, 2).ToString();
        this.MAXPoints.Text = MaxPoints.ToString();
        reader.Close();
    }
}