using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public class WaitListRepo
    {
        private readonly AppointmentSchedulingContext _context;

        public WaitListRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }

        public async Task Create(TblWaitlist waitlist)
        {
            await _context.TblWaitlists.AddAsync(waitlist);
            await _context.SaveChangesAsync();
        }
    }
}
