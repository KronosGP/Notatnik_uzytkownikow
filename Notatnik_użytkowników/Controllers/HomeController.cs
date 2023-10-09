using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Notatnik_użytkowników.Models;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Notatnik_użytkowników.Controllers
{
    public class HomeController : Controller
    {
        private readonly string XmlFilePath = "data.xml";

        public IActionResult Index()
        {
            var dane = ReadXML(XmlFilePath);
            return View(dane);
        }

        public IActionResult Create()
        {
            UsersModel temp = new UsersModel();
            temp.Id = Guid.NewGuid();
            temp.Birth = DateTime.Now;
            return PartialView("_PopUpUserPartial", temp);
        }

        public IActionResult GenerateReport(string format)
        {
            var users = ReadXML(XmlFilePath);
            if (format == "csv")
            {
                var csvContent = "Tytuł,Imie,Nazwisko,Data urodzenia,Wiek,Płec\n";
                foreach (var user in users)
                {
                    var tytul = user.SelectedGender == "Kobieta" ? "Pani" : "Pan";
                    var wiek = DateTime.Today.Year - user.Birth.Year;
                    csvContent += $"{tytul},{user.Name},{user.Surname},{user.Birth.ToShortDateString()},{wiek},{user.SelectedGender}\n";
                }
                var raportName = $"{DateTime.Now}.csv";
                var bytes = Encoding.UTF32.GetBytes(csvContent);
                return File(bytes, "text/csv", raportName);
            }

                // Dodaj obsługę generowania raportu w innych formatach (np. Excel, PDF) według potrzeb
                // ...
            return RedirectToAction("Index");

        }


        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var users = ReadXML(XmlFilePath);
            var user = users.FirstOrDefault(u => u.Id==id);

            if (user == null)
            {
                return NotFound();
            }

            return PartialView("_PopUpUserPartial", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(UsersModel user, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                if (user.Birth > DateTime.Now)
                {
                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, type="Date", errors= error });
                }

                List<UsersModel> users = ReadXML(XmlFilePath);
                var NewUser = users.FirstOrDefault(u => u.Id == user.Id);
                string attributeKey = null;
                string attributeValue = null;
                foreach (var key in form.Keys)
                {
                    if (key.EndsWith("Key"))
                    {
                        attributeKey = form[key];
                    }
                    else if (key.EndsWith("Value"))
                    {
                        attributeValue = form[key];

                        // Dodaj atrybut do listy
                        if (!string.IsNullOrEmpty(attributeKey) && !string.IsNullOrEmpty(attributeValue))
                        {
                            user.Attributes.Add((attributeKey,attributeValue));
                        }
                    }
                    
                }

                if (NewUser != null)
                {
                    NewUser.Name = user.Name;
                    NewUser.Surname = user.Surname;
                    NewUser.Birth = user.Birth;
                    NewUser.SelectedGender = user.SelectedGender;
                    NewUser.Attributes = user.Attributes;
                }
                else
                {
                    users.Add(user);
                }
                foreach (var temp in users)
                    temp.GenderOptions.Clear();

                SaveToXml(users, XmlFilePath);

                return Json(new { success = true });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, errors = errors, type="Valid" });
        }

        private void SaveToXml(List<UsersModel> users, string xmlFilePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<UsersModel>));
            using (StreamWriter writer = new StreamWriter(xmlFilePath))
            {
                serializer.Serialize(writer, users);
            }
        }

        private List<UsersModel> ReadXML(string xmlFilePath)
        {
            if (System.IO.File.Exists(xmlFilePath))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<UsersModel>));
                    using (StreamReader reader = new StreamReader(xmlFilePath))
                    {
                        return (List<UsersModel>)serializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return new List<UsersModel>();
            }
            else
            {
                SaveToXml(new List<UsersModel>(), xmlFilePath);
                return new List<UsersModel>();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
