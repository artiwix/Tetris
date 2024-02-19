using UnityEngine;
using System.Collections.Generic;

public class ShapeMover : MonoBehaviour
{
  
  public float            MoveDownDelay = 0.8f; // Задержка движения вниз

  private float _moveDownTimer = 0; // Таймер движения вниз
  private Shape _targetShape;       // Целевая фигура
  private GameStateChanger _gameStateChanger;   // Скрипт изменения состояния игры
  private GameField        _gameField;          // Скрипт игрового поля
  private bool             _isActive;           // Флаг активности игры
                                                
  private List<Shape> _allShapes = new List<Shape>(); // Список всех фигур

  public void MoveShape(Vector2Int deltaMove)
  {
    // Если перемещение на deltaMove невозможно
    if (!CheckMovePossible(deltaMove)) { return; } // Выходим из метода
    for (int i = 0; i < _targetShape.Parts.Length; i++)  // Проходим по всем частям фигуры
    {
      // НОВОЕ: Вместо набора действий вызываем здесь метод MoveShapePart()
      MoveShapePart(_targetShape.Parts[i], deltaMove);
    }
  }

  public void InitVariables() 
  { 
    //_targetShape.InitParts();
    _gameField        = GetComponent<GameField>();
    _gameStateChanger = GetComponent<GameStateChanger>();
  }

  public void SetTargetShape(Shape targetShape) { 
    _targetShape = targetShape;
    _targetShape.InitParts();
    
    if (!_allShapes.Contains(targetShape)) // НОВОЕ: Если список не содержит целевую фигуру
      _allShapes.Add(targetShape);         // НОВОЕ: Добавляем её туда
  }

  public void SetActive(bool value) { _isActive = value; } // Присваиваем переменной _isActive значение value

  private void Update()
  {
    if (!_isActive) { return; }
    // НОВОЕ: Обозначаем блоки текущей фигуры как пустые
    SetShapePartCellsEmpty(_targetShape, true);

    HorizontalMove(); // Вызываем метод горизонтального движения
    VerticalMove();   // Вызываем метод вертикального движения

    Rotate();

     // НОВОЕ: Проверяем, достигла ли текущая фигура нижней границы поля
    bool reachBottom = CheckIfShapeTouchBottom();

    // НОВОЕ: Проверяем, есть ли другие фигуры под текущей фигурой на поле
    bool reachOtherShape = CheckOtherShape();
 
    // НОВОЕ: Обозначаем блоки текущей фигуры как заполненные
    SetShapePartCellsEmpty(_targetShape, false);

    // НОВОЕ: Если текущая фигура достигла нижней границы или есть другие фигуры под ней
    if (reachBottom || reachOtherShape)
    {
      // НОВОЕ: Вместо набора действий вызываем здесь метод EndMovement()
      EndMovement();
    }
  }

