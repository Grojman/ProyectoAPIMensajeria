window.addEventListener("load", () => {
    let client = new WebSocket(url);
    document.querySelector("button").addEventListener("click", (event) => {
        client.send(`${event.target.Id}/${document.querySelector("#name").Value}/${document.querySelector("#pass").Value}`)
    })
    client.onmessage = (event) => {
        let data = JSON.parse(event.data)
        switch (data.Status) {
            case 0:
                window.location.href = `mainPage/main.html?Id=${data.Id}`
                break
            case 1:
                let error = document.querySelector("#error") 
                error.textContent = data.ErrorMessage
                error.className = "show"
                break
        }
    }
})