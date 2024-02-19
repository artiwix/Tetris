using UnityEngine;

public class GameStateChanger : MonoBehaviour
{
  
  public GameObject GameScreen;     // Экран игры 
  public GameObject GameEndScreen; // Экран конца игры

  private ShapeSpawner _shapeSpawner; // Скрипт появления фигур
  private GameField  _gameField;  // Скрипт игрового поля
  private ShapeMover _shapeMover; // Скрипт движения фигур

  public void EndGame()
  {
    SwitchScreens(false);         // Устанавливаем экран конца игры
    _shapeMover.SetActive(false); // Отключаем движение фигур
  }

  public void RestartGame()
  {
    // Логику перезапуска допишем позже
  }

  public void SpawnNextShape()
  {
    Shape nextShape = _shapeSpawner.SpawnNextShape();  // Создаём переменную nextShape, в которую записываем следующую фигуру, сгенерированную ShapeSpawner
    _shapeMover.SetTargetShape(nextShape);             // Устанавливаем следующую фигуру в ShapeMover, который отвечает за перемещение фигур

    // Сдвигаем фигуру в заданную позицию на игровом поле
    _shapeMover.MoveShape(Vector2Int.right * (int)(_gameField.FieldSize.x * 0.5f) + 
                             Vector2Int.up * (_gameField.FieldSize.y - _gameField.InvisibleYFieldSize + nextShape.ExtraSpawnYMove));
  }

  private void FirstStartGame()
  {
    _gameField.FillCellsPositions(); // Заполняем ячейки игрового поля
    _shapeMover.InitVariables();

    StartGame();
  }

  private void StartGame()
  {
    SpawnNextShape();             // Показываем новую фигуру
    SwitchScreens(true);          // Устанавливаем экран игры
    _shapeMover.SetActive(true);   // Включаем движение фигур
  }

  private void Start()
  {
    InitVariables();
    FirstStartGame(); // Вызываем метод FirstStartGame();
  }

  private void InitVariables()
  {
    _gameField    = GetComponent<GameField>();
    _shapeMover   = GetComponent<ShapeMover>();
    _shapeSpawner = GetComponent<ShapeSpawner>();
  }

  

  private void SwitchScreens(bool isGame)
  {
    GameScreen.SetActive(isGame);     // Активируем экран игры
    GameEndScreen.SetActive(!isGame); // Скрываем экран завершения игры
  }
}