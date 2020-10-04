using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiStudent.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Student> Get()
        {
            return StudentRepo.Student;
        }

        [HttpGet("{id}")]
        public Student Get(int id)
        {
            foreach (Student st in StudentRepo.Student)
            {
                if (st.id == id)
                {
                    return st;
                }
            }
            return null;
        }


        [HttpDelete("{id}")]
        public List<Student> Delete(int id)
        {
            Student student = StudentRepo.Student.Find(x => x.id == id);
            if (student == null)
            {

            }
            else
            {
                StudentRepo.Student.Remove(student);
            }
            return StudentRepo.Student;
        }


    }
}