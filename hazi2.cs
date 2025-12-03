namespace monitorok
{
    internal class Program
    {
        static List<Monitor> lista = new List<Monitor>();

        class Monitor
        {
            public string Manufacturer { get; set; }
            public string Model { get; set; }
            public int Size { get; set; }
            public string LCDType { get; set; }
            public int ResolutionX { get; set; }
            public int ResolutionY { get; set; }
            public string Range { get; set; }
            public string Interface { get; set; }
            public bool LFC { get; set; }
            public bool Freesync2 { get; set; }
            public bool HDR { get; set; }

            public Monitor(string sor)
            {
                var adat = sor.Split(';');

                Manufacturer = adat[0];
                Model = adat[1];
                Size = int.Parse(adat[2].Replace("\"", ""));
                LCDType = adat[3];

                var res = adat[4].Split('x');
                ResolutionX = int.Parse(res[0]);
                ResolutionY = int.Parse(res[1]);

                Range = adat[5];
                Interface = adat[6];
                LFC = adat[7] == "Yes";
                Freesync2 = adat[8] == "Yes";
                HDR = adat[9] == "Yes";
            }
        }

        static void beolvas()
        {
            StreamReader sr = new StreamReader("Monitorok.csv");
            sr.ReadLine();

            while (!sr.EndOfStream)
            {
                lista.Add(new Monitor(sr.ReadLine()));
            }

            sr.Close();
        }

        static void Main(string[] args)
        {
            beolvas();

            // 1. 
            var elsoFeladat = lista.Count(x => x.Interface.Contains("DisplayPort"));
            Console.WriteLine("DisplayPortos monitorok: " + elsoFeladat);

            // 2. 
            var masodikFeladat = lista.Count(x => x.ResolutionX >= 2000);
            Console.WriteLine("Legalább 2000 pixeles oszlop: " + masodikFeladat);

            // 3. 
            var harmadikFeladat = lista.Where(x => x.HDR && x.Size >= 27).Select(x => $"{x.Manufacturer} {x.Model} ({x.Size}\")");


            Console.WriteLine("HDR és legalább 27\":");
            Console.WriteLine(string.Join("\n", harmadikFeladat));

            // 4. 
            var negyedikFeladat = lista.GroupBy(x => x.Manufacturer).OrderBy(x => x.Key).Select(x => $"{x.Key}: {x.Count()} db");

            Console.WriteLine("Márkánkénti darabszám:");
            Console.WriteLine(string.Join("\n", negyedikFeladat));

            // 5. 
            var ipsDarab = lista.Count(x => x.LCDType == "IPS");
            var tnDarab = lista.Count(x => x.LCDType == "TN");

            var otodikFeladat =
                ipsDarab == tnDarab ? "Ugyanannyi van."
                : (ipsDarab > tnDarab ? "IPS-ből van több." : "TN-ből van több.");

            Console.WriteLine(otodikFeladat);

            // 6. 
            var hatodikFeladat = lista.Where(x => x.Size == 27 && x.Manufacturer != "HP" && x.Manufacturer != "Lenovo")
                .GroupBy(x => x.Manufacturer).Select(x => $"{x.Key}: {x.Count()} db");

            Console.WriteLine("27\"-os monitorok (HP és Lenovo nélkül):");
            Console.WriteLine(string.Join("\n", hatodikFeladat));

            // 7.
            var hetedikFeladat = lista.Where(x => x.LCDType == "IPS").OrderByDescending(x => x.ResolutionX * x.ResolutionY)
                .ThenBy(x => x.Model).Select(x => $"{x.Manufacturer} {x.Model} - {x.ResolutionX}x{x.ResolutionY}");

            Console.WriteLine("IPS monitorok felbontás szerint:");
            Console.WriteLine(string.Join("\n", hetedikFeladat));
        }
    }
}
