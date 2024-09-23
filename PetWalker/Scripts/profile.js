function showSection(sectionId) {
    const sections = document.querySelectorAll('.content-section');
    sections.forEach(section => {
        section.classList.remove('active');
    });
    document.getElementById(sectionId).classList.add('active');
}

// Show the POSTS section by default
showSection('posts');

function openPostPopup() {
    document.getElementById('post-popup').style.display = 'block';
}

function closePopup() {
    document.getElementById('post-popup').style.display = 'none';
}

function openAddPetPopup() {
    document.getElementById('add-pet-popup').style.display = 'block';
}

function closeAddPetPopup() {
    document.getElementById('add-pet-popup').style.display = 'none';
}

function openWalkDetailsPopup() {
    document.getElementById('walk-details-popup').style.display = 'block';
}

function closeWalkDetailsPopup() {
    document.getElementById('walk-details-popup').style.display = 'none';
}

