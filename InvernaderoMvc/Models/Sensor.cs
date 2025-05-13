using System;

namespace ArcgisLoginApp.Models
{
    public class Sensor
    {
        public int id { get; set; }
        public int idnodo { get; set; }
        public double temperatue { get; set; }
        public double humidity { get; set; }
        public double nivel { get; set; }
        public double ph { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
