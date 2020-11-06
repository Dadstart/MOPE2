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
