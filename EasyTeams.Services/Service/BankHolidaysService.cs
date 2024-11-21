using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Services.IService;
using EasyTeams.Data.Models.Domain;
using System.Net.Http;
using Newtonsoft.Json;

namespace EasyTeams.Services.Service
{
    // Service to retrieve bank holidays
    public class BankHolidaysService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://www.gov.uk/bank-holidays.json"; //link to API

        // Constructor
        public BankHolidaysService()
        {
            _httpClient = new HttpClient();
        }

        // Method to get the bank holidays
        public async Task<List<DateTime>> GetBankHolidays(int year)
        {
            var bankHolidays = new List<DateTime>();

            // Try to get the bank holidays
            try
            {
                var response = await _httpClient.GetAsync($"https://www.gov.uk/bank-holidays.json"); ;
                if (response.IsSuccessStatusCode)
                {

                    // Read the content of the response
                    var content = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON
                    var allBankHolidays = JsonConvert.DeserializeObject<Root>(content);

                    // Check if the bank holidays are not null and if bank holidays are available for England and Wales
                    if (allBankHolidays != null && allBankHolidays.EnglandAndWales.Events != null)
                    {
                        // Loop through the bank holidays for England and Wales and add them to the list
                        foreach (var bankHoliday in allBankHolidays.EnglandAndWales.Events)
                        {
                            if (DateTime.TryParse(bankHoliday.Date, out var date))
                            {
                                bankHolidays.Add(date.Date);
                            }
                        }
                    }
                }
            } 

            // Catching the exception and logging it
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}"); // Log the error
            }
            return bankHolidays;  // Return the bank holidays
        }
    }
}
