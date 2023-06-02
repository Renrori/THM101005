var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build()

connection.start()
    .then(() => {
        console.log("connection");
    })
    .catch((err) => {
        console.log(err.toString());
    });

connection.on("UpdContent", (connection) => {
    var li = document.createElement("li");
    li.setAttribute("class", "list-group-item");
    li.textContent = connection;
    document.getElementById("connectingList").appendChild(li);
});

connection.on("UpdSelfID", (id) => {
    var selfID = document.getElementById("SelfID");
    selfID.innerHTML = id;
});

connection.on("SendtoAdmin", (msg, name) => {
    var li = document.createElement("li");
    li.setAttribute("class", "clearfix odd");
    var textdiv = document.createElement("div");
    textdiv.setAttribute("class", "coversation-text")
    var wrapdiv = document.createElement("div");
    wrapdiv.setAttribute("class", "ctext-wrap")
    var i = document.createElement("i")
    i.textContent = name;
    var p = document.createElement("p")
    p.textContent = msg;
    wrapdiv.appendChild(i).appendChild(p)
    textdiv.appendChild(wrapdiv);
    li.appendChild(textdiv);
    document.getElementById("messageList").appendChild(li);
});

document.getElementById("sendMsg").addEventListener("click", function (event) {
    var message = document.getElementById("msgforadmin").value;
    connection.invoke("SendMessage", message).catch((err) => {
        return console.log(err.toString())
    });
    document.getElementById("msgforadmin").value = "";
    event.preventDefault();
});