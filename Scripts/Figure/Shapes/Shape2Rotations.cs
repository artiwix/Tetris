using UnityEngine;

public class Shape2Rotations : Shape
{
  
  bool _rotated = false; // Флаг состояния поворота

  
  public override void Rotate()  // Переопределённый метод вращения фигуры
  { 
    // Определяем множитель для поворота в зависимости от текущего состояния поворота
    // Если фигура уже повёрнута, устанавливаем отрицательный множитель
    float rotateMultiplier = _rotated ? -1 : 1;
    Vector2 rotatePosition = Parts[0].transform.position;  // Получаем позицию части, которую будем использовать для поворота 
 
    for (int i = 0; i < Parts.Length; i++) {                                                    // Проходим по всем частям фигуры
      Parts[i].transform.RotateAround(rotatePosition, Vector3.forward, 90f * rotateMultiplier); // Поворачиваем часть вокруг позиции, которую мы выбрали, чтобы изменить её положение
      Parts[i].transform.Rotate(Vector3.forward, -90f * rotateMultiplier);                      // Поворачиваем часть на -90 градусов вокруг оси Z, чтобы её спрайт всегда отображался вертикально
    }

    _rotated = !_rotated;   // Меняем значение флага для следующего поворота
  }
}