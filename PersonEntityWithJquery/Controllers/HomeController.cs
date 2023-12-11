using Newtonsoft.Json;
using PersonEntityWithJquery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PersonEntityWithJquery.Controllers
{
    public class HomeController : Controller
    {

        personEntityEntities db = new personEntityEntities();

        public ActionResult Index()
        {
            List<Department> DeptList=db.Departments.ToList();
            ViewBag.ListofDepartment = new SelectList(DeptList, "DepartmentId", "DepartmentName");
            return View();
        }

        public JsonResult GetPersonList()
        {
            List<PersonViewModel> PrsList = db.Persons
                .Where(x => x.IsDeleted == false)
                .Select(x => new PersonViewModel
            {
                PersonId= x.PersonId,
                PersonName= x.PersonName,
                Email= x.Email,
                DepartmentName=x.Department.DepartmentName
            }).ToList();


            return Json(PrsList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPersonById(int PersonId) {

            Person model = db.Persons.Where(x => x.PersonId == PersonId).SingleOrDefault();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling= ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SaveDataInDatabase(PersonViewModel model)
        {
            var result = false;
            try
            {
                if (model.PersonId > 0)
                {
                    Person Prs = db.Persons.SingleOrDefault(x=>x.IsDeleted== false && x.PersonId==model.PersonId);
                    Prs.PersonName= model.PersonName;
                    Prs.Email= model.Email;
                    Prs.DepartmentId= model.DepartmentId;
                    db.SaveChanges();
                    result=true;
                }
                else
                {
                    Person Prs = new Person();
                    Prs.PersonName = model.PersonName;
                    Prs.Email = model.Email;
                    Prs.DepartmentId = model.DepartmentId;
                    Prs.IsDeleted = false;
                    db.Persons.Add(Prs);
                    db.SaveChanges();
                    result = true;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletePersonRecord(int PersonId)
        {
            bool result = false;
            Person Prs = db.Persons.SingleOrDefault(x=>x.IsDeleted==false && x.PersonId==PersonId);
            if(Prs != null )
            {
                Prs.IsDeleted = true;
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}