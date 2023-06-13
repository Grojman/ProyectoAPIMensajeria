window.addEventListener("load", () => {
    let client = new WebSocket(url);
    document.querySelector("button").addEventListener("click", (event) => {
        //COMPROBAR QUE LOS CAMPOS NO ESTÁN VACÍOS
        document.querySelector("#name").value && document.querySelector("#pass").value ? client.send(`${event.target.id}/${document.querySelector("#name").value}/${document.querySelector("#pass").value}`) : alert("Los campos no pueden estar vacíos")
        event.preventDefault()
    })
    client.onmessage = (event) => {
        let data = JSON.parse(event.data)
        switch (data.Status) {
            case 0:
                window.location.href = `mainPage/main.html?Id=${data.Id}&Nickname=${document.querySelector("#name").value}`
                break
            case 1:
                let error = document.querySelector("#error") 
                error.textContent = data.ErrorMessage
                error.className = "show"
                break
        }
    }
})