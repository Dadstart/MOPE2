import * as monaco from "monaco-editor";

function getLanguageType(contentType: string | null): string {
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


function getFetchUri(path: string): string {
    return "http://" + document.location.host + "/" + path;
}

function getPartUri() {
    const urlParams = new URLSearchParams(document.location.search);
    return urlParams.get("part");
}

function updateTheme(darkMode: boolean) {
    console.log("updating theme to " + darkMode);
    if (darkMode) {
        document.body.style.background = "rgb(30,30,30)";
        monaco.editor.setTheme("vs-dark");
    } else {
        document.body.style.background = "rgb(255,255,255)";
        monaco.editor.setTheme("vs");
    }

}

function onKeyUp(e: KeyboardEvent) {
    // handle save
    if (e.ctrlKey && e.code === 's' && isDirty)
        postFile();
}

let isDirty = false;
let codeEditor: monaco.editor.IStandaloneCodeEditor;

function onContentChanged(e: monaco.editor.IModelContentChangedEvent) {
    if (!isDirty) {
        isDirty = true;
        var request = new Request(getFetchUri("dirty/" + getPartUri()), {
            method: "POST"
        });

        fetch(request).catch(reason => window.alert("Error setting dirty state: " + reason));
    }
}

async function fetchPart(part: string | null): Promise<Response> {
    if (part == null) {
        throw new Error("Missing part to fetch");
    }

    try {
        const fetchUri = getFetchUri("part/" + part);
        const response = await fetch(fetchUri);
        
        if (!response.ok)
           throw new Error("HTTP error fetching " + fetchUri + "\r\n" + response.status + " " + response.statusText);

        return response;
    } catch (error) {
        throw new Error(error);
    }
}

export function loadDiffer() {
    const urlParams = new URLSearchParams(document.location.search);
    updateTheme(urlParams.get("theme") === "dark");

    const leftPart = fetchPart(urlParams.get("leftPart"));
    const rightPart = fetchPart(urlParams.get("rightPart"));


}


export function loadEditor() {
    const urlParams = new URLSearchParams(document.location.search);
    updateTheme(urlParams.get("theme") === "dark");

    document.addEventListener("keyup", onKeyUp);

    const partUri = getPartUri();
    fetchPart(partUri)
        .then(response => {
            if (!response.ok) {
                window.alert("Error fetching " + partUri);
                return;
            }
            response.text().then(text => {
                const container = document.getElementById('container');
                if (container === null)
                    throw new Error("Editor container missing");

                codeEditor = monaco.editor.create(container, {
                    value: text,
                    language: getLanguageType(response.headers.get("Content-Type")),
                    automaticLayout: true,
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

export function postFile() {
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

export function setReadOnly(val: boolean) {
    isReadOnly = val;
    codeEditor.updateOptions({
        readOnly: val
    });
}
