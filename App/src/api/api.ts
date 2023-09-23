const baseUrl = "http://localhost:7071/api"

function sendBusNotice(busNotice: BusNotice) {
  var myHeaders = new Headers();
  myHeaders.append("Content-Type", "application/json");

  var raw = JSON.stringify(busNotice)

  var requestOptions = {
    method: 'POST',
    headers: myHeaders,
    body: raw
  };

  fetch(`${baseUrl}/busNotices`, requestOptions)
}

async function getBusNotices(): Promise<ProcessedNotice[]> {
  var requestOptions = {
    method: 'GET',
  };

  var raw = await fetch(`${baseUrl}/processedNotices`, requestOptions)
    .then(response => response.text())
  var notice = JSON.parse(raw)
  return notice
}

async function getBusNotice(id: number): Promise<ProcessedNotice> {
  var requestOptions = {
    method: 'GET',
  };

  var raw = await fetch(`${baseUrl}/processedNotices/${id}`, requestOptions)
    .then(response => response.text())
  var notice = JSON.parse(raw)
  return notice
}

