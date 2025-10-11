package com.tetris4dummies

import androidx.test.espresso.Espresso.onView
import androidx.test.espresso.assertion.ViewAssertions.matches
import androidx.test.espresso.matcher.ViewMatchers.isDisplayed
import androidx.test.espresso.matcher.ViewMatchers.withId
import androidx.test.espresso.matcher.ViewMatchers.withText
import androidx.test.ext.junit.rules.ActivityScenarioRule
import androidx.test.ext.junit.runners.AndroidJUnit4
import org.junit.Rule
import org.junit.Test
import org.junit.runner.RunWith

/**
 * Instrumented test for MainActivity
 */
@RunWith(AndroidJUnit4::class)
class MainActivityTest {
    
    @get:Rule
    val activityRule = ActivityScenarioRule(MainActivity::class.java)
    
    @Test
    fun testTitleIsDisplayed() {
        onView(withId(R.id.titleTextView))
            .check(matches(isDisplayed()))
            .check(matches(withText("Tetris4Dummies")))
    }
    
    @Test
    fun testScoreIsDisplayed() {
        onView(withId(R.id.scoreTextView))
            .check(matches(isDisplayed()))
            .check(matches(withText("Score: 0")))
    }
}
