using System;
using System.Windows;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;


namespace Kurs_Project_BD
{

    public partial class MainWindow : Window
    {
        public static String dbFileName = $"identifier.sqlite";
        String _sqlQuery;
        private SQLiteConnection _mDbConn;
        private SQLiteCommand _mSqlCmd;
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            _mDbConn = new SQLiteConnection();
            _mSqlCmd = new SQLiteCommand();
            _sqlQuery = $"SELECT * FROM users " +
                       $"WHERE login = '{login.Text}'";
            if (login.Text == "" || password.Password == "")
            {
                MessageBox.Show("Поля Логин/Пароль не могут быть пустыми",
                    "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            
            if (!MainFunctions.IsValidLogin(login.Text))
            {
                MessageBox.Show(
                    "Логин должен быть от 4 до 20 символов, должен состоять из букв английского алфавита и цифр и не должен начинаться с цифры.",
                    "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            if (!MainFunctions.IsValidPassword(password.Password))
            {
                MessageBox.Show(
                    "Пароль должен быть от 8 до 50 символов. И не должен содержать букв кириллицы.",
                    "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            var reader = MainFunctions.ConnectToDB(_sqlQuery, "SELECT").ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    
                    Profile prof = new Profile(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                        reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                    if (prof.Password == password.Password)
                    {
                        switch (prof.TypeOfUser)
                        {
                            case "Ученик": 
                                MessageBox.Show("Переход в Панель ученика");
                                Student student = new Student(prof);
                                student.Show();
                                this.Close();
                                break;
                            case "Учитель": 
                                MessageBox.Show("Переход в Панель учителя");
                                Teacher teacher = new Teacher(prof);
                                teacher.Show();
                                this.Close();
                                break;
                            case "Администратор системы": 
                                MessageBox.Show("Переход в Админ Панель");
                                Admin admin = new Admin(prof);
                                admin.Show();
                                this.Close();
                                break;
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("Неправильный пароль",
                            "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Такого логина нет в БД",
                        "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                }

        }

        private void Form1_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
}