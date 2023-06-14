let chats = []
let Id
window.addEventListener("load", () => {
    let client = new WebSocket(url);
    document.querySelector("h1").textContent = `${urlParams.get('Nickname')} main page`
    Id = urlParams.get('Id')
    client.addEventListener("open", () => {client.send(`get-msgs/${Id}`)})
    client.onmessage = (event) => {
        let data = JSON.parse(event.data)
        console.log(data)
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
    chats.push(new Group(data.GroupName, data.Users.map(n => new User(n.UserId, n.Nickname), data.GroupId)))
    //CREAR UN ELEMENTO EN LA LISTA CON EL NUEVO GRUPO
    let grupo = document.createElement("li")
    grupo.id = data.GroupId
    grupo.textContent = data.GroupName
    grupo.addEventListener("click", (event) =>{
        showGroup(chats.find(n => n.Id == event.target.id))
    })
    document.querySelector("#grupos").appendChild(grupo)
}
function newMessage(data) {
    let mensaje = buildMessage(data.Sender, data.Message, data.Date)
    chats.find(n => n.Id == data.Destination).Messages.push(mensaje)
}

function showGroup(grupo) {
    let mensajes = document.querySelector("#mensajes")
    mensajes.innerHTML = ""
    grupo.Messages.forEach(n => mensajes.innerHTML+= n)
}
function buildMessage(sender, content, date) {
    //TEMPORAL
    let prueba = document.createElement("p")
    prueba.textContent = content
    return prueba
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
class User {
    Id
    Name
    constructor(id, name) {
        Id = id
        Name = name
    }
}