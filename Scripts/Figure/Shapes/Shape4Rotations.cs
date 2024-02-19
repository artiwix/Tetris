using UnityEngine;

public class Shape4Rotations : Shape
{
  // Переопределённый метод вращения фигуры
  public override void Rotate()
  {
    Vector2 rotatePosition = Parts[0].transform.position;  // Получаем позицию части, которую будем использовать как центр для поворотов

    for (int i = 0; i < Parts.Length; i++)  {                                // Проходим по всем частям фигуры
      Parts[i].transform.RotateAround(rotatePosition, Vector3.forward, 90f); // Поворачиваем часть вокруг позиции, которую мы выбрали, чтобы изменить её положение
      Parts[i].transform.Rotate(Vector3.forward, -90f);                      // Поворачиваем часть на -90 градусов вокруг оси Z, чтобы её спрайт всегда отображался вертикально
    }
  }
}