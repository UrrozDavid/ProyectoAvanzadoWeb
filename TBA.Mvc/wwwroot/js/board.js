// wwwroot/js/board.js

let draggedCard = null;

function handleDragStart(e) {
    draggedCard = e.currentTarget;
    e.currentTarget.classList.add('dragging');
    e.dataTransfer.setData('text/plain', draggedCard.dataset.cardId);
}

function handleDragEnd(e) {
    e.currentTarget.classList.remove('dragging');
    draggedCard = null;
}

function handleDrop(e) {
    // 'this' viene del ondrop en el HTML y contiene dataset.listId
    e.preventDefault();
    const newListIdStr = this.dataset.listId;
    console.log("🔵 raw newListId: " + newListIdStr);

    const cardIdStr = e.dataTransfer.getData('text/plain');
    const cardId = parseInt(cardIdStr, 10);
    const newListId = parseInt(newListIdStr, 10);

    if (isNaN(cardId) || isNaN(newListId)) {
        console.error("IDs inválidos:", cardIdStr, newListIdStr);
        return;
    }

    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenInput ? tokenInput.value : "";

    fetch('/Tasks/UpdateStatus', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify({ CardId: cardId, NewListId: newListId })
    })
        .then(function (resp) {
            if (!resp.ok)
                return resp.text().then(function (txt) { throw new Error(txt); });
            console.log("✅ Actualización exitosa");

            const cardElem = document.querySelector("[data-card-id='" + cardId + "']");
            const newZone = this.querySelector('.list-cards');
            if (cardElem && newZone) {
                newZone.appendChild(cardElem);
                updateTaskCount(this);
                const oldZone = cardElem.closest('.dropzone');
                if (oldZone && oldZone !== this)
                    updateTaskCount(oldZone);
            }
        }.bind(this))
        .catch(function (err) {
            console.error("❌ Error al actualizar:", err);
            alert("Error actualizando tarjeta: " + err.message);
        });
}

function updateTaskCount(zone) {
    if (!zone) return;
    const count = zone.querySelectorAll('.task-card').length;
    const span = zone.querySelector('h5 .task-count');
    if (span) {
        span.textContent = "(" + count + ")";
    } else {
        console.warn("No se encontró .task-count en", zone);
    }
}


document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.task-card').forEach(card => {
        card.addEventListener('dragstart', handleDragStart);
        card.addEventListener('dragend', handleDragEnd);
    });
    document.querySelectorAll('.dropzone').forEach(zone => {
        zone.addEventListener('dragover', e => e.preventDefault());
        zone.addEventListener('drop', handleDrop);
    });
});
