//$(document).ready(function () {
//    google.maps.event.addDomListener(window, 'load', initialize);
//});

//function initialize() {
//    var input = document.getElementById('location');
//    var autocomplete = new google.maps.places.Autocomplete(input);
//}
//function initMap() {
//    const input = document.getElementById("location");
//    const options = {
//        types: ["address"],
//    };

//    const autocomplete = new google.maps.places.Autocomplete(input, options);

//    autocomplete.addListener("place_changed", () => {
//        infowindow.close();
//        marker.setVisible(false);

//        const place = autocomplete.getPlace();

//        if (!place.geometry || !place.geometry.location) {
//            // User entered the name of a Place that was not suggested and
//            // pressed the Enter key, or the Place Details request failed.
//            window.alert("No details available for input: '" + place.name + "'");
//            return;
//        }
//    })

//}
//window.initMap = initMap;

function initMap() {
    const map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: 40.749933, lng: -73.98633 },
        zoom: 13,
        mapTypeControl: false,
    });
    const card = document.getElementById("pac-card");
    const input = document.getElementById("pac-input");
    const options = {
        fields: ["formatted_address", "geometry", "name"],
        strictBounds: false,
        types: ["establishment"],
    };

    map.controls[google.maps.ControlPosition.TOP_LEFT].push(card);

    const autocomplete = new google.maps.places.Autocomplete(input, options);

    // Bind the map's bounds (viewport) property to the autocomplete object,
    // so that the autocomplete requests use the current map bounds for the
    // bounds option in the request.
    autocomplete.bindTo("bounds", map);

    const infowindow = new google.maps.InfoWindow();
    const infowindowContent = document.getElementById("infowindow-content");

    infowindow.setContent(infowindowContent);

    const marker = new google.maps.Marker({
        map,
        anchorPoint: new google.maps.Point(0, -29),
    });

    autocomplete.addListener("place_changed", () => {
        infowindow.close();
        marker.setVisible(false);

        const place = autocomplete.getPlace();

        if (!place.geometry || !place.geometry.location) {
            // User entered the name of a Place that was not suggested and
            // pressed the Enter key, or the Place Details request failed.
            window.alert("No details available for input: '" + place.name + "'");
            return;
        }

        // If the place has a geometry, then present it on a map.
        if (place.geometry.viewport) {
            map.fitBounds(place.geometry.viewport);
        } else {
            map.setCenter(place.geometry.location);
            map.setZoom(17);
        }

        marker.setPosition(place.geometry.location);
        marker.setVisible(true);
        infowindowContent.children["place-name"].textContent = place.name;
        infowindowContent.children["place-address"].textContent =
            place.formatted_address;
        infowindow.open(map, marker);
    });

}

window.initMap = initMap;