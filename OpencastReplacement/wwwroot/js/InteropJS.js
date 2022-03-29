window.clipboardCopy = {
    copyText: function (text) {
        navigator.clipboard.writeText(text)
            .then(function () {
                //alert("Copied to clipboard!");
            })
            .catch(function (error) {
                alert(error);
            });
    },
    copyImage: function (data, type) {
        var data = [new ClipboardItem({ [type]: data })];
        navigator.clipboard.write(data)
            .then(function () {
                alert("copied to clipboard");
            })
            .catch(function (error) {
                alert(error);
            });
    },
    readTextFromClipboard: function (objRef) {
        navigator.permissions.query({ name: "clipboard-read" }).then((result) => {
            if (result.state == "granted" || result.state == "prompt") {
                navigator.clipboard.readText().then((text) => {
                    //return text;
                    objRef.invokeMethodAsync("ReceiveText", text);
                });
            };
        });
    },
    readDataFromClipboard: function () {
        navigator.permissions.query({ name: "clipboard-read" }).then((result) => {
            if (result.state == "granted" || result.state == "prompt") {
                navigator.clipboard.read().then((data) => {
                    if (!data[0].types.includes("image/png")) {
                        alert("Keine passende Bilddatei gefunden. Kann nicht darauf zugreifen");
                    } else {
                        data[0].getType("image/png").then((blob) => {
                            var imgUrl = URL.createObjectURL(blob);
                        });
                    };
                });
            };
        });
    },
};