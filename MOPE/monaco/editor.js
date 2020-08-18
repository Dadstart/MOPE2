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

function onKeyUp(e) {
    // handle save
    if (e.ctrlKey && e.code === 's' && isDirty)
        postFile();
}

let isDirty = false;

function onContentChanged(e) {
    if (!isDirty) {
        isDirty = true;
        var request = new Request(getFetchUri("dirty/" + getPartUri()), {
            method: "POST"
        });

        fetch(request).catch(reason => window.alert("Error setting dirty state: " + reason));
    }
}

function loadEditor() {
    const urlParams = new URLSearchParams(document.location.search);
    updateTheme(urlParams.get("theme") === "dark");

    document.addEventListener("keyup", onKeyUp);

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
                    options: {
                        scrollbar: {
                            horizontal: "visible",
                            vertical: "visible"
                        }
                    }
                });

                monaco.editor.getModels()[0].onDidChangeContent(onContentChanged);

            });

        }).catch(error => {
            console.log(error);
            window.alert("Error fetching " + partUri);
        });
}

function postFile() {
    if (!isDirty)
        return;

    fetch(getFetchUri("post/" + getPartUri()), {
        body: monaco.editor.getModels()[0].getValue(),
        method: "POST"
    }).then(() => {
        isDirty = false;
    }).catch((reason) => {
        window.alert("Error saving: " + reason);
    });
}

