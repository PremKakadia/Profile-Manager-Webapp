using Consult.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Firebase.Auth;
using FireSharp.Exceptions;
using Newtonsoft.Json;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNet.Identity;
using Google.Type;
using Google.Cloud.Storage.V1;
using FireSharp.Config;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Hosting;
using Google.Cloud.Firestore;
using FireSharp.Interfaces;
using FireSharp.Response;
using Google.Apis.Storage.v1.Data;
using Firebase.Storage;
using System.IO;

namespace Consult.Controllers
{
    public class EditController : Controller
    {
        FirebaseAuthProvider auth;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public EditController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig("AIzaSyCpWgF--9l_rSkeSCboSy3NuQjNgpROX_o"));
        }

        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            //AuthSecret = "nj4aEjsxCd7BtHO2uij3o9nrLM4LznPEmx8Wr0vj",
            BasePath = "https://consult673-default-rtdb.asia-southeast1.firebasedatabase.app"
        };
        IFirebaseClient client;
        StorageClient storageClient;

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(TeamLogIn log)
        {
            try
            {
                //create user
                await auth.CreateUserWithEmailAndPasswordAsync(log.Email.Trim(), log.Password.Trim());

                return RedirectToAction("LogIn");

            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);

                return View(log);

            }
        }


        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(Team model, TeamLogIn log)
        {
            try
            {
                var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(log.Email, log.Password);
                string token = fbAuthLink.FirebaseToken;

                if (token != null)
                {
                    //getting id from session
                    HttpContext.Session.SetString("_UserToken", log.Email.Trim());
                    TempData["userEmail"] = log.Email.Trim();

                    client = new FireSharp.FirebaseClient(config);
                    string userEmail = log.Email.Trim();
                    model.Email = userEmail;
                    log.Id = userEmail.Replace('.', ',');

                    //checking for user data
                    FirebaseResponse response = client.Get("Users/" + log.Id);
                    Team? data = JsonConvert.DeserializeObject<Team>(response.Body);
                    //adding data if new user logs in
                    if (data == null)
                    {
                        //adding default image
                        Stream stream;
                        var path = Path.Combine("wwwroot/images/profile.png");
                        using (stream = new FileStream(path, FileMode.Open))
                        {
                            var task = new FirebaseStorage("consult673.appspot.com")
                                               .Child("images").Child(log.Id)
                                               .PutAsync(stream);
                            model.PhotoUrl = await task;
                        }
                        //adding empty model
                        SetResponse setResponse = client.Set("Users/" + log.Id, model);

                        if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            ModelState.AddModelError(string.Empty, "Added Succesfully");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Something went wrong!!");
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {

                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(log);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                HttpContext.Session.Remove("_UserToken");
                return View(log);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("_UserToken");
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public ActionResult EditProfile(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users/" + id);
            Team data = JsonConvert.DeserializeObject<Team>(response.Body);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(Team model, TeamLogIn log)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                var userEmail = HttpContext.Session.GetString("_UserToken");
                model.Email = userEmail;
                log.Id = userEmail.Replace('.', ',');

                //saving user profile photo
                if (model.Photo != null)
                {
                    Stream stream;

                    var path = Path.Combine("wwwroot/images/", log.Id);
                    using (stream = new FileStream(path, FileMode.Create))
                    {
                        await model.Photo.CopyToAsync(stream);
                    }
                    using (stream = new FileStream(path, FileMode.Open))
                    {
                        var task = new FirebaseStorage("consult673.appspot.com")
                                           .Child("images").Child(log.Id)
                                           .PutAsync(stream);

                        //saving the url of photo
                        model.PhotoUrl = await task;
                        model.Photo = null;
                    }
                }
                //saving user profile data
                SetResponse setResponse = client.Set("Users/" + log.Id, model);

                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError(string.Empty, "Added Succesfully");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong!!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }

        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Users/" + id);
            return RedirectToAction("LogOut");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}