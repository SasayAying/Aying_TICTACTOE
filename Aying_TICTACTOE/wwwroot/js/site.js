document.querySelectorAll('.cell').forEach(cell => {
    cell.addEventListener('click', () => {
        const row = cell.getAttribute('data-row');
        const col = cell.getAttribute('data-col');
        fetch('/Index?handler=Move&row=${row}&col=${col}', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
        }).then(response => location.reload());
    });
});