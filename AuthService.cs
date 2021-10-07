using Microsoft.EntityFrameworkCore;
using Signup.AppData;
using Signup.Models;
using Signup.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Signup.Services
{
    public class AuthService
    {
        private readonly StudentContext _context;
        public AuthService(StudentContext context)
        {
            _context = context;
        }



        public StudentModel InsertStudent(StudentModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var entity = _context.Students.Add(model).Entity;
            _context.SaveChanges();


            return entity;
        }
        public StudentModel IsValid(int id)
        {

            var StudentData = _context.Students.Where(x => x.Id == id).FirstOrDefault();
            if (StudentData != null)
            {
                
                StudentData.isValid = true;

                _context.Entry(StudentData).State = EntityState.Modified;
                _context.SaveChanges();
            }

            return StudentData;

             
        }

        //public async Task<StudentModel> CreateUserAsync(StudentModel model)
        //{
        //    var user = new StudentModel()
        //    {
        //        Name = model.Name,
        //        Email = model.Email,
        //    Password = model.Password,


        //    };
        //    var result = await _userManager.CreateAsync(user, userModel.Password);
        //    if (result.Succeeded)
        //    {
        //        await GenerateEmailConfirmationTokenAsync(user);
        //    }
        //    return result;
        //}

        //   }

        public StudentModel CheckStudent(StudentModel model)
        {
            StudentModel db = new StudentModel();

            if (model.Email is null || model.Password is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            else
            {

                var obj = _context.Students.Where(a => a.Email.ToLower().Equals(model.Email.ToLower()) && a.Password.Equals(model.Password)).FirstOrDefault();


                return obj;
            }

        }

        public StudentModel GetStudent(StudentModel model)
        {
            StudentModel db = new StudentModel();



            var obj = _context.Students.Where(a => a.Id == model.Id).FirstOrDefault();
            return obj;


        }


        public StudentModel UpdateStudent(StudentModel model)
        {
            StudentModel db = new StudentModel();



            var StudentData = _context.Students.Where(x => x.Id == model.Id).FirstOrDefault();
            if (StudentData != null)
            {
                StudentData.Name = model.Name;

                _context.Entry(StudentData).State = EntityState.Modified;
                _context.SaveChanges();
            }

            return StudentData;


        }


    }
}


