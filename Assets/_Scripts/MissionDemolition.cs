using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;

    [Header("Inscribed")]
    public TMP_Text uitLevel;  
    public TMP_Text uitShots;  
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("Dynamic")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";

    void Awake()
    {
        S = this; // Ensure Singleton is set early
    }

    void Start()
    {
        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void Update()
    {
        UpdateGUI();

        if ((mode == GameMode.playing) && Goal.goalMet)
        {
            mode = GameMode.levelEnd;
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);
            Invoke("NextLevel", 2f);
        }
    }

    void StartLevel()
    {
        if (castle != null)
        {
            Destroy(castle);
        }

        Projectile.DESTROY_PROJECTILES();

        if (level < levelMax) // Prevent array out-of-bounds
        {
            castle = Instantiate<GameObject>(castles[level]);
            castle.transform.position = castlePos;
        }

        Goal.goalMet = false;
        UpdateGUI();
        mode = GameMode.playing;
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI()
    {
        if (uitLevel != null)
            uitLevel.text = $"Level: {level + 1} of {levelMax}";

        if (uitShots != null)
            uitShots.text = $"Shots Taken: {shotsTaken}"; // No reset
    }

    void NextLevel()
    {
        level++;
        if (level == levelMax)
        {
            level = 0;
            shotsTaken = 0; // Reset shots only when the game restarts
            SceneManager.LoadScene("Game_Over");
            return;
        }
        StartLevel();
    }

    static public void SHOT_FIRED()
    {
        S.shotsTaken++; // Increment shots across levels
    }

    static public GameObject GET_CASTLE()
    {
        return S.castle;
    }
}