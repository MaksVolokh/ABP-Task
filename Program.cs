using ABP_Task.Models;
using ABP_Task;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp;
using System.Net;
using System.Text.Json;

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
            string toyota = CarMake.Toyota;

            // Step 1:
            string baseAddress = url + toyota + "/?function=getModels&market=EU";
            await GetCarModelsAsync(context, baseAddress);

            // Step 2:
            string model = "281220";
            string complectationAddress = url + toyota + $"/?function=getComplectations&market=EU&model={model}&startDate=198210&endDate=198610";
            await GetModelComplectationAsync(context, complectationAddress);

            // Step 3:to be provided later
            string urlData = url + toyota + $"/?function=getGroups&market=EU&model={model}&modification=CV10LUEMEXW&complectation=001";
            await SavingDataOnTheFirstLevelAsync(context, urlData);

            // Step 4:to be provided later
            string urlDataSecondLevel = url + toyota + $"/?function=getSubGroups&market=EU&model={model}&modification=CV10L-UEMEXW&complectation=001&group=1";
            await SavingDataOnTheSecondLevelAsync(context, urlDataSecondLevel);

            // Step 5:
            string schemeAddress = url + toyota + $"/?function=getParts&market=EU&model={model}&modification=CV10L-UEMEXW&complectation=001&group=3&subgroup=5202";
            await SavePictureAsync(context, schemeAddress);

            ShowCars(toyota);
            ShowComplectation(toyota, model);
        }

        static async Task GetCarModelsAsync(IBrowsingContext context, string url)
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
                    CarMake = CarMake.Toyota,
                    Name = name?.TextContent ?? "",
                    Code = code?.TextContent ?? "",
                    DateRange = dateRange?.TextContent ?? "",
                    Complectation = complectation?.TextContent ?? "",
                });
            }
        }

        static async Task GetModelComplectationAsync(IBrowsingContext context, string url)
        {
            try
            {
                IDocument document = await context.OpenAsync(url);

                // Getting table rows. 
                string tableSelector = "tbody";
                IElement? rowsSelector = document.QuerySelector(tableSelector);

                // Check if table exists and has rows.
                if (rowsSelector == null || !rowsSelector.HasChildNodes)
                {
                    throw new Exception("Unable to find table rows. Please check that web site is working.");
                }

                // Loop through rows and extract data.
                foreach (var row in rowsSelector.Children.Skip(1)) // Skip header row.
                {
                    var complectation = new CarComplectation
                    {
                        Complectation = row.Children[0].TextContent,
                        Date = row.Children[1].TextContent,
                        Engine1 = row.Children[2].TextContent,
                        Body = row.Children[3].TextContent,
                        Grade = row.Children[4].TextContent,
                        Transmission = row.Children[5].TextContent,
                        GearShiftType = row.Children[6].TextContent,
                        DriversPosition = row.Children[7].TextContent,
                        NumberOfDoors = row.Children[8].TextContent,
                        Destination1 = row.Children[9].TextContent,
                        Destination2 = row.Children[10].TextContent
                    };

                    FakeDb.CarComplectations.Add(complectation);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while scraping the webpage.", ex);
            }
        }

        static async Task SavingDataOnTheFirstLevelAsync(IBrowsingContext context, string url)
        {
            try
            {
                IDocument document = await context.OpenAsync(url);

                
            }
            catch (WebException e)
            {
                throw new Exception($"Failed to download data: {e.Message}");
            }
            catch (JsonException e)
            {
                throw new Exception($"Failed to parse JSON: {e.Message}");
            }
        }

        static async Task SavingDataOnTheSecondLevelAsync(IBrowsingContext context, string url)
        {
            
        }



        static async Task SavePictureAsync(IBrowsingContext context, string url)
        {
            try
            {
                IDocument document = await context.OpenAsync(url);

                string imageSelector = "div.ImageArea > div > img";

                IHtmlImageElement? image = document.QuerySelector<IHtmlImageElement>(imageSelector);
                string? source = image?.Source;

                if (source != null)
                {
                    string hash = GetHashValue(source);
                    string imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                    Directory.CreateDirectory(imagesFolderPath);

                    using var client = new HttpClient();
                    var response = await client.GetAsync(source);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsByteArrayAsync();
                    string filePath = Path.Combine(imagesFolderPath, $"Photo-{hash}.jpg");
                    await File.WriteAllBytesAsync(filePath, content);
                }
                else
                {
                    Console.WriteLine("Picture not found!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while downloading the picture: {ex.Message}");
            }
        }

        static string GetHashValue(string source)
        {
            return source?.Split("hash=")[1] ?? Guid.NewGuid().ToString();
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