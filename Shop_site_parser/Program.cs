using System;
using System.Collections.Generic;

using Shop_site_parser.Classes;
using System.IO;
using System.Text.Json;
using AngleSharp.Html.Parser;
using AngleSharp;
using System.Net.Http;
using System.Net;
using System.Text;
using AngleSharp.Html.Dom;
using System.Linq;
using Telegram.Bot;
using SQLite;

namespace Shop_site_parser
{
    class Program
    {
        static void Main(string[] args)
        {


            try
            {
                Settings cSettings = new Settings();

                AvitoShop ashhh = new AvitoShop(cSettings.getShopSettings("Avito"), "TestShopDB.db", cSettings.getBotToken());
                ashhh.ParseSite();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press eny key to exit");
            Console.ReadKey();

            Environment.Exit(0);
        }
    }
}
