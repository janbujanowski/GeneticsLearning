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
            var iteracja = 5;
            var tryb = "b";
            bool nowyKod = true;
            var filename = $"JanBujanowski_{tryb}_log";
            var workingDir = $"C:\\Users\\janbu\\OneDrive\\Studia\\Rok V sem III\\EksperymentGenetyczny\\Iteracja{iteracja}";
            //var workingDir = "C:\\Logi1";
            var lines = File.ReadAllLines(Path.Combine(workingDir, filename + ".txt"));
            lines = lines.Add("SEPARATOR");
            StringBuilder sb = new StringBuilder();
            StringBuilder sbCsv = new StringBuilder();
            StringBuilder sbCsvNajlepsi = new StringBuilder();
            StringBuilder sbCsvPorownanie = new StringBuilder();

            string popsize = "Rozmiar";
            string pmutacji = "P. mutacji";
            string lprob = "Ilość prób";
            string krzyzowanie = "Op.";
            string zbior = "Zbiór";
            string czas = "T[m]";
            string nrpopulacji = "Generacja";
            string wynik = "Wynik";
            sbCsvPorownanie.AppendLine($"{popsize},{pmutacji},{lprob},{krzyzowanie},{zbior},{czas},{nrpopulacji},{wynik}");
            var i = 1;
            var j = 0;
            while (i < lines.Count() - 13)
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
                    var pierwszyLine = lines[i].Split(' ')[2].Split(',');
                    var pierwszy = $"{pierwszyLine[1]}.{pierwszyLine[2]}";
                    while (!lines[i].Contains("Best:"))
                    {
                        var line = lines[i].Split(' ')[2].Split(',');
                        if (nowyKod)
                        {
                            sbCsv.AppendLine($"{line[0]},{line[1]}.{line[2]},{line[3]} {lines[i].Split(' ')[3]}");
                        }
                        else
                        {
                            sbCsv.AppendLine($"{line[0]},{line[1]}.{line[2]}");
                        }

                        i++;
                    }
                    var najlepszy = lines[i - 1].Split(' ')[2].Split(',');
                    sbCsvPorownanie.AppendLine($"{najlepszy[0]},{najlepszy[1]}.{najlepszy[2]},={najlepszy[1]}.{najlepszy[2]}/{pierwszy}");
                    sbCsvNajlepsi.AppendLine($"{najlepszy[0]},{najlepszy[1]}.{najlepszy[2]}");
                    sbCsvNajlepsi.AppendLine("Genotyp:," + lines[i].Split(':')[3]);
                    sb.AppendLine(lines[i]);
                    File.WriteAllText(Path.Combine(workingDir, $"{filename}_{j}temp.csv"), sbCsv.ToString(), Encoding.UTF8);
                    sbCsv.Clear();
                }
                i++;
            }
            File.WriteAllText(Path.Combine(workingDir, $"{filename}_najlepsi_{iteracja}.csv"), sbCsvNajlepsi.ToString(), Encoding.UTF8);
            File.WriteAllText(Path.Combine(workingDir, $"{filename}_porownanie_{iteracja}.csv"), sbCsvPorownanie.ToString(), Encoding.UTF8);
            List<string[]> linesOfFiles = new List<string[]>();
            for (int m = 0; m < j; m++)
            {
                linesOfFiles.Add(File.ReadAllLines(Path.Combine(workingDir, $"{filename}_{m}temp.csv")));
            }
            linesOfFiles.OrderByDescending(x => x.Length);
            sb.Clear();
            for (int m = 0; m < linesOfFiles[0].Length; m++)
            {
                //var sepratedLogLines = File.ReadAllLines(Path.Combine(workingDir, $"{filename}_{m}.txt"));
                foreach (var csvLines in linesOfFiles)
                {
                    if (csvLines.Length > m)
                    {
                        sb.Append(csvLines[m] + ",");
                    }
                    else
                    {
                        sb.Append(",,,");
                    }
                }
                sb.AppendLine();
            }
            File.WriteAllText(Path.Combine(workingDir, $"{filename}_{iteracja}.csv"), sb.ToString(), Encoding.UTF8);

            for (int m = 0; m < j; m++)
            {
                File.Delete(Path.Combine(workingDir, $"{filename}_{m}temp.csv"));
                File.Delete(Path.Combine(workingDir, $"{filename}_{m}.txt"));
            }
        }
    }
}
