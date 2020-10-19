using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;

namespace Tema4
{
    class Program
    {
        private static CloudTable studentsTable;
        private static CloudTableClient tableClient;
        private static TableOperation tableOperation;
        private static TableResult tableResult;
        private static List<StudentEntity> students  = new List<StudentEntity>();
        static void Main(string[] args)
        {
            Task.Run(async () => { await Initialize(); })
                .GetAwaiter()
                .GetResult();
        }
        static async Task Initialize()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;"
            + "AccountName=nibudatc;"
            + "AccountKey=gfhnwzTQlTuAL5sN8VoB+GrP6YCkWzOJe1IywFJAM+IxcCsD/vA7HszjfAkOmkEwwotaZbSgmnMr3E9uXX07Mw==;"
            + "EndpointSuffix=core.windows.net";

            var account = CloudStorageAccount.Parse(storageConnectionString);
            tableClient = account.CreateCloudTableClient();

            studentsTable = tableClient.GetTableReference("Students");

            await studentsTable.CreateIfNotExistsAsync();
            
            int option = -1;
            do
            {
                System.Console.WriteLine("1.Add student.");
                System.Console.WriteLine("2.Update student.");
                System.Console.WriteLine("3.Delete student.");
                System.Console.WriteLine("4.Show all students.");
                System.Console.WriteLine("5.Exit");
                System.Console.WriteLine("Please enter your selection:");
                string opt = System.Console.ReadLine();
                option =Int32.Parse(opt);
                switch(option)
                {
                    case 1:
                        await AddNewStudent();
                        break;
                    case 2:
                        await EditStudent();
                        break;
                    case 3:
                        await DeleteStudent();
                        break;
                    case 4:
                        await DisplayStudents();
                        break;
                    case 5:
                        System.Console.WriteLine("Thank you for visit!");
                        break;
                }
            }while(option != 5);
            
        }
        private static async Task<StudentEntity> RetrieveRecordAsync(CloudTable table,string partitionKey,string rowKey)
        {
            tableOperation = TableOperation.Retrieve<StudentEntity>(partitionKey, rowKey);
            tableResult = await table.ExecuteAsync(tableOperation);
            return tableResult.Result as StudentEntity;
        }
        private static async Task AddNewStudent()
        {
            System.Console.WriteLine("Insert university:");
            string university = Console.ReadLine();
            System.Console.WriteLine("Insert cnp:");
            string cnp = Console.ReadLine();
            System.Console.WriteLine("Insert firstName:");
            string firstName = Console.ReadLine();
            System.Console.WriteLine("Insert lastName:");
            string lastName = Console.ReadLine();
            System.Console.WriteLine("Insert faculty:");
            string faculty = Console.ReadLine();
            System.Console.WriteLine("Insert year of study:");
            string year = Console.ReadLine();

            StudentEntity stud = await RetrieveRecordAsync(studentsTable, university, cnp);
            if(stud == null)
            {
                var student = new StudentEntity(university,cnp);
                student.FirstName = firstName;
                student.LastName = lastName;
                student.Faculty = faculty;
                student.Year = Convert.ToInt32(year);
                var insertOperation = TableOperation.Insert(student);
                await studentsTable.ExecuteAsync(insertOperation);
                System.Console.WriteLine("Record inserted!");
            }
            else
            {
                System.Console.WriteLine("Record exists!");
            }
        }
        private static async Task EditStudent()
        {
            System.Console.WriteLine("Insert university:");
            string university = Console.ReadLine();
            System.Console.WriteLine("Insert cnp:");
            string cnp = Console.ReadLine();
            StudentEntity stud = await RetrieveRecordAsync(studentsTable, university, cnp);
            if(stud != null)
            {
                System.Console.WriteLine("Record exists!");
                var student = new StudentEntity(university,cnp);
                System.Console.WriteLine("Insert firstName:");
                string firstName = Console.ReadLine();
                System.Console.WriteLine("Insert lastName:");
                string lastName = Console.ReadLine();
                System.Console.WriteLine("Insert faculty:");
                string faculty = Console.ReadLine();
                System.Console.WriteLine("Insert year of study:");
                string year = Console.ReadLine();
                student.FirstName = firstName;
                student.LastName = lastName;
                student.Faculty = faculty;
                student.Year = Convert.ToInt32(year);
                student.ETag = "*";
                var updateOperation = TableOperation.Replace(student);
                await studentsTable.ExecuteAsync(updateOperation);
                System.Console.WriteLine("Record updated!");
            }
            else
            {
                System.Console.WriteLine("Record does not exists!");
            }
        }
        private static async Task DeleteStudent()
        {
            System.Console.WriteLine("Insert university:");
            string university = Console.ReadLine();
            System.Console.WriteLine("Insert cnp:");
            string cnp = Console.ReadLine();

            StudentEntity stud = await RetrieveRecordAsync(studentsTable, university, cnp);
            if(stud != null)
            {
                var student = new StudentEntity(university,cnp);
                student.ETag = "*";
                var deleteOperation = TableOperation.Delete(student);
                await studentsTable.ExecuteAsync(deleteOperation);
                System.Console.WriteLine("Record deleted!");
            }
            else
            {
                System.Console.WriteLine("Record does not exists!");
            }
        }
        private static async Task<List<StudentEntity>> GetAllStudents()
        {
            TableQuery<StudentEntity> tableQuery = new TableQuery<StudentEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<StudentEntity> result = await studentsTable.ExecuteQuerySegmentedAsync(tableQuery,token);
                token = result.ContinuationToken;
                students.AddRange(result.Results);
            }while(token != null);
            return students;
        }
        private static async Task DisplayStudents()
        {
            await GetAllStudents();

            foreach(StudentEntity std in students)
            {
                Console.WriteLine("Student faculty : {0}", std.Faculty);
                Console.WriteLine("Student firstName : {0}", std.FirstName);
                Console.WriteLine("Student lastName : {0}", std.LastName);
                Console.WriteLine("Student year : {0}", std.Year);
                Console.WriteLine("******************************");
            }
            students.Clear();
            
        }
    }
}