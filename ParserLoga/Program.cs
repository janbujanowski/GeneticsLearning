using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLoga
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = "JanBujanowski_log";
            //var workingDir = "C:\\Users\\janbu\\OneDrive\\Studia\\Rok V sem III\\EksperymentGenetyczny\\Logi1";
            var workingDir = "C:\\Logi1";
            var lines = File.ReadAllLines(Path.Combine(workingDir, filename + ".txt"));

            StringBuilder sb = new StringBuilder();
            StringBuilder sbCsv = new StringBuilder();
            var i = 5;
            var j = 0;
            while (i < lines.Count())
            {
                sb.AppendLine(lines[i]);
                if (lines[i].Contains("SEPARATOR"))
                {
                    File.WriteAllText(Path.Combine(workingDir, $"{filename}_{j}.txt"), sb.ToString(), Encoding.UTF8);
                    j++;
                    sb.Clear();
                }
                if (lines[i].Contains("New instance"))
                {
                    var paramki = lines[i].Split(',');
                    sbCsv.AppendLine($"Rozmiar populacji,{paramki[2]}");
                    sbCsv.AppendLine($"Prawdopodobieństwo mutacji,{paramki[3]}.{paramki[4]}");
                    sbCsv.AppendLine($"Ilosc prób mutacji,{paramki[5]}");
                    sbCsv.AppendLine($"Krzyżowanie,{paramki[7]}");
                    sbCsv.AppendLine($"Zbiór,{paramki[10]}");
                    sbCsv.AppendLine($"Czas pracy w minutach,{paramki[11]}");
                }
                if (lines[i].Contains("Heavens csv"))
                {
                    i++;
                    while (!lines[i].Contains("Best:"))
                    {
                        var line = lines[i].Split(' ')[2].Split(',');
                        sbCsv.AppendLine($"{line[0]},{line[1]}.{line[2]}");
                        i++;
                    }
                    sb.AppendLine(lines[i]);
                    File.WriteAllText(Path.Combine(workingDir, $"{filename}_{j}.csv"), sbCsv.ToString(),Encoding.UTF8);
                    sbCsv.Clear();
                }
                i++;
            }
            List<string[]> linesOfFiles = new List<string[]>();
            for (int m = 0; m < j; m++)
            {
                linesOfFiles.Add(File.ReadAllLines(Path.Combine(workingDir, $"{filename}_{m}.csv")));
            }
            linesOfFiles.OrderByDescending(x => x.Length);
            sb.Clear();
            for (int m = 0; m < linesOfFiles[0].Length; m++)
            {
                foreach (var csvLines in linesOfFiles)
                {
                    if (csvLines.Length > m)
                    {
                        sb.Append(csvLines[m] +",");
                    }
                    else
                    {
                        sb.Append(",,");
                    }
                }
                sb.AppendLine();
            }
            File.WriteAllText(Path.Combine(workingDir, $"{filename}.csv"), sb.ToString(), Encoding.UTF8);
        }
    }
}
