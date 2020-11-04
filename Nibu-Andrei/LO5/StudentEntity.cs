using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Models
{
    public class StudentEntity : TableEntity
    {
        public StudentEntity(string University, string Cnp)
        {
            this.PartitionKey = University;
            this.RowKey = Cnp;
        }
        public StudentEntity() {}

        public string FirstName {get; set;}
        public string LastName {get; set;}
        public string Faculty {get; set;}
        public int Year {get; set;}
    }
}