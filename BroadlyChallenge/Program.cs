using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace BroadlyChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            string urlClasses = "http://challenge.broadly.com/classes";

            // Load list of classes
            ClassList classList = new ClassList();

            classList = _download_serialized_json_data<ClassList>(urlClasses);

            Console.WriteLine("ClassList Loaded OK? {0}", classList.note);
            Console.WriteLine("List of URLs:");
            foreach (string urlClass in classList.classes) {

                Console.WriteLine(urlClass);
            }

            Console.WriteLine();

            // for debugging
            //Console.ReadLine();

            // Parse each class from the list
            Session session = new Session();
            string nextSession = "";
            int countOver25 = 0;
            if (classList.classes != null)
            {
                foreach (string classToParse in classList.classes)
                {
                    session = _download_serialized_json_data<Session>(classToParse);

                    //Parse students
                    if (session != null)
                    {
                        countOver25 = 0;
                        Console.WriteLine("Room # {0}", session.room);
                        foreach (Student studentToParse in session.students)
                        {
                            Console.WriteLine("Student: {0}, Age: {1}, ID: {2}", studentToParse.name,studentToParse.age,studentToParse.id);
                            if (studentToParse.age > 25) countOver25++;
                        }
                        Console.WriteLine("Total students in this session: {0}",session.students.Count().ToString());
                        Console.WriteLine("Students over 25 years old: {0}", countOver25.ToString());
                        int totalStudents = session.students.Count();
                        double avgOver25 = ((double)countOver25/(double)totalStudents) * 100.0;
                        Console.WriteLine("Average attendance over 25 years old: {0}%", avgOver25.ToString("F2"));
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                Console.WriteLine("Error reading {0}.  Processing aborted.", urlClasses);
            }

            Console.WriteLine("Press Enter to Finish.");
            Console.ReadLine();
        }

        private static T _download_serialized_json_data<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception) { }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
        }
    }
}