  private void HorizontalMove()
  {
    // Если была нажата клавиша влево или A
         if (Input.GetKeyDown(KeyCode.LeftArrow ) || Input.GetKeyDown(KeyCode.A)) { MoveShape(Vector2Int.left ); } // Перемещаем фигуру влево
    // Иначе, если была нажата клавиша вправо или D
    else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) { MoveShape(Vector2Int.right); } // Перемещаем фигуру вправо
  }

  private void VerticalMove()
  {
    _moveDownTimer += Time.deltaTime;  // Увеличиваем таймер на значение прошедшего времени

    // Если прошло достаточно времени или была нажата клавиша вниз или S
    if (_moveDownTimer >= MoveDownDelay || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
    {
      Debug.Log("Move!!!");
      _moveDownTimer = 0;         // Обнуляем таймер
      MoveShape(Vector2Int.down); // Перемещаем фигуру вниз
    }
  }

  private bool CheckMovePossible(Vector2Int deltaMove)
  {
    // Проходим по всем частям фигуры
    for (int i = 0; i < _targetShape.Parts.Length; i++)
    {
      Vector2Int newPartCellId = _targetShape.Parts[i].CellId + deltaMove; // Вычисляем новый номер ячейки для текущей части фигуры

      if ( newPartCellId.x < 0  // Если новая позиция выходит за границы игрового поля
        || newPartCellId.y < 0
        || newPartCellId.x >= _gameField.FieldSize.x 
        || newPartCellId.y >= _gameField.FieldSize.y
        || !_gameField.GetCellEmpty(newPartCellId)) {
        Debug.Log("CheckMovePossible return FALSE");
        return false; // Возвращаем false
      }
    }
    return true; // Иначе возвращаем true
  }

  private bool CheckIfShapeTouchBottom()
  {
    for (int i = 0; i < _targetShape.Parts.Length; i++) { // Проходим по всем частям фигуры
      if (_targetShape.Parts[i].CellId.y == 0) {          // Проверяем, находится ли текущая часть фигуры на нижней границе игрового поля (ячейка с индексом y, равным 0)
        return true;                                      // Если хотя бы одна часть фигуры находится на нижней границе игрового поля, возвращаем true
      }
    }
    
    return false; // Если ни одна часть фигуры не находится на нижней границе игрового поля, возвращаем false
  }

  private void Rotate()
  {
    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))  { // Если нажата клавиша вверх или W

      // НОВОЕ: Создаём новый массив startCellIds 
      // Присваиваем ему значения текущих позиций ячеек фигуры
      Vector2Int[] startCellIds = _targetShape.GetPartCellIds();

      _targetShape.Rotate(); // Поворачиваем фигуру
      UpdateByWalls();       // Обновляем позицию фигуры при столкновении со стенами
      UpdateByBottom();      // Обновляем позицию фигуры при столкновении с низом
      TrySetShapeInCells();     // Устанавливаем позицию фигуры в ячейках

      // НОВОЕ: Пытаемся установить фигуру в ячейки поля
      bool shapeSetted = TrySetShapeInCells();

      // НОВОЕ: Если фигуру не получилось установить
      if (!shapeSetted)
      {
        // НОВОЕ: Возвращаем её на исходную позицию
        MoveShapeToCellIds(_targetShape, startCellIds);
      }
    }
  }

  private bool TrySetShapeInCells()
  {
    for (int i = 0; i < _targetShape.Parts.Length; i++) { // Проходим по всем частям фигуры
      Vector2    shapePartPosition = _targetShape.Parts[i].transform.position;      // Получаем текущую позицию части фигуры
      Vector2Int newPartCellId     = _gameField.GetNearestCellId(shapePartPosition); // Получаем ближайший номер ячейки на игровом поле

      // НОВОЕ: Если ячейка, в которую мы пытаемся установить фигуру, уже занята
      if (!_gameField.GetCellEmpty(newPartCellId))
      {
        // НОВОЕ: Возвращаем false
        return false;
      }

      Vector2    newPartPosition   = _gameField.GetCellPosition(newPartCellId);      // Получаем позицию ячейки на игровом поле

      _targetShape.Parts[i].CellId = newPartCellId;       // Устанавливаем номер ячейки для части фигуры
      _targetShape.Parts[i].SetPosition(newPartPosition); // Устанавливаем позицию части фигуры в новой ячейке
    }
    return true;
  }

  private void UpdateByWalls()
  {
    UpdateByWall(true);  // Обновляем позицию фигуры относительно правой стены
    UpdateByWall(false);  // Обновляем позицию фигуры относительно левой стены
  }

  private void UpdateByWall(bool right)
  {
    for (int i = 0; i < _targetShape.Parts.Length; i++) {     // Проходим по всем частям фигуры по i
      if (CheckWallOver(_targetShape.Parts[i], right)) {      // Если часть фигуры выходит за стену
        for (int j = 0; j < _targetShape.Parts.Length; j++) { // Проходим по всем частям фигуры по j

          // Двигаем часть фигуры в противоположную сторону (влево или вправо) на одну ячейку
          _targetShape.Parts[j].transform.position += (right ? -1 : 1) * Vector3.right * _gameField.CellSize.x; 
        }
      }
    }
  }

  private bool CheckWallOver(ShapePart part, bool right)
  {
    float wallDistance = 0; // Задаём нулевое расстояние до стены
    float multiplier = 1;

    if (right)   // Если проверяется правая стена
    {
      // Вычисляем расстояние между позицией части фигуры и правой стеной
      wallDistance = part.transform.position.x - (_gameField.GetFirstCellPoint().position.x + (_gameField.FieldSize.x - 1) * _gameField.CellSize.x);
      wallDistance = GetRoundedWallDistance(wallDistance); // Округляем расстояние до ближайшего целого числа

      multiplier = 1f;  // Если расстояние не равно 0 и положительно
      
    } else {                                                                           // Иначе, если проверяется левая стена
      wallDistance = part.transform.position.x - _gameField.GetFirstCellPoint().position.x; // Вычисляем расстояние между позицией части фигуры и левой стеной
      wallDistance = GetRoundedWallDistance(wallDistance);                             // Округляем расстояние до ближайшего целого числа

      multiplier = -1f;  // Если расстояние не равно 0 и отрицательно
    }
    
    return IsShapeOutsideFromWall(multiplier * wallDistance); // Возвращаем false, когда ни одна часть фигуры не выходит за стену
  }

  private float GetRoundedWallDistance(float distance)
  {
    int roundValue = 100;                          // Задаём число для округления до двух знаков после запятой
    distance = Mathf.Round(distance * roundValue); // Округляем расстояние до указанного количества знаков после запятой

    return distance;                               // Возвращаем округлённое значение расстояния
  }

  private void UpdateByBottom()
  {
    for (int i = 0; i < _targetShape.Parts.Length; i++) // Проходим по всем частям фигуры по i
    { 
      if (CheckBottomOver(_targetShape.Parts[i])) // Если часть фигуры выходит за пол
      {
        for (int j = 0; j < _targetShape.Parts.Length; j++)                               // Проходим по всем частям фигуры по j
          _targetShape.Parts[j].transform.position += Vector3.up * _gameField.CellSize.y; // Двигаем часть фигуры на одну ячейку вверх
      }
    }
  }

  private bool CheckBottomOver(ShapePart part)
  {
    float wallDistance = part.transform.position.y - _gameField.GetFirstCellPoint().position.y;  // Вычисляем расстояние между позицией части фигуры и полом
          wallDistance = GetRoundedWallDistance(wallDistance);                              // Округляем расстояние до ближайшего целого числа

    return IsShapeOutsideFromWall((-1f) * wallDistance);
  }

  private bool IsShapeOutsideFromWall(float wallDistance)
  {
    if (wallDistance != 0 && wallDistance > 0) return true;

    return false;
  }

  // Проверяем касание с другой фигурой
  private bool CheckOtherShape()
  {
    Vector2Int checkingCellId; // НОВОЕ: Создаём переменную для проверки номеров ячеек
    
    // Проходим по всем частям текущей фигуры
    for (int i = 0; i < _targetShape.Parts.Length; i++)
    {
      checkingCellId = _targetShape.Parts[i].CellId + Vector2Int.down;                                   // НОВОЕ: Вычисляем номер ячейки, которая находится под текущей частью фигуры
      if (!_gameField.GetCellEmpty(checkingCellId) && !_targetShape.CheckContainsCellId(checkingCellId)) // НОВОЕ: Если ячейка заполнена и не принадлежит текущей фигуре
        return true;                                                                                     // Возвращаем true
    }
    // Возвращаем false
    return false;
  }

  // Меняем состояние блоков текущей фигуры
  private void SetShapePartCellsEmpty(Shape shape, bool value)
  {
    // Проходим по всем частям текущей фигуры
    for (int i = 0; i < shape.Parts.Length; i++)
    {
      // НОВОЕ: Вызываем метод SetShapePartCellEmpty() для каждой части фигуры
      SetShapePartCellEmpty(shape.Parts[i], value);
    }
  }

  private void SetShapePartCellEmpty(ShapePart part, bool value)
  {
    // Если часть фигуры не активна (не видна)
    if (!part.GetActive()) { return; } // Выходим из метода

    // Устанавливаем состояние ячейки (пустая или нет) для заданной части фигуры
    _gameField.SetCellEmpty(part.CellId, value);
  }

  // Перемещаем фигуру на указанные позиции ячеек
  private void MoveShapeToCellIds(Shape shape, Vector2Int[] cellIds)
  {
    // Проходим по всем частям фигуры
    for (int i = 0; i < shape.Parts.Length; i++)
    {
      // Перемещаем i-тую часть фигуры на позицию ячейки с индексом i в массиве cellIds
      MoveShapePartToCellId(shape.Parts[i], cellIds[i]);
    }
  }
  // Перемещаем часть фигуры на указанную позицию ячейки
  private void MoveShapePartToCellId(ShapePart part, Vector2Int cellId)
  {
    // Получаем новую позицию для части фигуры на основе заданной позиции ячейки
    Vector2 newPartPosition = _gameField.GetCellPosition(cellId);

    // Присваиваем части фигуры новую позицию ячейки
    part.CellId = cellId;

    // Устанавливаем позицию части фигуры на игровом поле
    part.SetPosition(newPartPosition);
  }

  private bool CheckShapeTopOver()
  {
    // Вычисляем позицию самой верхней ячейки на игровом поле
    float topCellYPosition = _gameField.GetFirstCellPoint().position.y + (_gameField.FieldSize.y - _gameField.InvisibleYFieldSize - 2) * _gameField.CellSize.y;

    // Проходим по всем частям фигуры
    for (int i = 0; i < _targetShape.Parts.Length; i++)
    {
      // Вычисляем расстояние между позицией части фигуры и потолком
      float wallDistance = _targetShape.Parts[i].transform.position.y - topCellYPosition;

      // Округляем расстояние до ближайшего целого числа
      wallDistance = GetRoundedWallDistance(wallDistance);

      // Если расстояние не равно 0 и положительно
      if (wallDistance != 0 && wallDistance > 0)
      {
        // Возвращаем true, чтобы показать, что часть фигуры касается потолка
        return true;
      }
    }
    // Возвращаем false, когда ни одна часть фигуры не касается потолка
    return false;
  }

  private void MoveShapePart(ShapePart part, Vector2Int deltaMove)
  {
    // Вычисляем новый номер ячейки для текущей части фигуры
    Vector2Int newPartCellId = part.CellId + deltaMove;

    // Перемещаем часть фигуры на новую позицию
    MoveShapePartToCellId(part, newPartCellId);
  }

  private void TryRemoveFilledRows()
  {
    bool[] rowFillings = _gameField.GetRowFillings();  // Получаем массив, который указывает, заполнена ли каждая строка игрового поля

    for (int i = rowFillings.Length - 1; i >= 0; i--) { // Проходим по каждой строке, начиная с последней
      if (rowFillings[i])                               // Если строка заполнена
        RemoveRow(i);                                   // Удаляем её
    }
  }

  private void RemoveRow(int id)
  {
    int       checkingRowId;  // Создаём переменную для проверки номера строк
    Shape     shape;          // Создаём переменную для текущей фигуры
    ShapePart part;           // Создаём переменную для текущей части фигуры

    // Проходим по каждой строке от видимого игрового поля до невидимого поля
    for (int i = 0; i < _gameField.FieldSize.y - _gameField.InvisibleYFieldSize; i++)
    {
      checkingRowId = i; // Устанавливаем проверяемый номер строки

      for (int j = 0; j < _allShapes.Count; j++) // Проходим по каждой фигуре в списке всех фигур
      {
        shape = _allShapes[j]; // Получаем текущую фигуру
 
        for (int k = 0; k < shape.Parts.Length; k++) // Проходим по каждой части фигуры
        {
          part = shape.Parts[k]; // Получаем текущую часть фигуры

          if (part.CellId.y != checkingRowId || !part.GetActive())  // Если текущая часть фигуры не находится в проверяемой строке или не активна
            continue;                                               // Переходим к следующей части в цикле с помощью специальной команды continue

          if (part.CellId.y > id)                 // Если номер строки текущей части фигуры больше, чем номер удаляемой строки
          {
            SetShapePartCellEmpty(part, true);    // Устанавливаем состояние ячейки текущей части фигуры как пустое
            MoveShapePart(part, Vector2Int.down); // Перемещаем текущую часть фигуры вниз
            SetShapePartCellEmpty(part, false);   // Устанавливаем состояние ячейки текущей части фигуры как заполненное
          }
          else if (part.CellId.y == id)           // Иначе, если номер строки текущей части фигуры равен номеру удаляемой строки
          {
            SetShapePartCellEmpty(part, true);    // Устанавливаем состояние ячейки текущей части фигуры как пустое
            shape.RemovePart(part); // Удаляем текущую часть фигуры

            if (shape.CheckNeedDestroy()) // Если фигура больше не нужна
            {
              _allShapes.Remove(shape);  // Удаляем фигуру из списка всех фигур
              Destroy(shape.gameObject); // Уничтожаем объект фигуры

                   // Уменьшаем индекс цикла на единицу
              j--; // Чтобы не пропустить следующую фигуру в списке после удаления текущей
            }
          }
        }
      }
    }
  }

  private void EndMovement()
  {
    // Если фигура достигла потолка
    if (CheckShapeTopOver()) { _gameStateChanger.EndGame(); } // Завершаем игру
    else {                                                    // Иначе
      TryRemoveFilledRows();                                  // Пробуем удалить заполненные строки
      _gameStateChanger.SpawnNextShape();                     // Запускаем новую фигуру
    }
  }
}
