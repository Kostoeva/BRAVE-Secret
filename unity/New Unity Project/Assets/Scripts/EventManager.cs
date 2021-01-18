using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{

    #region Editor Variables

    [Tooltip("The referenced set")]
    public GameObject[] m_CardsSet;

    [Tooltip("The width of the grid")]
    public int m_Width;

    [Tooltip("The height of the grid")]
    public int m_Height;

    [SerializeField]
    [Tooltip("The start button for game1.")]
    private GameObject m_startButton;

    [SerializeField]
    [Tooltip("The start button for game2.")]
    private GameObject m_startButton2;

    [SerializeField]
    [Tooltip("The start button for game3.")]
    private GameObject m_startButton3;

    [SerializeField]
    [Tooltip("Displayed Message")]
    public Text msg;

    [SerializeField]
    [Tooltip("The submit button.")]
    private GameObject m_submitButton;

    [Tooltip("Number of cards to test on.")]
    public int num_Orig;

    [SerializeField]
    [Tooltip("Boolean used in game2.")]
    private bool club;

    [SerializeField]
    [Tooltip("Boolean used in game2.")]
    private bool diamond;

    [SerializeField]
    [Tooltip("Boolean used in game2.")]
    private bool heart;

    [SerializeField]
    [Tooltip("Boolean used in game2.")]
    private bool spades;

    #endregion

    /*
     * Default Club as true for game2
     */ 
    public void Start()
    {
        if (!club && !diamond && !heart && !spades)
        {
            club = true;
        }
        
    }

    #region Private Variables

    //An array stored all original cards.
    private GameObject[] original_cards;

    //The referenced array stored all original cards tested on.
    private int[] test_Original;

    //An array stored all original cards tested on.
    private GameObject[] test_Or;

    //Stored all selected cards id
    public LinkedList<int> selected = new LinkedList<int>();

    //Record for game 1
    private int[] game1_record = new int[2];

    //Record for game 2
    private int[] game2_record = new int[2];

    //Record for game 3
    private int[] game3_record = new int[2];

    //To indicate which game it is rn
    private int game;

    //Getter used in GameObstacle
    public int game_record()
    {
        if (game1_record[0] > 0 || game2_record[0] > 0 || game3_record[0] > 0)
        {
            return 1;
        } else if (game1_record[1] > 0 || game2_record[1] > 0 || game3_record[1] > 0)
        {
            return 2;
        }
        else
        {
            return 0;
        }
        
    }

    //Getter used in playercontroller
    public int Game
    {
        get
        {
            return this.game;
        }
    }
    #endregion

    #region GridScale
    /* Below are variables helping to set the position of the cards according to the width and height
     * of the grids.
     */
    //The width of the card.
    float card_width;

    //The height of the card.
    float card_height;

    //A helper index = total width of a row / the width of the screen width
    float width_index;

    //A helper index = total height of a row / the height of the screen width
    float height_index;

    //The width of the space betwen two cards
    float width_space;

    //The height of the space betwen two cards
    float height_space;
    #endregion  

    #region Start the game

    /*
     * 
     */
    public void startGame(int index)
    {
        switch (index)
        {
            case 1:
                startgame1();
                break;
            case 2:
                startgame2();
                break;
            case 3:
                startgame3();
                break;
            default:
                Debug.Log("Error starting game.");
                break;
        }
    }
    /* The coroutine function to generate cards and spawn cards for game1
     * 1. Give a message
     * 2. Generate tested cards and destroy them in 3 seconds (save cards in test_Or for detroying purpose further)
     * 3. Generate original cards after that (save the cards in original_cards for detroting purpose further)
     * 4. The referenced list is saved also for checking success purpose
     * Input; NULL
     * Output: corotine funtion
     * */
    private IEnumerator showOriginalCards1(int remember_time = 2, int show_time = 3)
    {
        msg.text = "Try to remeber the following cards in 3 seconds";
        yield return new WaitForSeconds(remember_time);
        test_Original = generateOriginalCards();
        test_Or = spawn_originalCards(test_Original);
        yield return new WaitForSeconds(show_time);
        msg.text = "Select the original cards";
        destroyOriginal();
        int[] referenced_list = generateCard();
        original_cards = spawn_Cards(referenced_list); 
    }

    /* Start game1
     * 1. Instantiate a linkedlist for selected
     * 2. Disable the start buttons and enable the submit button
     * 3. start the coroutine
     * Input: NULL
     * Output: void
     * */
    private void startgame1()
    {
        game = 1;
        selected = new LinkedList<int>();
        disable_inable_StartButton();
        StartCoroutine(showOriginalCards1());
    }

    /* The coroutine function to generate cards and spawn cards for game2
     * 1. Give a message
     * 2. Generate original cards after that (save the cards in original_cards for detroting purpose further)
     * 3. The referenced list is saved also for checking success purpose
     * Input; NULL
     * Output: corotine funtion
     * */
    private IEnumerator showOriginalCards2(int remember_time = 2)
    {
        
        msg.text = ignore();
        yield return new WaitForSeconds(remember_time);
        destroyOriginal();
        int[] referenced_list = generateCard2();
        original_cards = spawn_Cards(referenced_list);
        if (original_cards == null)
        {
            Debug.Log("No original");
        }
    }

    /* Start game2
     * 1. Instantiate a linkedlist for selected
     * 2. Disable the start buttons and enable the submit button
     * 3. start the coroutine
     * Input: NULL
     * Output: void
     * */
    private void startgame2()
    {
        game = 2;
        selected = new LinkedList<int>();
        disable_inable_StartButton();
        StartCoroutine(showOriginalCards2());
    }


    /* The coroutine function to generate cards and spawn cards for game3
     * 1. Give a message
     * 2. Generate tested cards and destroy them in 3 seconds (save cards in test_Or for detroying purpose further)
     * 3. Generate original cards after that (save the cards in original_cards for detroting purpose further)
     * 4. The referenced list is saved also for checking success purpose
     * Input; NULL
     * Output: corotine funtion
     * */
    private IEnumerator showOriginalCards3(int remember_time = 2, int show_time = 3)
    {
        msg.text = "Try to remember the following cards in 3 seconds";
        yield return new WaitForSeconds(remember_time);
        test_Original = generateOriginalCards();
        test_Or = spawn_game3(test_Original);
        yield return new WaitForSeconds(show_time);
        msg.text = "Sort the cards in reversed order they appear before";
        destroyOriginal();
        int[] referenced_list = generateCard3();
        original_cards = spawn_game3(referenced_list, 1);

    }

    /* Start game3
     * 1. Instantiate a linkedlist for selected
     * 2. Disable the start buttons and enable the submit button
     * 3. start the coroutine
     * Input: NULL
     * Output: void
     * */
    private void startgame3()
    {
        game = 3;
        selected = new LinkedList<int>();
        disable_inable_StartButton();
        StartCoroutine(showOriginalCards3());
    }
    #endregion

    #region Generate Cards

    /* Generate test cards for game3
     * Input: NULL
     * Output: int[], a referenced array for spawning card
     * */
    private int[] generateCard3()
    {
        //The total amount of the cards
        int num = num_Orig;
        int[] result = new int[num];
        for (int i = 0; i < num; i++)
        {
            int index = UnityEngine.Random.Range(1, 53);
            while (true)
            {
                int pos = Array.IndexOf(result, index);
                if (pos == -1)
                {
                    break;
                }
                index = UnityEngine.Random.Range(1, 53);
            }
            result[i] = index;
        }
        //Put original into the generated cards
        for (int j = 0; j < num_Orig; j++)
        {
            int test_pos = Array.IndexOf(result, test_Original[j]);
            if (test_pos == -1)
            {
                while (true)
                {
                    int test_index = UnityEngine.Random.Range(0, result.Length);
                    if (Array.IndexOf(test_Original, result[test_index]) == -1)
                    {
                        result[test_index] = test_Original[j];
                        break;
                    }
                    else
                    {
                        test_index = UnityEngine.Random.Range(0, result.Length);
                    }
                }
            }
        }
        return result;
    }

    /* Generate test cards for game2
     * Input: NULL
     * Output: int[], a referenced array for spawning card
     * */
    private int[] generateCard2()
    {
        //The total amount of the cards
        int num = m_Width * m_Height;
        int[] result = new int[num];
        for (int i = 0; i < num; i++)
        {
            int index = UnityEngine.Random.Range(1, 53);
            while (true)
            {
                int pos = Array.IndexOf(result, index);
                if (pos == -1)
                {
                    break;
                }
                index = UnityEngine.Random.Range(1, 53);
            }
            result[i] = index;
        }
        return result;
    }

    /* Generate test cards for game1
     * Input: NULL
     * Output: int[], a referenced array for spawning card
     * */
    private int[] generateOriginalCards()
    {
        int[] result = new int[num_Orig];
        for (int i = 0; i < num_Orig; i++)
        {
            int index = UnityEngine.Random.Range(1, 53);
            while (true)
            {
                int pos = Array.IndexOf(result, index);
                if (pos == -1)
                {
                    break;
                }
                index = UnityEngine.Random.Range(1, 53);
            }
            result[i] = index;
        }
        return result;
    }

    /* Randomly select m_Hei * m_Wid amount of unique cards and return those cards as an array of the index
     * in the m_cardsSet
     * Input: NULL
     * Output: int[], a referenced array for spawning card
     */
    private int[] generateCard()
    {
        //The total amount of the cards
        int num = m_Height * m_Width;
        int[] result = new int[num];
        for (int i = 0; i < num; i++)
        {
            int index = UnityEngine.Random.Range(1, 53);
            while (true)
            {
                int pos = Array.IndexOf(result, index);
                if (pos == -1)
                {
                    break;
                }
                index = UnityEngine.Random.Range(1, 53);
            }
            result[i] = index;
        }
        if(test_Original == null)
        {
            return result;
        }
        //Put original into the generated cards
        for (int j = 0; j < num_Orig; j++)
        {
            int test_pos = Array.IndexOf(result, test_Original[j]);
            if (test_pos == -1)
            {
                while (true)
                {
                    int test_index = UnityEngine.Random.Range(0, result.Length);
                    if (Array.IndexOf(test_Original, result[test_index]) == -1)
                    {
                        result[test_index] = test_Original[j];
                        break;
                    } else
                    {
                        test_index = UnityEngine.Random.Range(0, result.Length);
                    }
                }
            }
        }
        return result;
    }
    #endregion

    #region Spawn
        
    private GameObject[] spawn_game3(int[] referenced, int ori=0)
    {
        SetCardScale_game3();
        GameObject[] result = new GameObject[num_Orig];
        float offset = 0;
        if (num_Orig % 2 == 0)
        {
            offset = (card_width + width_space) * 0.5f;
        }
        int index = 0;
        foreach (int card in referenced)
        {
            GameObject a = Instantiate(m_CardsSet[card - 1], new Vector3(offset + width_space * (index - num_Orig / 2) + card_width * (index - num_Orig / 2), 7.3f), Quaternion.identity);
            a.transform.Rotate(0, 180, 0);
            a.transform.localScale += new Vector3(width_index - 1, height_index - 1, 0);
            if (ori == 1)
            {
                a.AddComponent<BoxCollider>();
                a.AddComponent<CardInfomation>();
                a.GetComponent<CardInfomation>().id = card - 1;
                a.tag = "Card";
            }
            
            result[index] = a;
            index++;
        }
        return result;
    }
    /* 1. Set scales for tested cards
     * 2. Instantiate the tested cards according to the referenced index returned by generate cards.
     * Input: int[], the referenced array returned by generate cards.
     * Output: GameObject[], the array of all spawned gameobject (tested cards) for destroying purpose later.
     */
    private GameObject[] spawn_originalCards(int[] referenced)
    {
        GameObject[] result = new GameObject[num_Orig];
        int num_per_row = 3;
        setTestedGridScale(num_per_row: num_per_row);
        float offset = 0;
        if (num_per_row % 2 == 0)
        {
            offset = (card_width + width_space) * 0.5f;
        }
        for (int i = 0; i < num_Orig; i++)
        {
            GameObject a = Instantiate(m_CardsSet[referenced[i] - 1], new Vector3(offset + width_space * (i % num_per_row - num_per_row / 2) + card_width * (i % num_per_row - num_per_row / 2), 7.3f - (i / num_per_row)), Quaternion.identity);
            a.transform.Rotate(0, 180, 0);
            a.transform.localScale += new Vector3(width_index - 1, height_index - 1, 0);
            if (game == 3) //TODO:should be deleted, it's useless because of spawn_game3
            {
                Debug.Log("Game 3 Card Information");
                a.AddComponent<BoxCollider>();
                a.AddComponent<CardInfomation>();
                a.GetComponent<CardInfomation>().id = referenced[i] - 1;
                a.tag = "Card";
            }           
            result[i] = a;
        }
        return result;
    }


    /* 1. Destroy previous spawned original cards.
     * 2. Set scales for original cards
     * 3. Instantiate the cards according to the referenced index returned by generate cards.
     * Input: int[], the referenced array returned by generate cards.
     * Output: GameObject[], the array of all spawned gameobject (original cards) for destroying purpose later.
     */
    private GameObject[] spawn_Cards(int[] r)
    {
        destroyCard();
        setGridScale();
        float offset = 0;
        Debug.Log("Game2 Spawning");
        if (m_Width%2 == 0)
        {
            offset = (card_width + width_space) * 0.5f;
        }
        GameObject[] result = new GameObject[m_Height * m_Width];
        for (int i = 0; i < m_Height; i++)
        {
            
            for (int j = 0; j < m_Width; j++)
            {
                GameObject a = Instantiate(m_CardsSet[r[i * m_Width + j] - 1], new Vector3(offset + width_space * (j - m_Width / 2) + card_width * (j - m_Width / 2), 7.7f - height_space * i - card_height * i, 0), Quaternion.identity);
                a.transform.Rotate(0, 180, 0);
                a.transform.localScale += new Vector3(width_index - 1, height_index - 1, 0);
                a.AddComponent<BoxCollider>();
                a.AddComponent<CardInfomation>();
                a.GetComponent<CardInfomation>().id = r[i * m_Width + j] - 1;
                a.tag = "Card";
                result[i * m_Width + j] = a;
            }
        }
        return result;
    }

    #endregion

    #region Helper Function
    /* The function disabling the start button and inabling the submit button when the start button is hit.
     * Input: NULL
     * Ouput: void
     */
     private void disable_inable_StartButton()
    {
        Destroy(m_startButton.GetComponent<BoxCollider>());
        Destroy(m_startButton2.GetComponent<BoxCollider>());
        Destroy(m_startButton3.GetComponent<BoxCollider>());
        m_submitButton.AddComponent<BoxCollider>();
        m_submitButton.GetComponent<BoxCollider>().isTrigger = true;
        m_submitButton.GetComponent<BoxCollider>().size = new Vector3(13, 1, 0);
    }

    /* The function disabling the submit button and inabling the start button when the submit button is hit.
     * Input: NULL
     * Ouput: void
     */
    private void disable_inbale_SubmitButton()
    {
        Destroy(m_submitButton.GetComponent<BoxCollider>());
        m_startButton.AddComponent<BoxCollider>();
        m_startButton.GetComponent<BoxCollider>().isTrigger = true;
        m_startButton.GetComponent<BoxCollider>().size = new Vector3(10, 1, 0);
        m_startButton2.AddComponent<BoxCollider>();
        m_startButton2.GetComponent<BoxCollider>().isTrigger = true;
        m_startButton2.GetComponent<BoxCollider>().size = new Vector3(10, 1, 0);
        m_startButton3.AddComponent<BoxCollider>();
        m_startButton3.GetComponent<BoxCollider>().isTrigger = true;
        m_startButton3.GetComponent<BoxCollider>().size = new Vector3(10, 1, 0);
    }

    /* The function using and updating m_Wid, m_Hei, card_width, card_height, width_space, height_space,
     * width_index, and height_index for tested cards
     * 1. The logic is to first manually set a scale for width and height for cards as well as the disatance between the cards and set a value 
     * manually for the expected width and height of the screen.
     * 2. Use the ratio to adjust the width, height, space distance.
     * 3. Those scales will be used when the spawning function is called.
     * Input: NULL
     * Output: void
     */
    private void setTestedGridScale(float frame_width = 4f, float frame_height = 1.6f, int num_per_row=3)
    {
        card_width = 1.27f;
        card_height = 1.8f;
        width_space = 0.5f;
        height_space = 0.5f;
        width_index = 1;
        height_index = 1;
        int row_num = ((num_Orig - 1)>=0? num_Orig - 1:0) / num_per_row + 1;
        float current_width = num_per_row * card_width + (num_per_row - 1) * width_space;
        Debug.Log(current_width);
        Debug.Log(frame_width);
        width_index = frame_width / current_width;
        Debug.Log(width_index);
        card_width *= width_index;
        width_space *= width_index;
        float current_Height = row_num * card_height + (row_num - 1) * height_space;
        height_index = frame_height / current_Height;
        card_height *= height_index;
        height_space *= height_index;
    }

    private void SetCardScale_game3()
    {
        card_width = 1.27f;
        card_height = 1.8f;
        width_space = 0.5f;
        height_space = 0.5f;
        width_index = 1;
        height_index = 1;
        float curWid = m_Width * card_width + (m_Width - 1) * width_space;
        width_index = 10f / curWid;
        if (width_index < 0.5f)
        {
            width_index = 0.5f;
        }
        card_width *= width_index;
        width_space *= width_index;
        float curHei = m_Height * card_height + (m_Height - 1) * height_space;
        height_index = 1.6f / curHei;
        if (height_index < 0.5f)
        {
            height_index = 0.5f;
        }
        card_height *= height_index;
        height_space *= height_index;
        card_height *= height_index;
        height_space *= height_index;
    }
    /* The function using and updating m_Wid, m_Hei, card_width, card_height, width_space, height_space,
     * width_index, and height_index for original cards
     * 1. The logic is to first manually set a scale for width and height for cards as well as the disatance between the cards and set a value 
     * manually for the expected width and height of the screen.
     * 2. Use the ratio to adjust the width, height, space distance.
     * 3. Those scales will be used when the spawning function is called.
     * Input: NULL
     * Output: void
     */
    private void setGridScale(float frame_width=4f, float frame_height=1.6f, float mini_threshold = 0.5f)
    {
        card_width = 1.27f;
        card_height = 1.8f;
        width_space = 0.5f;
        height_space = 0.5f;
        width_index = 1;
        height_index = 1;
        float current_width = m_Width * card_width + (m_Width - 1) * width_space;
        width_index = frame_width / current_width;
        if (width_index < mini_threshold)
        {
            width_index = mini_threshold;
        }
        card_width *= width_index;
        width_space *= width_index;
        float current_Height = m_Height * card_height + (m_Height - 1) * height_space;
        height_index = frame_height / current_Height;
        if (height_index < mini_threshold)
        {
            height_index = mini_threshold;
        }
        card_height *= height_index;
        height_space *= height_index;
        
    }

    /* Check if the player select correct card in game1
     * Input: NULL
     * Output: bool, true for selectin all cards correctly, false for failed tempt
     */
    private bool checkSuccess()
    {
        foreach(int i in test_Original)
        {
            if (!selected.Contains(i-1))
            {
                return false;
            }
        }
        return true;
    } 

    /* The helper function for generating message in game2
     * Input: NUll
     * Output: String, the message to display at the start of game2
     */
    private String ignore()
    {
        String[] temp = new String[4];
        for (int i = 0; i < 4; i++)
        {
            temp[i] = "";
        }
        int count = 0;
        if (club)
        {
            temp[0] = "Clubs";
            count++;
        }
        if (diamond)
        {
            temp[1] = "Diamonds";
            count++;
        }
        if (heart)
        {
            temp[2] = "Hearts";
            count++;
        }
        if (spades)
        {
            temp[3] = "Spades";
            count++;
        }
        String msg = "Select all ";
        for (int i = 0; i < 4; i++)
        {
            if (temp[i] != "")
            {
                msg += temp[i];
                count -= 1;
                if (count == 0)
                {
                    msg += ".";
                }
                else
                {
                    msg += ", ";
                }
            }
        }
        return msg;
    } 

    /* To check if the card (passed in by parameter) should be selected or not in game2.
     * Input: int card, the id of a card
     * Output: bool, if this card should be selected
     */
    private bool checkSuccess21(int card)
    {
        Debug.Log("CheckSuccess21, "+card);
        if (card <= 12 &&club)
        {
            return true;
        } else if (card <= 25 && diamond)
        {
            return true;
        } else if (card <= 38 && heart)
        {
            return true;
        } else if (card <= 51 && spades)
        {
            return true;
        }
        return false;
    }

    /* Check if all cards that should be selected is succeffully selected by the player
     * Input: int[], an array to record the number of cards that need to be selected
     * Output: bool, if player succeffully select all the cards
     */
    private bool checkSuccess22(int[] result)
    {
        for(int i = 0; i < original_cards.Length; i++)
        {
            //Debug.Log("Length" + original_cards.Length);
            //Debug.Log("Checking"+i);
            int temp = original_cards[i].GetComponent<CardInfomation>().id;
            if (checkSuccess21(temp))
            {
                if (!selected.Contains(temp))
                {
                    return false;
                }
                result[0] += 1;
            }
        }
        return true;
    }
    #endregion

    #region Destroy

    /* Destroy the elements in the original_cards (original cards).
     * Input: NULL
     * Ouput: void
     */
    private void destroyCard()
    {
        if (original_cards != null)
        {
            
            for (int i = 0; i < original_cards.Length; i++)
            {
                Debug.Log("Destroying"+i);
                Destroy(original_cards[i]);
            }
        }
    }

    /* Destroy the elements in the test_Or (tested cards).
     * Input: NULL
     * Ouput: void
     */
    private void destroyOriginal()
    {
        if (test_Or != null)
        {
            for (int i = 0; i < test_Or.Length; i++)
            {
                Destroy(test_Or[i]);
            }
        }
    }

    #endregion

    #region Submit

    /* The decoy submit function will actually run the corresponded submit() function for the correct game
     * Input: NULL
     * Output: void
     */
    public void submit()
    {
        if(game == 1)
        {
            submit1();
        } else if (game == 2)
        {
            submit2();
        } else
        {
            submit3();
        }
    }

    /* The real submit function for game1: 
     * 1. Enable the start buttons and disable the submit button (for collider)
     * 2. See if the player's attemp is succssful
     * 3. Update the record for game 1
     * Input: NULL
     * Output: void
     */
    private void submit1()
    {
        disable_inbale_SubmitButton();
        if (selected.Count != num_Orig)
        {
            game1_record[0] += 1;
            msg.text = "Failed tempt: select wrong number of cards";
        } else {
            bool success = checkSuccess();
            if (success)
            {
                msg.text = "Successful! Good job!";
                game1_record[0] += 1;
            }
            else
            {
                msg.text = "Failed temp: wrong cards selected";
                game1_record[0] += 1;
            }
        }
        destroyCard();
        selected = new LinkedList<int>();
    }

    /* The real submit function for game2: 
     * 1. Enable the start buttons and disable the submit button (for collider)
     * 2. See if the player's attemp is succssful
     * 3. Update the record for game 2
     * Input: NULL
     * Output: void
     */
    private void submit2()
    {
        disable_inbale_SubmitButton();
        int[] result = new int[1];
        result[0] = 0;
        bool s_temp = checkSuccess22(result);
        if (s_temp == false)
        {
            msg.text = "Failed temp";
            game2_record[0] += 1;
            destroyCard();
            return;
        }
        if (result[0] == selected.Count)
        {
            msg.text = "Successful! Good job!";
            game2_record[1] += 1;
        } else
        {
            msg.text = "Failed temp";
            game2_record[0] += 1;
        }
       
        destroyCard();
    }

    /* The real submit function for game3: 
     * 1. Enable the start buttons and disable the submit button (for collider)
     * 2. See if the player's attemp is succssful
     * 3. Update the record for game 3
     * Input: NULL
     * Output: void
     */
    private void submit3()
    {
        disable_inbale_SubmitButton();
        int[] temp = new int[num_Orig];
        int index = 0;
        if (selected.Count!=num_Orig)
        {
            msg.text = "Failed temp";
            game3_record[0] += 1;
            destroyCard();
            return;
        }
        foreach(int card in selected)
        {
            temp[index] = card;
            index++;
        }
        for(int i = 0; i < num_Orig; i++)
        {
            if (temp[i] != test_Original[num_Orig - i - 1] - 1) {
                Debug.Log(temp[i]);
                Debug.Log(test_Original[num_Orig - i - 1]);
                msg.text = "Failed temp";
                game3_record[0] += 1;
                destroyCard();
                return;
            }
        }
        msg.text = "Successful! Good job!";
        game3_record[1] += 1;
        destroyCard();
    }
    #endregion
}
