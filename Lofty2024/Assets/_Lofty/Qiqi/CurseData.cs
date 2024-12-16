using System;

[Serializable]
public class CurseData
{
    public int CurseIndex;
    public CurseType CurseType;
    public int CurseTurn;
    public CurseUI CurseUI;
    public bool CurseActivated;
    
    public CurseData(CurseType _curseType,int _curseTurn, CurseUI _curseUI)
    {
        this.CurseType = _curseType;
        this.CurseTurn = _curseTurn;
        this.CurseUI = _curseUI;
    }
}
