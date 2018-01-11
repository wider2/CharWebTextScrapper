using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string siteToScrape = "", inputsString = "", item = "";
            Boolean cflag = false, eflag = false, vflag = false, wflag = false;
            Stopwatch timer = new System.Diagnostics.Stopwatch();

            foreach (string arg in args)
            {
                if (arg == "-w")
                {
                    //Console.WriteLine("count");
                    wflag = true;
                }
                //Console.WriteLine(arg);
                if (arg == "-v") vflag = true;
                if (arg == "-c") cflag = true;
                if (arg == "-e") eflag = true;


                if (arg.IndexOf("www") > 0)
                {
                    siteToScrape = arg;
                }
                else
                {
                    inputsString = arg;
                }
            }
            if (siteToScrape != "")
            {
                if (!siteToScrape.Contains("http://")) siteToScrape = "http://" + siteToScrape;
            }
            Console.WriteLine("site = " + siteToScrape + "; words = " + inputsString + "\n");

            //Console.ReadLine(); //as pause



            if (siteToScrape == "")
            {
                Console.WriteLine("Error connection to site " + siteToScrape);
            }
            else
            {
                timer.Start();
                string scrapedText = TestScraper.scrapeIt(siteToScrape);
                timer.Stop();
                if (vflag == true)
                {
                    Console.WriteLine("\nTime spend on data scraping: {0}", timer.Elapsed);
                }



                timer.Reset();
                timer.Start();
                string cur_text = scrapedText.ToLower();
                char[] separator = new char[] { ',' };
                string[] words = inputsString.Split(separator);
                if (wflag == true)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>();
                    for (int i = 0; i < words.Length; i++)
                    {
                        item = words[i].ToLower().Trim();

                        MatchCollection matches = Regex.Matches(cur_text, item);
                        dictionary[item] = matches.Count;
                    }
                    var sortedDict = (from entry in dictionary orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
                    foreach (KeyValuePair<string, int> pair in sortedDict)
                    {
                        Console.WriteLine("Word: " + pair.Key + ": " + pair.Value + "");
                    }
                }



                if (cflag == true)
                {
                    int letters = 0, digits = 0;
                    foreach (Char c in scrapedText)
                    {
                        if (char.IsLetter(c))
                        {
                            letters += 1;
                        }
                        if (char.IsDigit(c))
                        {
                            digits += 1;
                        }
                    }
                    Console.WriteLine("\nFound chars = " + letters.ToString());
                    Console.WriteLine("Found digits = " + digits.ToString());
                    Console.WriteLine("");
                }




                if (eflag == true)
                {
                    var sentences = scrapedText.Split(new[] { ". " }, StringSplitOptions.RemoveEmptyEntries);
                    Console.WriteLine("Found sentences:\n");
                    for (int i = 0; i < words.Length; i++)
                    {
                        item = words[i].ToLower().Trim();

                        var wmatches = from sentence in sentences
                                       where sentence.ToLower().Trim().Contains(item.ToLower())
                                       select sentence;
                        //wmatches.ToList();                
                        foreach (var fs in wmatches)
                        {
                            Console.WriteLine("" + fs.ToString().Trim() + "\n");
                        }
                    }
                }
                if (vflag == true)
                {                  
                }
                timer.Stop();
            } //siteToScrape


            
            if (vflag == true)
            {
                Console.WriteLine("\nTime spend on data processing: {0}", timer.Elapsed);
            }
        }
    }
    public static class TestScraper
    {
        public static string scrapeIt(string siteToScrape)
        {
            string HTML = getHTML(siteToScrape);
            string text = stripCode(HTML);
            return text;
        }
        public static string getHTML(string siteToScrape)
        {
            string response = "";
            WebResponse objResponse;
            WebRequest objRequest = System.Net.HttpWebRequest.Create(siteToScrape);
            objResponse = objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream(), Encoding.UTF8))
            {
                response = sr.ReadToEnd();
                sr.Close();
            }
            return response;
        }

        public static string stripCode(string the_html)
        {
            the_html = Regex.Replace(the_html, "<script.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            the_html = Regex.Replace(the_html, "<style.*?</style>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            the_html = Regex.Replace(the_html, "</?[a-z][a-z0-9]*[^<>]*>", "");

            the_html = Regex.Replace(the_html, "<img(.| )*?/>", "");
            the_html = Regex.Replace(the_html, "(x09)?", "");
            the_html = Regex.Replace(the_html, "(x20){2,}", " ");
            the_html = Regex.Replace(the_html, "(x0Dx0A)+", " ");

            the_html = Regex.Replace(the_html, "<!--(.|\\s)*?-->", "");
            the_html = Regex.Replace(the_html, "<!(.|\\s)*?>", "");
            the_html = Regex.Replace(the_html, "[\t\r\n]", " ");

            return the_html;
        }
    }
}