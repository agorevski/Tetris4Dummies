package com.tetris4dummies

/**
 * Represents a single block piece in Tetris4Dummies
 */
class Block(var x: Int, var y: Int) {
    
    fun moveLeft() {
        x -= 1
    }
    
    fun moveRight() {
        x += 1
    }
    
    fun moveDown() {
        y += 1
    }
    
    fun getPosition(): Pair<Int, Int> {
        return Pair(x, y)
    }
}
