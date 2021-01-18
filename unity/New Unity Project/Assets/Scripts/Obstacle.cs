using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Obstacle 
{
    void startGame();

    int gameState();

    void endGame();
}
