using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telegram.Bot;
using Telegram.Bot.Args;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Device.Location;
using XLabs.Platform.Services.Geolocation;
using StackExchange.Redis;
using GoogleMaps.LocationServices;
using Google.Cloud.Translation.V2;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace basicBot
{
    class Program
    {
        private static readonly ITelegramBotClient bot = new TelegramBotClient("744384290:AAGRosbDuV-eHs-H1g9omffGeC8wUSaxmsU");
       


        static void Main(string[] args)
        {

            bot.OnMessage += bot_Sporocilo;
            bot.StartReceiving();
            Console.ReadLine();
            bot.StopReceiving();

        }

        private static string VrniPrevod(string beseda)
        {
            var credential = GoogleCredential.FromFile(@"C:\Users\Tomi\source\repos\basicBot\basicBot\obj\My First Project-0cb7ec634541.json");
            var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            TranslationClient client = TranslationClient.Create(credential);
            TranslationResult result = client.TranslateText(beseda, LanguageCodes.Slovenian);
            return result.TranslatedText.ToString();
        }

        private static void bot_Sporocilo(object sender, MessageEventArgs e)
        {

            if (e.Message.Text=="Pozdravljeni!" || e.Message.Text=="Zdravo") 
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id, "Pozdravljen" + e.Message.Chat.FirstName);
            }
            if (e.Message.Text.Contains("vreme") && e.Message.Text.Contains("danes"))
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id, "Kje pa?");   
            }
            if (e.Message.Text.Contains("V kraju"))
            {
                string input = e.Message.Text;
                string lastWord = input.Split(' ').Last();
                bot.SendTextMessageAsync(e.Message.Chat.Id, Vrni_Temparaturo(lastWord));
            }
        }


        private static string Vrni_Temparaturo(string city)
        {

            string results = "";

            using (WebClient wc = new WebClient())
            {
                results = wc.DownloadString("https://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=0a84b501e8af8c3a8a78b8011694ae5c");
                dynamic jo = JObject.Parse(results);
                var id = jo.id;
                var temperatura = jo.main.temp - 273.15;
                string main = VrniPrevod((string)jo.weather[0].description.ToString());

                return $"Na/V {city} je {temperatura:##.##} stopnij celzij in je {main}";
            }

        }
    }
}
