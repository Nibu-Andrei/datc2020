using System.Collections.Generic;

namespace ApiStudent
{
    public static class StudentRepo
    {
        public static List<Student> Student = new List<Student>()
        {
            new Student()
            {
                id=1,
                nume="Nibu",
                prenume="Andrei",
                facultate="AC",
                an=4
            },
            new Student()
            {
                id=2,
                nume="Dinamo",
                prenume="Bucuresti",
                facultate="AC",
                an=1
            }
        };
    }
}