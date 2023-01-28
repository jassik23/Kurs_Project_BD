using System.Data.Common;

namespace Kurs_Project_BD;

public class MainStructures
{
    
}

public class Profile
{
    public Profile(string lastName, string firstName, string patronymic, string passport, string login, string password, string typeOfUser)
    {
        LastName = lastName;
        FirstName = firstName;
        Patronymic = patronymic;
        Passport = passport;
        Login = login;
        Password = password;
        TypeOfUser = typeOfUser;
    }

    public Profile()
    {
        LastName = "";
        FirstName = "";
        Patronymic = "";
        Login = "";
        Password = "";
        TypeOfUser = "Ученик";
    }
    public string LastName{ get; set; }
    public string FirstName{ get; set; }
    public string Patronymic{ get; set; }
    public string Passport{ get; set; }
    public string Login{ get; set; }
    public string Password{ get; set; }
    public string TypeOfUser{ get; set; }
    
}


public class StudentReport
{
    public int Id{ get; set; }
    public string NameOfTest{ get; set; }
    public string DateTest{ get; set; }
    public int Points{ get; set; }
    public int MaxPoints{ get; set; }
    
}

public class Report
{
    public string LastName{ get; set; }
    public string FirstName{ get; set; }
    public string Patronymic{ get; set; }
    public string Passport{ get; set; }
    public string DateTest{ get; set; }
    public int Points{ get; set; }

}


public class Test
{
    public int Id { get; set; }
    public string TestNaming { get; set; }
    public int MaxPoints { get; set; }
}

public class Results
{
    public int Id { get; set; }
    public string Fio  { get; set; }
    public string Passport  { get; set; }
    public string TestName  { get; set; }
    public string DateResult  { get; set; }
    public int Points { get; set; }
    public int IdTest { get; set; }
}