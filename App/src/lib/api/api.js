const baseUrl = "http://localhost:7071/api"

function sendBusNotice(busNotice) {
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

async function getBusNotices() {
  var requestOptions = {
    method: 'GET',
  };

  var raw = await fetch(`${baseUrl}/processedNotices`, requestOptions)
    .then(response => response.text())
  var notice = JSON.parse(raw)
  return notice
}

async function getBusNotice(id) {
  var requestOptions = {
    method: 'GET',
  };

  var raw = await fetch(`${baseUrl}/processedNotices/${id}`, requestOptions)
    .then(response => response.text())
  var notice = JSON.parse(raw)
  return notice
}