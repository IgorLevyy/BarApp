namespace TradeBarApp
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            string path = "ABBV-IQFeed-SMART-Stocks-Minute-Trade_1.txt";
            string path1 = "ABBV-IQFeed-SMART-Stocks-Minute-Trade_2.txt";
            string path2 = "ABBV-IQFeed-SMART-Stocks-Minute-Trade_3.txt";

            List<Bar> listBar = new List<Bar>();
            List<Bar> listBar1 = new List<Bar>();
            List<Bar> listBar2 = new List<Bar>();

            if (File.Exists(path))
            {
                string[] readText = File.ReadAllLines(path);
                readText = readText.Skip(1).ToArray();

                foreach (string s in readText)
                {
                    string[] subs = s.Split(',');
                    listBar.Add(ConvertToObj(subs));
                    Console.WriteLine(s);
                }

                Task1(listBar);
                Console.WriteLine("Task1 complete");
                Task2(listBar);
                Console.WriteLine("Task2 complete");
            }
            else
            {
                Console.WriteLine("Файл не найден");
            }

            string[] readText1 = File.ReadAllLines(path1);
            readText1 = readText1.Skip(1).ToArray();

            string[] readText2 = File.ReadAllLines(path2);
            readText2 = readText2.Skip(1).ToArray();

            foreach (string s in readText1)
            {
                string[] subs = s.Split(',');
                listBar1.Add(ConvertToObj(subs));
            }

            foreach (string s in readText2)
            {
                string[] subs = s.Split(',');
                listBar2.Add(ConvertToObj(subs));
            }

            Task3(listBar1, listBar2);
            Console.WriteLine("Task3 complete");
        }

        static Bar ConvertToObj(string[] str)
        {
            return new Bar
            {
                Symbol = str[0],
                Description = str[1],
                Date = DateTime.ParseExact($"{str[2]} {str[3]}", "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                Open = float.Parse(str[4], CultureInfo.InvariantCulture),
                High = float.Parse(str[5], CultureInfo.InvariantCulture),
                Low = float.Parse(str[6], CultureInfo.InvariantCulture),
                Close = float.Parse(str[7], CultureInfo.InvariantCulture),
                TotalVolume = Int32.Parse(str[8])
            };
        }

        static string[] ConvertToArrayStr(IEnumerable<Bar> listBar)
        {
            List<string> resultStr = new List<string>();
            resultStr.Add($"\"Symbol\",\"Description\",\"Date\",\"Time\",\"Open\",\"High\",\"Low\",\"Close\",\"TotalVolume\"");

            foreach (var item in listBar)
            {
                resultStr.Add(String.Join(",", item));
            }

            return resultStr.ToArray();
        }

        static void Task1(List<Bar> listBar)
        {
            IEnumerable<Bar> highByDayList = listBar.OrderByDescending(x => x.High)
                                                    .GroupBy(x => new { x.Date.Day, x.Date.Month, x.Date.Year })
                                                    .Select(x => x.First());

            IEnumerable<Bar> lowByDayList = listBar.OrderBy(x => x.Low)
                                                   .GroupBy(x => new { x.Date.Day, x.Date.Month, x.Date.Year })
                                                   .Select(x => x.First());

            IEnumerable<Bar> resultList = highByDayList.Union(lowByDayList).OrderBy(x => x.Date).ThenBy(x => x.Low);

            File.WriteAllLines("Task1.txt", ConvertToArrayStr(resultList));
        }

        static void Task2(List<Bar> listBar)
        {
            List<Bar> resultList = new List<Bar>();
            var groupByHourList = listBar.GroupBy(x => new { x.Date.Day, x.Date.Month, x.Date.Year, x.Date.Hour });

            foreach (var item in groupByHourList)
            {
                Bar firstBar = item.First();

                resultList.Add(new Bar
                {
                    Symbol = firstBar.Symbol,
                    Description = firstBar.Description,
                    Date = new DateTime(firstBar.Date.Year, firstBar.Date.Month, firstBar.Date.Day, firstBar.Date.Hour, 0, 0),
                    Open = item.OrderBy(x => x.Date.Minute).First().Open,
                    High = item.Max(x => x.High),
                    Low = item.Min(x => x.Low),
                    Close = item.OrderByDescending(x => x.Date.Minute).First().Close,
                    TotalVolume = item.Sum(x => x.TotalVolume)
                });
            }

            File.WriteAllLines("Task2.txt", ConvertToArrayStr(resultList));
        }

        static void Task3(List<Bar> oldListBar, List<Bar> newListBar)
        {
            IEnumerable<Bar> exceptNew = newListBar.Except(oldListBar, new BarComparer());

            IEnumerable<Bar> exceptOld = oldListBar.Except(newListBar, new BarComparer());

            IEnumerable<Bar> unionList = newListBar.Union(oldListBar).Distinct(new BarComparer());
            IEnumerable<Bar> intersectList = newListBar.Intersect(oldListBar, new BarComparer());
            IEnumerable<Bar> uniqList = unionList.Except(intersectList, new BarComparer()).OrderBy(x => x.Date);

            File.WriteAllLines("Task3_1.txt", ConvertToArrayStr(exceptNew));
            File.WriteAllLines("Task3_2.txt", ConvertToArrayStr(exceptOld));
            File.WriteAllLines("Task3_3.txt", ConvertToArrayStr(uniqList));
        }
    }
}
