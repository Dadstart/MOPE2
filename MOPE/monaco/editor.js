function getLanguageType(contentType) {
    switch (contentType) {
        case "application/javascript":
            return "javascript";
        case "application/json":
            return "json";
        case "application/xml":
            return "xml";
        case "text/css":
            return "css";
        case "text/html":
            return "html";
        case null:
            return "text";
    }

    // special case for xml with specialized content type
    if (contentType.endsWith("+xml"))
        return "xml";

    // default to text
    return "text";
}

function isDarkMode() {
    const urlParams = new URLSearchParams(document.location.search);
    return urlParams.get("theme") === "dark";
}


function getFetchUri(path) {
    return "http://" + document.location.host + "/" + path;
}

function getPartUri() {
    const urlParams = new URLSearchParams(document.location.search);
    return urlParams.get("part");
}

function updateTheme(darkMode) {
    console.log("updating theme to " + darkMode);
    if (darkMode) {
        document.body.style.background = "rgb(30,30,30)";
        monaco.editor.setTheme("vs-dark");
    } else {
        document.body.style.background = "rgb(255,255,255)";
        monaco.editor.setTheme("vs");
    }

}

function loadEditor() {
    //const darkMode = isDarkMode();

    //if (darkMode) {
    //    document.body.style.background = "rgb(30,30,30)";
    //}

    updateTheme(isDarkMode());

    const partUri = getPartUri();
    const fetchUri = getFetchUri("part/" + partUri);
    console.log("fetching " + partUri + " from " + fetchUri);
    fetch(fetchUri)
        .then(response => {
            if (!response.ok) {
                window.alert("Error fetching " + partUri);
                return;
            }
            response.text().then(text => {
                var editor = monaco.editor.create(document.getElementById('container'), {
                    value: text,
                    language: getLanguageType(response.headers.get("Content-Type")),
                    automaticLayout: true,
                    //theme: darkMode ? "vs-dark" : "vs",
                    options: {
                        codeLens: false,
                        scrollbar: {
                            horizontal: "visible",
                            vertical: "visible"
                        }
                    }
                });

            });

        }).catch(error => {
            console.log(error);
            window.alert("Error fetching " + partUri);
        });
}

function postFile() {
    var request = new Request(getFetchUri("post/" + getPartUri()), {
        body: editor.getModel().getValue(),
        method: "POST"
    });

    fetch(request).catch(reason => window.alert("Error saving: " + reason));
    request.body = editor.getModel().getValue();
}