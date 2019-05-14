using AlgorytmEwolucyjny;
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
            lines = lines.Add("SEPARATOR");
            StringBuilder sb = new StringBuilder();
            StringBuilder sbCsv = new StringBuilder();
            StringBuilder sbCsvNajlepsi = new StringBuilder();
            StringBuilder sbCsvPorownanie = new StringBuilder();

            string popsize = "Rozmiar";
            string pmutacji = "P. mutacji";
            string lprob = "Ilosc prób";
            string krzyzowanie = "Krzyżowanie ";
            string zbior = "zbior";
            string czas = "czaspracy";
            string nrpopulacji = "nr populacji";
            string wynik = "Wynik ";
            sbCsvPorownanie.AppendLine($"{popsize},{pmutacji},{lprob},{krzyzowanie},{zbior},{czas},{nrpopulacji},{wynik}");
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
                    sbCsv.AppendLine($"{popsize},{paramki[2]}");
                    sbCsv.AppendLine($"{pmutacji},{paramki[3]}.{paramki[4]}");
                    sbCsv.AppendLine($"{lprob},{paramki[5]}");
                    sbCsv.AppendLine($"{krzyzowanie},{paramki[7]}");
                    sbCsv.AppendLine($"{zbior},{paramki[10]}");
                    sbCsv.AppendLine($"{czas},{paramki[11]}");
                    sbCsv.AppendLine($"{nrpopulacji},{wynik}");

                    sbCsvNajlepsi.AppendLine($"{popsize},{paramki[2]}");
                    sbCsvNajlepsi.AppendLine($"{pmutacji},{paramki[3]}.{paramki[4]}");
                    sbCsvNajlepsi.AppendLine($"{lprob},{paramki[5]}");
                    sbCsvNajlepsi.AppendLine($"{krzyzowanie},{paramki[7]}");
                    sbCsvNajlepsi.AppendLine($"{zbior},{paramki[10]}");
                    sbCsvNajlepsi.AppendLine($"{czas},{paramki[11]}");
                    sbCsvNajlepsi.AppendLine($"{nrpopulacji},{wynik}");

                    
                    sbCsvPorownanie.Append($"{paramki[2]},{paramki[3]}.{paramki[4]},{paramki[5]},{paramki[7]},{paramki[10]},{paramki[11]},");

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
                    var najlepszy = lines[i - 1].Split(' ')[2].Split(',');
                    sbCsvPorownanie.AppendLine($"{najlepszy[0]},{najlepszy[1]}.{najlepszy[2]}");
                    sbCsvNajlepsi.AppendLine($"{najlepszy[0]},{najlepszy[1]}.{najlepszy[2]}");
                    sbCsvNajlepsi.AppendLine("Genotyp:," + lines[i].Split(':')[4]);
                    sb.AppendLine(lines[i]);
                    File.WriteAllText(Path.Combine(workingDir, $"{filename}_{j}.csv"), sbCsv.ToString(), Encoding.UTF8);
                    sbCsv.Clear();
                }
                i++;
            }
            File.WriteAllText(Path.Combine(workingDir, $"{filename}_najlepsi.csv"), sbCsvNajlepsi.ToString(), Encoding.UTF8);
            File.WriteAllText(Path.Combine(workingDir, $"{filename}_porownanie.csv"), sbCsvPorownanie.ToString(), Encoding.UTF8);
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
                        sb.Append(csvLines[m] + ",");
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
