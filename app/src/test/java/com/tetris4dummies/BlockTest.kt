package com.tetris4dummies

import org.junit.Before
import org.junit.Test
import org.junit.Assert.*

/**
 * Unit tests for the Block class
 */
class BlockTest {
    
    private lateinit var block: Block
    
    @Before
    fun setUp() {
        block = Block(5, 0)
    }
    
    @Test
    fun testBlockInitialization() {
        assertEquals(5, block.x)
        assertEquals(0, block.y)
    }
    
    @Test
    fun testMoveLeft() {
        block.moveLeft()
        assertEquals(4, block.x)
        assertEquals(0, block.y)
    }
    
    @Test
    fun testMoveRight() {
        block.moveRight()
        assertEquals(6, block.x)
        assertEquals(0, block.y)
    }
    
    @Test
    fun testMoveDown() {
        block.moveDown()
        assertEquals(5, block.x)
        assertEquals(1, block.y)
    }
    
    @Test
    fun testGetPosition() {
        val position = block.getPosition()
        assertEquals(5, position.first)
        assertEquals(0, position.second)
    }
    
    @Test
    fun testMultipleMoves() {
        block.moveLeft()
        block.moveDown()
        block.moveDown()
        block.moveRight()
        block.moveRight()
        
        assertEquals(6, block.x)
        assertEquals(2, block.y)
    }
}
