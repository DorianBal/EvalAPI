// Fonction pour récupérer les données depuis l'API
async function fetchLeaderboard() {
    try {
        // Requête GET vers votre API
        const response = await fetch('http://localhost:5000/api/leaderboard');
        
        // Vérification de la réponse
        if (!response.ok) {
            throw new Error('Erreur lors de la récupération des données');
        }

        // Conversion des données en JSON
        const data = await response.json();

        // Affichage des données dans le tableau
        populateLeaderboard(data);
    } catch (error) {
        console.error('Erreur :', error.message);
    }
}

// Fonction pour injecter les données dans le tableau HTML
function populateLeaderboard(data) {
    const tbody = document.getElementById('leaderboard-body');
    tbody.innerHTML = ''; // Nettoyer le contenu précédent

    // Boucle pour insérer chaque utilisateur dans une ligne
    data.forEach((user, index) => {
        const row = document.createElement('tr');
        
        row.innerHTML = `
            <td>${index + 1}</td>
            <td>${user.username}</td>
            <td>${user.score}</td>
        `;
        
        tbody.appendChild(row);
    });
}

// Appel initial pour récupérer les données
fetchLeaderboard();
