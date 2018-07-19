using UnityEngine;

namespace Asteroids
{
  public class LevelManager : MonoBehaviour
  {

    private static LevelManager _instance;

    public static LevelManager Instance
    {
      get { return _instance; }
    }

    private const int MAX_ENEMY_BULLETS = 10;
    private const int MAX_PLAYER_BULLETS = 5;

    [SerializeField]
    private int _difficulty = 0;
    [SerializeField]
    private int _maxEnemies = 2;
    [SerializeField]
    private int _maxAsteroids = 4;
    [SerializeField]
    private int _initialLives = 3;
    [SerializeField]
    private float _bulletSpeed = 5;
    
    [Header("Prefabs")]

    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _asteroidPrefab;
    [SerializeField]
    private GameObject _playerBulletPrefab;
    [SerializeField]
    private GameObject _playerBulletHitPrefab;
    [SerializeField]
    private GameObject _enemyBulletPrefab;
    [SerializeField]
    private GameObject _enemyBulletHitPrefab;

    [Space]

    [SerializeField]
    private Transform _stageRoot;
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
    private Transform[] _enemyBullets;
    private Transform[] _playerBullets;

    private float _inputSteer;
    private float _inputThrust;
    private bool _inputShoot;
    private bool _inputWarp;

    private float _lastThrustTime;
    private float _lastShootTime;
    private readonly float _thrustDecayTime = .5f;
    private readonly float _shootTimeInterval = .5f;

    public Player Player
    {
      get { return _player; }
    }

    public int Level
    {
      get { return _difficulty + 1; }
    }

    #region Unity Lifecycle

    private void Awake()
    {
      if (_instance != null)
      {
        Destroy(this);

        return;
      }

      _instance = this;

      SetupLevel();
      SetupPlayer();
    }

    private void Start()
    {
      _camera = Camera.main;

      AudioManager.Instance.StopMusic();
      AudioManager.Instance.LevelMusic();
    }

    private void FixedUpdate()
    {
      UpdatePlayer();
      UpdateAsteroids();
      UpdateEnemies();
      UpdateBullets();
    }

    private void Update()
    {
      UpdateInputs();
    }

    #endregion

    public void SetupLevel(int difficulty)
    {
      _difficulty = difficulty;

      SetupLevel();
    }

    private void SetupLevel()
    {
      if (_enemyBullets == null)
      {
        _enemyBullets = new Transform[MAX_ENEMY_BULLETS];

        for (int i = 0; i < MAX_ENEMY_BULLETS; i++)
        {
          GameObject instance = Instantiate(_enemyBulletPrefab, _stageRoot);

          instance.SetActive(false);

          _enemyBullets[i] = instance.transform;
        }
      }

      if (_playerBullets == null)
      {
        _playerBullets = new Transform[MAX_PLAYER_BULLETS];

        for (int i = 0; i < MAX_PLAYER_BULLETS; i++)
        {
          GameObject instance = Instantiate(_playerBulletPrefab, _stageRoot);

          instance.SetActive(false);

          _playerBullets[i] = instance.transform;
        }
      }

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
          GameObject enemyInstance = Instantiate(_enemyPrefab, _enemiesRoot);

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
          GameObject asteroidInstance = Instantiate(_asteroidPrefab, _asteroidsRoot);

          _asteroids[i] = asteroidInstance.GetComponent<Meteor>();
        }

        _asteroids[i].transform.position = Vector3.zero;
      }
    }

    private void SetupPlayer()
    {
      if (_player == null)
      {
        GameObject playerInstance = Instantiate(_playerPrefab, _stageRoot);

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

      if (_inputShoot)
        AddPlayerBullet();

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

        AddEnemyBullet(enemy.transform);

        EdgeWarp(enemy);
      }
    }

    private void UpdateBullets()
    {
      for (int i = 0; i < MAX_PLAYER_BULLETS; i++)
      {
        Transform bullet = _playerBullets[i];

        if (!bullet.gameObject.activeSelf)
          continue;

        Vector3 newPos = bullet.position;

        newPos += bullet.up * _bulletSpeed * Time.fixedDeltaTime;

        bullet.position = newPos;

        EdgeDisable(bullet);
      }

      for (int i = 0; i < MAX_ENEMY_BULLETS; i++)
      {
        Transform bullet = _enemyBullets[i];

        if (!bullet.gameObject.activeSelf)
          continue;

        Vector3 newPos = bullet.position;

        newPos += bullet.up * _bulletSpeed * Time.fixedDeltaTime;

        bullet.position = newPos;

        EdgeDisable(bullet);
      }
    }

    private void AddPlayerBullet()
    {
      if (_lastShootTime >= Time.fixedTime - _shootTimeInterval)
        return;

      for (int i = 0; i < MAX_PLAYER_BULLETS; i++)
      {
        Transform bullet = _playerBullets[i];

        if (bullet.gameObject.activeSelf)
          continue;

        bullet.SetPositionAndRotation(_player.transform.position, _player.transform.rotation);
        bullet.gameObject.SetActive(true);

        _lastShootTime = Time.fixedTime;

        AudioManager.Instance.Laser();

        break;
      }
    }

    private void AddEnemyBullet(Transform entity)
    {

    }

    private void EdgeDisable(Transform entity)
    {
      if (entity == null || !entity.gameObject.activeSelf)
        return;
      
      Vector3 screenPos = _camera.WorldToScreenPoint(entity.position);

      if (screenPos.x < -_edgeWarpOffset.x ||
          screenPos.x > Screen.width + _edgeWarpOffset.x ||
          screenPos.y < -_edgeWarpOffset.y ||
          screenPos.y > Screen.height + _edgeWarpOffset.y)
        entity.gameObject.SetActive(false);
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
