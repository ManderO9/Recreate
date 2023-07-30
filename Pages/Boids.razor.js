
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
        context.fillStyle = "rgba(" + boid.color.r + "," + boid.color.g + "," + boid.color.b + "," + boid.color.a + ")";

        // Draw it
        context.beginPath();
        context.moveTo(boid.headPointX + boid.positionX, boid.headPointY + boid.positionY);
        context.lineTo(boid.leftWingPointX + boid.positionX, boid.leftWingPointY + boid.positionY);
        context.lineTo(boid.stomachPointX + boid.positionX, boid.stomachPointY + boid.positionY);
        context.lineTo(boid.rightWingPointX + boid.positionX, boid.rightWingPointY + boid.positionY);
        context.closePath();
        context.fill();
    }
}

