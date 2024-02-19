using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public float LineWidth = 1.0f;
  private int _currentLineIndex = 0;

  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
    if(Input.GetKeyDown(KeyCode.A) && _currentLineIndex > -1)
    {
      _currentLineIndex--;
    }

    if (Input.GetKeyDown(KeyCode.D) && _currentLineIndex > -1)
    {
      _currentLineIndex++;
    }
    transform.position = new Vector3(_currentLineIndex * LineWidth, 0, 0);
  }
}
