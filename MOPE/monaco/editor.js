function getPartUri() {
    const urlParams = new URLSearchParams(document.location.search);
    return urlParams.get("part");
}

function onKeyUp(e) {
    // handle save
    if (e.ctrlKey && e.code === 's' && isDirty)
        postFile();
}

let isDirty = false;
let codeEditor;

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
                codeEditor = monaco.editor.create(document.getElementById('container'), {
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

                // for some reason setting this option when calling monaco.editor.create isn't working
                isReadOnly = urlParams.get("readonly") === "true";
                setReadOnly(isReadOnly);

            });

        }).catch(error => {
            console.log(error);
            window.alert("Error fetching " + partUri);
        });
}

function postFile() {
    if (!isDirty || isReadOnly)
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

let isReadOnly = false;
function setReadOnly(val) {
    isReadOnly = val;
    codeEditor.updateOptions({ readOnly: val });
}