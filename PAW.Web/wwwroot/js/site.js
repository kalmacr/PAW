// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getExchangeRate() {
    const myHeaders = new Headers();
    myHeaders.append("UserAgent", "curl/8.7.1");
    myHeaders.append("Accept", "*/*");

    const requestOptions = {
        method: "GET",
        headers: myHeaders,
        redirect: "follow"
    };

    fetch("https://api.hacienda.go.cr/indicadores/tc", requestOptions)
        .then((response) => response.text())
        .then((result) => {
            return result.json();
        })
        .catch((error) => console.error(error));
}