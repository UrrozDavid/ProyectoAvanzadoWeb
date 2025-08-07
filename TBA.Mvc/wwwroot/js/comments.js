document.addEventListener('DOMContentLoaded', () => {

    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenInput ? tokenInput.value : '';

    document.querySelectorAll('.card-comments').forEach(container => {
        const cardId = container.dataset.cardId;
        const btnToggle = container.querySelector('.toggle-comments');
        const listEl = container.querySelector('.comments-list');
        const addForm = container.querySelector('.add-comment');
        const textarea = container.querySelector('.new-comment');
        const addBtn = container.querySelector('.add-comment-btn');

        btnToggle.addEventListener('click', async () => {
            listEl.classList.toggle('d-none');
            addForm.classList.toggle('d-none');
            if (!listEl.dataset.loaded) {
                try {
                    const res = await fetch(`/Tasks/GetComments?cardId=${cardId}`);
                    if (!res.ok) throw new Error(await res.text());
                    const comments = await res.json();
                    listEl.innerHTML = comments.map(c => `
            <div class="list-group-item">
              <small class="text-muted">
                ${new Date(c.createdAt).toLocaleString()}
              </small>
              <p class="mb-1">
                <strong>${c.createdBy}:</strong> ${c.commentText}
              </p>
            </div>
          `).join('');
                    listEl.dataset.loaded = 'true';
                } catch (err) {
                    console.error(err);
                    alert('Error cargando comentarios: ' + err.message);
                }
            }
        });

        // 2) Añadir un comentario nuevo
        addBtn.addEventListener('click', async () => {
            const text = textarea.value.trim();
            if (!text) return;

            const payload = {
                cardId: +cardId,
                commentText: text
            };

            try {
                const res = await fetch('/Tasks/AddComment', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(payload)
                });
                if (!res.ok) {
                    const msg = await res.text();
                    throw new Error(msg);
                }
                // reset y recarga
                textarea.value = '';
                listEl.dataset.loaded = '';
                btnToggle.click();
            } catch (err) {
                console.error(err);
                alert('Error guardando comentario: ' + err.message);
            }
        });
    });
});
