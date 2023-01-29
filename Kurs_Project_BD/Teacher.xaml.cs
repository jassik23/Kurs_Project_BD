using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Kurs_Project_BD;

public partial class Teacher : Window
{
    private string _sqlQuery;
    private Profile _profile;
    private int _selectedTest;
    private int _selectedResult;
    private List<Test> _allTests;
    private List<Results> _allResults;
    private List<Profile> _allProfiles;
    public Teacher(Profile prof)
    {
        _selectedTest = -1;
        _selectedResult = -1;
        _profile = prof;
        _allTests = new List<Test>();
        _allResults = new List<Results>();
        _allProfiles = new List<Profile>();
        InitializeComponent();
        UpdateStudents(null, null);
        UpdateTests(null,null);
        UpdateResults(null, null);
        if (prof.Patronymic == "")
        {
            ProfileTeacherText.Text =
                $"Панель учителя. Учитель: \n {prof.LastName} {prof.FirstName.Substring(0, 1)}.";
        }
        else
        {
            ProfileTeacherText.Text =
                $"Панель учителя. Учитель: \n {prof.LastName} {prof.FirstName.Substring(0, 1)}. {prof.Patronymic.Substring(0, 1)}.";
        }
    }

    private void ChangePassword_Click(object sender, EventArgs e)
    {
        EditUser editUser = new EditUser(_profile, "change_password",null);
        editUser.Show();
        this.Close();
    }
    
