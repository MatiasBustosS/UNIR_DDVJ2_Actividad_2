using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public static HudManager Instance;

    [Header("GameObjects")]
    [SerializeField] private Transform aim;
    [SerializeField] private Transform Slider;
    [SerializeField] private Transform PipeMania;
    [SerializeField] private Transform FlowFree;
    [SerializeField] private Transform CirclePuzzle;

    [Header("Puzzles UI")]
    [SerializeField] private CircleBoard circlePuzzle;
    [SerializeField] private FlowBoard flowFreePuzzle;
    [SerializeField] private ManiaBoard pipeManiaPuzzle;

    public CircleBoard circleBoard => circlePuzzle;
    public FlowBoard flowBoard => flowFreePuzzle;
    public ManiaBoard pipeManiaBoard => pipeManiaPuzzle;
    
    private PuzzleManager puzzleManager;
    
    private bool isTarget = false;
    
    public bool IsTarget => isTarget;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }


    public void OnTarget(bool target)
    {
        aim.localScale = !target ? Vector3.one : Vector3.one * 2f;
    }

    public void SetPuzzleManager(PuzzleManager manager)
    {
        puzzleManager = manager;
    }
    
    public void OpenPuzzle(PuzzleType getType)
    {
        if(isTarget) return;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        aim.gameObject.SetActive(false);
        
        isTarget = true;
        
        print(getType.ToString());
        switch (getType)
        {
            case PuzzleType.Slider:
                Slider.gameObject.SetActive(true);
                break;
            case PuzzleType.PipeMania:
                PipeMania.gameObject.SetActive(true);
                break;
            case PuzzleType.FlowFree:
                FlowFree.gameObject.SetActive(true);
                break;
            case PuzzleType.CirclePuzzle:
                CirclePuzzle.gameObject.SetActive(true);
                break;
            
            default:
                break;
        }
    }

    public void CheckWin()
    {
        puzzleManager.CheckWin();
    }
    
    public void ClosePuzzle()
    {
        isTarget = false;
        aim.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
