using ABP_Task.Models;
using ABP_Task;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp;
using System.Net;

namespace ABP_Task
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // to be possible to see Cyrillic language in console.
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IConfiguration config = Configuration.Default.WithDefaultLoader();
            IBrowsingContext context = BrowsingContext.New(config);
            string url = "https://www.ilcats.ru/";
            string toyota = "toyota";

            // Step 1:
            string baseAddress = url + toyota + "/?function=getModels&market=EU";
            await GetCarModels(context, baseAddress);

            // Step 2:
            string model = "281220";
            string complectationAddress = url + toyota + $"/?function=getComplectations&market=EU&model={model}&startDate=198210&endDate=198610";
            await GetModelComplectation(context, complectationAddress);

            // Step 3:
            // To be provided later. 

            // Step 4:
            // To be provided later.

            // Step 5:
            string schemeAddress = url + toyota + $"/?function=getParts&market=EU&model={model}&modification=CV10L-UEMEXW&complectation=001&group=3&subgroup=5202";
            await SavePicture(context, schemeAddress);

            ShowCars(toyota);
            ShowComplectation(toyota, model);
        }

        static async Task GetCarModels(IBrowsingContext context, string url)
        {
            IDocument document = await context.OpenAsync(url);

            // Getting numbers of cars.
            string querySelector = "div.List.Multilist";
            IElement? carDocument = document.QuerySelector(querySelector);

            int carsCount;
            try
            {
                carsCount = carDocument.ChildElementCount;
            }
            catch (Exception)
            {
                throw new Exception("Unable to get cars. Please check that web site is working.");
            }

            // Filling up car models props. 
            for (int i = 1; i <= carsCount; i++)
            {
                string nameSelector = $"div.List.Multilist > div:nth-child({i}) > div.Header > div.name";
                IElement? name = document.QuerySelector(nameSelector);
                string codeSelector = $"div.List.Multilist > div:nth-child({i}) > div.List > div.List > div.id";
                IElement? code = document.QuerySelector(codeSelector);
                string dateRangeSelector = $"div.List.Multilist > div:nth-child({i}) > div.List > div.List > div.dateRange";
                IElement? dateRange = document.QuerySelector(dateRangeSelector);
                string complectationSelector = $"div.List.Multilist > div:nth-child({i}) > div.List > div.List > div.modelCode";
                IElement? complectation = document.QuerySelector(complectationSelector);

                FakeDb.Cars.Add(new CarModel
                {
                    Name = name?.TextContent ?? "",
                    Code = code?.TextContent ?? "",
                    DateRange = dateRange?.TextContent ?? "",
                    Complectation = complectation?.TextContent ?? "",
                });
            }
        }

        static async Task GetModelComplectation(IBrowsingContext context, string url)
        {
            IDocument document = await context.OpenAsync(url);

            // Getting number of table length. 
            string tableSelector = "tbody";
            IElement? rowsSelector = document.QuerySelector(tableSelector);
            int rowsNumber;
            try
            {
                rowsNumber = rowsSelector.ChildElementCount;
            }
            catch (Exception)
            {
                throw new Exception("Unable to table rows. Please check that web site is working.");
            }

            // Filling up cars comlectations. 
            // starting from 2, due to 1 is a header.
            for (int i = 2; i <= rowsNumber; i++)
            {
                IElement? cell1 = document.QuerySelector($"tbody > tr:nth-child({i}) > td:nth-child(1)");
                IElement? cell2 = document.QuerySelector($"tbody > tr:nth-child({i}) > td:nth-child(2)");
                IElement? cell3 = document.QuerySelector($"tbody > tr:nth-child({i}) > td:nth-child(3)");
                IElement? cell4 = document.QuerySelector($"tbody > tr:nth-child({i}) > td:nth-child(4)");
                IElement? cell5 = document.QuerySelector($"tbody > tr:nth-child({i}) > td:nth-child(5)");
                IElement? cell6 = document.QuerySelector($"tbody > tr:nth-child({i}) > td:nth-child(6)");
                IElement? cell7 = document.QuerySelector($"tbody > tr:nth-child({i}) > td:nth-child(7)");
                IElement? cell8 = document.QuerySelector($"tbody > tr:nth-child({i}) > td:nth-child(8)");
                IElement? cell9 = document.QuerySelector($"tbody > tr:nth-child({i}) > td:nth-child(9)");
                IElement? cell10 = document.QuerySelector($"tbody > tr:nth-child({i}) > td:nth-child(10)");
                IElement? cell11 = document.QuerySelector($"tbody > tr:nth-child({i}) > td:nth-child(11)");

                FakeDb.CarComplectations.Add(new CarComplectation
                {
                    Complectation = cell1?.TextContent ?? "",
                    Date = cell2?.TextContent ?? "",
                    Engine1 = cell3?.TextContent ?? "",
                    Body = cell4?.TextContent ?? "",
                    Grade = cell5?.TextContent ?? "",
                    Transmission = cell6?.TextContent ?? "",
                    GearShiftType = cell7?.TextContent ?? "",
                    DriversPosition = cell8?.TextContent ?? "",
                    NumberOfDoors = cell9?.TextContent ?? "",
                    Destination1 = cell10?.TextContent ?? "",
                    Destination2 = cell11?.TextContent ?? "",
                });
            }
        }

        static async Task SavePicture(IBrowsingContext context, string url)
        {
            IDocument document = await context.OpenAsync(url);

            string imageSelector = "div.ImageArea > div > img";

            IHtmlImageElement? image = document.QuerySelector<IHtmlImageElement>(imageSelector);
            string source = image?.Source;

            if (source != null)
            {
                // getting unique value from the picture
                var hash = source?.Split("hash=")[1];

                // creating Image folder in the root
                string imagesFolderPath = "../../../Images";
                Directory.CreateDirectory(imagesFolderPath);

                // save image
                WebClient webClient = new WebClient();
                webClient.DownloadFile(source, $"{imagesFolderPath}/Photo-{hash}.jpg");
            }
            else
            {
                Console.WriteLine("Picture not found!");
            }
        }

        static void ShowCars(string mark)
        {
            Console.WriteLine($"***{mark.ToUpper()} models***");
            foreach (var car in FakeDb.Cars)
            {
                Console.WriteLine(car);
            }

            // add blank line.
            Console.WriteLine();
        }

        static void ShowComplectation(string mark, string model)
        {
            Console.WriteLine($"*** Complectation of {mark.ToUpper()}, model: {model} ***");

            Console.WriteLine($"|{nameof(CarComplectation.Complectation),13}|{nameof(CarComplectation.Date),17}|{nameof(CarComplectation.Engine1),7}|{nameof(CarComplectation.Body),4}|{nameof(CarComplectation.Grade),5}|" +
                $"ATM,MTM|{nameof(CarComplectation.GearShiftType),14}|{nameof(CarComplectation.DriversPosition),15}|{nameof(CarComplectation.NumberOfDoors),13}|" +
                $"{nameof(CarComplectation.Destination1),12}|{nameof(CarComplectation.Destination2),12}|");

            foreach (var complectation in FakeDb.CarComplectations)
            {
                Console.WriteLine(complectation);
            }

            // add blank line.
            Console.WriteLine();
        }
    }
}