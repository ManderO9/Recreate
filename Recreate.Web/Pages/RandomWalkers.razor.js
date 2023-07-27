
export function drawImage(data) {

    // Get the canvas to draw on
    var canvas = document.querySelector("#random-walkers-canvas");

    // Get the canvas 2d context
    var context = canvas.getContext("2d");

    // Create a clamped array from the byte array passed in
    var clampedArray = Uint8ClampedArray.from(data);

    // Create image data to put in the canvas
    var imageData = new ImageData(clampedArray, canvas.width, canvas.height);
    
    // Display the image data in the canvas
    context.putImageData(imageData, 0, 0);
}

