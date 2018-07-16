using UnityEngine;

namespace Asteroids
{
  public class LevelManager : MonoBehaviour
  {

    [SerializeField]
    private int _difficulty = 0;
    [SerializeField]
    private int _maxEnemies = 2;
    [SerializeField]
    private int _maxAsteroids = 4;
    [SerializeField]
    private int _initialLives = 3;

    [Space]
    
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _asteroidPrefab;

    private Destructible[] _enemies;
    private Destructible[] _asteroids;
    private Destructible _player;
    private Transform[] _bullets;

    private float _inputX;
    private float _inputY;
    private bool _inputShoot;
    private bool _inputWarp;

    private float _lastShootTime;
    private readonly float _shootTimeInterval = .5f;

    #region Unity Lifecycle

    void Awake()
    {
      SetupLevel();
      SetupPlayer();
    }

    void FixedUpdate()
    {
      UpdatePlayer();
    }

    void Update()
    {
      UpdatePlayer();
    }

    #endregion

    public void SetupLevel(int difficulty)
    {
      _difficulty = difficulty;

      SetupLevel();
    }

    private void SetupLevel()
    {
      SetupAsteroids();
      SetupEnemies();
    }

    private void SetupEnemies()
    {
      _enemies = new Destructible[_maxEnemies];
      

    }

    private void SetupAsteroids()
    {
      _asteroids = new Destructible[_maxAsteroids];


    }

    private void SetupPlayer()
    {
      if (_player == null)
      {
        GameObject playerInstance = Instantiate(_playerPrefab);

        _player = playerInstance.GetComponent<Destructible>();
      }

      _player.gameObject.transform.position = Vector3.zero;
    }

    private void UpdateInputs()
    {
      
    }

    private void UpdatePlayer()
    {

    }
  }
}
