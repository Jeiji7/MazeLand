using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallImage : MonoBehaviour
{
    // Переменная для хранения загруженного префаба
    private GameObject imagePrefab;
    private GameObject Canvas;

    // Переменная для хранения инстанцированного объекта картинки
    private GameObject instantiatedImage;
    private GameObject instantiatedCanvas;
   

    void Start()
    {
        // Загрузка префаба картинки из папки Resources
        imagePrefab = Resources.Load<GameObject>("Image");

        Canvas = Resources.Load<GameObject>("Canvas");
        instantiatedCanvas = Instantiate(Canvas);
        // Убедимся, что префаб загружен успешно
        if (imagePrefab == null)
        {
            Debug.LogError("Не удалось загрузить префаб изображения из папки Resources");
        }
    }

    void Update()
    {
        // Проверяем, нажата ли клавиша Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Если картинка еще не показана, инстанцируем её
            if (instantiatedImage == null)
            {
                instantiatedImage = Instantiate(imagePrefab, instantiatedCanvas.transform);
            }
            else
            {
                // Если картинка уже есть, убираем её
                Destroy(instantiatedImage);
            }
        }
    }
}
