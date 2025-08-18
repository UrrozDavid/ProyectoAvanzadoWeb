const apiHost = "https://localhost:7084";

const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${apiHost}/hubs/notification`, { withCredentials: true })
    .withAutomaticReconnect() // Reconectar automáticamente si se cae la conexión
    .build();

connection.on("ReceiveNotification", message => {
    const badge = document.getElementById("notification-badge");
    let count = parseInt(badge.textContent, 10) || 0;
    count++;
    badge.textContent = count;

    const menu = document.querySelector("#notificationsToggle + .dropdown-menu");

    // Eliminar texto "No hay notificaciones" si está presente
    const noNotif = menu.querySelector("li.px-3 small");
    if (noNotif && noNotif.textContent.includes("No hay notificaciones")) {
        noNotif.parentElement.remove();
    }

    const li = document.createElement("li");
    li.className = "px-3";
    li.innerHTML = `<small>${message}</small>`;
    menu.prepend(li); // Lo agregamos al inicio para que la notificación más reciente quede arriba
});

connection.start()
    .catch(err => console.error("SignalR error:", err));
