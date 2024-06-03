using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
    public class DoctorVerificationModel
    {
        public string Specialization { get; set; }
        public IFormFile MedicalLicense { get; set; } 
        public IFormFile IdProof { get; set; }
        public IFormFile MedicalDegreeCertificate { get; set; }
        public IFormFile PostgraduateMedicalDegreeCertificate { get; set; }
        public IFormFile SpecializationCertificate { get; set; }
    }
}
