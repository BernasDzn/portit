function setInfoText(userData) {
    
    document.getElementById('object-details').style.display = 'block';

    const infoTitleElement = document.getElementById('container-title');
    const infoDescriptionElement = document.getElementById('container-description');
    const killButtonElement = document.getElementById('kill');

    if (infoTitleElement) infoTitleElement.textContent = userData.title || "Unknown Object";
    if (infoDescriptionElement) {
        const desc = userData.description || "No description available.";
        infoDescriptionElement.innerHTML = desc.replace(/\n/g, '<br>');
    }
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