using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Services.ServiceModels;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
namespace Services.AuthServices
{
    public class DoctorServices
    {
        private readonly UserRepo _userRepo;
        private readonly SpecializationRepo _specializationRepo;
        private readonly AvailabilityRepo _availabilityRepo;

        public DoctorServices(UserRepo userRepo,SpecializationRepo specializationRepo, AvailabilityRepo availabilityRepo)
        {
            _userRepo = userRepo;
            _specializationRepo = specializationRepo;
            _availabilityRepo = availabilityRepo;

        }

        public List<SearchModel> GetDoctors()
        {
            var doctors = _userRepo.GetByRole("doctor");
            var specializations = _specializationRepo.GetSepcialiazations();
            var doctor_specailizations = _specializationRepo.GetDoctorSpecialization();

            var query = from doctor in doctors
                        join doctor_specialization in doctor_specailizations
                        on doctor.UserId equals doctor_specialization.DoctorId
                        select new
                        {
                            FirstName = doctor.FirstName,
                            LastName = doctor.LastName,
                            Email = doctor.Email,
                            PhoneNumber = doctor.PhoneNumber,
                            Address = doctor.Address,
                            UserImage = doctor.UserImage,
                            SpecializationId = doctor_specialization.SpecializationId
                        };
            var result = (from doctor in query
                          join specialization in specializations
                          on doctor.SpecializationId equals specialization.SpecializationId
                          select new SearchModel(
                               doctor.FirstName,
                             doctor.LastName,
                             doctor.Email,
                              doctor.PhoneNumber,
                              doctor.Address,
                              doctor.UserImage,
                              specialization.SpecializationName,
                              specialization.DegreeName
                              )).ToList();
            return result;
        }

        public List<SearchModel> GetDoctorsByName(string name)
        {
            var doctors = _userRepo.GetByRole("doctor");
            var specializations = _specializationRepo.GetSepcialiazations();
            var doctor_specailizations = _specializationRepo.GetDoctorSpecialization();
            string[] nameParts = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            string lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            var searchResult = new List<TblUser>();
            if(nameParts.Length > 1)
            {
                 searchResult = doctors
               .Where(doctor => string.IsNullOrEmpty(firstName) || doctor.FirstName.ToLower().Contains(firstName.ToLower()))
               .Where(doctor => string.IsNullOrEmpty(lastName) || doctor.LastName.ToLower().Contains(lastName.ToLower()))
               .ToList();
            }
            else
            {
                searchResult = doctors
                              .Where(doctor => doctor.FirstName.ToLower().Contains(firstName.ToLower()) ||
                                       doctor.LastName.ToLower().Contains(firstName.ToLower())).ToList();
            }

           var query = from doctor in searchResult
                        join doctor_specialization in doctor_specailizations
                        on doctor.UserId equals doctor_specialization.DoctorId
                        select new
                        {
                            FirstName = doctor.FirstName,
                            LastName = doctor.LastName,
                            Email = doctor.Email,
                            PhoneNumber =doctor.PhoneNumber,
                            Address = doctor.Address,
                            UserImage = doctor.UserImage,
                            SpecializationId = doctor_specialization.SpecializationId
                        };
            var result = (from doctor in query
                         join specialization in specializations
                         on doctor.SpecializationId equals specialization.SpecializationId
                         select new SearchModel(
                              doctor.FirstName,
                            doctor.LastName,
                            doctor.Email,
                             doctor.PhoneNumber,
                             doctor.Address,
                             doctor.UserImage,
                             specialization.SpecializationName,
                             specialization.DegreeName
                             )).ToList();
            return result;

        }

        public List<SearchModel> GetDoctorsBySpecialization(string speciality, string location)
        {
            var doctors = _userRepo.GetByRole("doctor");
            var specializations = _specializationRepo.GetSepcialiazations();
            var doctor_specailizations = _specializationRepo.GetDoctorSpecialization();


            var query = from doctor in doctors
                        join doctor_specialization in doctor_specailizations
                        on doctor.UserId equals doctor_specialization.DoctorId
                        select new
                        {
                            FirstName = doctor.FirstName,
                            LastName = doctor.LastName,
                            Email = doctor.Email,
                            PhoneNumber = doctor.PhoneNumber,
                            Address = doctor.Address,
                            UserImage = doctor.UserImage,
                            SpecializationId = doctor_specialization.SpecializationId
                        };
            var query1 = (from doctor in query
                          join specialization in specializations
                          on doctor.SpecializationId equals specialization.SpecializationId
                          select new SearchModel(
                               doctor.FirstName,
                             doctor.LastName,
                             doctor.Email,
                              doctor.PhoneNumber,
                              doctor.Address,
                              doctor.UserImage,
                              specialization.SpecializationName,
                              specialization.DegreeName
                              )).ToList();
            var searchResult = query1
                               .Where(doctor => doctor.Specialization.ToLower().Contains(speciality.ToLower()) && doctor.Address.ToLower().Contains(location.ToLower()))
                               .ToList();

            return searchResult;

        }

        public List<AvailabilityData> GetAvailability(string email)
        {
            var doctor = _userRepo.GetUser(email);
            var availability = _availabilityRepo.GetAvailability(doctor.UserId);

            var groupedByDate = availability.GroupBy(a => a.AvailabilityStartDatetime.Date);

            List<AvailabilityData> availabilityData = groupedByDate.Select(group =>
             new AvailabilityData
             {
                 Date = group.Key.ToString("yyyy-MM-dd"),
                 TimeSlots = group.Select(a => new TimeSlot
                 {
                     AvailabilityId = a.AvailabilityId,
                     StartTime = a.AvailabilityStartDatetime.ToString("HH:mm"),
                     EndTime = a.AvailabilityEndDatetime.ToString("HH:mm")
                 }).OrderBy(slot => DateTime.Parse(slot.StartTime)).ToList() 
             }).OrderByDescending(data => DateTime.Parse(data.Date)).ToList();


            return availabilityData;

        }
    }
}
