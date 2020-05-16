///
//Просто супер быдлокод, не смотрите сюда, пожайлуста
//


const addPlanetForm = document.forms.formAdd;//Получаем саму форму
const editionPlanetForm = document.forms.editionForm;//Получаем саму форму
const addPlanetLabel = document.getElementById("idFormAdd");//Для скрытия формы получаем label
const editionPlanetLabel = document.getElementById("idFormAdd2");//Для скрытия формы получаем label
addPlanetLabel.addEventListener("click", hiddenForm);//Для скрытия формы 
editionPlanetLabel.addEventListener("click", hiddenForm2);//Для скрытия формы 
var index = 0;//индекс элемента в selectList
document.getElementById('editionPlanetDiv').onmousedown = replace;
document.getElementById('addPlanetDiv').onmousedown = replace;
function replace(e) { // 1. отследить нажатие
    var selectedform = document.getElementById(e.currentTarget.id);
    console.log(e.currentTarget.id)
    // подготовить к перемещению

    var coords = getCoords(selectedform);//принимаем координаты формы 
    var shiftX = e.pageX - coords.left;//получаем смещение относительно края формы
    var shiftY = e.pageY - coords.top;//получаем смещение относительно края формы
    selectedform.style.position = 'absolute';  // 2. разместить на том же месте, но в абсолютных координатах
    moveAt(e);//передвегаем 
    // переместим в body, чтобы форма была точно не внутри position:relative
    //document.body.appendChild(selectedform); //делал и без этого, вроде все ок
    selectedform.style.zIndex = 1000; // показывать форму над другими элементами

    // передвинуть форму под координаты курсора
    function moveAt(e) {
        selectedform.style.left = e.pageX - shiftX + 'px'; //двигаем на координаты курсора отнимая смещение относительно захвата формы
        selectedform.style.top = e.pageY - shiftY + 'px'; //двигаем на координаты курсора отнимая смещение относительно захвата формы
    }

    // 3, перемещать по экрану
    selectedform.onmousemove = function (e) { //делал через событие привязанное к selectedform, вроде все также
        moveAt(e);
    }

    // 4. отследить окончание переноса
    selectedform.onmouseup = function () {
        selectedform.onmousemove = null;//делал через событие привязанное к selectedform, вроде все также
        selectedform.onmouseup = null;
    }
}
function getCoords(elem) {   // кроме IE8-
    var box = elem.getBoundingClientRect();
    return {
        top: box.top + pageYOffset, //возвращаем положение объекта 
        left: box.left + pageXOffset //возвращаем положение объекта
    };
}
addPlanetLabel.ondragstart = function () {//удаляем стандартный dragstart
    return false;
};

//Ивент для скрытия формы
function hiddenForm(event) {
    if (addPlanetForm.getAttribute("hidden")) {
        addPlanetForm.removeAttribute("hidden");
    }
    else {
        addPlanetForm.setAttribute("hidden", "true");
    }
}

function hiddenForm2(event) {
    if (editionPlanetForm.getAttribute("hidden")) {
        editionPlanetForm.removeAttribute("hidden");
    }
    else {
        editionPlanetForm.setAttribute("hidden", "true");
    }

}
//
//Задаем размер поля 
//и добавляем событие на ресайз окна браузера 
window.addEventListener('resize', resizeWindow);
var sizeWindow = {
    intViewportHeight: window.innerHeight,
    intViewportWidth: window.innerWidth
};
function resizeWindow(event) {
    sizeWindow.intViewportHeight = window.innerHeight;
    sizeWindow.intViewportWidth = window.innerWidth;
    console.log(sizeWindow.intViewportHeight);
    console.log(sizeWindow.intViewportWidth);
};
//
//делаем запрос с помощью Promise
//
function set(url, data) {
    return new Promise(function (succeed, fail) {
        var request = new XMLHttpRequest();
        request.open("POST", url, true);
        request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        request.addEventListener("load", function () {
            if (request.status < 400)
                succeed(request.response);
            else
                fail(new Error("Request failed: " + request.statusText));
        });
        request.addEventListener("error", function () {
            fail(new Error("Network error"));
        });
        request.send(data);
    });
}
function get(url) {
    return new Promise(function (succeed, fail) {
        var request = new XMLHttpRequest();
        request.open("GET", url, true);
        request.addEventListener("load", function () {
            if (request.status < 400)
                succeed(request.response);
            else
                fail(new Error("Request failed: " + request.statusText));
        });
        request.addEventListener("error", function () {
            fail(new Error("Network error"));
        });
        request.send();
    });
}
///
///Передаем данные размера поля запросом 
///
var dataPole = "w=" + encodeURIComponent(sizeWindow.intViewportWidth) + "&h=" + encodeURIComponent(sizeWindow.intViewportHeight);
set("/Home/Pole", dataPole).then(function (text) {
    console.log(text);
}, function (error) {
    console.log("Error!!!");
    console.log(error);
});
///
///Добавляем данные планеты
///
const selectList = document.getElementById("selectList");
const addButton = document.getElementById('submit');
const namePlanet = document.getElementById('namePlanet');
const xPlanet = document.getElementById('xPlanet');
const yPlanet = document.getElementById('yPlanet');
const massPlanet = document.getElementById('massPlanet');
const deleteButton = document.getElementById('deleteButton');
addButton.addEventListener('click', addClick);
deleteButton.addEventListener('click', deleteClick);

