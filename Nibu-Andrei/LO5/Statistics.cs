using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Models
{
    public class Statistics : TableEntity
    {
        public Statistics(string university,string date)
        {
            this.PartitionKey = university;
            this.RowKey = date.ToString();
        }
        public Statistics() {}
        public int TotalNrOfStudents{get; set;} 
    }
}
