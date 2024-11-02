using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallImage : MonoBehaviour
{
    // ���������� ��� �������� ������������ �������
    private GameObject imagePrefab;
    private GameObject Canvas;

    // ���������� ��� �������� ����������������� ������� ��������
    private GameObject instantiatedImage;
    private GameObject instantiatedCanvas;
   

    void Start()
    {
        // �������� ������� �������� �� ����� Resources
        imagePrefab = Resources.Load<GameObject>("Image");

        Canvas = Resources.Load<GameObject>("Canvas");
        instantiatedCanvas = Instantiate(Canvas);
        // ��������, ��� ������ �������� �������
        if (imagePrefab == null)
        {
            Debug.LogError("�� ������� ��������� ������ ����������� �� ����� Resources");
        }
    }

    void Update()
    {
        // ���������, ������ �� ������� Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ���� �������� ��� �� ��������, ������������ �
            if (instantiatedImage == null)
            {
                instantiatedImage = Instantiate(imagePrefab, instantiatedCanvas.transform);
            }
            else
            {
                // ���� �������� ��� ����, ������� �
                Destroy(instantiatedImage);
            }
        }
    }
}
