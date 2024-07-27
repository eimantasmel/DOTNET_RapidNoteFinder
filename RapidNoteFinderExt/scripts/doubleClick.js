export default function doubleClickEvent(lastKeyDownContainer, callback) {
    const currentTime = new Date().getTime();
    if (lastKeyDownContainer.lastKeyDown === null) {
        lastKeyDownContainer.lastKeyDown = currentTime;
    } else {
        const elapsedTime = currentTime - lastKeyDownContainer.lastKeyDown;
        if (elapsedTime < 300) { // Adjust the time frame as needed
            callback();
            // Reset the last key down time for next detection
            lastKeyDownContainer.lastKeyDown  = null;
        } else {
            lastKeyDownContainer.lastKeyDown  = currentTime;
        }
    }
}



