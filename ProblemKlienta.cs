using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srodowisko
{
    public class ProblemKlienta
    {
        Coords[] ParsedCitiesToVisit;
        public ProblemKlienta()
        {
            LoadCities("cities.tsp");
        }
        public int Rozmiar(int numer_zbioru = 0)
        {
            return ParsedCitiesToVisit.Length;
        }
        public double Ocena(int[] sciezka)
        {
            Coords[] result = new Coords[sciezka.Length];
            for (int i = 0; i < sciezka.Length; i++)
            {
                result[i] = ParsedCitiesToVisit[sciezka[i]];
            }
            double distance = 0;
            for (int i = 0; i < result.Length - 2; i++)
            {
                distance += DistanceBetweenCoords(result[i], result[i + 1]);
            }
            return distance;
        }
        public void LoadCities(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                int start = FindStartingIndex(lines);
                var end = FindEndIndex(lines);
                this.ParsedCitiesToVisit = new Coords[end - start];
                for (int i = start; i < end; i++)
                {
                    string[] values = lines[i].Split(' ');
                    double lattitude = double.Parse(values[1]);
                    double longitude = double.Parse(values[2]);
                    ParsedCitiesToVisit[i - start] = new Coords()
                    {
                        X = lattitude,
                        Y = longitude
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't read the cities file - Cannot continue. Exception message: {ex.Message}");
            }
        }
        private int FindEndIndex(string[] lines)
        {
            var end = lines.Length - 1;
            while (!lines[end].Contains("EOF"))
            {
                end--;
            }
            return end;
        }
        private int FindStartingIndex(string[] lines)
        {
            var i = 0;
            while (!char.IsDigit(lines[i][0]))
            {
                i++;
            }
            return i;
        }
        public struct Coords
        {
            public double X, Y;
        }
        private static double DistanceBetweenCoords(Coords startCoord, Coords endCoord)
        {
            return Math.Sqrt(Math.Pow(endCoord.Y - startCoord.Y, 2) + Math.Pow(endCoord.X - startCoord.X, 2));
        }
    }
}
