using System;
using System.Data.SQLite;
using System.Windows;

namespace Kurs_Project_BD;

public partial class EditResult : Window
{
    private string _typeWork;
    private int _resultID;
    private Profile _profile;
    private Results _results;
    public EditResult(Profile prof, string typeWork, Results editResult)
    {
        _results = editResult;
        _resultID = 0;
        _profile = prof;
        _typeWork = typeWork;
        InitializeComponent();
        switch (typeWork)
        {
            case "edit":
                _resultID = _results.IdTest;
                Info.Text = $"Изменение результата. \n ФИО: {_results.Fio} \n Название теста: {_results.TestName} \n Дата тестирования: {_results.DateResult}";
                CbFio.Visibility = Visibility.Hidden;
                CbTestName.Visibility = Visibility.Hidden ;
                TbPoints.Text = _results.Points.ToString();
                DpDate.Visibility = Visibility.Hidden;
                BtnReset.Visibility = Visibility.Hidden;
                TbkDate.Visibility = Visibility.Hidden;
                TbkFio.Visibility = Visibility.Hidden;
                TbkTest.Visibility = Visibility.Hidden;
                break;
            case "add":
                Info.Text = "Добавление результата";
                break;
        }
        string sqlQuery;
        sqlQuery = $"SELECT CASE WHEN patronymic = '' " +
                   $"THEN passport || ' ' || lastName || ' ' || firstName " +
                   $"ELSE passport || ' ' || lastName || ' ' || firstName || ' ' || patronymic END FROM users " +
                   $"WHERE userType = 'Ученик';";
        var reader = MainFunctions.ConnectToDB(sqlQuery, "SELECT").ExecuteReader();
        if (!reader.HasRows)
        {
            MessageBox.Show(
                "В базе данных нет учеников. Обратитесь к администратору!",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            Teacher teacher = new Teacher(_profile);
            teacher.Show();
            this.Close();
        }
        while (reader.Read())
        {
            CbFio.Items.Add(reader.GetString(0));
        }
        reader.Close();

        sqlQuery = $"SELECT id_test || ' ' || name_test FROM tests";
        reader = MainFunctions.ConnectToDB(sqlQuery, "SELECT").ExecuteReader();
        if (!reader.HasRows)
        {
            MessageBox.Show(
                "В базе данных нет тестов. Добавьте тесты!",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            Teacher teacher = new Teacher(_profile);
            teacher.Show();
            this.Close();
        }
        while (reader.Read())
        {
            CbTestName.Items.Add(reader.GetString(0));
        }
        reader.Close();
    }

    private bool IsValidData(bool isEdit)
    {
        if (TbPoints.Text == "")
        {
            MessageBox.Show(@"Поле 'Баллы' не может быть пустым",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        int points = -1;
        if (!int.TryParse(TbPoints.Text, out points))
        {
            MessageBox.Show("Баллы должны быть числом!",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }

        if (points <= 0)
        {
            MessageBox.Show("Баллы должны быть больше 0!",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (!isEdit)
        {
            if (DpDate.Text == "")
            {
                MessageBox.Show("Поле 'Дата' не может быть пустым",
                    "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                return false;
            }
            if (CbTestName.Text == "")
            {
                MessageBox.Show("Поле 'Тест' не может быть пустым",
                    "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                return false;
            }
            if (CbFio.Text == "")
            {
                MessageBox.Show("Поле 'ФИО' не может быть пустым",
                    "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                return false;
            }
            if (DateTime.Compare(DateTime.Parse(DpDate.Text), DateTime.Now) >= 0)
            {
                MessageBox.Show("Дата не может быть позже сегодняшнего дня",
                    "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                return false;
            }
            _resultID = int.Parse(CbTestName.Text.Split(' ')[0]);
        }
        string sqlQuery;
        sqlQuery = $"SELECT max_points FROM tests WHERE id_test = '{_resultID}'";
        var reader = MainFunctions.ConnectToDB(sqlQuery, "SELECT").ExecuteReader();
        reader.Read();
        int maxPoints = reader.GetInt32(0);
        reader.Close();
        if (points > maxPoints)
        {
            MessageBox.Show("Баллы должны быть не больше максимальных баллов теста! \n" +
                            $"Максимальное количество баллов теста - {maxPoints}",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        return true;
    }
    
    
    
    private void BtnSubmit_Click(object sender, RoutedEventArgs e)
    {
        string sqlQuery;
        switch (_typeWork)
        {
            case "edit":
                if (!IsValidData(true))
                {
                    return;
                }
                sqlQuery = $"UPDATE results SET points = {TbPoints.Text} WHERE id_result = '{_results.Id}'";
                MainFunctions.ConnectToDB(sqlQuery, "UPDATE");
                MessageBox.Show("Результат изменен успешно!",
                    "Изменение результата",MessageBoxButton.OK,MessageBoxImage.Asterisk);
                break;
            case "add":
                if (!IsValidData(false))
                {
                    return;
                }
                string passport = CbFio.Text.Split(' ')[0] + " " + CbFio.Text.Split(' ')[1] + " " +
                                  CbFio.Text.Split(' ')[2];
                sqlQuery = $"INSERT INTO results (passport, id_test, date_result, points) VALUES " +
                           $"('{passport}'," +
                           $"'{CbTestName.Text.Split(' ')[0]}'," +
                           $"'{DpDate.SelectedDate.Value.ToString("yyyy-MM-dd")}'," +
                           $"'{TbPoints.Text}')";
                MainFunctions.ConnectToDB(sqlQuery, "UPDATE");
                MessageBox.Show("Результат добавлен успешно!",
                    "Добавление результата",MessageBoxButton.OK,MessageBoxImage.Asterisk);
                break;
        }

        Teacher teacher = new Teacher(_profile);
        teacher.Show();
        this.Close();
    }

    private void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        CbFio.Text = "";
        CbTestName.Text = "";
        TbPoints.Text = "";
        DpDate.Text = "";
    }

    private void BtnReturn_Click(object sender, RoutedEventArgs e)
    {
        Teacher teacher = new Teacher(_profile);
        teacher.Show();
        this.Close();
    }
}