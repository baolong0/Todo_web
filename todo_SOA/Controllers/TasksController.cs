using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

            userID = Convert.ToString(TempData["UserID"]);
            ViewBag.UserName = TempData["UserName"];
            TempData.Keep();
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

        // GET: Tasks/Details/5
        public ActionResult List()
        {

            return View();
        }

        // GET: Tasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: Tasks/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: Tasks/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tasks/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }
    }
}