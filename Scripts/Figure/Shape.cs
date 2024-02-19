using UnityEngine;

public abstract class Shape : MonoBehaviour
{
  // Переменная для дополнительного смещения двух фигур
  // Объявим её позже на префабах в инспекторе
  public int ExtraSpawnYMove;

  // Массив объектов типа ShapePart с начальным размером 0
  public ShapePart[] Parts = null;

  public void InitParts() {
    Parts = GetComponentsInChildren<ShapePart>();
  }

  public abstract void Rotate();
  public Vector2Int[] GetPartCellIds()
  {
    // Создаём новый массив типа Vector2Int с размером, равным длине массива частей фигуры
    Vector2Int[] startCellIds = new Vector2Int[Parts.Length];

    // Проходим по всем частям фигуры
    for (int i = 0; i < Parts.Length; i++)
    {
      // Записываем в элемент startCellIds[i] значение номера ячейки i-того элемента массива Parts
      startCellIds[i] = Parts[i].CellId;
    }

    // Возвращаем массив startCellIds
    return startCellIds;
  }

  // Удаляем часть фигуры
  public void RemovePart(ShapePart part)
  {
    for (int i = 0; i < Parts.Length; i++) {  // Проходим по всем частям фигуры
      if (Parts[i] == part)                   // Если текущая часть равна части, которую нужно удалить
        part.SetActive(false);                // Устанавливаем активность части в false (она становится невидимой)
    }
  }

  // Проверяем, нужно ли удалить фигуру
  public bool CheckNeedDestroy()
  {
    for (int i = 0; i < Parts.Length; i++) { // Проходим по всем частям фигуры
      if (Parts[i].GetActive())              // Если активность текущей части равна true (её видно)
        return false;                        // Возвращаем false (фигуру не нужно удалять)
    }
    // Если все части фигуры не видимы, возвращаем true
    // В этом случае фигуру нужно удалить
    return true;
  }

  // Проверяем, есть ли в фигуре часть с заданным номером
  public bool CheckContainsCellId(Vector2Int cellId)
  {
    for (int i = 0; i < Parts.Length; i++) {  // Проходим по всем частям фигуры
      if (Parts[i].CellId == cellId)          // Если номер текущей части равен указанному
        return true;                          // Возвращаем true
    }
    return false;                             // Если не нашли такой номер, возвращаем false
  }
}