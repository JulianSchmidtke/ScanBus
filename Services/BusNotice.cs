
using System;

namespace ScanBus.Function;
public class BusNotice
{

        public string Base64Image { get; set;}
        public string ImageContentType {get; set; }
        public string Latitude {get; set; }
        public string Longitude {get; set; }
        public string Driver {get; set; }
        public string BusId {get; set; }
        public DateTime UTCTimeStamp {get; set; }
}