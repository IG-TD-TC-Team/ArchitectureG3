namespace DAL.Models
{
    public class Student : AbstractUser
    {
       
        public Student(string userName, string firstName, string lastName, string password)
        {

            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
        }
                        
        public override string UserName { get ; set ; }
        public override string FirstName { get; set; }
        public override string LastName { get ; set ; }
        public override string Password { get ; set ; }

    }
}
