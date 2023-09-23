class BusNotice {
    constructor(
        public BusId: string,
        public Driver: string,
        public Latitude: string,
        public Longitude: string,
        public UTCTimeStamp: string,
        public ImageContentType: string,
        public Base64Image: string,
    ) { }
}