document.addEventListener("DOMContentLoaded", function () {
    const arrivalTimeElement = document.getElementById("arrivalTimeValue");
    const parkedDurationElement = document.getElementById("parkedDuration");

    if (!arrivalTimeElement || !parkedDurationElement) return;

    const arrivalTime = arrivalTimeElement.value;
    const duration = getParkedDuration(arrivalTime);
    parkedDurationElement.textContent = duration;
});
