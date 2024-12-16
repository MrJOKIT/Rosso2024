public interface ITakeDamage
{ 
    public void TakeDamage(int damage);
    public void AddCurseStatus(CurseType _curseType,int _turnTime); 
    public void CurseHandle();
    public void CurseUIUpdate();
}
