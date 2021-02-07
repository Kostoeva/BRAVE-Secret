using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObstacle : MonoBehaviour, Obstacle
{
    GameObject game;

    [SerializeField]
    [Tooltip("Prefab for WOME")]
    GameObject wome;

    private int m_gamestate = -1;

    public void startGame()
    {
        game = Instantiate(wome, new Vector3(transform.position.x - 0.5f, transform.position.y + 1.5f, transform.position.z - 2), Quaternion.identity);
        m_gamestate = 0;

    }

    public int gameState()
    {
        if (game != null)
        {
            m_gamestate = game.GetComponent<EventManagerWOME>().game_record();
        }
        return m_gamestate;
    }

    public void endGame()
    {
        m_gamestate = -1;
        Destroy(game);
    }

    void Start()
    {
        startGame();
    }
}
