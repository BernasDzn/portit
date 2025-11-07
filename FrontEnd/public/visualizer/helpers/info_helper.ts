function setInfoText(userData) {
    
    document.getElementById('object-details').style.display = 'block';

    const infoTitleElement = document.getElementById('container-title');
    const infoDescriptionElement = document.getElementById('container-description');
    const killButtonElement = document.getElementById('kill');

    if (infoTitleElement) infoTitleElement.textContent = userData.title || "Unknown Object";
    if (infoDescriptionElement) infoDescriptionElement.textContent = userData.description || "No description available.";
    if (killButtonElement) {
        if (userData.killable) {
            killButtonElement.style.display = 'inline-block';
            killButtonElement.onclick = () => {
                alert(`${userData.title} has been killed!`);
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