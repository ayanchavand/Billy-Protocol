using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance { get; private set; }

    [Header("Active Settings (overridden per scene)")]
    public GameObject ghostPrefab;
    public Material ghostMaterial;
    public LayerMask ghostLayer = 1 << 8;
    public bool infiniteGhosts = false;
    public int maxGhosts = 10;

    // Prevent re-entrant calls
    private bool isCreatingGhost = false;
    private readonly List<Ghost> ghosts = new List<Ghost>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // initial UI update
            //if (UIHandler.Instance != null)
                //UIHandler.Instance.ChangeGhostNumber(maxGhosts);
        }
        else Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var config = FindObjectOfType<LevelGhostConfig>();
        if (config != null && config.settings != null)
            ApplySettings(config.settings);
    }

    private void ApplySettings(GhostSettings s)
    {
        ghostPrefab = s.ghostPrefab;
        ghostMaterial = s.ghostMaterial;
        ghostLayer = s.ghostLayer;
        infiniteGhosts = s.infiniteGhosts;
        maxGhosts = s.maxGhosts;

        //if (UIHandler.Instance != null)
            //UIHandler.Instance.ChangeGhostNumber(maxGhosts);

        // Optional: clear old ghosts if your design wants to reset on each level
        ClearAllGhosts();
    }

    public GameObject CreateGhost(Vector3 position, Quaternion rotation, GhostState state)
    {
        // if already creating, skip this call
        if (isCreatingGhost) return null;
        isCreatingGhost = true;

        GameObject ghostGO = null;
        try
        {
            if (!ghostPrefab)
                return null;

            if (!infiniteGhosts && ghosts.Count >= maxGhosts)
            {
                var oldest = ghosts[0];
                if (oldest != null)
                    Destroy(oldest.gameObject);
                ghosts.RemoveAt(0);
            }

            ghostGO = Instantiate(ghostPrefab, position, rotation);
            SetLayerRecursively(ghostGO, ghostLayer);

            var ghostComponent = ghostGO.GetComponent<Ghost>() ?? ghostGO.AddComponent<Ghost>();
            ghostComponent.Initialize(state);

            GameStatsManager.Instance.RegisterGhost();
            ghosts.Add(ghostComponent);
        }
        finally
        {
            // ensure flag is always reset
            isCreatingGhost = false;
        }

        return ghostGO;
    }

    public void ClearAllGhosts()
    {
        foreach (var ghost in ghosts)
            if (ghost != null)
                Destroy(ghost.gameObject);
        ghosts.Clear();
    }

    public void ResetLevel()
    {
        ClearAllGhosts();
        FindObjectOfType<PlatformerController>()?.ResetToStart();
    }

    void SetLayerRecursively(GameObject obj, LayerMask layer)
    {
        int layerIndex = Mathf.RoundToInt(Mathf.Log(layer.value, 2));
        obj.layer = layerIndex;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}

[System.Serializable]
public class GhostState
{
    public enum StateType { Standing, Crouching, Jumping, Falling, Moving }

    public StateType stateType;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public bool wasGrounded;
    public float horizontalInput;

    public GhostState(StateType type, Vector3 pos, Quaternion rot, Vector3 vel, bool grounded, float input = 0)
    {
        stateType = type;
        position = pos;
        rotation = rot;
        velocity = vel;
        wasGrounded = grounded;
        horizontalInput = input;
    }
}

public class Ghost : MonoBehaviour
{
    GhostState state;

    public void Initialize(GhostState ghostState)
    {
        state = ghostState;
        ApplyState();
        SetupCollider();
        //SetupVisuals();
    }

    void ApplyState()
    {
        transform.SetPositionAndRotation(state.position, state.rotation);
        if (state.stateType == GhostState.StateType.Crouching)
            transform.localScale = new Vector3(1f, 0.5f, 1f);
    }

    void SetupCollider()
    {
        foreach (var col in GetComponents<Collider>())
            if (!(col is CharacterController)) Destroy(col);

        var collider = gameObject.AddComponent<BoxCollider>();
        var reference = FindObjectOfType<PlatformerController>()?.GetComponent<CharacterController>();

        collider.size = reference ? new Vector3(reference.radius * 2, reference.height, reference.radius * 2) : Vector3.one;
        collider.center = reference ? reference.center : Vector3.zero;

        if (state.stateType == GhostState.StateType.Crouching)
        {
            collider.size = new Vector3(collider.size.x, collider.size.y * 0.5f, collider.size.z);
            collider.center = new Vector3(collider.center.x, collider.center.y * 0.5f, collider.center.z);
        }
    }

    void SetupVisuals()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            var mat = rend.material;
            if (GhostManager.Instance.ghostMaterial)
                rend.material = GhostManager.Instance.ghostMaterial;
            else if (mat.HasProperty("_Color"))
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.5f);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        var col = GetComponent<BoxCollider>();
        if (col) Gizmos.DrawWireCube(transform.position, col.size);
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, state?.stateType.ToString() ?? "Ghost");
    }
#endif
}
