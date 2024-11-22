using System;

namespace CaseStudy.Models
{
    public class FundReturn
    {
        public int Id { get; set; }
        public int FundId { get; set; }
        public decimal? OneMonthReturn { get; set; }      
        public decimal? ThreeMonthReturn { get; set; }    
        public decimal? SixMonthReturn { get; set; }      
        public decimal? YearToDateReturn { get; set; }    
        public decimal? OneYearReturn { get; set; }
        public decimal? ThreeYearReturn { get; set; }
        public decimal? FiveYearReturn { get; set; }
        public virtual Fund Fund { get; set; }
    }

}