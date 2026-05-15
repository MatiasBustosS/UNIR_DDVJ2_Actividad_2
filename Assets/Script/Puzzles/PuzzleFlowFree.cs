using UnityEngine;

public class PuzzleFlowFree : PuzzleManager
{
    public override PuzzleType puzzleType => PuzzleType.FlowFree;
    public override bool solved { get; set; }
    
    
    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }

}

