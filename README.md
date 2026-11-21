# Slot-Machine-Game

How to Run

Go to "Build/WebGL" and double-click "RunGame.bat" to launch the game in your browser.

About the Game

A simple 2D slot machine game built in Unity. The goal is to match all three center symbols to win. The game includes animated reels, random outcomes, a betting system, and symbol-based payouts.

Features

- 3 Reels: Each reel displays 3 symbols, with the center row acting as the winning line.
- Random Outcomes: Results are generated before the reels start spinning.
- Smooth Animations: Reels spin vertically, slow down, and stop on the generated result.
- Betting System: Increase or decrease your bet in steps of 100.
- Balance System: Start with 1,000 credits and manage your balance with each spin.
- Payouts: Matching symbols reward the player based on their multiplier.

How It Works

The player selects a bet and spins the reels. The bet is deducted from the balance, and the game randomly generates a result for each reel. If all three center symbols match, the player receives a payout based on the symbol's multiplier. Otherwise, the bet is lost.

Built With

- Unity 2D
- C#
- TextMeshPro
- WebGL

Development Approach

The game was built step by step, starting with the visual setup and reel animations, followed by the game logic, betting system, random results, and payout handling. The code is separated into "SlotReel" for reel animations and "SlotGameManager" for the game logic.

This project was created as a game development assignment and uses virtual credits only.
