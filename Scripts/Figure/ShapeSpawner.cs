using UnityEngine;

public class ShapeSpawner : MonoBehaviour
{
  // Массив префабов фигур, пока пустой
  // Заполним его позже в инспекторе
  public Shape[] ShapePrefabs = new Shape[0];

  // Метод для появления случайной фигуры
  public Shape SpawnNextShape() {
    
    Shape randomPrefab = ShapePrefabs[Random.Range(0, ShapePrefabs.Length)]; // Выбираем случайную фигуру из массива
    return Instantiate(randomPrefab);                                        // Возвращаем объект этой фигуры
  }
}
