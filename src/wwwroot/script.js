let dialog;
let durationSelect;
let intervalId;

let currentDuration = 10;
let selectedDuration = 10;

window.addEventListener("DOMContentLoaded", () => {
  dialog = document.getElementById("settings-dialog");
  durationSelect = document.getElementById("duration-select");
  dialog.addEventListener("close", () => onDialogClose());
  durationSelect.addEventListener("change", () => onDurationChange());
  window.addEventListener("mousemove", () => onUserInteraction(), {
    once: true,
  });
});

getNextImage();
reconfigureDiaShow(currentDuration);

function getNextImage() {
  fetch("/image/next", { method: "GET" }).then((response) => {
    if (response.status !== 200) {
      console.log(`Failed to load dia: ${response.statusText}`);
      return;
    }
    response.blob().then((imageBlob) => {
      const imageObjectURL = URL.createObjectURL(imageBlob);
      const image = document.getElementById("image");
      image.src = imageObjectURL;
    });
  });
}

function onDialogClose() {
  currentDuration = selectedDuration;
  reconfigureDiaShow(currentDuration);
  setTimeout(() => {
    window.addEventListener("mousemove", () => onUserInteraction(), {
      once: true,
    });
  }, 2000);
}

function onDurationChange() {
  selectedDuration = durationSelect.value;
  console.log(selectedDuration);
}

function onUserInteraction() {
  dialog;
  if (dialog.open !== true) {
    dialog.showModal();
  }
}

function reconfigureDiaShow(intervalSeconds) {
  console.log(intervalSeconds);
  clearInterval(intervalId);
  intervalId = setInterval(() => {
    getNextImage();
  }, intervalSeconds * 1000);
}
