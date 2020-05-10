using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace _171_gravifield
{ 


    public static class Calculator
    {
        private static FastMath math = new FastMath();
        public static DirectBitmap Result;

        public const double G = 6.67191 * 1e-11;
        static double max, min = 0, speed;
        static double mx = 0, m = 0, my = 0;

        //TODO: параллельность
        //TODO: проверки "на дурака"
        public static void Calculate(Map map) 
        {
            int w = map.width;
            int h = map.height;
            DirectBitmap TempResult = new DirectBitmap(w, h);
            foreach (Planet t in map.planets)
                max = Math.Max(max, t.mass);
            max *= G;
            max /= Math.Sqrt(w * h);
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                {
                    double Fx = 0, Fy = 0;
                    foreach (Planet p in map.planets)
                    {
                        double xx = p.x - i; //расстояние по Х до планеты
                        double yy = p.y - j; //расстояние по Y до планеты
                        if ((xx == 0) && (yy == 0)) yy = 0.5; //Чтобы не делить на ноль
                        double F = p.gmass / (xx * xx + yy * yy); //сократили формулу 
                        double aa = math.calcatan2(yy, xx);
                        Fx += F * Math.Cos(aa);
                        Fy += F * Math.Sin(aa);
                    }
                    double res = Math.Sqrt(Fx * Fx + Fy * Fy);
                    int precolor = (int)Calculator.map(res, min, max, 0, 510);//переименовать, что ли?
                    if (precolor > 255)
                        TempResult.SetPixel(i, j, Color.FromArgb(precolor - 255, 510 - precolor, 0));
                    else TempResult.SetPixel(i, j, Color.FromArgb(0, precolor, 255 - precolor));
                }
            Result = TempResult;
        }


        //Перегоняем по логарифмическим правилам числа к нужному размеру
        private static double map(double x, double a1, double a2, double b1, double b2)
        {
            if (x == 0) return 0;
            if (a1 == 0) a1 = 1e-5;
            if (a2 == 0) a2 = 1e-5;
            if (x > a2) return b2;
            if (x < a1) return b1;
            return b1 + b2 * (Math.Log10(x) - Math.Log10(a1)) / (Math.Log10(a2) - Math.Log10(a1));
        }
    }

    public class Map
    {
        public List<Planet> planets;

        public int width=0, height=0; //по умолчанию

        public Map()
        {
            planets = new List<Planet>();
        }

        private void addPlanet(Planet p)
            => planets.Add(p);

        public void addPlanet(string name, int x, int y, double mass) //Для основного использования
        {
            if (x >= width || x < 0 || y >= height || y < 0)
                throw new Exception("Planet is not on the map!");
            if (getPlanet(name) != null)
                throw new Exception("Planet with this name already exists!");
            addPlanet(new Planet(name, x, y, mass));
        }

        public void editPlanet(string name, int x, int y, double mass)
        {
            if (getPlanet(name) != null)
            {
                deletePlanet(name);
                addPlanet(name, x, y, mass);
            }
            else throw new Exception("Planet does not exist!");
        }

        public void editPlanetName(string oldName, string newName, int x, int y, double mass)
        {
            if (oldName == newName)
                editPlanet(oldName, x, y, mass);
            deletePlanet(oldName);
            addPlanet(newName, x, y, mass);
        }

        public void deletePlanet(string name)
        {
            int x = planets.FindIndex(planet => planet.name == name);
            if (x > -1)
                planets.RemoveAt(x);
            //It does not exist anyway ¯\_(ツ)_/¯
        }

        public Planet getPlanet(int num)
            => planets[num];

        public Planet getPlanet(string name)
            => planets.Find(planet => planet.name == name);
    }

    public class Planet
    {
        public string name;
        public int x, y;
        public double mass, gmass;

        public Planet(string name, int x, int y, double mass)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.mass = mass;
            this.gmass = mass * Calculator.G;
        }

        public Planet() //Just in case, never needed
        {

        }
    }
}