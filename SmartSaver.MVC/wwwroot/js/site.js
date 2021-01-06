
window.onload = function () {
    const preloader = document.getElementsByClassName('load')[0];
    const inputElem = document.getElementsByTagName('form');
    const modals = document.getElementsByClassName('close');
    console.log(inputElem.length);

    for (var i = 0; i < inputElem.length; i++) {
        inputElem[i].addEventListener('submit', function () {
            for (var j = 0; j < modals.length; j++) {
                modals[j].click();
            }
            preloader.style.visibility = "visible";
        });
    }
}
