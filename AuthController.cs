using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using Signup.Models;
using Signup.Services;
using Microsoft.AspNetCore.Http;
using Signup.Presentation;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Signup.AppData;
using System.Text;
using System.Net.Mail;

namespace Signup.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly StudentContext _context;

        public AuthController(AuthService authService ,IHostingEnvironment hosting ,StudentContext context)
        {
            _authService = authService;
            _hostingEnvironment = hosting;
            _context = context;
        }

        public static Regex emailRegex = new Regex(
    @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
    RegexOptions.CultureInvariant | RegexOptions.Singleline);



        public static bool IsValidEmailFormat(string emailInput)
        {
            return emailRegex.IsMatch(emailInput);
        }
        
        public IActionResult Secure()
        {
            return View();
        }

        public IActionResult Register()
        {
            ViewData["Title"] = "Register";

            return View();
        }
        [HttpGet("Login")]
        public IActionResult Login(string returnUrl)
            {

                ViewData["ReturnUrl"] = returnUrl;

            return View();
        }




        //[Route("login")]
        //[HttpPost]
        //public async Task<IActionResult> Login(StudentModel model, string returnUrl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _accountRepository.PasswordSignInAsync(model);
        //        if (result.Succeeded)
        //        {
        //            if (!string.IsNullOrEmpty(returnUrl))
        //            {
        //                return LocalRedirect(returnUrl);
        //            }
        //            return RedirectToAction("Login", "Auth");
        //        }
        //        if (result.IsNotAllowed)
        //        {
        //            ModelState.AddModelError("", "Not allowed to login");
        //        }
        //        else if (result.IsLockedOut)
        //        {
        //            ModelState.AddModelError("", "Account blocked. Try after some time.");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Invalid credentials");
        //        }

        //    }

        //    return View();
        //}








        public IActionResult Dashboard(StudentModel model)
        {
            try { 

        ResultModel resultModel = new ResultModel();

            if (model.Email is null || model.Password is null)
            {
                resultModel.Status = false;
                resultModel.Message = "Email and Password cannot be null or empty";



                return View("Login", resultModel);
            }

          
                var entity = _authService.CheckStudent(model);
                if (entity.isValid is true)
                {
                    }
                if (entity is null)
                {
                    resultModel.Status = false;
                    resultModel.Id = entity?.Id ?? 0;
                    resultModel.Message = "Access Denied";
                    TempData["Error"] = "Error invalid email and password";
                    return View("Login");
                }
                else
                {
                      resultModel.Status = true;
                    resultModel.Id = entity.Id;
                    resultModel.Name = entity.Name;
                    resultModel.Email = entity.Email;
                    resultModel.Message = "Successfully Login";

                   
                    return View(resultModel);
                    }

            } 



            catch (Exception e)
            {

              ResultModel resultModel = new ResultModel();
                resultModel.Status = false;
                resultModel.Message = e.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resultModel);

            }


        }  

        public IActionResult UpdateUser(StudentModel model)
        {
            UpdateModel st = new UpdateModel();

            var entity = _authService.UpdateStudent(model);
            st.Id = entity.Id;
            st.Name = entity.Name;

            return Ok(st);
           
        }



        
       
        public IActionResult RegisterUserAjax(StudentModel model)
        {
            ResultModel resultModel = new ResultModel();

            if (model is null)
            {
                resultModel.Status = false;
                resultModel.Message = "Form data cannot be null";

                return StatusCode(StatusCodes.Status400BadRequest, resultModel);
            }

            if (
                string.IsNullOrWhiteSpace(model.Name)
                || model.Name is null
                || string.IsNullOrWhiteSpace(model.Email)
                || model.Email is null
                || string.IsNullOrWhiteSpace(model.Password)
                || model.Password is null
                )
            {
                resultModel.Status = false;
                resultModel.Message = "Name, Email and Password cannot be null or empty";

                return StatusCode(StatusCodes.Status400BadRequest, resultModel);
            }

            if (!IsValidEmailFormat(model.Email))
            {
                resultModel.Status = false;
                resultModel.Message = "Invalid Email format!";

                return StatusCode(StatusCodes.Status400BadRequest, resultModel);
            }

            if (model.Password.Length < 8)
            {
                resultModel.Status = false;
                resultModel.Message = "Minimum no. of characters in password have to be 8";

                return StatusCode(StatusCodes.Status400BadRequest, resultModel);
            }
           
            try
            {
                var entity = _authService.InsertStudent(model);
                model.isValid = false;

                resultModel.Status = true;
                resultModel.Id = entity.Id;
                resultModel.Message = "Successfully Registered";
                BuildEmailTemplate(model.Id);
               
                return Ok(resultModel);
            }
            catch (Exception e)
            {
                resultModel.Status = false;
                resultModel.Message = e.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resultModel);

            }
           

        }

        public void BuildEmailTemplate(int regiD)
        {
            //string body = System.IO.File.ReadAllText(_hostingEnvironment.MapPath("~/template/") + "Confirm" + "html");
            var web_root = _hostingEnvironment.WebRootPath;
            string filename = "Confirm";
            string path = Path.Combine(_hostingEnvironment.WebRootPath, "template");
            path = Path.Combine(path, filename + ".html");
            string body = System.IO.File.ReadAllText(path);
            var info = _context.Students.Where(x=>x.Id==regiD).FirstOrDefault();
            var url = "https://localhost:44352/"+"Auth/Confirm?regId="+regiD;
            body = body.Replace("abc.com", url);
            body = body.ToString();
            BuildEmailTemplate("Your Account is Created Suucessfully",body,info.Email);
        }

        public static void BuildEmailTemplate(string subjecttext, string bodytext, string sendio)
        {
            string from, to, bcc, cc, subject, body;
            from = "muhammadusama.aimsol@gmail.com";
            to = sendio.Trim();
            bcc = "";
            cc = "";
            subject = subjecttext;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodytext);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add( new MailAddress(to));
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(cc));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);


        }

        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("muhammadusama.aimsol@gmail.com", "Usama@268");
            try
            {
                client.Send(mail);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult Confirm(int regId)
        {
            var entity = _authService.IsValid(regId);
            return View("Login");
        }

       
      

        //public IActionResult Dashboard(ResultModel model)
        //
        //   ////FetchModel st = new FetchModel();
           
        //   ////     var entity = _authService.GetStudent(model);
        //   ////     st.Id = entity.Id;
        //   ////     st.Name = entity.Name;
        //   ////     st.Email = entity.Email;
        //    return View(model);


        //}
    }
}
