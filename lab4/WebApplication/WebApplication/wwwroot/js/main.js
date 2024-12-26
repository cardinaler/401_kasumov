async function ExpCreate() {
    const response = await fetch("/experiments", {
        method: 'PUT',
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            CrossingShare: document.getElementById("crossingShare").value,
            TournamentsShare: document.getElementById("tournamentsShare").value,
            MutationShare: document.getElementById("mutationShare").value,
            IndividNums: document.getElementById("individNums").value,
            MatrixSize: document.getElementById("matrixSize").value
        })
    });
    const message = await response.json();
    document.getElementById("experimentId").value = `${message.expid}`;

    renderMatrix(message.matrix)

}
async function DoStep() {
    const experimentId = document.getElementById('experimentId').value;
    const response = await fetch(`/experiments/${experimentId}`, {
        method: 'POST'
    });
    if (response.ok) {
        const message = await response.json();
        document.getElementById("best").value = message.result;
        document.getElementById("epochNum").value = message.epoch;
    }
    else {
        const error = await response.text();
        document.getElementById('best').value = `Error: ${error}`;
        document.getElementById("epochNum").value = "Error";
    }  
};
async function StartOpt()
{
    const experimentId = document.getElementById('experimentId').value;

    if (!experimentId) {
        document.getElementById('best').value = `Error:`;
        return;
    }
    const response = await fetch(`/experiments/${experimentId}/start`, { method: 'POST' });
    if (responce.ok) {
        const eventSource = new EventSource(`/experiments/${experimentId}/stream`);
        eventSource.onmessage = (event) => {
            const data = JSON.parse(event.data);
            document.getElementById('best').value = `Best Fitness: ${data.best}`;
            document.getElementById('epochNum').value = `${data.epochs}`;
        };
        eventSource.onerror = () => {
            eventSource.close();
            document.getElementById('best').value = `Experiment ${experimentId} stopped or error occurred.`;
        };
    }
    else {
        const error = await response.text();
        document.getElementById('best').value = `Error: ${error}`;
    }
};

async function StopOpt()
{
    const experimentId = document.getElementById('experimentId').value;
    if (!experimentId) {
        document.getElementById('best').value = `Error:`;
        return;
    }
    const response = await fetch(`/experiments/${experimentId}/stop`, { method: 'POST' }); 
    const message = await response.json();
    if (response.ok) {
        document.getElementById("best").value = message.result;
        document.getElementById("epochNum").value = message.epoch;
    } else {
        const error = await response.text();
        document.getElementById('best').value = `Error: ${error}`;
    }
}

async function deleteExpe() {
    const experimentId = document.getElementById('experimentId').value;

    if (!experimentId) {
        alert("Please enter a valid Experiment ID.");
        return;
    }
    const response = await fetch(`/experiments/${experimentId}`, {
        method: 'DELETE',
    });
    if (response.ok) {
        document.getElementById('best').value = ` Эксперимент ${experimentId} удалён.`;
    } else {
        const error = await response.text();
        document.getElementById('best').value = `Error: ${error}`;
    }
    document.getElementById('epochNum').value = "";

};
function renderMatrix(matrixString) {
    try {
        matrix = JSON.parse(matrixString);

        if (!Array.isArray(matrix) || !matrix.every(row => Array.isArray(row))) {
            console.error("Преобразованные данные должны быть матрицей (массивом массивов).");
            return;
        }

        const container = document.getElementById("table-container");

        if (!container) {
            console.error("Не найден контейнер с id 'table-container'.");
            return;
        }

        const table = document.createElement("table");
        table.style.borderCollapse = "collapse";
        table.style.width = "100%";
        table.style.textAlign = "center";

        const headerRow = document.createElement("tr");
        const emptyHeader = document.createElement("th");
        emptyHeader.textContent = "";
        emptyHeader.style.backgroundColor = "#f0f0f0";
        emptyHeader.style.border = "1px solid black";
        headerRow.appendChild(emptyHeader);

        for (let i = 0; i < matrix.length; i++) {
            const th = document.createElement("th");
            th.textContent = i;
            th.style.backgroundColor = "#f0f0f0";
            th.style.border = "1px solid black";
            th.style.padding = "4px";
            headerRow.appendChild(th);
        }
        table.appendChild(headerRow);

        matrix.forEach((row, rowIndex) => {
            const tr = document.createElement("tr");

            const th = document.createElement("th");
            th.textContent = rowIndex;
            th.style.backgroundColor = "#f0f0f0";
            th.style.border = "1px solid black";
            th.style.padding = "4px";
            tr.appendChild(th);

            row.forEach(cell => {
                const td = document.createElement("td");
                td.textContent = cell;
                td.style.border = "1px solid black";
                td.style.padding = "4px";
                tr.appendChild(td);
            });

            table.appendChild(tr);
        });

        container.innerHTML = "";
        container.appendChild(table);

    } catch (error) {
        console.error("Error:", error);
    }
}
