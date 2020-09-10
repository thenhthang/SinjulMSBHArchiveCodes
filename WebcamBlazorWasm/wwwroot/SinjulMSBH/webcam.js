
let video = null;
let canvas = null;
let photo = null;
let context = null;
let streaming = false;
let width = 0;
let height = 0;
let filter = null;

const onStart = async options => {

    video = document.getElementById(options.videoId);
    canvas = document.getElementById(options.canvasId);
    photo = document.getElementById(options.photoId);
    context = canvas.getContext('2d');
    width = options.width;
    height = options.height;
    filter = options.filter;

    try {
        const stream =
            await navigator.mediaDevices.getUserMedia(
                {
                    video: options.videoEnable,
                    audio: options.audioEnable
                }
            );
        video.srcObject = stream;
        video.play();
    } catch (err) {
        console.log("An error occurred: " + err);
    }

    video.addEventListener('canplay', () => {
        if (!streaming) {
            height = video.videoHeight / (video.videoWidth / width);
            if (isNaN(height)) height = width / (4 / 3);
            video.setAttribute('width', width);
            video.setAttribute('height', height);
            canvas.setAttribute('width', width);
            canvas.setAttribute('height', height);
            streaming = true;
        }
    }, false);

    video.addEventListener("play", () => timercallback(), false);
}

const timercallback = () => {
    if (video.paused || video.ended) return;
    computeFrame();
    setTimeout(() => timercallback(), 0);
}

const computeFrame = () => {
    context.drawImage(video, 0, 0, width, height);
    context.filter = filter;
}

const onTakePicture = () => {
    const context = canvas.getContext('2d');
    if (width && height) {
        canvas.width = width;
        canvas.height = height;
        context.drawImage(video, 0, 0, width, height);

        const data = canvas.toDataURL('image/png');
        photo.setAttribute('src', data);
    } else onClearPhoto();
}

const onClearPhoto = () => {
    const context = canvas.getContext('2d');
    context.fillStyle = "#AAA";
    context.fillRect(0, 0, canvas.width, canvas.height);

    const data = canvas.toDataURL('image/png');
    photo.setAttribute('src', data);
}


window.WebCamFunctions = {
    start: async options => await onStart(options),
    takePicture: () => onTakePicture(),
};