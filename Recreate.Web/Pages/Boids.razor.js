
export function drawImage(data) {

    // Get the canvas to draw on
    var canvas = document.querySelector("#boids-canvas");

    // Get the canvas 2d context
    var context = canvas.getContext("2d");

    // Fill the entire screen with black
    context.fillStyle = "#1e1e20";
    context.fillRect(0, 0, canvas.width, canvas.height);

    // For each boid
    for (var i = 0; i < data.length; i++) {
        var boid = data[i];

        // Set fill color
        context.fillStyle = "rgba(" + boid.color.r + "," + boid.color.g + "," + boid.color.b + "," + boid.color.a + ")";// "#" + boid.color.name;

        // Draw it
        context.beginPath();
        context.arc(boid.xPosition, boid.yPosition, 10, 0, 2 * Math.PI);
        context.fill(); 
    }



}

