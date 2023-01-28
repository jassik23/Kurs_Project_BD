using System;
using System.Collections.Generic;
using System.Windows;
using System.Data.SQLite;
using System.Windows.Controls;

namespace Kurs_Project_BD;

public partial class Admin : Window
{
    private String _sqlQuery;
    private SQLiteConnection _mDbConn;
    private SQLiteCommand _mSqlCmd;
    private readonly List<Profile> _allProfiles;
    private readonly Profile adminProfile;
    private int _selectedProfile;
    public Admin(Profile prof)
    {
        InitializeComponent();
        _selectedProfile = -1;
        adminProfile = prof;
        if (prof.Patronymic == "")
        {
            ProfileAdminText.Text =
                $"Панель администратора. Администратор: \n {prof.LastName} {prof.FirstName.Substring(0, 1)}.";
        }
        else
        {
            ProfileAdminText.Text =
                $"Панель администратора. Администратор: \n {prof.LastName} {prof.FirstName.Substring(0, 1)}. {prof.Patronymic.Substring(0, 1)}.";
        }

        _allProfiles = new List<Profile>();
        UpdateProfilesTable(null,null);
    }
    
    private void profile_Click(object sender, RoutedEventArgs e)
    {
        var item = (sender as ListView).SelectedItem;
        int i = lvUsers.SelectedIndex;
        if (item != null)
        {
            _selectedProfile = i;
        }
    }
    private void Form1_Close(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
    private void UpdateProfilesTable(object sender, RoutedEventArgs routedEventArgs)
    {
        lvUsers.Items.Clear();
        var i = 0;
        _allProfiles.Clear();
        _sqlQuery = $"SELECT * FROM users WHERE NOT(passport = '{adminProfile.Passport}') ORDER BY userType, lastName, firstName";
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
    private void AuthReturn(object sender, EventArgs e)
    {
        MainWindow window = new MainWindow();
        window.Show();
        this.Close();
    }

    private void btnAddUser_click(object sender, EventArgs e)
    {
        EditUser editUser = new EditUser(adminProfile,"add_new_profile", null);
        editUser.Show();
        this.Close();
    }

    private void btnEditUser_Click(object sender, EventArgs e)
    {
        if (_selectedProfile == -1)
        {
            MessageBox.Show("Для изменения выберите профиль.");
            return;
        }
        EditUser editUser = new EditUser(adminProfile,"edit_profile", _allProfiles[_selectedProfile]);
        editUser.Show();
        this.Close();
    }

    private void BtnRemoveUser_OnClick(object sender, RoutedEventArgs e)
    {
        if (_selectedProfile == -1)
        {
            MessageBox.Show("Для изменения выберите профиль.");
            return;
        }
        string sqlQuery;
        sqlQuery = $"SELECT * FROM results WHERE passport = '{_allProfiles[_selectedProfile].Passport}'";
        var reader = MainFunctions.ConnectToDB(sqlQuery, "SELECT").ExecuteReader();
        if (reader.HasRows)
        {
            var result = MessageBox.Show(
                "С этим пользователем есть связанные записи. При удалении этого пользователя удалятся и записи. " +
                "Вы точно хотите удалить пользователя?", 
                "Сообщение", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Information, 
                MessageBoxResult.No, 
                MessageBoxOptions.DefaultDesktopOnly);
            if (result == MessageBoxResult.No)
            {
                MessageBox.Show("Пользователь не будет удалён");
                return;
            }
            reader.Close();
            sqlQuery = $"DELETE FROM results WHERE passport = '{_allProfiles[_selectedProfile].Passport}'";
            MainFunctions.ConnectToDB(sqlQuery, "UPDATE");
        }
        reader.Close();
        sqlQuery = $"DELETE FROM users WHERE passport = '{_allProfiles[_selectedProfile].Passport}'";
        MainFunctions.ConnectToDB(sqlQuery, "UPDATE");
        MessageBox.Show("Пользователь удалён");
        UpdateProfilesTable(null,null);

    }
    private void ChangePassword_Click(object sender, EventArgs e)
    {
        EditUser editUser = new EditUser(adminProfile, "change_password",null);
        editUser.Show();
        this.Close();
    }
    
}