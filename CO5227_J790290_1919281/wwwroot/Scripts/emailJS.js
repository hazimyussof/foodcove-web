function sendMail(params) {
    var tempParams = {
        from_name: document.getElementById("fromName").value,
        message: document.getElementById("msg").value,
    };

    emailjs.send('f00dc0ve', 'template_merhpki', tempParams)
        .then(function (res) {
            console.log("success", res.status);
        })
}