using CsvHelper;
using CsvHelper.Configuration;
using Evangelion01.Contracts;
using Evangelion01.Contracts.Models;
using Evangelion01.DataTool;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using System.Globalization;

// connect to Cosmos DB
var cosmosClientBuilder = new CosmosClientBuilder(EnvironmentConfig.DatabaseConnectionString);
var cosmosClient = cosmosClientBuilder
    .WithConnectionModeDirect()
    .WithThrottlingRetryOptions(TimeSpan.FromSeconds(60), 5)
    .WithContentResponseOnWrite(false)
    .WithBulkExecution(true)
    .Build();

// get reference to database and container
var database = cosmosClient.GetDatabase(EnvironmentConfig.DatabaseName);

// csv reader config
var csvConfig = new CsvConfiguration(new CultureInfo("en-US"));
csvConfig.Delimiter = ",";
csvConfig.HasHeaderRecord = true;
csvConfig.IgnoreBlankLines = true;

// -- import students data
Console.WriteLine("IMPORTING NAMES...");
using var studentFileReader = new StreamReader("sn_students_import.csv");
using var studentCsvReader = new CsvReader(studentFileReader, csvConfig);

var studentsContainer = database.GetContainer(Student.Container);
await foreach (var student in studentCsvReader.EnumerateRecordsAsync(new Student()))
{
    Console.WriteLine("Importing: {0}", student.Name);
    //await studentsContainer.CreateItemAsync(student);
}

// -- import grades data
Console.WriteLine("IMPORTING GRADES...");
using var gradeFileReader = new StreamReader("sn_grades_import.csv");
using var gradeCsvReader = new CsvReader(gradeFileReader, csvConfig);

using var failedIdsFileWriter = new StreamWriter("failed.csv");

var gradeInsertTasks = new List<Task>();
var gradesContainer = database.GetContainer(Grade.Container);

Console.WriteLine("Starting bulk import for grades...");
var records = gradeCsvReader.EnumerateRecordsAsync(new GradeCsv());
await foreach (var grade in records)
{
    var item = new Grade
    {
        GradeId = grade.GradeId,
        StudentId = grade.StudentId,
        Subject = grade.Subject,
        Semester = grade.Semester,
        Score = grade.Score,
        Categories = EntityValueConverter.GetCategoriesFromSubject(grade.Subject).ToList()
    };

    // start new insert task
    var currentTask = gradesContainer.CreateItemAsync(item, new PartitionKey(item.GradeId))
        .ContinueWith(itemResponse =>
        {
            if (!itemResponse.IsCompletedSuccessfully)
            {
                failedIdsFileWriter.WriteLine(item.GradeId);
                AggregateException innerExceptions = itemResponse.Exception.Flatten();
                if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
                {
                    Console.WriteLine($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
                }
                else
                {
                    Console.WriteLine($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
                }
            }
        });

    // add task to list
    gradeInsertTasks.Add(currentTask);
}

// Wait until all are done
Console.WriteLine("Awaiting bulk import for grades..."); 
await Task.WhenAll(gradeInsertTasks);

Console.WriteLine("IMPORT COMPLTETE!");
