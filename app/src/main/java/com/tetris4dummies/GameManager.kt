package com.tetris4dummies

/**
 * Manages the game state and scoring for Tetris4Dummies
 */
class GameManager {
    private var score: Int = 0
    private var level: Int = 1
    
    fun getScore(): Int = score
    
    fun getLevel(): Int = level
    
    fun addScore(points: Int) {
        score += points
        updateLevel()
    }
    
    fun resetGame() {
        score = 0
        level = 1
    }
    
    private fun updateLevel() {
        level = (score / 100) + 1
    }
    
    fun blockPlaced() {
        addScore(10)
    }
}
