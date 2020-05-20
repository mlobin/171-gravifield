using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _171_gravifield.Controllers
{
    public class HomeController : Controller
    {
        public static Map MapPlanets = null;
        public ActionResult Index()
        {
            return File(Server.MapPath("/Views/Home/Index.html"), "text/html");
        }

        public ActionResult GetCss()
        {
            return File(Server.MapPath("/Views/Home/css/styles.css"), "text/css");
        }

        public ActionResult GetJs()
        {
            return File(Server.MapPath("/Views/Home/js/scripts.js"), "text/js");
        }
        public JsonResult Data() //Вывод всего
        {
            if (Calculator.Picture is null) //подсчет не начат
            {
 
                return Json(new Tuple<string, List<Planet>>("",new List<Planet>()), JsonRequestBehavior.AllowGet); 
            }
            else
            {
                //подсчет для картинки
                byte[] pic = Calculator.Picture;
                string Base64 = Convert.ToBase64String(pic);
                string Url = string.Format("data:image/png;base64,{0}", Base64);
                ViewBag.Image = Url;
                ViewBag.x = MapPlanets.width;
                ViewBag.y = MapPlanets.height;
                Tuple<string, List<Planet>> t = new Tuple<string, List<Planet>>(Base64, MapPlanets.planets);
                return Json(t, JsonRequestBehavior.AllowGet); 
            }
        }

        [HttpPost]
        public ActionResult Pole(int w, int h) //Получение x y
        {
            MapPlanets = new Map();
            MapPlanets.width = w;
            MapPlanets.height = h;
            return RedirectToAction("Data");   
        }

       [HttpPost]
        public ActionResult Add(string name, int x, int y, double mass)
        {
            MapPlanets.addPlanet(name, x, y, mass);
            Calculator.getResult(MapPlanets);
            return RedirectToAction("Data");  
        }

        public ActionResult Delete(string id) 
        {
            MapPlanets.deletePlanet(id);
            Calculator.getResult(MapPlanets);
            return RedirectToAction("Data"); 
        }

        /*public ActionResult Count() //Посчитать
        {
            Calculator.Calculate(MapPlanets);
            return RedirectToAction("Index");
        }*/

        public ActionResult Clear(string id)//Очистка всего
        {
            MapPlanets = new Map();
            Calculator.Picture = null;
            return RedirectToAction("Data");
        }
    }
}