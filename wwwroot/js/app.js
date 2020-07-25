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

var whoami;
var botInfo;

Promise.all([
    fetch("/views/navigation.handlebars").then((response) => response.text()),
    fetch("/api/Auth/whoami").then((response) => response.json()),
    fetch("/api/BotInfo").then((response) => response.json())
])
    .then((responses) => {
        const template = responses[0];
        whoami = responses[1];;
        botInfo = responses[2];
        var compiled = Handlebars.compile(template);
        //var tmp = document.implementation.createHTMLDocument();
        //tmp.body.innerHTML = compiled({ whoami: apiResp });
        //console.log(tmp.body.children);
        //for (var i = 0; i < tmp.body.children.length; i++) {
        //    document.getElementById('navbarCollapse').appendChild(tmp.body.children[i]);
        //}
        document.getElementById("navbar").outerHTML = compiled({ whoami: whoami, botInfo: botInfo });
        navbarReady();
    });

function navbarReady() {
    routie('', function () {
        setActiveNavItem("home")
        fetch("/views/home.handlebars")
            .then((response) => response.text())
            .then((template) => {
                var compiled = Handlebars.compile(template);
                document.getElementById('app').innerHTML = compiled({ botInfo: botInfo });
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
}