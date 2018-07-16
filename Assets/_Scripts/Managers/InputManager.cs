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
    private string _shootInput = "Fire1";
    [SerializeField]
    private string _warpInput = "Fire2";

    public float Steering
    {
      get { return Input.GetAxis(_steeringInput); }
    }

    public float Thruster
    {
      get { return Input.GetAxis(_thrusterInput); }
    }

    public float Shoot
    {
      get { return Input.GetAxis(_shootInput); }
    }

    public float Warp
    {
      get { return Input.GetAxis(_warpInput); }
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
