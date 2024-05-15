using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
    public  class AvailabilityDTO
    {
        public DateTime AvailabilityStartDatetime { get; set; }

        public DateTime AvailabilityEndDatetime { get; set; }

        public int AppointmentCount { get; set; }
    }
}
