using System;
using System.Windows;
using System.Data.SQLite;
namespace Kurs_Project_BD;

public partial class Student : Window
{
    private Profile myProfile;
    public Student(Profile prof)
    {
        myProfile = prof;
        InitializeComponent();
        string _sqlQuery = $"SELECT id_result, name_test, date_result, points, max_points FROM results " +
                           $"LEFT JOIN tests USING (id_test) " +
                           $"WHERE passport = '{prof.Passport}'";
        if (prof.Patronymic == "")
        {
            nameOfReport.Text =
                $"Отчет о пройденных тестах. Ученик: {prof.LastName} {prof.FirstName.Substring(0, 1)}.";
        }
        else
        {
            nameOfReport.Text =
                $"Отчет о пройденных тестах. Ученик: {prof.LastName} {prof.FirstName.Substring(0, 1)}. {prof.Patronymic.Substring(0, 1)}.";
        }
        
        var reader = MainFunctions.ConnectToDB(_sqlQuery, "SELECT").ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Tests.Items.Add(new StudentReport()
                    {Id = reader.GetInt32(0),
                        NameOfTest = reader.GetString(1),
                        DateTest = reader.GetString(2),
                        Points = reader.GetInt32(3),
                        MaxPoints = reader.GetInt32(4)}
                    );
                }
            }
            else
            {
                MessageBox.Show("У вас нет результатов тестов. Вы можете посидеть в пустом экране");
                
            }
        
    }

    private void Form1_Close(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    private void ChangePassword_Click(object sender, EventArgs e)
    {
        EditUser editUser = new EditUser(myProfile, "change_password",null);
        editUser.Show();
        this.Close();
    }
    

    private void AuthReturn(object sender, EventArgs e)
    {
        MainWindow window = new MainWindow();
        window.Show();
        this.Close();
    }
}