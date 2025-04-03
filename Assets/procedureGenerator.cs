using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int chunkSize = 50;          // Размер чанка
    public int resolution = 20;         // Разрешение для сглаживания
    public float amplitude = 3f;        // Амплитуда шума
    public float frequency = 0.01f;     // Частота шума
    public float offset = 0.5f;
    public int seed = 42;               // Сид для генерации
    public Transform player;            // Игрок для отслеживания
    public GameObject groundPrefab;     // Префаб земли
    public int maxChunks = 5;           // Максимальное количество чанков в сцене

    private List<GameObject> chunks = new List<GameObject>();  // Список чанков
    private float lastX = 0;            // Последняя X позиция
    private float lastY = 0;            // Последняя Y позиция
    private System.Random rand;         // Случайный генератор

    void Start()
    {
        rand = new System.Random(seed); // Инициализация генератора случайных чисел
        GenerateInitialChunks();        // Генерация начальных чанков
    }

    void Update()
    {
        // Генерация новых чанков, когда игрок перемещается
        if (player.position.x > lastX - (chunkSize * 2) && chunks.Count < maxChunks)
        {
            GenerateChunk();
        }
    }

    // Генерация начальных чанков
    void GenerateInitialChunks()
    {
        for (int i = 0; i < maxChunks; i++)
        {
            GenerateChunk();
        }
    }

    // Генерация нового чанка
    void GenerateChunk()
    {
        if (chunks.Count >= maxChunks)
        {
            Destroy(chunks[0]);
            chunks.RemoveAt(0);
        }

        // Проверяем, если у нас есть groundPrefab, используем его
        GameObject chunk = groundPrefab != null ? Instantiate(groundPrefab) : new GameObject("Chunk");

        // Получаем или добавляем LineRenderer
        LineRenderer line = chunk.GetComponent<LineRenderer>();

        if (line == null)
        {
            // Если нет компонента LineRenderer, добавляем его
            line = chunk.AddComponent<LineRenderer>();
        }

        line.useWorldSpace = true;
        line.loop = false;
        line.positionCount = chunkSize * resolution;

        // Настройки LineRenderer (ширина линии, материал и цвет)
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        line.material = new Material(Shader.Find("Sprites/Default")); // Убедитесь, что у вас есть подходящий материал

        List<Vector3> controlPoints = new List<Vector3>();

        // Если чанки уже есть, начинаем с последней точки
        if (chunks.Count > 0)
        {
            controlPoints.Add(new Vector3(lastX, lastY, 0));
        }

        // Генерация высоты для каждой точки
        for (int i = 0; i < chunkSize; i++)
        {
            float x = lastX + i;
            float y = GetTerrainHeight(x);
            controlPoints.Add(new Vector3(x, y, 0));
        }

        // Генерация плавных точек для линии
        Vector3[] smoothedPoints = GenerateSmoothPath(controlPoints, resolution);
        line.SetPositions(smoothedPoints);

        chunks.Add(chunk);

        // Обновляем последние координаты для следующего чанка
        lastX += chunkSize;
        lastY = controlPoints[controlPoints.Count - 1].y;
    }

    float GetTerrainHeight(float x)
    {

        // Перлин-шум для X-координаты
        float noise = Mathf.PerlinNoise(x * frequency, 0f); // Параметр 0f - это постоянное значение для Y
        noise = noise * 2f - 1f; // Преобразуем шум из диапазона [0, 1] в диапазон [-1, 1]

        // Применяем амплитуду и смещение
        return noise * amplitude + offset;
    }

    Vector3[] GenerateSmoothPath(List<Vector3> points, int resolution)
    {
        List<Vector3> smoothPath = new List<Vector3>();

        // Проходим по точкам и генерируем промежуточные, чтобы линия была гладкой
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p0 = i > 0 ? points[i - 1] : points[i];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = i < points.Count - 2 ? points[i + 2] : points[i + 1];

            // Интерполяция между точками
            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)resolution;
                Vector3 interpolatedPoint = CatmullRom(p0, p1, p2, p3, t);
                smoothPath.Add(interpolatedPoint);
            }
        }

        return smoothPath.ToArray();
    }

    // Метод CatmullRom для плавной интерполяции между точками
    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2 * p1) +
            (-p0 + p2) * t +
            (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
            (-p0 + 3 * p1 - 3 * p2 + p3) * t3
        );
    }
}