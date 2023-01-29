using System;
using System.Windows;
using System.Data.SQLite;
namespace Kurs_Project_BD;

public partial class Student : Window
{
    private Profile myProfile;
    public Student(Profile prof)
    {
        InitializeComponent();
        myProfile = prof;
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

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        StudentReportWindow reportWindow = new StudentReportWindow(myProfile);
        reportWindow.Show();
    }
}