using confirmUser.HelperRepo;
using confirmUser.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using userInformation.Helper;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace confirmUser.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly manageAPI _manage;
        private readonly IHttpClientFactory _clientFactory;
        //private readonly IUserService _userService;
        private readonly IEmailHelperRepo _emailHelperRepo;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private IStringLocalizer<HomeController> _localizer;
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;


        public HomeController(IHttpClientFactory clientFactory, ILogger<HomeController> logger, manageAPI manage, IEmailHelperRepo emailHelperRepo, IConfiguration configuration, IUserService userService, IStringLocalizer<HomeController> localizer)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _manage = manage;
            _emailHelperRepo = emailHelperRepo;
            _configuration = configuration;
            _userService = userService;
            _localizer = localizer;
        }


        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> RegisterPage()
        {

            var fuelUrl = $"api/FuelType/GetAll"; // Adjust the endpoint if needed
            var fuelResponse = await _manage.GetAsync<IEnumerable<FuelModel>>(fuelUrl);

            if (fuelResponse != null)
            {
                var fuelTypes = fuelResponse.Select(ft => new SelectListItem
                {
                    Value = ft.Id.ToString(),
                    Text = ft.FuelTypeEn
                }).ToList();

                // Set the first item as selected
                if (fuelTypes.Any())
                {
                    fuelTypes[0].Selected = true;
                }

                ViewBag.FuelTypes = fuelTypes;
            }
            else
            {
                ViewBag.FuelTypes = new List<SelectListItem>(); // Handle no data
            }


            var carUrl = $"api/CarType/GetAll"; // Adjust the endpoint if needed
            var carResponse = await _manage.GetAsync<IEnumerable<CarModel>>(carUrl);

            if (carResponse != null)
            {
                var carTypes = carResponse.Select(ct => new SelectListItem
                {
                    Value = ct.Id.ToString(),
                    Text = ct.TypeNameEn
                }).ToList();

                // Set the first item as selected
                if (carTypes.Any())
                {
                    carTypes[0].Selected = true;
                }

                ViewBag.CarTypes = carTypes;
            }
            else
            {
                ViewBag.CarTypes = new List<SelectListItem>(); // Handle no data
            }
            //var url = $"api/CarType/GetAll";

            //var fuelResponse =  await _manage.GetAsync<IEnumerable<FuelModel>>(url);

            //if (fuelResponse!=null)
            //{

            //    var FuelTypes = fuelResponse.Select(ft => new SelectListItem
            //    {
            //        Value = ft.Id.ToString(),
            //        Text = ft.FuelTypeEn
            //    });
            //}


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterPage(RegisterUserFormViewModel model)
        {
            if (model != null)
            {
                var addUrl = $"api/user/Add";
                var updateResult = await _manage.PostAsync(model, addUrl);
            }
            TempData["RegisteredUser"] = JsonConvert.SerializeObject(new
            {
                UserName = model.UserName,
                UserId = model.Id // Assuming you have an Id property in your model
            });

            return RedirectToAction("Index", "Home");


        }



        [HttpGet]
        public IActionResult ResetPassword()
        {
            var userExistJson = TempData["UserExist"] as string;
            var viewModel = JsonConvert.DeserializeObject<ResetPasswordViewModel>(userExistJson);
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {

            if (ModelState.IsValid) {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "Passwords do not match.");
                    return View(model);
                }
                else
                {
                    var success = _userService.ResetPasswordAsync(model.UserName, model.Password);
                    if (success != null)
                    {
                        //TempData["UserExist"] = JsonConvert.SerializeObject(model);
                        return RedirectToAction("Index");
                    }

                }
            }
            return View();
        }




        
        //[HttpPost]
        //public IActionResult LoginHomeView()
        //{
        //    // Retrieve user data from TempData
        //    var userExistJson = TempData["UserExist"] as string;
        //    if (!string.IsNullOrEmpty(userExistJson))
        //    {
        //        var userExist = JsonConvert.DeserializeObject<LoginUserFormViewModel>(userExistJson);
        //        return View(userExist);
        //    }
        //    return View(); // Or redirect to an error page if user data is missing
        //}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { valid = false, message = string.Join("-", errors) });
            }

            try
            {
                
                var url = $"/api/user/GetUser?username={Uri.EscapeDataString(model.UserName)}";

                var userExist = await _manage.GetAsync<LoginUserFormViewModel>(url);

                if (userExist != null && userExist.PassWord == model.PassWord)
                {
                    HttpContext.Session.SetString("UserName", userExist.UserName);
                    HttpContext.Session.SetInt32("UserId", (int)userExist.Id);
                    // Check if it's the user's first login
                    if (userExist.Flag == 0)
                    {
                        TempData["UserExist"] = JsonConvert.SerializeObject(model);

                        return RedirectToAction("ResetPassword", "Home");
                    }
                    else
                    {
                        TempData["UserExist"] = JsonConvert.SerializeObject(userExist);
                        return RedirectToAction("GetTrips", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "error password");
                    return Json(new { valid = false, message = "error password" });
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                // Return unexpected error to client
                return Json(new { valid = false, message = "An unexpected error occurred." });
            }
        }



        public ActionResult ForgotPassword()
        {
            return View();
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ResetNewPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        //        return Json(new { valid = false, message = string.Join("-", errors) });
        //    }

        //    try
        //    {
        //        // Hash the new password
        //        var hashedPassword = HashPassword(model.Password);

        //        // Update the user's password directly in the database
        //        var url = $"/api/user/GetUser?username={Uri.EscapeDataString(model.UserName)}";

        //        var user = await _manage.GetAsync<LoginUserFormViewModel>(url);

        //        if (user == null)
        //        {
        //            ModelState.AddModelError(string.Empty, "User not found.");
        //            return View(model);
        //        }

        //        user.password = hashedPassword;
        //         user.Flag = 1; // Update the flag to 1

        //        // Update the user in the database
        //        var updateUrl = $"/api/user/update?username={Uri.EscapeDataString(user.UserName)}";
        //        var updateResult = await _manage.PostAsync(user, updateUrl);



        //        return RedirectToAction("LoginHomeView", "Home");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception or handle as needed
        //        // Use a logging framework like Serilog, NLog, etc., to log exceptions
        //        ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
        //        return Json(new { valid = false, message = "An unexpected error occurred." });
        //    }
        //}

        //private string HashPassword(string password)
        //{
        //    // Implement your hashing logic here, e.g., using ASP.NET Core Identity's PasswordHasher
        //    var hasher = new PasswordHasher<IdentityUser>();
        //    return hasher.HashPassword(null, password);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(LoginUserFormViewModel model)
        {


            try
            {
                var url = $"/api/user/GetUser?username={Uri.EscapeDataString(model.UserName)}";
                var userExist = await _manage.GetAsync<LoginUserFormViewModel>(url);

                if (userExist != null)
                {
                    // Generate and send OTP
                    var otp = GenerateOtp();
                    var isSent = await SendOtpToUserAsync(userExist.UserName, otp);

                    if (isSent)
                    {
                        // Store OTP in TempData for later verification
                        TempData["Otp"] = otp;
                        TempData["UserEmail"] = userExist.UserName;
                        TempData["UserExist"] = JsonConvert.SerializeObject(userExist);
                        return RedirectToAction("VerifyOtp");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error sending OTP.");
                        return Json(new { valid = false, message = "Error sending OTP." });
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                    return Json(new { valid = false, message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return Json(new { valid = false, message = "An unexpected error occurred." });
            }
        }

        private string GenerateOtp()
        {
            // Generate a 6-digit OTP
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        private async Task<bool> SendOtpToUserAsync(string email, string otp)
        {
            // Implement your email sending logic here
            // For example, use an email service to send the OTP
            try
            {
                var emailBody = $"Generate OTP: <a href=''>{otp}</a>";
                var emailSubject = "Password Reset Request";

                // Send the email using IEmailHelperRepo
                var sentTo = new List<string> { email };

                var success = _emailHelperRepo.SendMail(emailBody, emailSubject, sentTo, null, null, null, null);

                return await Task.FromResult(success);
            }
            catch (Exception)
            {
                return false;
            }
        }
        [HttpGet]
        public async Task<IActionResult> VerifyOtp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(string enteredOtp)
        {
            var userExistJson = TempData["UserExist"] as string;
            var viewModel = JsonConvert.DeserializeObject<ResetPasswordViewModel>(userExistJson);
            var storedOtp = TempData["Otp"] as string;
            var userEmail = TempData["UserEmail"] as string;

            if (string.IsNullOrEmpty(storedOtp) || string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Error", "Home");
            }

            if (enteredOtp == storedOtp)
            {
                // Redirect to password reset page
                // return RedirectToAction("ResetPassword", new { email = userEmail });
                TempData["UserExist"] = JsonConvert.SerializeObject(viewModel);
                return RedirectToAction("ResetPassword", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP.");
                return View(); // Return the OTP entry view
            }
        }
        //private async Task<bool> SendPasswordAndUserNameByEmail(string email)
        //{
        //    // Implement your email sending logic here
        //    // For example, use an email service to send the OTP
        //    try
        //    {
        //        var emailBody = $"شكرا لأنضمامك لمجموعة الخليج للتأمين <a href='شكرا لأنضمامك لمجموعة الخليج للتأمين'></a>";
        //        var emailSubject = "شكرا لأنضمامك لمجموعة الخليج للتأمين";

        //        // Send the email using IEmailHelperRepo
        //        var sentTo = new List<string> { email };

        //        var success = _emailHelperRepo.SendMail(emailBody, emailSubject, sentTo, null, null, null, null);

        //        return await Task.FromResult(success);
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        [HttpGet]
        public async Task<IActionResult> TripDetails()
        {
            // Retrieve user data from TempData
            var userName = HttpContext.Session.GetString("UserName");
            var userId = HttpContext.Session.GetInt32("UserId");

            // Check if user data exists in session
            if (userName == null || userId == null)
            {
                return RedirectToAction("Login", "Home"); // Redirect to login if session data is missing
            }

            // Create the ViewModel with user information
            var model = new tripDetails
            {
                Name = userName,
                User = (int)userId
                // Initialize other properties if needed
            };

            // Retrieve application data
            var appUrl = $"api/App/GetAll"; // Adjust the endpoint if needed
            var appResponse = await _manage.GetAsync<IEnumerable<Applications>>(appUrl);

            if (appResponse != null)
            {
                var apps = appResponse.Select(ft => new SelectListItem
                {
                    Value = ft.Id.ToString(),
                    Text = ft.AppNameEn
                }).ToList();

                // Set the first item as selected
                if (apps.Any())
                {
                    apps[0].Selected = true;
                }

                ViewBag.Applications = apps;
            }
            else
            {
                ViewBag.Applications = new List<SelectListItem>(); // Handle no data
            }

            return View(model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TripDetails(tripDetails model)
        {
            //if (!ModelState.IsValid)
            //{
            //    // Collect all validation errors and return them as a JSON response
            //    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            //    return Json(new { valid = false, message = string.Join("-", errors) });
            //}

            try
            {
                var Url = "api/Trip/Add";
                var response = await _manage.PostAsync(model, Url); // Assuming _manage is your service for API calls

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                // Return unexpected error to client
                return Json(new { valid = false, message = "An unexpected error occurred." });
            }
        }
        //[HttpGet]
        //public async Task<IActionResult> GetTrips()
        //{
        //    try
        //    {
        //        var userId = HttpContext.Session.GetInt32("UserId");


        //        var trips = await _manage.GetAsync<List<tripDetails>>($"api/Trip/GetById?userId={userId}");

        //        if (trips == null || !trips.Any())
        //        {

        //            ViewBag.Message = "No trips available for this user.";
        //            return View("GetTrips", new List<tripDetails>());
        //        }


        //        var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");

        //        var result = trips.Select(trip => new
        //        {
        //            id = trip.Id,
        //            tripsNum = trip.TripsNum,
        //            hoursNum = trip.HoursNum,
        //            tripDistanceKM = trip.TripDistanceKM,
        //            tripDate = trip.TripDate.ToString("yyyy-MM-dd"),
        //            applicationName = applications.FirstOrDefault(a => a.Id == trip.ApplicationID)?.AppNameEn ?? "Unknown",
        //            taxFis = applications.FirstOrDefault(a => a.Id == trip.ApplicationID)?.TaxFis.ToString() ?? "Unknown"
        //        }).ToList();

        //        ViewBag.Message = result.Any() ? null : "No trips available for this user.";
        //        return View(result);
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        return StatusCode(500, "Error while communicating with the API.");
        //    }
        //    catch (JsonException ex)
        //    {
        //        return StatusCode(500, "Error while processing API response.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "An unexpected error occurred.");
        //    }
        //    // Retrieve user data from TempData
        //    //var userExistJson = TempData["UserExist"] as string;
        //    //if (!string.IsNullOrEmpty(userExistJson))
        //    //{
        //    //    var userExist = JsonConvert.DeserializeObject<LoginUserFormViewModel>(userExistJson);
        //    //    return View(userExist);
        //    //}
        //    return View(); // Or redirect to an error page if user data is missing
        //}
        //[HttpPost]
        //public async Task<IActionResult> GetTrips(tripDetails model)
        //{
        //    try
        //    {
        //        var userId = HttpContext.Session.GetInt32("UserId");


        //        var trips = await _manage.GetAsync<List<tripDetails>>($"api/Trip/GetById?userId={userId}");

        //        if (trips == null || !trips.Any())
        //        {

        //            ViewBag.Message = "No trips available for this user.";
        //            return View("GetTrips", new List<tripDetails>());
        //        }


        //        var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");

        //        var result = trips.Select(trip => new
        //        {
        //            id = trip.Id,
        //            tripsNum = trip.TripsNum,
        //            hoursNum = trip.HoursNum,
        //            tripDistanceKM = trip.TripDistanceKM,
        //            tripDate = trip.TripDate.ToString("yyyy-MM-dd"),
        //            applicationName = applications.FirstOrDefault(a => a.Id == trip.ApplicationID)?.AppNameEn ?? "Unknown",
        //            taxFis = applications.FirstOrDefault(a => a.Id == trip.ApplicationID)?.TaxFis.ToString() ?? "Unknown"
        //        }).ToList();

        //        ViewBag.Message = result.Any() ? null : "No trips available for this user.";
        //        return View(result);
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        return StatusCode(500, "Error while communicating with the API.");
        //    }
        //    catch (JsonException ex)
        //    {
        //        return StatusCode(500, "Error while processing API response.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "An unexpected error occurred.");
        //    }
        //}
        //[HttpGet]
        //public async Task<IActionResult> GetTrips(tripDetails model)
        //{
        //    try
        //    {
        //        var userId = HttpContext.Session.GetInt32("UserId");
        //        var trips = await _manage.GetAsync<List<tripDetails>>($"api/Trip/GetById?userId={userId}");
        //        if (trips == null)
        //        {
        //            ViewBag.Message = "No trips available for this user.";
        //            return View("GetTrips", new List<tripDetails>());
        //        }
        //        var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");

        //        var result = trips.Select(trip => new TripViewModel
        //        {
        //            Id = trip.Id,
        //            TripsNum = trip.TripsNum,
        //            HoursNum = trip.HoursNum,
        //            TripDistanceKM = trip.TripDistanceKM,
        //            TripDate = trip.TripDate.ToString("yyyy-MM-dd"),
        //            ApplicationName = applications.FirstOrDefault(a => a.Id == trip.ApplicationID)?.AppNameEn ?? "Unknown",
        //            TaxFis = applications.FirstOrDefault(a => a.Id == trip.ApplicationID)?.TaxFis.ToString() ?? "Unknown"
        //        }).ToList();

        //        return View(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions as needed
        //        return StatusCode(500, "An unexpected error occurred.");
        //    }
        //}
        public async Task<IActionResult> GetTrips(tripDetails model)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var response = await _manage.GetAsync<dynamic>($"api/Trip/GetById?userId={userId}");

                var data = response.data as JArray;
                var message = (string)response.message;

                // Deserialize data into a list of tripDetails
                var trips = data != null ? data.ToObject<List<tripDetails>>() : new List<tripDetails>();
                if (trips == null || !trips.Any())
                {
                    ViewBag.Message = message;
                    return View("GetTrips", new List<TripViewModel>());
                }

                var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");

                var result = trips.Select(trip => new TripViewModel
                {
                    Id = trip.Id,
                    TripsNum = trip.TripsNum,
                    HoursNum = trip.HoursNum,
                    TripDistanceKM = trip.TripDistanceKM,
                    TripDate = trip.TripDate.ToString("yyyy-MM-dd"),
                    ApplicationName = applications.FirstOrDefault(a => a.Id == trip.ApplicationID)?.AppNameEn ?? "Unknown",
                    TaxFis = applications.FirstOrDefault(a => a.Id == trip.ApplicationID)?.TaxFis.ToString() ?? "Unknown"
                }).ToList();

                return View(result);
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                return StatusCode(500, "An unexpected error occurred.");
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> GetTrips(tripDetails model)
        //{
        //    try
        //    {
        //        var userId = HttpContext.Session.GetInt32("UserId");
        //        //var pageSize = int.Parse(Request.Form["length"]);
        //        //var skip = int.Parse(Request.Form["start"]);
        //        var trips = await _manage.GetAsync<List<tripDetails>>($"api/Trip/GetById?userId={userId}");
        //        // var trips = await _manage.GetAsync<List<TripDetails>>("api/Trip/GetById");
        //        if (trips == null || !trips.Any())
        //        {
        //            // Return a custom JSON response indicating no data
        //            return Ok(new { recordsFiltered = 0, recordsTotal = 0, data = new List<object>(), message = "No trips available for this user." });
        //        }

        //        // Assuming _applications is a list or queryable collection of applications
        //        var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");

        //        var result = trips.Select(trip => new
        //        {
        //            id = trip.Id,
        //            tripsNum = trip.TripsNum,
        //            hoursNum = trip.HoursNum,
        //            tripDistanceKM = trip.TripDistanceKM,
        //            tripDate = trip.TripDate.ToString("yyyy-MM-dd"),
        //            applicationName = applications.FirstOrDefault(a => a.Id == trip.ApplicationID)?.AppNameEn ?? "Unknown",
        //            taxFis = applications.FirstOrDefault(a => a.Id == trip.ApplicationID)?.TaxFis.ToString() ?? "Unknown"
        //        }).ToList();

        //        var recordsTotal = result.Count;

        //        var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = result };

        //        return Ok(jsonData);
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        return StatusCode(500, "Error while communicating with the API.");
        //    }
        //    catch (JsonException ex)
        //    {
        //        return StatusCode(500, "Error while processing API response.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "An unexpected error occurred.");
        //    }
        //}

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            return LocalRedirect(returnUrl);
        }
        [HttpGet]
        public async Task<IActionResult> EditTrip(int id)
        {
            try
            {
                // Fetch trip details by ID
                var trip = await _manage.GetAsync<EditViewModel>($"/api/Trip/GetTripById?id={id}");

                if (trip == null)
                {
                    return NotFound(); // Handle case where trip is not found
                }

                // Fetch all available applications
                var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");

                // Ensure applications list is not null
                var applicationSelectList = applications?
                    .Select(app => new SelectListItem
                    {
                        Value = app.Id.ToString(),
                        Text = app.AppNameEn,
                        Selected = app.Id == trip.ApplicationID // Mark selected application
                    }).ToList() ?? new List<SelectListItem>();

                // Prepare the ViewModel
                var viewModel = new EditViewModel
                {
                    Id = trip.Id,
                    Name = trip.Name,
                    TripsNum = trip.TripsNum,
                    HoursNum = trip.HoursNum,
                    TripDistanceKM = trip.TripDistanceKM,
                    ApplicationID = trip.ApplicationID,
                    Applications = applicationSelectList, // Pass the SelectList here
                    User = trip.User,
                    TripDate = trip.TripDate
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return View(new EditViewModel()); // Pass an empty ViewModel in case of an error
            }
        }


        [HttpPost]
        public async Task<IActionResult> EditTrip(EditViewModel model)
        {
            try
            {
                
                // Await the asynchronous operation
                var isSuccess = await _manage.PostAsync(model, "api/Trip/update");

                if (isSuccess)
                {
                    // Optionally log success or handle further actions
                    return Json(new { success = true }); // Return success to client
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update user via API.");
                    return Json(new { success = false, message = "Failed to update user via API." });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return View(model); // Return the view with error message
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
        /// <summary>
        /// ////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //public async Task<IActionResult> GenerateReport()
        //{
        //    var appUrl = "api/App/GetAll";
        //    var appResponse = await _manage.GetAsync<IEnumerable<Applications>>(appUrl);
        //    var apps = appResponse?.Select(app => new SelectListItem
        //    {
        //        Value = app.Id.ToString(),
        //        Text = app.AppNameEn
        //    }).ToList() ?? new List<SelectListItem>();

        //    // Set the first item as selected if any applications are available
        //    if (apps.Any())
        //    {
        //        apps[0].Selected = true;
        //    }

        //    ViewBag.Applications = apps;
        //    return View();

        //}
        //**************************************************
        [HttpGet]
        public async Task<IActionResult> GenerateReport()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
           // var userId = HttpContext.Session.GetInt32("UserId");
            //var response = await _manage.GetAsync<dynamic>($"api/Trip/GetById?userId={userId}");

            //var data = response.data as JArray;
            //var message = (string)response.message;

            //// Deserialize data into a list of tripDetails
            //var trips = data != null ? data.ToObject<List<tripDetails>>() : new List<tripDetails>();
            var appUrl = "api/App/GetAll";
            var appResponse = await _manage.GetAsync<IEnumerable<Applications>>(appUrl);

            var apps = appResponse?.Select(app => new SelectListItem
            {
                Value = app.Id.ToString(),
                Text = app.AppNameEn
            }).ToList() ?? new List<SelectListItem>();

            // Set the first item as selected if any applications are available
            if (apps.Any())
            {
                apps[0].Selected = true;
            }
            ViewBag.Applications = apps;

            var report = new ReportModel
            {

            };
            return View(report);
        }
        [HttpPost]
        public async Task<IActionResult> GenerateReport(ReportFilterModel model)
        {
            
            var userId = HttpContext.Session.GetInt32("UserId");

            var tripUrl = $"api/user?UserId={userId}&ApplicationID={model.ApplicationID}&StartDate={model.StartDate:yyyy-MM-dd}&EndDate={model.EndDate:yyyy-MM-dd}";

            var tripResponse = await _manage.GetAsync<IEnumerable<tripDetails>>(tripUrl);

            var userInfo = await _manage.GetAsync<userInfo>($"api/user/GetPriceAndKmPerL?userId={userId}");

            var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");
            var taxFis = applications.FirstOrDefault(a => a.Id == tripResponse.FirstOrDefault()?.ApplicationID)?.TaxFis ?? 0;

            var report = new ReportModel
            {
                ApplicationID = model.ApplicationID,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                
                SumTripsNum = tripResponse.Sum(t => t.TripsNum),
                SumHoursNum = tripResponse.Sum(t => t.HoursNum),
                SumTripDistanceKM = tripResponse.Sum(t => t.TripDistanceKM),
                FeesPaidToTheCompany = (long)tripResponse.Sum(t => t.TripDistanceKM * taxFis),
                FuelCost = userInfo.KmPerL > 0 && tripResponse.Sum(t => t.TripDistanceKM) > 0
                    ? (long)((userInfo.Price / userInfo.KmPerL) * tripResponse.Sum(t => t.TripDistanceKM))
                    : 0
            };
            ViewBag.Applications = await GetApplications();
            //var reportModel = new ReportFilterModel
            //{
            //    ApplicationID = model.ApplicationID,
            //    StartDate = model.StartDate,
            //    EndDate = model.EndDate,
            //    ReportData = report
            //};

            return View(report);
        }
        private async Task<List<SelectListItem>> GetApplications()
        {
            var appUrl = "api/App/GetAll";
            var appResponse = await _manage.GetAsync<IEnumerable<Applications>>(appUrl);

            return appResponse?.Select(app => new SelectListItem
            {
                Value = app.Id.ToString(),
                Text = app.AppNameEn
            }).ToList() ?? new List<SelectListItem>();
        }
        /////////////////////////////////////////////////
        //[HttpGet]
        //public async Task<IActionResult> GenerateReport(ReportFilterModel model)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    var appUrl = $"api/user={userId}&ApplicationID={model.ApplicationID}&StartDate={model.StartDate}&EndDate={model.EndDate}";
        //    var tripResponse = await _manage.GetAsync<IEnumerable<tripDetails>>(appUrl);
        //    var userInfo = await _manage.GetAsync<userInfo>($"api/user/GetPriceAndKmPerL?userId={userId}");
        //    var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");
        //    var taxFis = applications.FirstOrDefault(a => a.Id == tripResponse.FirstOrDefault()?.ApplicationID)?.TaxFis ?? 0;
        //    //var taxFis = model.ApplicationID == tripResponse.FirstOrDefault()?.ApplicationID)?.TaxFis ?? 0;

        //    var report = new ReportModel
        //    {
        //        SumTripsNum = tripResponse.Sum(t => t.TripsNum),
        //        SumHoursNum = tripResponse.Sum(t => t.HoursNum),
        //        SumTripDistanceKM = tripResponse.Sum(t => t.TripDistanceKM),
        //        FeesPaidToTheCompany = (long)tripResponse.Sum(t => t.TripDistanceKM * taxFis),
        //        FuelCost = userInfo.KmPerL > 0 && tripResponse.Sum(t => t.TripDistanceKM) > 0
        //            ? (long)((userInfo.KmPerL / tripResponse.Sum(t => t.TripDistanceKM)) * userInfo.Price)
        //            : 0
        //    };

        //    return View(report);

        //}
        ////////////////////////////////////////////////////////
        ///

        //public async Task<IActionResult> GenerateReport(long? companyId, DateTime? startDate, DateTime? endDate)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        return RedirectToAction("Error", "Home");
        //    }

        //    // Fetch the list of applications
        //    var appUrl = "api/App/GetAll";
        //    var appResponse = await _manage.GetAsync<IEnumerable<Applications>>(appUrl);

        //    var apps = appResponse?.Select(app => new SelectListItem
        //    {
        //        Value = app.Id.ToString(),
        //        Text = app.AppNameEn
        //    }).ToList() ?? new List<SelectListItem>();

        //    // Set the first item as selected if any applications are available
        //    if (apps.Any())
        //    {
        //        apps[0].Selected = true;
        //    }

        //    ViewBag.Applications = apps;

        //    // Fetch trips and filter based on company and date range
        //    var response = await _manage.GetAsync<dynamic>($"api/Trip/GetById?userId={userId}");
        //    var data = response.data as JArray;
        //    var trips = data != null ? data.ToObject<List<tripDetails>>() : new List<tripDetails>();

        //    if (companyId.HasValue)
        //    {
        //        trips = trips.Where(t => t.ApplicationID == companyId.Value).ToList();
        //    }

        //    if (startDate.HasValue && endDate.HasValue)
        //    {
        //        trips = trips.Where(t => t.TripDate >= startDate.Value && t.TripDate <= endDate.Value).ToList();
        //    }
        //    please in this line make the user to add the data to show in view but in initialy the feild must empte
        //    var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");
        //    var userInfo = await _manage.GetAsync<userInfo>($"api/user/GetPriceAndKmPerL?userId={userId}");

        //    var taxFis = applications.FirstOrDefault(a => a.Id == trips.FirstOrDefault()?.ApplicationID)?.TaxFis ?? 0;

        //    var report = new ReportModel
        //    {
        //        SumTripsNum = trips.Sum(t => t.TripsNum),
        //        SumHoursNum = trips.Sum(t => t.HoursNum),
        //        SumTripDistanceKM = trips.Sum(t => t.TripDistanceKM),
        //        FeesPaidToTheCompany = (long)trips.Sum(t => t.TripDistanceKM * taxFis),
        //        FuelCost = userInfo.KmPerL > 0 && trips.Sum(t => t.TripDistanceKM) > 0
        //            ? (long)((userInfo.KmPerL / trips.Sum(t => t.TripDistanceKM)) * userInfo.Price)
        //            : 0
        //    };

        //    return View(report);
        //}
        /// <summary>
        /// //////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        //public async Task<IActionResult> GenerateReport(long? applicationId, DateTime? startDate, DateTime? endDate)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        return RedirectToAction("Error", "Home");
        //    }

        //    // Fetch the list of applications
        //    var appUrl = "api/App/GetAll";
        //    var appResponse = await _manage.GetAsync<IEnumerable<Applications>>(appUrl);

        //    var apps = appResponse?.Select(app => new SelectListItem
        //    {
        //        Value = app.Id.ToString(),
        //        Text = app.AppNameEn
        //    }).ToList() ?? new List<SelectListItem>();

        //    ViewBag.Applications = apps;

        //    if (!applicationId.HasValue || !startDate.HasValue || !endDate.HasValue)
        //    {
        //        // Return the view with empty fields if no application or date range is selected
        //        return View(new ReportFilterModel
        //        {
        //            ApplicationID = applicationId ?? 0,
        //            StartDate = startDate ?? DateTime.MinValue,
        //            EndDate = endDate ?? DateTime.MinValue
        //        });
        //    }

        //    // Fetch trips and filter based on company and date range
        //    var response = await _manage.GetAsync<dynamic>($"api/Trip/GetById?userId={userId}");
        //    var data = response.data as JArray;
        //    var trips = data != null ? data.ToObject<List<tripDetails>>() : new List<tripDetails>();

        //    if (applicationId.HasValue)
        //    {
        //        trips = trips.Where(t => t.ApplicationID == applicationId.Value).ToList();
        //    }

        //    if (startDate.HasValue && endDate.HasValue)
        //    {
        //        trips = trips.Where(t => t.TripDate >= startDate.Value && t.TripDate <= endDate.Value).ToList();
        //    }

        //    var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");
        //    var userInfo = await _manage.GetAsync<userInfo>($"api/user/GetPriceAndKmPerL?userId={userId}");

        //    var taxFis = applications.FirstOrDefault(a => a.Id == trips.FirstOrDefault()?.ApplicationID)?.TaxFis ?? 0;

        //    var report = new ReportModel
        //    {
        //        SumTripsNum = trips.Sum(t => t.TripsNum),
        //        SumHoursNum = trips.Sum(t => t.HoursNum),
        //        SumTripDistanceKM = trips.Sum(t => t.TripDistanceKM),
        //        FeesPaidToTheCompany = (long)trips.Sum(t => t.TripDistanceKM * taxFis),
        //        FuelCost = userInfo.KmPerL > 0 && trips.Sum(t => t.TripDistanceKM) > 0
        //            ? (long)((userInfo.KmPerL / trips.Sum(t => t.TripDistanceKM)) * userInfo.Price)
        //            : 0
        //    };

        //    var model = new ReportFilterModel
        //    {
        //        ApplicationID = applicationId.Value,
        //        StartDate = startDate.Value,
        //        EndDate = endDate.Value,
        //        ReportData = report
        //    };

        //    return View("GenerateReport", model);
        //}


        //[HttpGet]
        //public async Task<IActionResult> GenerateReport(ReportFilterModel filter)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (filter.ApplicationID == 0 || filter.StartDate == DateTime.MinValue || filter.EndDate == DateTime.MinValue)
        //    {
        //        // Redirect or show an error if no valid input is provided
        //        return View("ReportFilter", new ReportFilterModel());
        //    }

        //    var trips = await _manage.GetAsync<List<tripDetails>>(
        //        $"api/Trip/GetByFilters?applicationId={filter.ApplicationID}&startDate={filter.StartDate:yyyy-MM-dd}&endDate={filter.EndDate:yyyy-MM-dd}");
        //    var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");
        //    var userInfo = await _manage.GetAsync<userInfo>($"api/user/GetPriceAndKmPerL?userId={userId}");

        //    var taxFis = applications.FirstOrDefault(a => a.Id == trips.FirstOrDefault()?.ApplicationID)?.TaxFis ?? 0;

        //    var report = new ReportModel
        //    {
        //        SumTripsNum = trips.Sum(t => t.TripsNum),
        //        SumHoursNum = trips.Sum(t => t.HoursNum),
        //        SumTripDistanceKM = trips.Sum(t => t.TripDistanceKM),
        //        FeesPaidToTheCompany = (long)trips.Sum(t => t.TripDistanceKM * taxFis),
        //        FuelCost = userInfo.KmPerL > 0 && trips.Sum(t => t.TripDistanceKM) > 0
        //            ? (long)((userInfo.KmPerL / trips.Sum(t => t.TripDistanceKM)) * userInfo.Price)
        //            : 0
        //    };

        //    var model = new ReportFilterModel
        //    {
        //        ApplicationID = filter.ApplicationID,
        //        StartDate = filter.StartDate,
        //        EndDate = filter.EndDate,
        //        ReportData = report
        //    };

        //    return View("ReportFilter", model);
        //}


        //private long CalculateFuelCost(IEnumerable<tripDetails> trips)
        //{
        //    // Implement your logic to calculate fuel cost based on trip details
        //    // For example, you might calculate it based on trip distance
        //    return (long)trips.Sum(t => t.TripDistanceKM * 0.5); // Example calculation
        //}
        [HttpGet]
        public async Task<IActionResult> view(int id)
        {
            try
            {
                    // Fetch trip details by ID
                    var trip = await _manage.GetAsync<tripDetails>($"/api/Trip/GetTripById?id={id}");

                    if (trip == null)
                    {
                        return NotFound(); // Handle case where trip is not found
                    }
                var applications = await _manage.GetAsync<List<Applications>>("api/App/GetAll");

                // Ensure applications list is not null
                var applicationSelectList = applications?
                    .Select(app => new SelectListItem
                    {
                        Value = app.Id.ToString(),
                        Text = app.AppNameEn,
                        Selected = app.Id == trip.ApplicationID // Mark selected application
                    }).ToList() ?? new List<SelectListItem>();

                var ViewModel = new TripViewModel
                    {
                    Id = id,
                        
                    Name=trip.Name,
                    TripDistanceKM = trip.TripDistanceKM,
                    HoursNum = trip.HoursNum,
                    TripsNum = trip.TripsNum,
                    TripDate = trip.TripDate.ToString("yyyy-MM-dd"),
                    ApplicationName = applications.FirstOrDefault(a => a.Id == trip.ApplicationID)?.AppNameEn ?? "Unknown",

                    };

                

                return View(ViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return View(); // Return to view with error message
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
