using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Rezervacija_Avio_Karata.Models
{
    public static class IdGenerator
    {
        private static readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Id.txt");
        private static int reservationCounter;
        private static int reviewCounter;
        private static int flightCounter;

        static IdGenerator()
        {
            InitializeCounters();
        }

        private static void InitializeCounters()
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "Reservation:0;Review:0;Flight:0");
                reservationCounter = 0;
                reviewCounter = 0;
                flightCounter = 0;
            }
            else
            {
                string content = File.ReadAllText(path);
                string[] counters = content.Split(';');
                foreach (var counter in counters)
                {
                    string[] parts = counter.Split(':');
                    switch (parts[0])
                    {
                        case "Reservation":
                            reservationCounter = int.Parse(parts[1]);
                            break;
                        case "Review":
                            reviewCounter = int.Parse(parts[1]);
                            break;
                        case "Flight":
                            flightCounter = int.Parse(parts[1]);
                            break;
                    }
                }
            }
        }

        private static void SaveCounters()
        {
            string content = $"Reservation:{reservationCounter};Review:{reviewCounter};Flight:{flightCounter}";
            File.WriteAllText(path, content);
        }

        public static int GenerateReservationId()
        {
            reservationCounter += 1;
            SaveCounters();
            return reservationCounter;
        }

        public static int GenerateReviewId()
        {
            reviewCounter += 1;
            SaveCounters();
            return reviewCounter;
        }

        public static int GenerateFlightId()
        {
            flightCounter += 1;
            SaveCounters();
            return flightCounter;
        }
    
}
}