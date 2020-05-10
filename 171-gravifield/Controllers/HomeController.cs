using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _171_gravifield.Controllers
{
    public class HomeController : Controller
    {
        public static Map MapPlanets = new Map();
        
        public JsonResult Index() //Вывод всего
        {
            if (Calculator.Result is null) //подсчет не начат
            {
                ViewBag.x = MapPlanets.width;
                ViewBag.y = MapPlanets.height;
                return Json(MapPlanets.planets, JsonRequestBehavior.AllowGet); 
            }
            else
            {
                //подсчет для картинки
                byte[] pic = Calculator.Result.ConvertToByteArray();
                string Base64 = Convert.ToBase64String(pic);
                string Url = string.Format("data:image/png;base64,{0}", Base64);
                ViewBag.Image = Url;
                ViewBag.x = MapPlanets.width;
                ViewBag.y = MapPlanets.height;
                return Json(MapPlanets.planets, JsonRequestBehavior.AllowGet); 
            }
        }

        [HttpPost]
        public ActionResult Pole(int w, int h) //Получение x y
        {
            MapPlanets.width = w;
            MapPlanets.height = h;
            if (!(Calculator.Result is null))
            {
                Calculator.Result = null;//очищаем картинку
            }
            return RedirectToAction("Index");   
        }

        [HttpPost]
        public ActionResult Add(string name, int x, int y, double mass)
        {
            MapPlanets.addPlanet(name, x, y, mass);
            if(!(Calculator.Result is null))
            {
                Calculator.Result = null;//очищаем картинку
            }
            return RedirectToAction("Index");  
        }

        public ActionResult Delete(string id) 
        {
            MapPlanets.deletePlanet(id);
            Calculator.Result = null;//очищаем картинку
            return RedirectToAction("Index"); 
        }

        public ActionResult Count() //Посчитать
        {
            Calculator.Calculate(MapPlanets);
            return RedirectToAction("Index");
        }

        public RedirectResult Clear(string id)//Очистка всего
        {
            MapPlanets = new Map();
            Calculator.Result = null;
            return Redirect("/Home/Index");
        }
    }
}