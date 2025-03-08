let currentPlayer = "X";
let board = ["", "", "", "", "", "", "", "", ""];
let gameActive = true;
let xWins = 0, oWins = 0, draws = 0;
let isSinglePlayer = false; // Default to multiplayer mode

// Function to handle player moves
function makeMove(cellIndex) {
    if (board[cellIndex] === "" && gameActive) {
        board[cellIndex] = currentPlayer;
        document.getElementById(`cell-${cellIndex}`).textContent = currentPlayer;
        checkWinner();

        if (gameActive) {
            currentPlayer = currentPlayer === "X" ? "O" : "X";

            // If in single-player mode and it's the AI's turn, make its move
            if (isSinglePlayer && currentPlayer === "O") {
                makeAIMove();
            }
        }
    }
}

// Function to make a random AI move
function makeAIMove() {
    const emptyCells = board.map((val, idx) => val === "" ? idx : null).filter(val => val !== null);
    if (emptyCells.length > 0) {
        const randomCell = emptyCells[Math.floor(Math.random() * emptyCells.length)];
        makeMove(randomCell);
    }
}

// Function to enable single-player mode
function setSinglePlayer() {
    isSinglePlayer = true;
    resetGame();
}

// Function to enable multiplayer mode
function setMultiplayer() {
    isSinglePlayer = false;
    resetGame();
}

// Function to check for a winner or draw
function checkWinner() {
    const winningCombinations = [
        [0, 1, 2], [3, 4, 5], [6, 7, 8], // Rows
        [0, 3, 6], [1, 4, 7], [2, 5, 8], // Columns
        [0, 4, 8], [2, 4, 6]             // Diagonals
    ];

    for (const combo of winningCombinations) {
        const [a, b, c] = combo;
        if (board[a] && board[a] === board[b] && board[a] === board[c]) {
            highlightWinningCells(combo);
            document.getElementById("game-over-message").textContent = `Player ${board[a]} wins!`;
            document.getElementById("game-over-message").style.display = "block";
            updateScoreboard(board[a]);
            gameActive = false; // Game ends
            return;
        }
    }

    // Check for a draw
    if (!board.includes("")) {
        draws++;
        document.getElementById("draws").textContent = draws;
        document.getElementById("game-over-message").textContent = "It's a draw!";
        document.getElementById("game-over-message").style.display = "block";
        updateScoreboard("draw");
        gameActive = false; // Game ends
    }
}

// Function to highlight winning cells
function highlightWinningCells(combo) {
    combo.forEach(index => {
        document.getElementById(`cell-${index}`).classList.add("winning-cell");
    });
}

// Function to reset the game
function resetGame() {
    // Reset the board array
    board = ["", "", "", "", "", "", "", "", ""];

    // Clear the UI
    document.querySelectorAll(".cell").forEach(cell => {
        cell.textContent = ""; // Clear cell content
        cell.classList.remove("winning-cell"); // Remove winning-cell class
    });

    // Clear the game-over message
    document.getElementById("game-over-message").textContent = "";
    document.getElementById("game-over-message").style.display = "none";

    // Reset the current player
    currentPlayer = "X";

    // Reset the game state
    gameActive = true;
}

// Function to update the scoreboard
function updateScoreboard(winner) {
    if (winner === "X") xWins++;
    else if (winner === "O") oWins++;
    else if (winner === "draw") draws++;

    document.getElementById("x-wins").textContent = xWins;
    document.getElementById("o-wins").textContent = oWins;
    document.getElementById("draws").textContent = draws;
}