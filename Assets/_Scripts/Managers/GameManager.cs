using UnityEngine;

namespace Asteroids
{
  public class GameManager : MonoBehaviour
  {

    private static GameManager _instance;

    public static GameManager Instance
    {
      get { return _instance; }
    }

    [SerializeField]
    private Ranking _ranking;

    private int _score;
    private float _lastSyncedScore = 0;
    private float _syncScoreInterval = 30;

    public int Score
    {
      get { return _score; }
      set { _score = value; }
    }

    #region Unity Monobehavior

    private void Awake()
    {
      if (_instance != null)
      {
        Destroy(this);

        return;
      }

      _instance = this;
    }

    void Update()
    {
      SyncScore();
    }

    #endregion

    private void NewGame()
    {

    }

    private void RestartGame()
    {

    }

    private void SyncScore(bool force = false)
    {
      _lastSyncedScore -= Time.deltaTime;

      if (!force && _lastSyncedScore > 0)
        return;



      _lastSyncedScore = _syncScoreInterval;
    }


    private void LeaveGame()
    {

    }

    private void ExitGame()
    {

    }

  }
}
