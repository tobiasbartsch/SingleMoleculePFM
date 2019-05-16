using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SingleMoleculePFM
{
    static class utils
    {
        public static void write(double[,] timeseries)
        {
            StreamWriter file = new StreamWriter(@"WLC.csv");
            for (int j = 0; j < timeseries.GetLength(0); j++)
            {
                for (int i = 0; i < timeseries.GetLength(1); i++)
                {
                    file.Write(timeseries[j, i]);
                    file.Write(",");
                }
                file.Write("\n"); // go to next line
            }
        }

        public static int[] readmsequence(string filename)
        {
            List<int> listA = new List<int>();
            using (var reader = new StreamReader(filename))
            {   
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    int x = 0;
                    Int32.TryParse(line, out x);

                    listA.Add(x);
                }
            }
            return listA.ToArray();
        }
    }
}
