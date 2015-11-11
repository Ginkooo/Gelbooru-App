using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;

namespace GelbooruApp
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction,
              int uParam, string lpvParam, int fuWinIni);

        private static readonly int MAX_PATH = 260;
        private static readonly int SPI_GETDESKWALLPAPER = 0x73;
        private static readonly int SPI_SETDESKWALLPAPER = 0x14;
        private static readonly int SPIF_UPDATEINIFILE = 0x01;
        private static readonly int SPIF_SENDWININICHANGE = 0x02;

        static string GetDesktopWallpaper()
        {
            string wallpaper = new string('\0', MAX_PATH);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, (int)wallpaper.Length, wallpaper, 0);
            return wallpaper.Substring(0, wallpaper.IndexOf('\0'));
        }

        static void SetDesktopWallpaper(string filename)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filename,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }



        static void Main(string[] args)
        {


            var html = new HtmlDocument();

            var client = new WebClient();
            while (true)
            {
                string htmlCode;
                int idImageIndex;
                do
                {
                    htmlCode = client.DownloadString("http://gelbooru.com/index.php?page=post&s=random");
                    html.LoadHtml(htmlCode);
                    idImageIndex = htmlCode.IndexOf("id=\"image\"");
                } while (idImageIndex <= 0);

                int imgStartIndex = htmlCode.LastIndexOf("http", idImageIndex); ;
                int imgEndIndex = htmlCode.IndexOf("\"", imgStartIndex);
                string imgString = htmlCode.Substring(imgStartIndex, imgEndIndex - imgStartIndex);
                string pathObrazek = @"obrazek.jpg";
                client.DownloadFile(imgString, "obrazek.jpg");
                
                SetDesktopWallpaper(Path.GetFullPath(pathObrazek));

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
                System.Threading.Thread.Sleep(60 * 1000);
            }





            }
    }
}
