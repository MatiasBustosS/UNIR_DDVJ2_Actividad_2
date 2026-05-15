using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCircle : PuzzleManager
{
    public override PuzzleType puzzleType => PuzzleType.CirclePuzzle;
    
    private DiskPuzzle discA;
    private DiskPuzzle discB;
    private DiskPuzzle discC;
    
    [Header("Initial Position")]
    [SerializeField] private int posA;
    [SerializeField] private int posB;
    [SerializeField] private int posC;
    
    [Header("Solution")]
    [SerializeField] private int solutionA;
    [SerializeField] private int solutionB;
    [SerializeField] private int solutionC;
    public override bool solved { get; set; }


    private void Start()
    {
        discA = HudManager.Instance.DiskA;
        discB = HudManager.Instance.DiskB;
        discC = HudManager.Instance.DiskC;
    }

    public override void Initialize()
    {
        discA.SetInitialPos(posA);
        discB.SetInitialPos(posB);
        discC.SetInitialPos(posC);
    }

    private void Update()
    {
        
        
        
        
        
        
        
        
        
        
        if (solved) return;

        if(!discA.IsAtIndex(solutionA)) return;
        if(!discB.IsAtIndex(solutionB)) return;
        if(!discC.IsAtIndex(solutionC)) return;
            
        solved = true;
        Debug.Log("PUZZLE SOLVED");
        
    }

}
