using UnityEngine;

public class GameField : MonoBehaviour
{
  // Количество скрытых ячеек сверху поля
  public int InvisibleYFieldSize = 4;
  public Vector2 CellSize;         // Размер ячейки (по X и Y)
  public Vector2Int FieldSize;     // Размер поля (по X и Y)
  
  private GameFieldCell[,] _cells; // двухмерный массив из позиций каждой ячейки
  private Transform FirstCellPoint; // Позиция первой ячейки

  public void FillCellsPositions()
  {
    FirstCellPoint = transform.GetChild(0);
    _cells = new GameFieldCell[FieldSize.x, FieldSize.y]; // Создаём двухмерный массив ячеек с заданными размерами

    for (int x = 0; x < FieldSize.x; x++) { // Проходим по первым координатам всех ячеек (i)
      for (int y = 0; y < FieldSize.y; y++) { // Проходим по вторым координатам всех ячеек (j)
        
        // Вычисляем позицию ячейки на основе позиции первой ячейки, размеров ячейки и значений i, j
        Vector2 cellPosition = (Vector2)FirstCellPoint.position + Vector2.right * x * CellSize.x + Vector2.up * y * CellSize.y;

        GameFieldCell newCell = new GameFieldCell(cellPosition); // Создаём новую ячейку с вычисленной позицией
        _cells[x, y] = newCell;                                  // Записываем созданную ячейку в соответствующую позицию двухмерного массива ячеек
      }
    }
  }

  public Vector2   GetCellPosition(Vector2Int cellId) { return GetCellPosition(GetCell(cellId.x, cellId.y)); } // Получаем ячейку по заданным координатам
  public Vector2   GetCellPosition(int x, int y)      { return GetCellPosition(GetCell(x       , y       )); } // Получаем ячейку по её координатам
  public Transform GetFirstCellPoint()                { return FirstCellPoint; }

  public void SetCellEmpty(Vector2Int cellId, bool value) // Делаем ячейку пустой
  {
    GameFieldCell cell = GetCell(cellId.x, cellId.y); // Получаем ячейку по указанным координатам

    if (cell != null) {               // Если такая ячейка есть
      cell.IsEmpty = value; // Устанавливаем значение пустоты ячейки
    }
  }
  
  public bool GetCellEmpty(Vector2Int cellId)          // Получаем значение пустоты ячейки
  {
    GameFieldCell cell = GetCell(cellId.x, cellId.y); // Получаем ячейку по указанным координатам
    
    if (cell != null) {                 // Если такая ячейка есть
      return cell.IsEmpty; // Устанавливаем значение пустоты ячейки
    }

    return false;
  }

  private Vector2 GetCellPosition(GameFieldCell cell) {
    if (cell == null) { return Vector2.zero; } // Если такой ячейки нет
    return cell.Position;                      // Иначе возвращаем позицию ячейки
  }

  public Vector2Int GetNearestCellId(Vector2 position)
  {
    // Записываем в переменную resultDistance максимально возможное значение между проверяемой позицией и ячейкой поля
    // То есть мы находим ближайшую ячейку к заданной
    float resultDistance = float.MaxValue;

    int resultX = 0, resultY = 0;                     // Записываем в переменные resultX и resultY нули
    for (int x = 0; x < FieldSize.x; x++) {           // Проходим по всем значениям X игрового поля
      for (int y = 0; y < FieldSize.y; y++) {         // Проходим по всем значениям Y игрового поля
        
        Vector2 cellPosition = GetCellPosition(x, y); // Получаем позицию ячейки с координатами x, y

        float distance = (cellPosition - position).magnitude; // Вычисляем расстояние между текущей ячейкой и переданной позицией
        if (distance < resultDistance) {                      // Если текущее расстояние меньше resultDistance
          resultDistance = distance;                          // Записываем в resultDistance новое значение distance
          resultX = x;                                        // Записываем в resultX новое значение i
          resultY = y;                                        // Записываем в resultY новое значение j
        }
      }
    }
    
    return new Vector2Int(resultX, resultY);                  // Возвращаем новый вектор Vector2Int, который означает номер заданной ячейки
  }

  public bool[] GetRowFillings()
  {
    // Создаём массив значений заполненности строк
    // Размер массива = высота поля - невидимая область
    bool[] rowFillings = new bool[FieldSize.y - InvisibleYFieldSize];
    bool isRowFilled; // Флаг заполненности текущей строки

    for (int j = 0; j < rowFillings.Length; j++)  // Проходим по всем строкам игрового поля
    { 
      // Предполагаем, что текущая строка полностью заполнена
      isRowFilled = true;
      for (int i = 0; i < FieldSize.x; i++) // Проходим по всем столбцам игрового поля
      {
        if (_cells[i, j].IsEmpty)      // Если текущая ячейка пуста
        {
          isRowFilled = false;              // То текущая строка не заполнена
          break;                            // Выходим из цикла
        }
      }
      // Записываем значение заполненности текущей строки в соответствующий элемент массива
      rowFillings[j] = isRowFilled;
    }
    // Возвращаем массив значений заполненности строк
    return rowFillings;
  }

  private GameFieldCell GetCell(int x, int y)
  {
    // Если номер ячейки некорректный
    if (x < 0 || y < 0 || x >= FieldSize.x || y >= FieldSize.y) {
      return null; // Возвращаем пустое значение null
    }
   
    return _cells[x, y];  // Иначе возвращаем ячейку с заданными координатами в двухмерном массиве ячеек
  } 
}