    private void UpdateStudents(object sender, EventArgs e)
    {
        lvUsers.Items.Clear();
        var i = 0;
        _allProfiles.Clear();
        _sqlQuery = $"SELECT * FROM users WHERE userType = 'Ученик'";
        var reader = MainFunctions.ConnectToDB(_sqlQuery, "SELECT").ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                _allProfiles.Add(new Profile()
                    {
                        LastName = reader.GetString(0),
                        FirstName = reader.GetString(1),
                        Patronymic = reader.GetString(2),
                        Passport = reader.GetString(3),
                        Login = reader.GetString(4),
                        Password = reader.GetString(5),
                        TypeOfUser = reader.GetString(6),
                    }
                );
                lvUsers.Items.Add(_allProfiles[i]);
                i++;
            }
        }
    }
    
    
    private void UpdateTests(object sender, EventArgs e)
    {
        lvTests.Items.Clear();
        var i = 0;
        _allTests.Clear();
        _sqlQuery = $"SELECT * FROM tests ORDER BY id_test, name_test";
        var reader = MainFunctions.ConnectToDB(_sqlQuery, "SELECT").ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    _allTests.Add(new Test()
                        {
                            Id = reader.GetInt32(0),
                            TestNaming = reader.GetString(1),
                            MaxPoints = reader.GetInt32(2),
                            
                        }
                    );
                    CbTestName.Items.Add($"{_allTests[i].Id} {_allTests[i].TestNaming}");
                    lvTests.Items.Add(_allTests[i]);
                    i++;
                }
            }
    }

    private void UpdateResults(object sender, EventArgs e)
    {
        lvResults.Items.Clear();
        var i = 0;
        _allResults.Clear();
        _sqlQuery = $"SELECT id_result, CASE WHEN patronymic = '' " +
                    $"THEN lastName || ' ' || firstName " +
                    $"ELSE lastName || ' ' || firstName || ' ' || patronymic END, " +
                    $"name_test, date_result, points, u.passport, t.id_test  FROM results " +
                    $"LEFT JOIN tests t on t.id_test = results.id_test " +
                    $"LEFT JOIN users u on u.passport = results.passport " +
                    $"ORDER BY 2,4,5,3;";
        var reader = MainFunctions.ConnectToDB(_sqlQuery, "SELECT").ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                _allResults.Add(new Results()
                    {
                        Id = reader.GetInt32(0),
                        Fio = reader.GetString(1),
                        TestName = reader.GetString(2),
                        DateResult = reader.GetDateTime(3).ToString("dd.MM.yyyy"),
                        Points = reader.GetInt32(4),
                        Passport = reader.GetString(5),
                        IdTest = reader.GetInt32(6)
                    
                    }
                );
                lvResults.Items.Add(_allResults[i]);
                i++;
            }
        }
    }
    
    private void Form1_Close(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void profile_Click(object sender, MouseButtonEventArgs e)
    {
        var item = (sender as ListView).SelectedItem;
        int i = lvTests.SelectedIndex;
        if (item != null)
        {
            _selectedTest = i;
        }
    }
    private void result_Click(object sender, MouseButtonEventArgs e)
    {
        var item = (sender as ListView).SelectedItem;
        int i = lvResults.SelectedIndex;
        if (item != null)
        {
            _selectedResult = i;
        }
    }

    private void AuthReturn(object sender, RoutedEventArgs e)
    {
        MainWindow window = new MainWindow();
        window.Show();
        this.Close();
    }

    private void btnAddTest_OnClick(object sender, RoutedEventArgs e)
    {
        EditTest editTest = new EditTest(null, "add", _profile);
        editTest.Show();
        this.Close();
    }

    private void btnEditTest_OnClick(object sender, RoutedEventArgs e)
    {
        if (_selectedTest == -1)
        {
            MessageBox.Show("Для изменения выберите тест.");
            return;
        }
        EditTest editTest = new EditTest(_allTests[_selectedTest], "edit", _profile);
        editTest.Show();
        this.Close();
    }

    private void BtnRemoveTest_OnClick(object sender, RoutedEventArgs e)
    {
        if (_selectedTest == -1)
        {
            MessageBox.Show("Для изменения выберите тест.");
            return;
        }
        string sqlQuery;
        sqlQuery = $"SELECT * FROM results WHERE id_test = '{_allTests[_selectedTest].Id}'";
        var reader = MainFunctions.ConnectToDB(sqlQuery, "SELECT").ExecuteReader();
        if (reader.HasRows)
        {
            MessageBox.Show(
                "Вы не можете удалить этот тест, так как с ним есть связанные записи в результатах. \n" +
                "Сначала удалите связанные записи, только потом удаляйте тест!", 
                "Сообщение", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
            return;
        }
        reader.Close();
        sqlQuery = $"DELETE FROM tests WHERE id_test = '{_allTests[_selectedTest].Id}'";
        var result = MessageBox.Show(
            "Вы точно хотите удалить тест? ", 
            "Сообщение", 
            MessageBoxButton.YesNo, 
            MessageBoxImage.Information, 
            MessageBoxResult.No, 
            MessageBoxOptions.DefaultDesktopOnly);
        if (result == MessageBoxResult.No)
        {
            MessageBox.Show("Тест не будет удалён");
            return;
        }

        MainFunctions.ConnectToDB(sqlQuery, "UPDATE");
        MessageBox.Show("Тест удалён");
        _selectedTest = -1;
        UpdateTests(null,null);
    }

    private void BtnRemoveUserResult_OnClick(object sender, RoutedEventArgs e)
    {
        if (_selectedResult == -1)
        {
            MessageBox.Show("Для удаления выберите результат.");
            return;
        }
        var result = MessageBox.Show(
            "Вы точно хотите удалить результат пользователя? ", 
            "Сообщение", 
            MessageBoxButton.YesNo, 
            MessageBoxImage.Information, 
            MessageBoxResult.No, 
            MessageBoxOptions.DefaultDesktopOnly);
        if (result == MessageBoxResult.No)
        {
            MessageBox.Show("Результат не будет удален");
            return;
        }
        string sqlQuery;
        sqlQuery = $"DELETE FROM results WHERE id_result = '{_allResults[_selectedResult].Id}'";
        MainFunctions.ConnectToDB(sqlQuery, "UPDATE");
        MessageBox.Show("Результат удалён");
        _selectedTest = -1;
        UpdateResults(null,null);
    }

    private void BtnEditUserResult_OnClick(object sender, RoutedEventArgs e)
    {
        if (_selectedResult == -1)
        {
            MessageBox.Show("Для изменения выберите результат.");
            return;
        }

        EditResult editResult = new EditResult(_profile, "edit", _allResults[_selectedResult]);
        editResult.Show();
        this.Close();
    }

    private void BtnAddUserResult_OnClick(object sender, RoutedEventArgs e)
    {
        EditResult editResult = new EditResult(_profile, "add", null);
        editResult.Show();
        this.Close();
    }

    private bool ValidationDataReport()
    {
        if (CbTestName.Text == "")
        {
            MessageBox.Show("Поле 'Тест' не может быть пустым!",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (DpDateStart.Text == "")
        {
            MessageBox.Show("Поле 'Дата от' не может быть пустым! ",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (DpDateEnd.Text == "")
        {
            MessageBox.Show("Поле 'Дата до' не может быть пустым!",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (DateTime.Compare(DpDateStart.SelectedDate.Value, DpDateEnd.SelectedDate.Value) == 1)
        {
            MessageBox.Show("Дата до не может быть раньше даты после",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (DateTime.Compare(DpDateStart.SelectedDate.Value, DateTime.Today) == 1)
        {
            MessageBox.Show("Дата от не может быть позднее сегодняшнего дня",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (DateTime.Compare(DpDateEnd.SelectedDate.Value, DateTime.Today) == 1)
        {
            MessageBox.Show("Дата до не может быть позднее сегодняшнего дня",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        return true;
    }
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (!ValidationDataReport())
        {
            return;
        }
        MessageBox.Show("Построение отчета 1/3",
            "Думаем...",MessageBoxButton.OK,MessageBoxImage.Information);
        MessageBox.Show("Построение отчета 2/3",
            "Думаем...",MessageBoxButton.OK,MessageBoxImage.Information);
        MessageBox.Show("Построение отчета 3/3",
            "Думаем...",MessageBoxButton.OK,MessageBoxImage.Information);
        MessageBox.Show("Отчет построен",
            "Подумали",MessageBoxButton.OK,MessageBoxImage.Information);
        ReportWindow reportWindow = new ReportWindow(CbTestName.Text,
            DpDateStart.SelectedDate.Value,
            DpDateEnd.SelectedDate.Value, _profile);
        if (reportWindow.IsEnabled)
        {
            reportWindow.Show();
        }
        
    }
}