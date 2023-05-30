window.addEventListener("load", () => {
    const client = new WebSocket("ws://localhost:14000/");
    client.onopen = () => console.log("Conectado!");
    client.onmessage = (event) => console.log(event.data)
})

