using System.Data.SQLite;
using System.Windows;

namespace Kurs_Project_BD;

public partial class EditTest : Window
{
    private string typeOfWork;
    private Test _test;
    private Profile _profile;
    public EditTest(Test test, string typeWork, Profile prof)
    {
        typeOfWork = typeWork;
        _test = test;
        _profile = prof;
        InitializeComponent();
        switch (typeOfWork)
        {
            case "edit":
                Info.Text = "Изменение теста";
                BtnReset.Visibility = Visibility.Hidden;
                TbTestName.Text = test.TestNaming;
                TbMaxPoints.Text = test.MaxPoints.ToString();
                break;
            case "add":
                Info.Text = "Добавление теста";
                break;
        }
    }

    private bool ValidationData(bool isEdit, int points)
    {
        if (TbMaxPoints.Text == "" || TbTestName.Text == "")
        {
            MessageBox.Show("Поля не должны быть пустыми",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (!MainFunctions.IsValidTestName(TbTestName.Text))
        {
            MessageBox.Show("Название теста должно состоять из русских букв. Должно быть не более 50 символов.",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        int maxPoints = -1;
        if (!int.TryParse(TbMaxPoints.Text, out maxPoints))
        {
            MessageBox.Show("Баллы должны быть числом!",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }

        if (maxPoints <= 0)
        {
            MessageBox.Show("Баллы должны быть больше 0!",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }

        if (isEdit && points != -1)
        {
            if (maxPoints < points)
            {
                MessageBox.Show($"Вы не можете поставить такое количество баллов, так как в результатах " +
                                $"максимальное количество баллов по этому тесту - {points}. Измените баллы в результатах для начала!",
                    "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                return false;
            }
        }
        return true;
        
    }
    
    
    private void BtnSubmit_Click(object sender, RoutedEventArgs e)
    {
        string sqlQuery;
        switch (typeOfWork)
        {
            case "edit":
                sqlQuery = $"SELECT IFNULL(MAX(points),-1) FROM results WHERE id_test = '{_test.Id}';";
                var reader = MainFunctions.ConnectToDB(sqlQuery, "SELECT").ExecuteReader();
                reader.Read();
                if (!ValidationData(true, reader.GetInt32(0)))
                {
                    return;
                };
                reader.Close();
                sqlQuery = $"UPDATE tests SET name_test = '{TbTestName.Text}', max_points = '{TbMaxPoints.Text}' " +
                           $"WHERE id_test = '{_test.Id}'";
                MainFunctions.ConnectToDB(sqlQuery, "UPDATE");
                MessageBox.Show("Тест изменен успешно!",
                    "Изменение теста",MessageBoxButton.OK,MessageBoxImage.Asterisk);
                break;
            case "add":
                if (!ValidationData(false, -1))
                {
                    return;
                };
                sqlQuery = $"INSERT INTO tests (name_test, max_points)  VALUES ('{TbTestName.Text}','{TbMaxPoints.Text}');";
                MainFunctions.ConnectToDB(sqlQuery, "UPDATE");
                MessageBox.Show("Тест добавлен успешно!",
                    "Добавление теста",MessageBoxButton.OK,MessageBoxImage.Asterisk);
                break;
        }
        Teacher teacher = new Teacher(_profile);
        teacher.Show();
        this.Close();
    }

    private void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        TbTestName.Text = "";
        TbMaxPoints.Text = "";
    }

    private void BtnReturn_Click(object sender, RoutedEventArgs e)
    {
        Teacher teacher = new Teacher(_profile);
        teacher.Show();
        this.Close();
    }
}