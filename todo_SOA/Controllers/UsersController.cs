using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using todo_SOA.Models;

namespace todo_SOA.Controllers
{
    public class UsersController : Controller
    {
        public static string userID = "";
        public static string serverEndPoin = "http://192.168.1.45:8081";
        // GET: Users
        public ActionResult Index()
        {
            ViewBag.Login = (userID == "") ? false : true;
            return View();
        }
        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(IFormCollection collection)
        {
            ViewBag.Login = false;
            string email = collection["email"];
            string password = collection["password"];
            string firstName = collection["firstName"];
            string lastName = collection["lastName"];
            try
            {
                var httpClient = new HttpClient();

                string url = serverEndPoin + "/users";
                // Khởi tạo http client
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url)
                };
                // Tạo StringContent
                string jsoncontent = "{\"email\": \"" + email + "\", \"password\": \"" + password + "\",\"firstName\": \"" + firstName + "\",\"lastName\": \"" + lastName + "\"}";
                var httpContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");
                httpRequestMessage.Content = httpContent;
                // call api
                var response = await httpClient.SendAsync(httpRequestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();
                var status = response.StatusCode;

                // Chuyển sang object 
                JavaScriptSerializer j = new JavaScriptSerializer();
                var obj = j.Deserialize<dynamic>(responseContent);


                switch (status)
                {
                    case System.Net.HttpStatusCode.OK:
                        ViewBag.Status = "Create sucessfull";
                        return RedirectToAction("index", "Tasks");
                    case System.Net.HttpStatusCode.BadRequest:
                        ViewBag.Status = "Create failed !!!";
                        string Res = "";
                        if (obj["reasons"] != null)
                        {
                            foreach (var item in obj["reasons"])
                            {
                                Res = Res + "\n" + item["path"] + " : " + item["message"];
                            }
                        }
                        ViewBag.Message = obj["message"] + "\n" + Res;
                        break;
                    case System.Net.HttpStatusCode.RequestTimeout:
                        ViewBag.Status = "Create failed !!!";
                        ViewBag.Message = "RequestTimeout";
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        ViewBag.Status = "Create failed !!!";
                        ViewBag.Message = "InternalServerError";
                        break;
                    case System.Net.HttpStatusCode.Created:
                        ViewBag.Status = "Create sucessfull";
                        return View();
                    default:
                        ViewBag.Message = obj["message"];
                        break;
                }
                return View();
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message.ToString();
                ViewBag.Status = "Create failed !!!";
                return View();
            }
        }
        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(IFormCollection collection)
        {
            ViewBag.Login = false;
            string email = collection["email"];
            string password = collection["password"];
            try
            {
                var httpClient = new HttpClient();

                string url = serverEndPoin + "/users/login";
                // Khởi tạo http client
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url)
                };
                // Tạo StringContent
                string jsoncontent = "{\"email\": \"" + email + "\", \"password\": \"" + password + "\"}";
                var httpContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");
                httpRequestMessage.Content = httpContent;
                // call api
                var response = await httpClient.SendAsync(httpRequestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();
                var status = response.StatusCode;

                // Chuyển sang object 
                JavaScriptSerializer j = new JavaScriptSerializer();
                var obj = j.Deserialize<dynamic>(responseContent);


                switch (status)
                {
                    case System.Net.HttpStatusCode.OK:
                        userID = obj["data"]["id"];
                        TempData["UserName"] = obj["data"]["firstName"];
                        TempData["userID"] = userID;
                        TempData.Keep();
                        ViewBag.Status = "Sign in sucessfull";
                        ViewBag.UserId = userID;
                        return RedirectToAction("index","Tasks");
                    case System.Net.HttpStatusCode.BadRequest:
                        ViewBag.Status = "sign in failed !!!";
                        string Res = "";
                        if (obj["reasons"] != null)
                        {
                            foreach (var item in obj["reasons"])
                            {
                                Res = Res + "\n" + item["path"] + " : " + item["message"];
                            }
                        }
                        ViewBag.Message=obj["message"] + "\n" + Res;
                        break;
                    case System.Net.HttpStatusCode.RequestTimeout:
                        ViewBag.Status = "sign in failed !!!";
                        ViewBag.Message="RequestTimeout";
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        ViewBag.Status = "sign in failed !!!";
                        ViewBag.Message="InternalServerError";
                        break;
                    default:
                        ViewBag.Message=obj["message"];
                        break;
                }
                return View();
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message.ToString();
                ViewBag.Status = "sign in failed !!!";
                return View();
            }
        }
    }
}