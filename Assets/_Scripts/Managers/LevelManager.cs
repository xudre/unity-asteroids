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

    [Space]

    [SerializeField]
    private Transform _playerRoot;
    [SerializeField]
    private Transform _enemiesRoot;
    [SerializeField]
    private Transform _asteroidsRoot;

    [Space]
    
    [SerializeField]
    private Vector2Int _edgeWarpOffset = new Vector2Int(10, 10);

    private Camera _camera;

    private Enemy[] _enemies;
    private Meteor[] _asteroids;
    private Player _player;
    private Transform[] _bullets;

    private float _inputSteer;
    private float _inputThrust;
    private bool _inputShoot;
    private bool _inputWarp;

    private float _lastThrustTime;
    private float _lastShootTime;
    private readonly float _thrustDecayTime = .5f;
    private readonly float _shootTimeInterval = .5f;

    #region Unity Lifecycle

    private void Awake()
    {
      SetupLevel();
      SetupPlayer();
    }

    private void Start()
    {
      _camera = Camera.main;
    }

    private void FixedUpdate()
    {
      UpdatePlayer();
      UpdateAsteroids();
      UpdateEnemies();
    }

    private void Update()
    {
      UpdateInputs();
    }

    private void LateUpdate()
    {
      _inputShoot = false;
      _inputWarp = false;
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
      _enemies = new Enemy[_maxEnemies];

      for (int i = 0; i < _maxEnemies; i++)
      {
        if (_enemies[i] == null)
        {
          GameObject enemyInstance = Instantiate(_enemyPrefab);

          enemyInstance.transform.parent = _enemiesRoot;

          _enemies[i] = enemyInstance.GetComponent<Enemy>();
        }

        _enemies[i].transform.position = Vector3.zero;
      }
    }

    private void SetupAsteroids()
    {
      _asteroids = new Meteor[_maxAsteroids];

      for (int i = 0; i < _maxAsteroids; i++)
      {
        if (_asteroids[i] == null)
        {
          GameObject asteroidInstance = Instantiate(_asteroidPrefab);

          asteroidInstance.transform.parent = _asteroidsRoot;

          _asteroids[i] = asteroidInstance.GetComponent<Meteor>();
        }

        _asteroids[i].transform.position = Vector3.zero;
      }
    }

    private void SetupPlayer()
    {
      if (_player == null)
      {
        GameObject playerInstance = Instantiate(_playerPrefab);

        playerInstance.transform.parent = _playerRoot;

        _player = playerInstance.GetComponent<Player>();

        _player.Life = _initialLives;
      }

      _player.transform.position = Vector3.zero;
    }

    private void UpdateInputs()
    {
      _inputSteer = InputManager.Instance.Steering;
      _inputThrust = InputManager.Instance.Thruster;
      _inputShoot = InputManager.Instance.Shoot > 0;
      _inputWarp = InputManager.Instance.Warp > 0;
    }

    private void UpdatePlayer()
    {
      if (_player == null || _player.Dead)
        return;

      _player.transform.Rotate(new Vector3(0, 0, -_inputSteer * _player.RotationSpeed * Time.fixedDeltaTime));

      if (_inputThrust > 0)
      {
        _player.Rigidbody.AddForce(_player.transform.up * _inputThrust * _player.ThrustSpeed * Time.fixedDeltaTime);

        _lastThrustTime = Time.fixedTime;
      }

      if (_player.Flames != null)
      {
        int flamesCount = _player.Flames.Length;
        float flameStage = 1 - (Time.fixedTime - _lastThrustTime) / _thrustDecayTime;

        flameStage = flameStage < 0 ? 0 : flameStage > 1 ? 1 : flameStage;

        Color flameColor = new Color(1, 1, 1, flameStage);

        for (int i = 0; i < flamesCount; i++)
        {
          SpriteRenderer flame = _player.Flames[i];

          flame.color = flameColor;
        }
      }

      EdgeWarp(_player);
    }

    private void UpdateAsteroids()
    {
      for (int i = 0; i < _maxAsteroids; i++)
      {
        Meteor meteor = _asteroids[i];

        if (meteor == null || meteor.Dead)
          continue;

        EdgeWarp(meteor);
      }
    }

    private void UpdateEnemies()
    {
      for (int i = 0; i < _maxEnemies; i++)
      {
        Enemy enemy = _enemies[i];

        if (enemy == null || enemy.Dead)
          continue;

        EdgeWarp(enemy);
      }
    }

    private void EdgeWarp(Destructible entity)
    {
      if (entity == null || entity.Dead)
        return;

      Vector2Int entitySize = entity.ScreenSize;
      Vector3 screenPos = _camera.WorldToScreenPoint(entity.transform.position);
      Vector2 entityPos = screenPos;

      if (screenPos.x < -entitySize.x - _edgeWarpOffset.x)
        entityPos.x = Screen.width + entitySize.x;
      else if (screenPos.x > Screen.width + entitySize.x + _edgeWarpOffset.x)
        entityPos.x = -entitySize.x;

      if (screenPos.y < -entitySize.y - _edgeWarpOffset.y)
        entityPos.y = Screen.height + entitySize.y;
      else if (screenPos.y > Screen.height + entitySize.y + _edgeWarpOffset.y)
        entityPos.y = -entitySize.y;

      Vector3 newPos = _camera.ScreenToWorldPoint(entityPos);

      newPos.z = 0;

      entity.transform.position = newPos;
    }
  }
}
