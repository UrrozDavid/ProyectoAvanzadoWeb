const apiHost = "https://localhost:7084";

// Construir la conexión SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${apiHost}/hubs/notification`, { withCredentials: true })
    .withAutomaticReconnect() // Reconectar automáticamente si se cae la conexión
    .build();

// Escuchar notificaciones del servidor
connection.on("ReceiveNotification", message => {
    // Actualizar badge
    const badge = document.getElementById("notification-badge");
    let count = parseInt(badge.textContent, 10) || 0;
    count++;
    badge.textContent = count;

    // Actualizar dropdown de notificaciones
    const menu = document.querySelector("#notificationsToggle + .dropdown-menu");

    // Eliminar texto "No hay notificaciones" si existe
    const noNotif = menu.querySelector("li.px-3 small");
    if (noNotif && noNotif.textContent.includes("No hay notificaciones")) {
        noNotif.parentElement.remove();
    }

    // Crear nuevo elemento de notificación
    const li = document.createElement("li");
    li.className = "px-3";
    li.innerHTML = `<small>${message}</small>`;
    menu.prepend(li); // Agregar al inicio para que lo más reciente quede arriba
});

// Función para iniciar la conexión y reintentar en caso de error
async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR conectado");
    } catch (err) {
        console.error("Error de SignalR:", err);
        setTimeout(startConnection, 5000); // reintentar en 5s
    }
}

// Iniciar la conexión
startConnection();

// Opcional: manejar reconexiones
connection.onreconnecting(error => {
    console.warn("Reconectando a SignalR...", error);
});

connection.onreconnected(connectionId => {
    console.log("Reconectado a SignalR, connectionId:", connectionId);
});

connection.onclose(error => {
    console.error("Conexión SignalR cerrada", error);
    startConnection(); // reintentar al cerrar
});
