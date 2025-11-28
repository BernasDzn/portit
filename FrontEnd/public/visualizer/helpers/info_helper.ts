function setInfoText(userData) {
    
    document.getElementById('object-details').style.display = 'block';

    const infoTitleElement = document.getElementById('container-title');
    const infoDescriptionElement = document.getElementById('container-description');
    const killButtonElement = document.getElementById('kill');

    if (infoTitleElement) infoTitleElement.textContent = userData.title || "Unknown Object";
    
    var descriptionText = userData.description || "No description available.";
    if (userData.details) {
        descriptionText += "\n\n--- Details ---";
        for (var [key, value] of Object.entries(userData.details)) {
            const displayKey = key.replace(/([A-Z])/g, ' $1').trim();
            const formattedKey = displayKey.charAt(0).toUpperCase() + displayKey.slice(1);
            
            if (value === null || value === undefined) {
                descriptionText += `\n${formattedKey}: N/A`;
            } else {
                descriptionText += `\n${formattedKey}: ${value}`;
            }
        }
    }
    
    if (infoDescriptionElement) infoDescriptionElement.innerHTML = descriptionText.replace(/\n/g, '<br>');
    
    if (killButtonElement) {
        if (userData.killable) {
            killButtonElement.style.display = 'inline-block';
            killButtonElement.onclick = () => {
                alert(`${userData.title} was removed.`);
                userData.killFunction();
                hideInfoText();
            }
        } else {
            killButtonElement.style.display = 'none';
        }
    }
}

function hideInfoText() {
    document.getElementById('object-details').style.display = 'none';
}

export { setInfoText, hideInfoText };