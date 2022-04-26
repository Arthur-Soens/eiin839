// This variable will contain the list of the stations of the chosen contract.
var response;
var url = "http://localhost:8733/Design_Time_Addresses/RoutingSoapRest/RestRouting/"
var listCoordinate;
var test = 6;
var alllayer = [];
var map;

function callAPI(url, requestType, params, finishHandler) {
    var fullUrl = url;

    // If there are params, we need to add them to the URL.
    if (params) {
        fullUrl += "?" + params;
    }

    // The js class used to call external servers is XMLHttpRequest.
    var caller = new XMLHttpRequest();
    caller.open(requestType, fullUrl, true);
    // The header set below limits the elements we are OK to retrieve from the server.
    caller.setRequestHeader ("Accept", "application/json");
    // onload shall contain the function that will be called when the call is finished.
    caller.onload=finishHandler;

    caller.send();
}

function retrieveCoord(){
    document.getElementById("step2").style.display = "none";
    var selectedStartAddress = document.getElementById("startAddress").value;
    var selectedEndAddress = document.getElementById("endAddress").value;
    if(selectedEndAddress==="" || selectedStartAddress===""){
        console.log(alllayer)
        var length = alllayer.length
        for(let i = 0; i < length;i++){
            console.log("ici")
            map.removeLayer(alllayer.pop())
        }
        document.getElementById("errorResponse").innerHTML = "Veuillez entrer quelque chose";
        document.getElementById("error").style.display = "block";
        document.getElementById("step2").style.display = "none";
        return;
        return;
    }
    callAPI(url+"Coordinate", "GET", "start="+document.getElementById("startAddress").value.replaceAll(' ', '+')+"&end="+document.getElementById("endAddress").value.replaceAll(' ', '+'), displayValue);
}

function displayValue(){
    if (this.status !== 200) {
        console.log("bad request");
    } else {
        console.log(alllayer)
        var length = alllayer.length
        for(let i = 0; i < length;i++){
            console.log("ici")
            map.removeLayer(alllayer.pop())
        }
        response = JSON.parse(this.responseText);
        var steps = []
        var duration = []
        var totduration=0;
        var dist = []

        if(response[0].isError){
            document.getElementById("errorResponse").innerHTML = response[0].message.replaceAll("\n","<br>");
            document.getElementById("error").style.display = "block";
            document.getElementById("step2").style.display = "none";
            return;
        }
        document.getElementById("error").style.display = "none";
        for(let i = 0; i< response.length; i++){
            console.log(response[i].properties.segments[0].steps)
            for(let j = 0; j<response[i].properties.segments[0].steps.length;j++){
                steps.push(response[i].properties.segments[0].steps[j].instruction)
                totduration += response[i].properties.segments[0].steps[j].duration;
                if(response[i].properties.segments[0].steps[j].duration >= 60) {
                    duration.push("temps estimé: " + Math.trunc(response[i].properties.segments[0].steps[j].duration/60) + "min - Distance : " + response[i].properties.segments[0].steps[j].distance);
                }
                else{
                    duration.push("temps estimé: " + Math.trunc(response[i].properties.segments[0].steps[j].duration) + "sec - Distance : " + response[i].properties.segments[0].steps[j].distance);
                }
            }
        }
        var text = ""
        for(let i = 0; i < steps.length;i++){
            text = text.concat(duration[i],"mètre - ")
            text = text.concat(steps[i],"\n");
        }

        console.log(text)
        document.getElementById("response").innerHTML = text.replaceAll("\n","<br>");
        var minute = Math.trunc(totduration/60);
        document.getElementById("temptot").innerHTML = Math.trunc(minute/60) + "h " + minute%60 + "min"
        document.getElementById("step2").style.display = "block";
        listCoordinate = response[0].geometry.coordinates
        console.log(response);

        map.setView(new ol.View({
            center: ol.proj.fromLonLat(response[0].geometry.coordinates[0]), // <-- Those are the GPS coordinates to center the map to.
            zoom: 10 // You can adjust the default zoom.
        }))


        var j = 0;
        for(let i = 0;i<response.length; i++){
            var coloration;
            if(j%2==0){
                var coloration = '#112233';
            }
            else{
                var coloration = '#5b92e5';
            }
            display(response[i].geometry.coordinates,coloration)
            j++
        }
    }
}


function display(listCoord,coloration) {
    // Create an array containing the GPS positions you want to draw
    var coords = listCoord
    var lineString = new ol.geom.LineString(coords);

    // Transform to EPSG:3857
    lineString.transform('EPSG:4326', 'EPSG:3857');

    // Create the feature
    var feature = new ol.Feature({
        geometry: lineString,
        name: 'Line'
    });

    // Configure the style of the line
    var lineStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: coloration,
            width: 5
        })
    });

    var source = new ol.source.Vector({
        features: [feature]
    });

    var vector = new ol.layer.Vector({
        source: source,
        style: [lineStyle]
    });
    alllayer.push(vector);
    map.addLayer(vector);
}

function createMap(){
    map = new ol.Map({
        target: 'map', // <-- This is the id of the div in which the map will be built.
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            })
        ],
        view: new ol.View({
            center: ol.proj.fromLonLat([7.0985774, 43.6365619]), // <-- Those are the GPS coordinates to center the map to.
            zoom: 10 // You can adjust the default zoom.
        })
    });
}
