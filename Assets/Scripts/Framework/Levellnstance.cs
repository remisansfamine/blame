using UnityEngine;

public class LevelInstance : MonoBehaviour
{
    public static LevelInstance Instance { get; private set; }

    [SerializeField] private GeneratorComponent generator;

    public GeneratorComponent Generator => generator;

    private void Awake()
    {
        // Check if there is already an instance
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another LevelInstance already exists. Destroying this one.");
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}