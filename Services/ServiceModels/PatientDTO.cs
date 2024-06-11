﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
    public class PatientDTO
    {
        public string Username { get;set; }
        public string FirstName { get;set; }    
        public string LastName { get;set; } 
        public string Email { get;set; }
        public string Phone { get;set; }
        public string Address { get;set; }
        public string ImagePath { get;set; }
    }
}
