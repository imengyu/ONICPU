var previousP0 = null;
function start() {
    io.P3 = storage.savedNowDays;
    if (storage.savedNowDays == NaN) {
        storage.savedNowDays = 31;
    }
}

function stop() {}

function tick() {
    var currentP0 = io.P0;

    if (previousP0 != null) {
        if (previousP0 >= 99 && currentP0 <= 1) {
            storage.savedNowDays = storage.savedNowDays + 1;
            __log("days:" + storage.savedNowDays);
        }
    }
    previousP0 = currentP0;
    io.P2 = storage.savedNowDays;
}
