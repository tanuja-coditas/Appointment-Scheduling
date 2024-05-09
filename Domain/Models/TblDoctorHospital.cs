using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblDoctorHospital
{
    public Guid DoctorId { get; set; }

    public Guid HospitalId { get; set; }

    public virtual TblUser Doctor { get; set; } = null!;

    public virtual TblHospital Hospital { get; set; } = null!;
}
