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
    public class TasksController : Controller
    {
        public string userID = "";
        public string serverEndPoin = "http://192.168.1.45:8081";
        // GET: Tasks
        public async Task<ActionResult> IndexAsync()
        {
            // Lưu trữ thông tin tạm của chương trình
            this.userID = Convert.ToString(TempData["UserID"]);
            ViewBag.UserName = TempData["UserName"];
            TempData.Keep();
            ViewBag.UserID = TempData["UserID"];
            TempData.Keep();
            ViewBag.Login = (userID == "") ? false : true;
            string url = serverEndPoin + "/tasks";

            // Khởi tạo http client
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", userID);
            // Get data
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var status = response.StatusCode;

            // Chuyển sang object 
            JavaScriptSerializer j = new JavaScriptSerializer();
            var obj = j.Deserialize<dynamic>(responseContent);
            List<TaskDT> listTask = new List<TaskDT>();
            foreach (var item in obj["data"])
            {
                TaskDT temp = new TaskDT();
                temp.Id = item["id"];
                temp.Name = item["name"];
                temp.DueTime = item["dueTime"];
                temp.Status = item["status"];
                temp.Tag = item["tagsList"][0]["name"];
                listTask.Add(temp);
            }
            ViewBag.DataSource = listTask;
            switch (status)
            {
                case System.Net.HttpStatusCode.OK:
                    ViewBag.Status = "Get list Successfull";
                    ViewBag.UserId = userID;
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    ViewBag.Status = "Get list UNSuccessfull";
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
                    ViewBag.Status = "Get list UNSuccessfull";
                    ViewBag.Message = "RequestTimeout";
                    break;
                case System.Net.HttpStatusCode.InternalServerError:
                    ViewBag.Status = "Get list UNSuccessfull";
                    ViewBag.Message = "InternalServerError";
                    break;
                default:
                    ViewBag.Message = obj["message"];
                    break;
            }
            return View();
        }


        public ActionResult Error(dynamic ViewBag)
        {

            return View(ViewBag);
        }

        // GET: Tasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(IFormCollection collection)
        {
            ViewBag.Login = (userID == "") ? false : true;

            try
            {
                this.userID = Convert.ToString(TempData["UserID"]);
                ViewBag.UserName = TempData["UserName"];
                TempData.Keep();
                var httpClient = new HttpClient();

                string url = serverEndPoin + "/tasks";
                string name = collection["name"];
                string description = collection["description"];
                string dueTime = DateTime.Parse(collection["duetime"]).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.sssZ");
                string tag = collection["tags"];
                // Khởi tạo http client
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url)
                };
                // Tạo StringContent
                string jsoncontent = "{\"taskName\": \"" + name + "\", \"description\": \"" + description + "\", \"dueTime\": \"" + dueTime + "\", \"tags\": [\"" + tag + "\"]}";
                var httpContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");
                httpRequestMessage.Content = httpContent;
                httpClient.DefaultRequestHeaders.Add("Authorization", userID);
                // call api
                var response = await httpClient.SendAsync(httpRequestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();
                var status = response.StatusCode;
                ViewBag.Message = responseContent;
                // Chuyển sang object 
                JavaScriptSerializer j = new JavaScriptSerializer();
                var obj = j.Deserialize<dynamic>(responseContent);


                switch (status)
                {
                    case System.Net.HttpStatusCode.OK:
                        ViewBag.Message = "Created";
                        break;
                    case System.Net.HttpStatusCode.Unauthorized:
                        break;
                    case System.Net.HttpStatusCode.Created:
                        ViewBag.Message = "New Task Created";
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
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
                        ViewBag.Message = "RequestTimeout";
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        ViewBag.Message = "InternalServerError";
                        break;
                    default:
                        ViewBag.Message = obj["message"];
                        break;
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message.ToString();
                ViewBag.Status = "Can't Create Task !!!";

                return Redirect(Error(ViewBag));
            }
        }

        public async Task<ActionResult> DetailAsync(string id)
        {
            ViewBag.Login = (userID == "") ? false : true;
            try
            {
                // lưu trữ và truy xuất thông tin biến tạm tại Client
                this.userID = Convert.ToString(TempData["UserID"]);
                ViewBag.UserName = TempData["UserName"];
                TempData.Keep();
                ViewBag.UserID = TempData["UserID"];
                TempData.Keep();
                ViewBag.Login = (userID == "") ? false : true;
                string url = serverEndPoin + "/tasks/" + id;

                // Khởi tạo http client
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", userID);
                // Get data
                var response = await httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                var status = response.StatusCode;
                // Chuyển sang object 
                JavaScriptSerializer j = new JavaScriptSerializer();
                var obj = j.Deserialize<dynamic>(responseContent);

                switch (status)
                {
                    case System.Net.HttpStatusCode.OK:
                        ViewBag.Status = "Get list Successfull";
                        ViewBag.UserId = userID;
                        Models.Task result = new Models.Task(obj["data"]["id"], obj["data"]["name"], obj["data"]["description"], obj["data"]["dueTime"], obj["data"]["status"], obj["data"]["tagsList"][0]["name"]);
                        return View(result);
                    case System.Net.HttpStatusCode.BadRequest:
                        ViewBag.Status = "Get list UNSuccessfull";
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
                        ViewBag.Status = "Get list UNSuccessfull";
                        ViewBag.Message = "RequestTimeout";
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        ViewBag.Status = "Get list UNSuccessfull";
                        ViewBag.Message = "InternalServerError";
                        break;
                    default:
                        ViewBag.Message = obj["message"];
                        break;
                }
                return View();
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message.ToString();
                ViewBag.Status = "sign in failed !!!";

                return Error(e);
            }

        }
        // GET: Tasks/Delete/5
        public async Task<ActionResult> DeleteAsync()
        {
            ViewBag.Login = (userID == "") ? false : true;
            this.userID = Convert.ToString(TempData["UserID"]);
            ViewBag.UserName = TempData["UserName"];
            TempData.Keep();
            ViewBag.UserID = TempData["UserID"];
            TempData.Keep();
            ViewBag.Login = (userID == "") ? false : true;
            string url = serverEndPoin + "/tasks";

            // Khởi tạo http client
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", userID);
            // Get data
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var status = response.StatusCode;

            // Chuyển sang object 
            JavaScriptSerializer j = new JavaScriptSerializer();
            var obj = j.Deserialize<dynamic>(responseContent);
            List<TaskDT> listTask = new List<TaskDT>();
            foreach (var item in obj["data"])
            {
                TaskDT temp = new TaskDT();
                temp.Id = item["id"];
                temp.Name = item["name"];
                temp.DueTime = item["dueTime"];
                temp.Status = item["status"];
                temp.Tag = item["tagsList"][0]["name"];
                listTask.Add(temp);
            }
            ViewBag.DataSource = listTask;
            switch (status)
            {
                case System.Net.HttpStatusCode.OK:
                    ViewBag.Status = "Get list Successfull";
                    ViewBag.UserId = userID;
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    ViewBag.Status = "Get list UNSuccessfull";
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
                    ViewBag.Status = "Get list UNSuccessfull";
                    ViewBag.Message = "RequestTimeout";
                    break;
                case System.Net.HttpStatusCode.InternalServerError:
                    ViewBag.Status = "Get list UNSuccessfull";
                    ViewBag.Message = "InternalServerError";
                    break;
                default:
                    ViewBag.Message = obj["message"];
                    break;
            }
            return View();
        }
        public async Task<ActionResult> DeleteItemAsync(string id)
        {
            ViewBag.Login = (userID == "") ? false : true;
            try
            {
                // lưu thông tin tạm của chương trình 
                this.userID = Convert.ToString(TempData["UserID"]);
                ViewBag.UserName = TempData["UserName"];
                TempData.Keep();
                ViewBag.UserID = TempData["UserID"];
                TempData.Keep();
                ViewBag.Login = (userID == "") ? false : true;
                string url = serverEndPoin + "/tasks/" + id;

                // Khởi tạo http client
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", userID);
                // Get data
                var response = await httpClient.DeleteAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                var status = response.StatusCode;
                // Chuyển sang object 
                JavaScriptSerializer j = new JavaScriptSerializer();
                var obj = j.Deserialize<dynamic>(responseContent);

                switch (status)
                {
                    case System.Net.HttpStatusCode.OK:
                        ViewBag.Status = "Delete Successfull";
                        ViewBag.UserId = userID;
                        return RedirectToAction("Delete", "Tasks");
                    case System.Net.HttpStatusCode.BadRequest:
                        ViewBag.Status = "Delete UNSuccessfull";
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
                        ViewBag.Status = "Delete UNSuccessfull";
                        ViewBag.Message = "RequestTimeout";
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        ViewBag.Status = "Delete UNSuccessfull";
                        ViewBag.Message = "InternalServerError";
                        break;
                    default:
                        ViewBag.Message = obj["message"];
                        break;
                }
                return RedirectToAction("Delete", "Tasks");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message.ToString();
                ViewBag.Status = "sign in failed !!!";

                return RedirectToAction("Delete", "Tasks");
            }
        }
    }
}