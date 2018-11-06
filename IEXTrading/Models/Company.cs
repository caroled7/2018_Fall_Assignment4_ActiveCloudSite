using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IEXTrading.Models
{
    public class Company
    {

        [Key]
        public string symbol { get; set; }
        public string companyName { get; set; }
        public string sector { get; set; }
        public float open { get; set; }
        public float close { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string latestVolume { get; set; }
        public string peRatio { get; set; }
        public float week52High { get; set; }
        public float week52Low { get; set; }
        public List<Equity> Equities { get; set; }

    }
}
