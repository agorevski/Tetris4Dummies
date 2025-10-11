package com.tetris4dummies

import org.junit.Before
import org.junit.Test
import org.junit.Assert.*

/**
 * Unit tests for the GameManager class
 */
class GameManagerTest {
    
    private lateinit var gameManager: GameManager
    
    @Before
    fun setUp() {
        gameManager = GameManager()
    }
    
    @Test
    fun testInitialScore() {
        assertEquals(0, gameManager.getScore())
    }
    
    @Test
    fun testInitialLevel() {
        assertEquals(1, gameManager.getLevel())
    }
    
    @Test
    fun testAddScore() {
        gameManager.addScore(50)
        assertEquals(50, gameManager.getScore())
    }
    
    @Test
    fun testLevelIncreaseAt100Points() {
        gameManager.addScore(100)
        assertEquals(2, gameManager.getLevel())
    }
    
    @Test
    fun testLevelIncreaseAt200Points() {
        gameManager.addScore(200)
        assertEquals(3, gameManager.getLevel())
    }
    
    @Test
    fun testResetGame() {
        gameManager.addScore(150)
        gameManager.resetGame()
        assertEquals(0, gameManager.getScore())
        assertEquals(1, gameManager.getLevel())
    }
    
    @Test
    fun testBlockPlaced() {
        gameManager.blockPlaced()
        assertEquals(10, gameManager.getScore())
    }
    
    @Test
    fun testMultipleBlocksPlaced() {
        for (i in 1..10) {
            gameManager.blockPlaced()
        }
        assertEquals(100, gameManager.getScore())
        assertEquals(2, gameManager.getLevel())
    }
}
