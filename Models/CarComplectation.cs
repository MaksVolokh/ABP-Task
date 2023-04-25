﻿
namespace ABP_Task.Models
{
    internal class CarComplectation
    {
        public string Complectation { get; set; }
        public string Date { get; set; }
        public string Engine1 { get; set; }
        public string Body { get; set; }
        public string Grade { get; set; }
        public string Transmission { get; set; }
        public string GearShiftType { get; set; }
        public string DriversPosition { get; set; }
        public string NumberOfDoors { get; set; }
        public string Destination1 { get; set; }
        public string Destination2 { get; set; }

        public override string ToString()
        {
            return $"|{Complectation,13}|{Date,17}|{Engine1,7}|{Body,4}|{Grade,5}|{Transmission,7}|{GearShiftType,14}|" +
                $"{DriversPosition,15}|{NumberOfDoors,13}|{Destination1,12}|{Destination2,12}|";
        }
    }
}
