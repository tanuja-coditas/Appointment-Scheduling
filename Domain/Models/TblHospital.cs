using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblHospital
{
    public Guid HospitalId { get; set; }

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public TblHospital()
    {
        HospitalId = Guid.NewGuid();
    }
}
