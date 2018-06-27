using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class GPGSManager : MonoBehaviour
{
    [Header("Button parameters")]
    public GameObject GPGSButtons;
    Image[] buttonImages;
    public Color enabledColor;
    public Color disabledColor;
    [Space]
    public Text signInButtonText;
    public Text authStatus;

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
        buttonImages = GPGSButtons.GetComponentsInChildren<Image>();

        // Create client configuration
        PlayGamesClientConfiguration config = new
            PlayGamesClientConfiguration.Builder()
            .Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;

        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
    }

    public void SignIn()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        }
        else
        {
            // Sign out of play games
            PlayGamesPlatform.Instance.SignOut();

            // Reset UI
            //signInButtonText.text = "Sign In";
            //authStatus.text = "";
        }
    }

    public void SignInCallback(bool success)
    {
        if (success)
        {
            Debug.Log("(Lollygagger) Signed in!");

            // Change sign-in button text
            //signInButtonText.text = "Sign out";

            // Show the user's name
            //authStatus.text = "Signed in as: " + Social.localUser.userName;

            SwitchButtonColors(enabledColor);
        }
        else
        {
            Debug.Log("(Lollygagger) Sign-in failed...");

            // Show failure message
            //signInButtonText.text = "Sign in";
            //authStatus.text = "Sign-in failed";

            SwitchButtonColors(disabledColor);
        }
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

    public void ShowAchievements()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else
        {
            Debug.Log("Cannot show Achievements, not logged in");
        }
    }

    /// <summary>
    /// Updates score on Leaderboard
    /// </summary>
    public void UpdateScoreLeaderBoard(int score, string leaderboardTokenID)
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(score, leaderboardTokenID, (bool success) =>
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
        if (Social.localUser.authenticated)
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
    }

    /// <summary>
    /// Para logros acumulativos
    /// </summary>
    /// <param name="achievement"></param>
    public void AchievementIncremented(string achievement)
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(achievement, 1, (bool success) =>
              {
                  Debug.Log("(Lollygagger) Sharpshooter Increment: " + success);
              });
        }
    }

    /// <summary>
    /// Log Out
    /// </summary>
    public void LogOut()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
    }


    #region UI

    void SwitchButtonColors(Color newColor)
    {
        foreach (Image img in buttonImages)
        {
            img.color = newColor;
        }
    }

    public void ShowButtons()
    {
        GPGSButtons.SetActive(true);        
    }

    public void HideButtons()
    {
        GPGSButtons.SetActive(false);
    }

    #endregion
}
