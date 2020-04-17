using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _171_gravifield.Controllers
{
    public class HomeController : Controller
    {
        public Map MapPlanets = new Map();
        public ActionResult Index() //ввод размеров поля
        {
            return View();
        }
        [HttpPost]
        public ActionResult Pole(int w, int h)
        {
            MapPlanets.width = w;
            MapPlanets.height = h;
            return RedirectToAction("ListManage");
        }
        [HttpGet]
        public ActionResult ListManage() //вывод текущего списка и кнопок
        {
            //if (MapPlanets.planets.Count != 0)
                return View(MapPlanets.planets);
            
            //else
               // return RedirectToAction("Add");
        }
        

        [HttpGet]
        public ActionResult Add() //нажата кнопка Добавить - вывод формы
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(string name, int x, int y, double mass)
        {
            
                MapPlanets.addPlanet(name, x, y, mass);
                return RedirectToAction("ListManage");
        }
        [HttpPost]
        public RedirectResult Delete(string nam) //нажата какая-то кнопка Удалить
        {
            MapPlanets.deletePlanet(nam);
            return Redirect("/Home/ListManage");
        }
        [HttpPost]
        public ActionResult Count() //нажата кнопка Посчитать
        {
            Calculator.Calculate(MapPlanets);
            return View(Calculator.Result);
        }
    }
}