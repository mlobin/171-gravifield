using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
                    double xx,yy, F, aa; //Чтобы не объявлять каждый раз
                    foreach (Planet p in map.planets)
                    {
                        xx = p.x - i; //расстояние по Х до планеты
                        yy = p.y - j; //расстояние по Y до планеты
                        if ((xx == 0) && (yy == 0)) yy = 0.5; //Чтобы не делить на ноль
                        F = p.gmass / (xx * xx + yy * yy); //сократили формулу 
                        aa = math.calcatan2(yy, xx);
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
        private static double map(double x, double a1, double a2, double b1, double b2, bool ln=false)
        {
            if (x == 0) return 0;
            if (a1 == 0) a1 = 1e-5;
            if (a2 == 0) a2 = 1e-5;
            if (x > a2) return b2;
            if (x < a1) return b1;
            if (ln)
                return b1 + b2 * (Math.Log(x) - Math.Log(a1)) / (Math.Log(a2) - Math.Log(a1));
            return b1 + b2 * (Math.Log10(x) - Math.Log10(a1)) / (Math.Log10(a2) - Math.Log10(a1));
        }

        public static void Gif(Map map)
        {
            MagickImageCollection collection = new MagickImageCollection();
            Dictionary<string, Tuple<double, double>> speeds = new Dictionary<string, Tuple<double, double>>();
            foreach (Planet p in map.planets)
                speeds[p.name] = new Tuple<double, double>(0, 0);
            for(int i = 0; i < 40; i++)
            {
                Calculate(map);
                Result.Save("D:/gif/Snakeware"+i.ToString()+".png");
                collection.Add("D:/gif/Snakeware" + i.ToString() + ".png");
                collection[i].AnimationDelay = 10;
                foreach(Planet p in map.planets)
                {
                    double Fx=0, Fy=0, xx = 0, yy = 0;
                    foreach(Planet p2 in map.planets)
                    {
                        if (p == p2) continue;
                        xx = p.x - p2.x;
                        yy = p.y - p2.y;
                        double F = p.gmass * p2.mass / (xx * xx + yy * yy);
                        double aa = Math.Atan2(xx, yy);
                        Fx += F * Math.Sin(aa);
                        Fy += F * Math.Cos(aa);
                    }
                    double accelx = Fx / p.mass, accely = Fy / p.mass;
                    speeds[p.name] = new Tuple<double, double>(speeds[p.name].Item1+accelx, speeds[p.name].Item2+accely);
                }
                Map tmp = new Map(map.width, map.height);
                tmp = map.Clone();
                double maxspeed = 0;
                foreach(var s in speeds)
                {
                    maxspeed = Math.Max(maxspeed, Math.Abs(s.Value.Item1));
                    maxspeed = Math.Max(maxspeed, Math.Abs(s.Value.Item2));
                }
                foreach(Planet p in map.planets)
                {
                    double speedx = speeds[p.name].Item1, speedy = speeds[p.name].Item2;
                    bool signx = speedx < 0, signy = speedy < 0;
                    speedx = Calculator.map(Math.Abs(speedx), 0, maxspeed, 0, Math.Sqrt(map.width * map.height) / 20, true);
                    speedy = Calculator.map(Math.Abs(speedy), 0, maxspeed, 0, Math.Sqrt(map.width * map.height) / 20, true);
                    speedx *= signx ? -1 : 1;
                    speedy *= signy ? -1 : 1;
                    int posx = p.x - (int)speedx, posy = p.y - (int)speedy;
                    posx = Math.Min(Math.Max(posx, 0), map.width-1);
                    posy = Math.Min(Math.Max(posy, 0), map.height-1);
                    tmp.editPlanet(p.name, posx, posy, p.mass);
                }
                map = tmp.Clone();
            }

            collection.Optimize();
            // Save gif
            collection.Write("D:/Snakeware.Animated.gif");
        }
    }

    public class Map
    {
        public List<Planet> planets;

        public int width, height;

        public Map(int x, int y)
        {
            planets = new List<Planet>();
            width = x; height = y;
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

        public Map Clone()
        {
            Map m = new Map(width, height);
            foreach (Planet p in planets)
                m.addPlanet(p.name, p.x, p.y, p.mass);
            return m;
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