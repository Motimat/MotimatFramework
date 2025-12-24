function danger_alert(inText) {
    Toastify({
        text: inText,
        position: 'center',
        duration: 3000,
        close: true,
        backgroundColor: "#dc3545",
    }).showToast();
}


function info_alert(inText) {
    Toastify({
        text: inText,
        position: 'center',
        duration: 3000,
        close: true,
    }).showToast();
}


function warning_alert(inText) {
    Toastify({
        text: inText,
        position: 'center',
        duration: 3000,
        close: true,
        backgroundColor: "#ff9900",
    }).showToast();
}

function success_alert(inText) {
    Toastify({
        text: inText,
        position: 'center',
        duration: 3000,
        close: true,
        backgroundColor: "#28a745",
    }).showToast();
}