///
//Добавляем планету
//
var Planet = {
    namePlanet: undefined,
    xPlanet: undefined,
    yPlanet: undefined,
    massPlanet: undefined
};
function addClick(e) {
    // Поле ввода имени
    //result - переменные для проверки правильности полей, булевы
    e.preventDefault();
    var expNamePlanet = /[a-z][a-z0-9]/i; //почему-то без разницы какое место занимает выражение. т.е. оно может начинаться или заканчиваться с цифры
    var resultNamePlanet = expNamePlanet.test(namePlanet.value);
    if (resultNamePlanet) {//Если валидация прошла
        Planet.namePlanet = namePlanet.value;
        namePlanet.style.borderColor = "green";
    }
    else namePlanet.style.borderColor = "red";


    //поле ввода X
    var expXPlanet = /[0-9]/;
    var resultX = expXPlanet.test(xPlanet.value);
    if (resultX && (parseInt(xPlanet.value, 10)) < sizeWindow.intViewportWidth) {
        Planet.xPlanet = parseInt(xPlanet.value, 10);
        xPlanet.style.borderColor = "green";
    }
    else xPlanet.style.borderColor = "red";

    //Поле ввода Y
    var expYPlanet = /[0-9]/;
    var resultY = expYPlanet.test(yPlanet.value);
    if (resultY && (parseInt(yPlanet.value, 10)) < sizeWindow.intViewportWidth) {
        Planet.yPlanet = parseInt(yPlanet.value, 10);
        yPlanet.style.borderColor = "green";
    }
    else yPlanet.style.borderColor = "red";

    //Поле ввода массы
    var massValue = parseFloat(massPlanet.value);
    var resultMass = (!Number.isNaN(massValue) && massValue < Number.MAX_VALUE && massValue > Number.MIN_VALUE);
    if (resultMass) {
        Planet.massPlanet = massValue;
        massPlanet.style.borderColor = "green";
    }
    else massPlanet.style.borderColor = "red";
    //Отправка данных на сервер
    if (resultNamePlanet && resultX && resultY && resultMass) {
        console.log("gooood");
        var dataAdd = "name=" + encodeURIComponent(Planet.namePlanet) + "&x=" + encodeURIComponent(Planet.xPlanet) + "&y="
            + encodeURIComponent(Planet.yPlanet) + "&mass=" + encodeURIComponent(Planet.massPlanet);
        set("/Home/Add", dataAdd).then(function (response) {
            return JSON.parse(response);
        }, function (error) {
            console.log("Error!!!");
            console.log(error);
        }).then(function (planetList) {
            addInList(planetList);
        });

        ///Очищаем поля
        /*
        namePlanet.value = '';
        namePlanet.style.borderColor = '';
        xPlanet.value = '';
        xPlanet.style.borderColor = '';
        yPlanet.value = '';
        yPlanet.style.borderColor = '';
        massPlanet.value = '';
        massPlanet.style.borderColor = '';
        */
        
    }
}

function addInList(planetList) {
    console.log(planetList);
    while (selectList.firstChild)// этот код удаляет содержимое списка 
    // перед добавлением новых элементов
    {
        selectList.removeChild(selectList.firstChild);
    }
    planetList.forEach(element => selectList.add(new Option(element.name, element.name)));
}

function deleteClick(e) {
    e.preventDefault();
    var selectedIndex = selectList.options.selectedIndex;
    // удаляем элемент
    var dataDelete = "id=" + encodeURIComponent(selectList.options[selectedIndex].value);
    set("/Home/Delete", dataDelete).then(function (response) {
        return JSON.parse(response);
    }, function (error) {
        console.log("Error!!!");
        console.log(error);
    }).then(function (planetList) {
        console.log(planetList);
        addInList(planetList);
    });
    selectList.options[selectedIndex] = null;
}