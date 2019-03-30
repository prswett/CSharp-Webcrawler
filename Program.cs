using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace testingstuff
{
    class Program
    {
        static void Main(string[] args)
        {
            //to few arguments
            if(args.Length < 2)
            {
                Console.WriteLine("Sorry you have not entered enough arguments");
                return;
            }

            //to many arguments
            if(args.Length > 2)
            {
                Console.WriteLine("Sorry you have entered too many arguments");
                return;
            }

            //if right amoung of arguments
            if (args.Length == 2)
            {
                Console.WriteLine("Processing arguments please wait...");
                //to catch 400-500 lv errors
                try {
                    using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                    {
                        //validate that the website passed in is a real website
                        Uri uriResult;
                        bool validate = Uri.TryCreate(args[0], UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                        //if its not a valid website
                        if (validate == false)
                        {
                            Console.WriteLine("Sorry the URL you have entered is not a valid URL");
                            return;
                        }
                        //if it is valid
                        else
                        {
                            //get website html to be parsed
                            client.BaseAddress = new Uri(args[0]);
                            HttpResponseMessage response = client.GetAsync(args[0]).Result;
                            response.EnsureSuccessStatusCode();
                            Byte[] byteArray = response.Content.ReadAsByteArrayAsync().Result;
                            string result = System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

                            int x = 0;
                            //make sure number of hops passed in is a valid integer
                            if (Int32.TryParse(args[1], out x))
                            {
                                crawl(result, x, args[0]);
                            }
                            else
                            {
                                Console.WriteLine("Sorry the number of hops you have requested is not a valid number");
                                return;
                            }
                        }
                    }

                }
                //custom message for 400-500lv exception
                catch(AggregateException e)
                {
                    string fixCompilerError = e.ToString();
                    Console.WriteLine("The website requested has returned a 400 - 500 level error. Exiting Program Now - Please try again");
                }
                    
                }
               
        }

        public static void crawl(string toParse, int numberOfHops, string currentURI)
        {
            //local variables
            string currentParse = toParse;
            string myCurrentURI = currentURI;
            bool breakFirstLoop = false;
            bool breakSecondLoop = false;
            List<string> accessedWebsites = new List<string>();
            accessedWebsites.Add(currentURI);

            //case if times to hop is 0
            if (numberOfHops == 0)
            {
                Console.WriteLine("This is the last URI - You have chosen 0 hops " + currentURI);
                Console.WriteLine("This is the HTML of the last site");
                Console.WriteLine("/////////////////////////////////////////////////////////");
                Console.WriteLine(currentParse);
                return;
            }

            //if number of hops is a negative number
            if (numberOfHops < 0)
            {
                Console.WriteLine("Sorry the number of hops you have requested cannot be less then 0");
                return;
            }
            //to catch for 400-500 lv erros
            try
            {
                //loop as many times as there are hops
                for (int i = 0; i < numberOfHops; i++)
                {
                    breakFirstLoop = false;
                    breakSecondLoop = false;
                    //parse for all absolute links
                    MatchCollection validWebsites = Regex.Matches(currentParse, @"<(a).*?href=([\""'])(.+?)([\""']).*?>", RegexOptions.Singleline);
                    foreach (Match link in validWebsites)
                    {
                        string temp = link.Groups[3].Value;
                        //parse again for all absolute links with http or https
                        MatchCollection httpLinks = Regex.Matches(temp, @"(http|https):\/\/([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?", RegexOptions.Singleline);
                        foreach (Match hlinks in httpLinks)
                        {
                            string properLink = hlinks.ToString();

                            //check if we've already visited that site
                            if (!(accessedWebsites.Contains(properLink)))
                            {
                                //if not then go to the newly aquired URI
                                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                                {
                                    client.BaseAddress = new Uri(properLink);
                                    HttpResponseMessage response = client.GetAsync(properLink).Result;
                                    response.EnsureSuccessStatusCode();
                                    Byte[] byteArray = response.Content.ReadAsByteArrayAsync().Result;
                                    string result = System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                                    currentParse = result;
                                    accessedWebsites.Add(properLink);
                                    myCurrentURI = properLink;
                                    breakFirstLoop = true;
                                    breakSecondLoop = true;
                                }
                            }
                            //we break out of these loops when we find a website
                            if (breakFirstLoop)
                            {
                                break;
                            }
                        }
                        //break out of these loops when we find a new website
                        if (breakSecondLoop)
                        {
                            break;
                        }
                    }
                }
            }
            //custom error message for 400-500 lv errors while hopping
            catch (AggregateException e)
            {
                string fixCompilerError = e.ToString();
                Console.WriteLine("while hopping the website requested has returned a 400 - 500 level error. Exiting Program Now - Please try again!");
            }
            //enter results to the console
            Console.WriteLine("This is the last URI in the hop: " + myCurrentURI);
            Console.WriteLine("This is the HTML of the last site");
            Console.WriteLine("/////////////////////////////////////////////////////////");
            Console.WriteLine(currentParse);
        }
    }
}
