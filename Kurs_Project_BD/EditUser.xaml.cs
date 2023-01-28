using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.Windows.Input;

namespace Kurs_Project_BD;

public partial class EditUser : Window
{
    private Profile _profile;
    private Profile _profileChange;
    private string parWork;
    public EditUser(Profile prof, string change, Profile profChange)
    {
        InitializeComponent();
        _profile = prof;
        parWork = change;
        switch (change)
        {
            case "change_password":
                Info.Text = "Сменить пароль";
                TbLastName.IsReadOnly = true;
                TbPassport.IsReadOnly = true;
                TbPatronymic.IsReadOnly = true;
                TbFirstName.IsReadOnly = true;
                TbLogin.IsReadOnly = true;
                CbUserType.Visibility = Visibility.Hidden;
                TypeTextBlock.Visibility = Visibility.Hidden;
                BtnReset.Visibility = Visibility.Hidden;
                
                TbLastName.Text = prof.LastName;
                TbPassport.Text = prof.Passport;
                TbPatronymic.Text = prof.Patronymic;
                TbFirstName.Text = prof.FirstName;
                TbLogin.Text = prof.Login;
                PbPassword.Text = prof.Password;
                break;
            case "add_new_profile":
                Info.Text = "Добавить пользователя";
                break;
            case "edit_profile":
                Info.Text = "Изменение данных пользователя";
                _profileChange = profChange;
                TbLastName.Text = _profileChange.LastName;
                TbPassport.Text = _profileChange.Passport;
                TbPatronymic.Text = _profileChange.Patronymic;
                TbFirstName.Text = _profileChange.FirstName;
                TbLogin.Text = _profileChange.Login;
                PbPassword.Text = _profileChange.Password;
                CbUserType.Text = _profileChange.TypeOfUser;
                BtnReset.Visibility = Visibility.Hidden;
                break;
        }
    }
    private void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        TbLastName.Text = "";
        TbPassport.Text = "";
        TbPatronymic.Text = "";
        TbFirstName.Text = "";
        TbLogin.Text = "";
        PbPassword.Text = "";
        CbUserType.Text = "";
    }

    private bool ValidationData()
    {
        if (TbPassport.Text == "" || TbLogin.Text == "" || TbFirstName.Text == "" || TbLastName.Text == "" || PbPassword.Text == "" || CbUserType.Text == "")
        {
            MessageBox.Show("Поля не должны быть пустыми",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (!MainFunctions.IsValidFio(TbLastName.Text))
        {
            MessageBox.Show("Фамилия должна состоять из русских букв. Должна начинаться с заглавной буквы.",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (!MainFunctions.IsValidFio(TbFirstName.Text))
        {
            MessageBox.Show("Имя должно состоять из русских букв. Должна начинаться с заглавной буквы.",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (!MainFunctions.IsValidFio(TbPatronymic.Text) && TbPatronymic.Text != "")
        {
            MessageBox.Show("Отчество должно состоять из русских букв. Должна начинаться с заглавной буквы.",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (!MainFunctions.IsValidPassport(TbPassport.Text))
        {
            MessageBox.Show("Паспорт должен формата 00 00 000000, где 0 - это любая цифра.",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (!MainFunctions.IsValidLogin(TbLogin.Text))
        {
            MessageBox.Show("Логин должен быть от 4 до 20 символов, должен состоять из букв английского алфавита и цифр и не должен начинаться с цифры.",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        if (!MainFunctions.IsValidPassword(PbPassword.Text))
        {
            MessageBox.Show("Пароль должен быть от 8 до 50 символов. И не должен содержать букв кириллицы.",
                "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }
        return true;
    }
    
    
    private void BtnSubmit_Click(object sender, RoutedEventArgs e)
    {
        string sqlQuery;
        switch (parWork)
        {
            case "change_password":
                var newPassword = PbPassword.Text;
                if (_profile.Password == newPassword)
                {
                    MessageBox.Show("Пароль не был изменен. Возврат назад",
                        "Изменение пароля",MessageBoxButton.OK,MessageBoxImage.Information);
                    Student student1 = new Student(_profile);
                    student1.Show();
                    this.Close();
                    return;
                }
                if (!MainFunctions.IsValidPassword(newPassword))
                {
                    MessageBox.Show("Пароль должен быть от 8 до 50 символов. Также в пароле не должно быть символов русского алфавита",
                        "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }
                sqlQuery = $"UPDATE users SET password = '{newPassword}' WHERE passport = '{_profile.Passport}'";
                MainFunctions.ConnectToDB(sqlQuery, "UPDATE");
                MessageBox.Show("Пароль успешно изменен",
                    "Изменение пароля",MessageBoxButton.OK,MessageBoxImage.Information);
                _profile.Password = newPassword;
                switch (_profile.TypeOfUser)
                {
                    case "Учитель":
                        Teacher teacher = new Teacher(_profile);
                        teacher.Show();
                        break;
                    case "Ученик" :
                        Student student = new Student(_profile);
                        student.Show();
                        break;
                    case "Администратор системы" :
                        Admin admin = new Admin(_profile);
                        admin.Show();
                        break;
                }
                this.Close();
                break;
            case "add_new_profile":
                if (ValidationData())
                {
                    sqlQuery = $"SELECT * FROM users WHERE passport = '{TbPassport.Text}'";
                    var reader = MainFunctions.ConnectToDB(sqlQuery, "SELECT").ExecuteReader();
                    if (reader.HasRows)
                    {
                        MessageBox.Show(
                            "Пользователь с таким паспортом уже существует. Вы не можете завести ещё один профиль!",
                            "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                        return;
                    }
                    reader.Close();
                    sqlQuery = $"SELECT * FROM users WHERE login = '{TbLogin.Text}'";
                    reader = MainFunctions.ConnectToDB(sqlQuery, "SELECT").ExecuteReader();
                    if (reader.HasRows)
                    {
                        MessageBox.Show(
                            "Пользователь с таким логином уже существует. Вы не можете завести ещё один профиль!",
                            "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                        return;
                    }
                    reader.Close();
                    sqlQuery = $"INSERT INTO users VALUES " +
                               $"('{TbLastName.Text}','{TbFirstName.Text}','{TbPatronymic.Text}','{TbPassport.Text}','{TbLogin.Text}','{PbPassword.Text}','{CbUserType.Text}');";
                    MainFunctions.ConnectToDB(sqlQuery, "UPDATE");
                    MessageBox.Show("Пользователь добавлен успешно!",
                        "Добавление пользователя",MessageBoxButton.OK,MessageBoxImage.Asterisk);
                    Admin admin = new Admin(_profile);
                    admin.Show();
                    this.Close();
                }
                else return;
                break;
            case "edit_profile":
                if (ValidationData())
                {
                    sqlQuery =
                        $"UPDATE users SET " +
                        $"lastName = '{TbLastName.Text}', firstName = '{TbFirstName.Text}', patronymic = '{TbPatronymic.Text}', " +
                        $"passport = '{TbPassport.Text}', password = '{PbPassword.Text}', login = '{TbLogin.Text}', userType = '{CbUserType.Text}' " +
                        $"WHERE passport = '{_profileChange.Passport}'";
                    MainFunctions.ConnectToDB(sqlQuery, "UPDATE");
                    MessageBox.Show("Пользователь изменен.",
                        "Изменение пользователя",MessageBoxButton.OK,MessageBoxImage.Information);
                    Admin admin = new Admin(_profile);
                    admin.Show();
                    this.Close();
                }
                break;
        }
    }

    private void BtnReturn_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Возврат в меню",
            "Возврат",MessageBoxButton.OK,MessageBoxImage.Information);
        switch (parWork)
        {
            case "change_password":
                switch (_profile.TypeOfUser)
                {
                    case "Учитель":
                        Teacher teacher = new Teacher(_profile);
                        teacher.Show();
                        break;
                    case "Ученик" :
                        Student student = new Student(_profile);
                        student.Show();
                        break;
                    case "Администратор системы" :
                        Admin admin1 = new Admin(_profile);
                        admin1.Show();
                        break;
                }
                this.Close();
                break;
            case "add_new_profile": case "edit_profile":
                Admin admin = new Admin(_profile);
                admin.Show();
                this.Close();
                break;
        }
    }
    
    private int _cnt = 0;
    private const int MaxLenght = 10;
    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var tb = (sender as TextBox);
 
        if (!Char.IsDigit(e.Text, 0))
        {
            e.Handled = true;
            return;
        }
        else _cnt++;
        if (_cnt > MaxLenght)
        {
            _cnt = MaxLenght;
            e.Handled = true;
            return;
        }
        tb.Text =  tb.Text + e.Text;
        if (_cnt == 2 || _cnt == 4)
        {
            tb.Text = tb.Text + " ";
        }
        e.Handled = true;
    }
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var tb = (sender as TextBox);
        tb.CaretIndex = tb.Text.Length;
        _cnt = tb.Text.Replace(" ", "").Replace("-", "").Length;
    }
    
    
}