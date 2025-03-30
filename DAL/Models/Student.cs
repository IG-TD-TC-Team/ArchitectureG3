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
            UserID = Guid.NewGuid().ToString().ToString();
            CardID = Guid.NewGuid().ToString().ToString();//Search for a real ID creator

        }


        public override string UserID { get; set ; }
        public override string CardID { get; set; } 
        public override string UserName { get ; set ; }
        public override string FirstName { get; set; }
        public override string LastName { get ; set ; }
        public override string Password { get ; set ; }


    }
}
