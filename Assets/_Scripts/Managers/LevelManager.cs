using NUnit.Framework;
using UnityEngine;
using Random = System.Random;

namespace Asteroids
{
  public class LevelManager : MonoBehaviour
  {

    private static LevelManager _instance;
    private static Random _rnd;

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

    private int _aliveEntities;

    private float _inputSteer;
    private float _inputThrust;
    private bool _inputShoot;
    private bool _inputWarp;

    private float _lastThrustTime;
    private float _lastPlayerShootTime;
    private float _lastEnemyShootTime;
    private float _warpCountdown;
    private float _immortalCountdown;
    private readonly float _thrustDecayTime = .5f;
    private readonly float _shootTimeInterval = .5f;
    private readonly float _warpInterval = 5f;
    private readonly float _immortalInterval = 2f;

    public Player Player
    {
      get { return _player; }
    }

    public int Level
    {
      get { return _difficulty + 1; }
    }

    public float WarpReady
    {
      get { return 1 - _warpCountdown / _warpInterval; }
    }

    public int Alive
    {
      get { return _aliveEntities; }
      set { _aliveEntities = value; }
    }

    public bool PlayerImmortal
    {
      get { return _immortalCountdown > 0; }
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
      _rnd = new Random();
    }

    private void Start()
    {
      _camera = Camera.main;

      SetupLevel();
      SetupPlayer();

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

      if (_aliveEntities < 1)
        SetupLevel(_difficulty + 1);
    }

    #endregion

    public void SetupLevel(int difficulty = 0)
    {
      _difficulty = difficulty;

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

      _aliveEntities = _enemies.Length + _asteroids.Length;

      _immortalCountdown = _immortalInterval;
    }

    public void PlayerRespawn()
    {
      _player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

      _immortalCountdown = _immortalInterval;

      _player.gameObject.SetActive(true);
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

        _enemies[i].transform.position = _camera.ScreenToWorldPoint(new Vector3(_rnd.Next(0, Screen.width), _rnd.Next(0, Screen.height), 0f));
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

        _asteroids[i].transform.position = _camera.ScreenToWorldPoint(new Vector3(_rnd.Next(0, Screen.width), _rnd.Next(0, Screen.height), 0f));
        _asteroids[i].Rigidbody.AddForce(_asteroids[i].transform.forward * 10f);
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

      _immortalCountdown = _immortalInterval;

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

      _immortalCountdown -= Time.fixedDeltaTime;

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

      _warpCountdown -= Time.fixedDeltaTime;

      if (_inputWarp)
        PlayerWarp();

      EdgeWarp(_player);
    }

    private void UpdateAsteroids()
    {
      for (int i = 0; i < _maxAsteroids; i++)
      {
        Meteor meteor = _asteroids[i];

        if (meteor == null || meteor.Dead)
          continue;

        meteor.transform.Rotate(new Vector3(0, 0, 5f * Time.fixedDeltaTime));

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

        enemy.transform.Rotate(new Vector3(0, 0, enemy.RotationSpeed * Time.fixedDeltaTime));
        enemy.Rigidbody.AddForce(enemy.transform.up * enemy.MoveSpeed * Time.fixedDeltaTime);

        AddEnemyBullet(enemy.transform);

        EdgeWarp(enemy);
      }
    }

    private void UpdateBullets()
    {
      // Player bullets:
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

      // Enemies bullets:
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
      if (_lastPlayerShootTime >= Time.fixedTime - _shootTimeInterval)
        return;

      for (int i = 0; i < MAX_PLAYER_BULLETS; i++)
      {
        Transform bullet = _playerBullets[i];

        if (bullet.gameObject.activeSelf)
          continue;

        bullet.SetPositionAndRotation(_player.transform.position, _player.transform.rotation);
        bullet.gameObject.SetActive(true);

        _lastPlayerShootTime = Time.fixedTime;

        AudioManager.Instance.Laser();

        break;
      }
    }

    private void AddEnemyBullet(Transform entity)
    {
      float interval = _shootTimeInterval * 2f - _shootTimeInterval / 100f * _difficulty;

      if (_immortalCountdown > 0 || _lastEnemyShootTime >= Time.fixedTime - interval)
        return;

      for (int e = 0; e < _maxEnemies; e++)
      {
        Enemy enemy = _enemies[e];

        if (!enemy.gameObject.activeSelf || enemy.Dead)
          continue;

        for (int i = 0; i < MAX_ENEMY_BULLETS; i++)
        {
          Transform bullet = _enemyBullets[i];

          if (bullet.gameObject.activeSelf)
            continue;

          Vector3 direction = _player.transform.position - enemy.transform.position;

          direction.z = 0;

          bullet.SetPositionAndRotation(enemy.transform.position, Quaternion.FromToRotation(Vector3.up, direction));
          bullet.Rotate(new Vector3(0f, 0f, _rnd.Next(10, 35)));

          bullet.gameObject.SetActive(true);

          _lastEnemyShootTime = Time.fixedTime;

          AudioManager.Instance.Laser();

          break;
        }
      }
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

    private void PlayerWarp()
    {
      if (_warpCountdown > 0)
        return;

      Vector3 newPos = _camera.ScreenToWorldPoint(new Vector3(_rnd.Next(0, Screen.width), _rnd.Next(0, Screen.height), 0));

      _player.transform.position = newPos;
      _player.transform.Rotate(new Vector3(0, 0, (float) (_rnd.NextDouble() * 360f)));

      _warpCountdown = _warpInterval;
    }

  }
}
