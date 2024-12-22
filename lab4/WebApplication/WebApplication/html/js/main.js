document.getElementById("sendBtn").addEventListener("click", send);
window.send = async function send() {
    const response = await fetch("/api/user", {
        method: "POST",
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
    document.getElementById("message").innerText = message.text;
}