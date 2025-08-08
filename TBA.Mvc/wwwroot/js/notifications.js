const apiHost = "https://localhost:7084"; 

const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${apiHost}/hubs/notification`, { withCredentials: true })
    .build();

connection.on("ReceiveNotification", message => {
    const badge = document.getElementById("notification-badge");
    let count = parseInt(badge.textContent, 10) || 0;
    badge.textContent = ++count;

    const menu = document.querySelector(
        "#notificationsToggle + .dropdown-menu"
    );
    const li = document.createElement("li");
    li.className = "px-3";
    li.innerHTML = `<small>${message}</small>`;
    menu.appendChild(li);
});

connection.start()
    .catch(err => console.error("SignalR error:", err));
