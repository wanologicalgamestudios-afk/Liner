using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }
    public float ElapsedTime { get; private set; }
    public bool IsRunning { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (IsRunning)
            ElapsedTime += Time.deltaTime;
    }

    public void StartTimer()
    {
        ElapsedTime = 0f;
        IsRunning = true;
    }

    public void StopTimer()
    {
        IsRunning = false;
    }
}