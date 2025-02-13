window.focusGameBoard = function () {
    const gameArea = document.querySelector(".board");
    if (gameArea) {
        gameArea.setAttribute("tabindex", "0"); // タブインデックスを追加
        gameArea.focus();
    }
};
