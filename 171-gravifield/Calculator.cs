using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace _171_gravifield
{
    public class Calculator
    {
        public Bitmap result
        {
            get { Calculate(); return _result; }
            set { _result = value; }
        }

        private Bitmap _result;

        private void Calculate()
        {

        }
    }

    public class Map
    {
        private List<Planet> planets;

        private int width, height;

        public Map(int x, int y)
        {
            planets = new List<Planet>();
            width = x; height = y;
        }

        private void addPlanet(Planet p)
            => planets.Add(p);

        public void addPlanet(string name, int x, int y) //To be used generally
        {
            if (x >= width || x < 0 || y >= height || y < 0)
                throw new Exception("Planet is not on the map!");
            if (getPlanet(name) != null)
                throw new Exception("Planet with this name already exists!");
            Planet p = new Planet(name, x, y);
            addPlanet(p);
        }

        public void editPlanet(string name, int x, int y)
        {
            if (getPlanet(name) != null)
            {
                deletePlanet(name);
                addPlanet(name, x, y);
            }
            else throw new Exception("Planet does not exist!");
        }

        public void editPlanetName(string oldName, string newName, int x, int y)
        {
            if (oldName == newName)
                editPlanet(oldName, x, y);
            deletePlanet(oldName);
            addPlanet(newName, x, y);
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

        public Planet(string name, int x, int y)
        {
            this.name = name;
            this.x = x;
            this.y = y;
        }

        public Planet() //Just in case, never needed
        {

        }
    }
}