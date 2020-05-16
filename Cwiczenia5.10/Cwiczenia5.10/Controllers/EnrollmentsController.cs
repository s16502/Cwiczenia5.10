using System;
using System.Linq;
using Cwiczenia5._10.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenia5._10.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly s16502Context _dbContext;

        public EnrollmentsController(s16502Context context)
        {
            _dbContext = context;
        }

        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            try
            {
                var studiesExists = _dbContext.Studies.Any(s => s.Name.Equals(request.Studies));
                if (!studiesExists)
                {
                    return null;
                }

                var studentAlreadyExists =
                    _dbContext.Student.Any(s => s.IndexNumber.Equals(request.IndexNumber));

                if (studentAlreadyExists)
                {
                    return null;
                }

                Student toAddStudent = new Student
                {
                    IndexNumber = request.IndexNumber,
                    LastName = request.LastName,
                    BirthDate = request.BirthDate,
                    FirstName = request.FirstName,
                    IdEnrollment = 1
                };

                _dbContext.Student.Add(toAddStudent);
                _dbContext.SaveChanges();

                
                int idStudy = _dbContext.Studies.Single(s => s.Name.Equals(request.Studies)).IdStudy;
                int idEnrollment = _dbContext.Enrollment.Where(e => e.Semester == 1 && e.IdStudy == idStudy)
                    .OrderByDescending(e => e.StartDate).First().IdEnrollment;

                if (idEnrollment == 0)
                {
                    idEnrollment = _dbContext.Enrollment.Max(e => e.IdEnrollment) + 1;
                    Enrollment newEnrollment = new Enrollment
                    {
                        IdEnrollment = idEnrollment,
                        Semester = 1,
                        IdStudy = idStudy,
                        StartDate = DateTime.Now
                    };
                    _dbContext.Enrollment.Add(newEnrollment);
                    _dbContext.SaveChanges();
                }

                toAddStudent.IdEnrollment = idEnrollment;
                _dbContext.SaveChanges();
                
                EnrollStudentResponse response = new EnrollStudentResponse
                    {
                        Semester = 1,
                        IdStudy = idStudy,
                        StartDate = DateTime.Now,
                        IdEnrollment = idEnrollment
                    };
                return Ok(response);               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }       
        }
    }
}