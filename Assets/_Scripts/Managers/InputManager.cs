using UnityEngine;

namespace Asteroids
{
  public class InputManager : MonoBehaviour
  {

    private static InputManager _instance;

    public static InputManager Instance
    {
      get { return _instance; }
    }

    [SerializeField]
    private string _steeringInput = "Horizontal";
    [SerializeField]
    private string _thrusterInput = "Vertical";
    [SerializeField]
    private string _shootInput = "Attack 1";
    [SerializeField]
    private string _warpInput = "Attack 2";

    public float Steering
    {
      get { return Input.GetAxis(_steeringInput); }
    }

    public float Thruster
    {
      get { return Input.GetAxis(_thrusterInput); }
    }

    public bool Shoot
    {
      get { return Input.GetKeyDown(_shootInput); }
    }

    public bool Warp
    {
      get { return Input.GetKeyDown(_warpInput); }
    }

    private void Awake()
    {
      if (_instance != null)
      {
        Destroy(this);

        return;
      }

      _instance = this;
    }

  }
}
