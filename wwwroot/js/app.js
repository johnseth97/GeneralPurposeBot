function setActiveNavItem(name) {
    var elements = document.getElementsByClassName('active');
    while (elements.length > 0) {
        elements[0].classList.remove('active');
    }
    document.getElementById("nav-" + name + "-link").classList.add('active');
}
function loadingSpinner(message) {
    var template = document.getElementById("loading-template").innerHTML;
    var compiled = Handlebars.compile(template);
    document.getElementById('app').innerHTML = compiled({ message: message });
}

routie('', function () {
    setActiveNavItem("home")
    fetch("/views/home.handlebars")
        .then((response) => response.text())
        .then((template) => {
            var compiled = Handlebars.compile(template);
            document.getElementById('app').innerHTML = compiled();
        })
})
routie('help', function () {
    setActiveNavItem("help")
    loadingSpinner("Fetching help...")
    Promise.all([
        fetch("/views/help.handlebars").then((response) => response.text()),
        fetch("/api/Help").then((response) => response.json())
    ])
        .then((responses) => {
            const template = responses[0];
            const apiResp = responses[1];
            var compiled = Handlebars.compile(template);
            document.getElementById('app').innerHTML = compiled({ modules: apiResp });
        })
})