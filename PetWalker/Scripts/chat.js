// Define objects to manage friends and chat elements
let friends = {
    list: document.querySelector('ul.people'), // List of all friend elements
    all: document.querySelectorAll('.left-messages .person'), // All friend elements
    name: '' // Placeholder for the selected friend's name
};

let chat = {
    container: document.querySelector('.right-messages'), // Chat container
    current: null, // Currently active chat
    person: null, // Currently selected person
    name: document.querySelector('.right-messages .top .name') // Name display in chat header
};

// Add event listeners to all friends
friends.all.forEach(f => {
    f.addEventListener('mousedown', () => {
        if (!f.classList.contains('active')) {
            setActiveChat(f);
        }
    });
});

// Function to set the active chat
function setActiveChat(f) {
    // Remove 'active' class from currently active friend
    const activeFriend = friends.list.querySelector('.active');
    if (activeFriend) {
        activeFriend.classList.remove('active');
    }

    // Add 'active' class to the clicked friend
    f.classList.add('active');

    // Update the currently active chat
    chat.current = chat.container.querySelector('.active-chat');
    chat.person = f.getAttribute('data-chat');

    // Remove 'active-chat' class from current chat and add it to the new one
    if (chat.current) {
        chat.current.classList.remove('active-chat');
    }
    const newActiveChat = chat.container.querySelector(`[data-chat="${chat.person}"]`);
    if (newActiveChat) {
        newActiveChat.classList.add('active-chat');
    }

    // Update the chat header with the selected friend's name
    friends.name = f.querySelector('.name').innerText;
    chat.name.innerHTML = friends.name;
}

// Initialize the first chat as active on page load
document.addEventListener('DOMContentLoaded', () => {
    if (friends.all.length > 0) {
        setActiveChat(friends.all[0]);
    }
});
