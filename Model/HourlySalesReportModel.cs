using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourlySalesReport.Model
{
    public class HourlySalesReportModel
    {
        //public int HistoricActualGuestCount { get; set; }
        private float _HistoricActualGuestCount;

        public float HistoricActualGuestCount
        {
            get { return _HistoricActualGuestCount; }
            set { _HistoricActualGuestCount = value; }
        }


        //public DateTime HistoriceDateTime { get; set; }
        private DateTime _HistoriceDateTime;

        public DateTime HistoriceDateTime
        {
            get { return _HistoriceDateTime; }
            set { _HistoriceDateTime = value; }
        }


        //public int HistoriceProjectedGuestCount { get; set; }
        private float _HistoriceProjectedGuestCount;

        public float HistoriceProjectedGuestCount
        {
            get { return _HistoriceProjectedGuestCount; }
            set { _HistoriceProjectedGuestCount = value; }
        }

    }
}
