using UnityEngine;

public class GameFieldCell
{
  public Vector2 Position { get; }  // Позиция ячейки на игровом поле
  public bool IsEmpty { get; set; }     // Флаг пустоты ячейки

  // Создаём объект класса GameFieldCell
  public GameFieldCell(Vector2 position) {
    Position = position;   // Присваиваем позиции переданное значение
    IsEmpty = true;       // Делаем ячейку изначально пустой
  }
  
  //public Vector2 GetPosition() { return _position; }  // Получаем позицию ячейки
  
  /*public bool GetIsEmpty() { return _isEmpty;  }  // Узнаём, пуста ли ячейка
  public void SetIsEmpty(bool value) { 
    _isEmpty = value; }  // Делаем ячейку пустой или заполненной*/
}