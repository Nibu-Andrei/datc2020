using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;

namespace Lab5App
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    internal class NewBaseType
    {
        public static int previousTotal = 0;
        private static CloudTable studentsTable;
        private static CloudTableClient tableClient;
        private static CloudTable universityMetrics;
        private static CloudTableClient tableMetrics;
        private static List<StudentEntity> students = new List<StudentEntity>();
        private static List<Statistics> metrics = new List<Statistics>();

        public static CloudTable StudentsTable { get => studentsTable; set => studentsTable = value; }
        public static List<Statistics> Metrics { get => metrics; set => metrics = value; }

        private static async Task DisplayStudents(string storageConnectionString)
        {
            await GetAllStudents();

            var accountMetrics = CloudStorageAccount.Parse(storageConnectionString);
            tableMetrics = accountMetrics.CreateCloudTableClient();

            universityMetrics = tableMetrics.GetTableReference("UniversityMetrics");
            await universityMetrics.CreateIfNotExistsAsync();
            await GetAllMetrics();
            List<int> totalStudents = new List<int>();
            int UptCounter = 0;
            int UmftCounter = 0;
            foreach (StudentEntity std in students)
            {
                if (std.PartitionKey == "UPT")
                    UptCounter++;
                else
                    UmftCounter++;
            }
            foreach (Statistics stat in Metrics)
            {
                totalStudents.Add(stat.TotalNrOfStudents);
            }

            int total = UptCounter + UmftCounter;
            previousTotal = Convert.ToInt32(totalStudents.Max());

            if (total != previousTotal)
            {
                var timeSpan1 = DateTime.Now.ToString("o");
                Statistics stat1 = new Statistics("UPT", timeSpan1);
                stat1.TotalNrOfStudents = UptCounter;
                var insertOperation1 = TableOperation.Insert(stat1);
                await universityMetrics.ExecuteAsync(insertOperation1);

                var timeSpan2 = DateTime.Now.ToString("o");
                Statistics stat2 = new Statistics("UMFT", timeSpan2);
                stat2.TotalNrOfStudents = UmftCounter;
                var insertOperation2 = TableOperation.Insert(stat2);
                await universityMetrics.ExecuteAsync(insertOperation2);

                var timeSpan3 = DateTime.Now.ToString("o");
                Statistics stat3 = new Statistics("Total", timeSpan3);
                stat3.TotalNrOfStudents = total;
                var insertOperation3 = TableOperation.Insert(stat3);
                await universityMetrics.ExecuteAsync(insertOperation3);

                students.Clear();

            }
        }
        private static async Task<List<Statistics>> GetAllMetrics()
        {
            TableQuery<Statistics> tableQuery = new TableQuery<Statistics>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<Statistics> result = await universityMetrics.ExecuteQuerySegmentedAsync(tableQuery, token);
                token = result.ContinuationToken;
                Metrics.AddRange(result.Results);
            } while (token != null);
            return Metrics;
        }
        private static async Task<List<StudentEntity>> GetAllStudents()
        {
            TableQuery<StudentEntity> tableQuery = new TableQuery<StudentEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<StudentEntity> result = await StudentsTable.ExecuteQuerySegmentedAsync(tableQuery, token);
                token = result.ContinuationToken;
                students.AddRange(result.Results);
            } while (token != null);
            return students;
        }
        static async Task Initialize()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;"
            + "AccountName=nibudatc;"
            + "AccountKey=gfhnwzTQlTuAL5sN8VoB+GrP6YCkWzOJe1IywFJAM+IxcCsD/vA7HszjfAkOmkEwwotaZbSgmnMr3E9uXX07Mw==;"
            + "EndpointSuffix=core.windows.net";

            var account = CloudStorageAccount.Parse(storageConnectionString);
            tableClient = account.CreateCloudTableClient();

            StudentsTable = tableClient.GetTableReference("Students");

            await StudentsTable.CreateIfNotExistsAsync();

            await DisplayStudents(storageConnectionString);
        }

        static void Main(string[] args)
        {
            Task.Run(async () => { await Initialize(); })
                .GetAwaiter()
                .GetResult();
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }

}