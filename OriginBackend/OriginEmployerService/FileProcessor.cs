using OriginDatabase.Models;
using OriginDatabase;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using OriginCommon.ViewModels;
using EmployerService.Controllers;

namespace OriginEmployerService
{
    public class FileProcessor : IFileProcessor
    {
        private readonly ILogger<EmployerController> _logger;
        private readonly UserManagementDbContext _context;

        public FileProcessor(ILogger<EmployerController> logger, UserManagementDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task ProcessFileAsync(string filePath, string employerName)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                HeaderValidated = null,
                IgnoreReferences = true,
            };

            var results = new List<ProcessingResult>();

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, config))
                {
                    var existingEmails = await _context.Users.Select(u => u.Email).ToListAsync();
                    var records = csv.GetRecordsAsync<User>();
                    await foreach (var record in records)
                    {
                        var result = new ProcessingResult { Email = record.Email };
                        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == record.Email);

                        if (existingUser != null)
                        {
                            existingUser.Country = record.Country;
                            existingUser.Salary = record.Salary;
                            result.IsSuccess = true;
                            result.Message = "Updated existing user.";
                        }
                        else
                        {
                            record.EmployerName = employerName;
                            _context.Users.Add(record);
                            result.IsSuccess = true;
                            result.Message = "Added new user.";
                        }

                        existingEmails.Remove(record.Email);

                        results.Add(result);
                    }

                    // Process remaining emails in existingEmails list (users not found in the eligibility file)
                    foreach (var email in existingEmails)
                    {
                        var deletedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                        if (deletedUser != null)
                        {
                            var result = new ProcessingResult
                            {
                                Email = deletedUser.Email,
                                IsSuccess = true,
                                Message = "User deleted."
                            };
                            results.Add(result);

                            _context.Users.Remove(deletedUser);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing file.");
                throw;
            }

            await GenerateReportFileAsync(results, "FileProcessReport.csv");
        }

        /// <summary>
        /// Generates a report file including which entries were successfully processed and which operation was performed (add/update)
        /// </summary>
        /// <param name="results"></param>
        /// <param name="reportFilePath"></param>
        /// <returns></returns>
        private async Task GenerateReportFileAsync(IEnumerable<ProcessingResult> results, string reportFilePath)
        {
            await using var writer = new StreamWriter(reportFilePath);
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<ProcessingResult>();
            await csv.NextRecordAsync();
            await csv.WriteRecordsAsync(results);
        }
    }
}
