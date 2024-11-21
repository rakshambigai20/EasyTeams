using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTeams.Data.Models.Domain
{

    // Class to represent the BankHolidays created with the help of http://json2csharp.com/
    //And https://stackoverflow.com/questions/36568213/json-parse-bank-holiday-calendar
    //JSON file from https://www.gov.uk/bank-holidays.json
    public class EnglandAndWales
    {
        public string Division { get; set; }
        public List<Event> Events { get; set; }
    }
    public class NorthernIreland
    {
        public string Division { get; set; }
        public List<Event> Events { get; set; }
    }
    public class Scotland
    {
        public string Division { get; set; }
        public List<Event> Events { get; set; }
    }
    public class Event
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Notes { get; set; }
        public bool Bunting { get; set; }
    }
    public class Root
    {
        [JsonProperty("england-and-wales")]
        public EnglandAndWales EnglandAndWales { get; set; }
        [JsonProperty("scotland")]
        public Scotland Scotland { get; set; }
        [JsonProperty("northern-ireland")]
        public NorthernIreland NorthernIreland { get; set; }
    }
}
