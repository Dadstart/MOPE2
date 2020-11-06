function getPartUri() {
    const urlParams = new URLSearchParams(document.location.search);
    return urlParams.get("part");
}

function onResize(e) {
    diffEditor.layout();
}

let diffEditor;

function loadDiffer() {
    const urlParams = new URLSearchParams(document.location.search);
    updateTheme(urlParams.get("theme") === "dark");

    window.addEventListener("resize", onResize);


    const partUri = getPartUri();
    const leftFetchUri = getFetchUri("left/" + partUri);
    const rightFetchUri = getFetchUri("right/" + partUri);
    console.log("fetching " + partUri);

    // fetch left part
    fetch(leftFetchUri)
        .then(response => {

            // handle fetch error
            if (!response.ok) {
                window.alert("Error fetching " + leftFetchUri);
                return;
            }

            // parse left response text
            response.text().then(text => {
                var leftModel = monaco.editor.createModel(text);

                // fetch right part
                fetch(rightFetchUri)
                    .then(response => {

                        // handle fetch error
                        if (!response.ok) {
                            window.alert("Error fetching " + rightFetchUri);
                            return;
                        }

                        // parse right response text
                        response.text().then(text => {
                            var rightModel = monaco.editor.createModel(text);

                            diffEditor = monaco.editor.createDiffEditor(document.getElementById("container"), {
                                // TODO: any options
                            });

                            diffEditor.setModel({
                                original: leftModel,
                                modified: rightModel
                            });

                        }); // end parse right response text
                    }); // end fetch right part
            }); // end parse left response text
        }).catch(error => {
            console.log(error);
            window.alert("Error fetching " + partUri);
        });
}
