class ProcessedNotice {
    constructor(
        public Id: number,
        public BusId: string,
        public Driver: string,
        public Address: Address,
        public Latitude: string,
        public Longitude: string,
        public LicensePlate: string,
        public UTCTimeStamp: string,
        public ImageContentType: string,
        public Base64Image: string,
        public Base64ImageCar: string,
        public Base64ImageLicensePlate: string
    ) { }
}