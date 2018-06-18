using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class GPGSManager : MonoBehaviour
{

    public Text debugText;

    private static GPGSManager instance = null;

    public static GPGSManager Instance
    {
        get
        {
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    private void Awake()
    {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

    }
    void Start()
    {

        //// AÑADIDO (Sin esto, funciona bien)
        // Create client configuration
        PlayGamesClientConfiguration config = new
            PlayGamesClientConfiguration.Builder()
            .Build();

        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;

        PlayGamesPlatform.InitializeInstance(config); //Añadido

        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();

        LogIn();
    }

    private void Update()
    {
        debugText.text = PlayGamesPlatform.Instance.localUser.authenticated.ToString();
    }

    /// <summary>
    /// Make Login and manage the succes or failure
    /// </summary>
    public void LogIn()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("Login Sucess");
            }
            else
            {
                Debug.Log("Login failed");
            }
        });
    }

    /// <summary>
    /// Shows Leaderboard
    /// </summary>
    public void ShowLeaderBoard()
    {
        Debug.Log("Show leaderboard");
        //Social.ShowLeaderboardUI (); // Show all leaderboard
        if (PlayGamesPlatform.Instance.localUser.authenticated)
            PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_progreso);
        else Debug.Log("Not autenticated");
    }

    /// <summary>
    /// Adds score to Leaderboard
    /// </summary>
    public void AddScoreLeaderBoard(int score)
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(score, GPGSIds.leaderboard_progreso, (bool success) =>
            {
                if (success)
                {
                    Debug.Log("Update Score Succeeded");
                }
                else
                {
                    Debug.Log("Update Score Fail");
                }
            });
        }
    }

    /// <summary>
    /// Unlocks reward
    /// </summary>
    /// <param name="achievement">Token of achievement (GPGSIds)</param>
    public void AchievementAccomplished(string achievement)
    {
        Social.ReportProgress(achievement, 200.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                Debug.Log("Achievement update Succeeded");
            }
            else
            {
                Debug.Log("Achievement update Fail");
            }
        });
    }

    /// <summary>
    /// Log Out
    /// </summary>
    public void LogOut()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
    }

}
