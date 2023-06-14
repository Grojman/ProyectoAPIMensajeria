let chats = []
let Id
window.addEventListener("load", () => {
    let client = new WebSocket(url);
    document.querySelector("h1").textContent = `${urlParams.get('Nickname')} main page`
    Id = urlParams.get('Id')
    client.send(`get-msgs/${Id}`)
    client.onmessage = (event) => {
        let data = JSON.parse(event.data)
        switch (data.Status) {
            case 2:
                client.send(`log-out/${Id}`)
                window.location.href = ".\index.html"
                break
            case 3:
                newGroup(data)
                break
            case 4:
                newMessage(data)
                break
        }
    }    
})
function newGroup(data) {
    //AÑADIR EL NUEVO GRUPO A LA LISTA DE GRUPOS
    chats.push(new Group(data.GroupName, data.Users, data.GroupId))
    //CREAR UN ELEMENTO EN LA LISTA CON EL NUEVO GRUPO
    let grupo = document.createElement("li")
    grupo.textContent = data.GroupName
    document.querySelector("#grupos").appendChild(grupo)
}
function newMessage(data) {
    //AÑADIR EL NUEVO MENSAJE A LOS MENSAJES DEL GRUPO CORRESPONDIENTE
    let mensaje = buildMessage()
    chats.find(n => n.Id == data.Destination).Messages.push(mensaje)
}

function showGroup() {
    //BORRAR LOS MENSAJES ANTERIORES
    //MOSTRAT DOSO LOS MENSAJES DEL GRUPO SELECCIONADO
}
function buildMessage(sender, content, date) {

}

class Group {
    Name
    Messages
    Users
    Id
    constructor(name, users, id) {
        Name = name
        Messages = []
        Users = users
        Id = id
    }
}