const API_BASE = 'https://localhost:7084/api/checklist';

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.card-checklist').forEach(container => {
        const cardId = container.dataset.cardId;
        const btnToggle = container.querySelector('.toggle-checklist');
        const listEl = container.querySelector('.checklist-items');
        const addForm = container.querySelector('.add-checklist-item');
        const inputNew = container.querySelector('.new-check-item');
        const addBtn = container.querySelector('.add-item-btn');

        btnToggle.addEventListener('click', async () => {
            listEl.classList.toggle('d-none');
            addForm.classList.toggle('d-none');
            if (!listEl.dataset.loaded) {
                const res = await fetch(`${API_BASE}/${cardId}`);
                if (!res.ok) return; // Manejo de error
                const items = await res.json();
                listEl.innerHTML = items
                    .map(i => `
            <li data-id="${i.checklistItemId}">
              <input type="checkbox" ${i.isDone ? 'checked' : ''}>
              <span class="${i.isDone ? 'text-decoration-line-through' : ''}">${i.text}</span>
              <button class="btn btn-sm btn-link delete-item">✕</button>
            </li>`)
                    .join('');
                listEl.dataset.loaded = true;
            }
        });

        listEl.addEventListener('change', async e => {
            if (e.target.type === 'checkbox') {
                const li = e.target.closest('li');
                const id = li.dataset.id;
                const done = e.target.checked;
                await fetch(`${API_BASE}/${id}/toggle?isDone=${done}`, { method: 'PUT' });
                li.querySelector('span')
                    .classList.toggle('text-decoration-line-through', done);
            }
        });

        listEl.addEventListener('click', async e => {
            if (e.target.classList.contains('delete-item')) {
                const li = e.target.closest('li');
                const id = li.dataset.id;
                await fetch(`${API_BASE}/${id}`, { method: 'DELETE' });
                li.remove();
            }
        });

        addBtn.addEventListener('click', async () => {
            const text = inputNew.value.trim();
            if (!text) return;
            const payload = { cardId: +cardId, text, position: listEl.children.length };
            const res = await fetch(API_BASE, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });
            if (!res.ok) return; // Manejo de error
            const newItem = await res.json();
            const li = document.createElement('li');
            li.dataset.id = newItem.checklistItemId;
            li.innerHTML = `
        <input type="checkbox">
        <span>${newItem.text}</span>
        <button class="btn btn-sm btn-link delete-item">✕</button>`;
            listEl.appendChild(li);
            inputNew.value = '';
        });
    });
});
