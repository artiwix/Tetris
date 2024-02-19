using UnityEngine;

public class ShapePart : MonoBehaviour
{
  public Vector2Int CellId; // Координаты ячейки (X, Y)

  // Устанавливаем позицию ячейки
  public void SetPosition(Vector2 position) { transform.position = position; }
  
  // Управляем активностью (видимостью) части фигуры
  public void SetActive(bool value) { gameObject.SetActive(value); } // Устанавливаем переданное значение активности
  
  // Получаем значение активности (видимости) части фигуры
  public bool GetActive() { return gameObject.activeSelf; }

}