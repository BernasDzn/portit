function setInfoText(title, description) {
    
    document.getElementById('object-details').style.display = 'block';

    const infoTitleElement = document.getElementById('container-title');
    const infoDescriptionElement = document.getElementById('container-description');

    if (infoTitleElement) {
        infoTitleElement.textContent = title;
    }

    if (infoDescriptionElement) {
        infoDescriptionElement.textContent = description;
    }
}

function hideInfoText() {
    document.getElementById('object-details').style.display = 'none';
}

export { setInfoText, hideInfoText };