const canvas = document.querySelector("#random-walkers-canvas");
const context = canvas.getContext("2d");


export function drawImage(data) {
    // Create a clamped array from the byte array passed in
    const clampedArray = Uint8ClampedArray.from(data);

    // Create image data to put in the canvas
    const imageData = new ImageData(clampedArray, canvas.width, canvas.height);

    // Display the image data in the canvas
    context.putImageData(imageData, 0, 0);
}

