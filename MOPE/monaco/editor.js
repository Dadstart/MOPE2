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

function loadEditor() {
    const urlParams = new URLSearchParams(document.location.search);
    const partUri = urlParams.get("part");
    const fetchUri = "http://" + document.location.host + "/part/" + partUri;
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
