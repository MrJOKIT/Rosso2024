public interface ITakeDamage
{ 
    public void TakeDamage(int damage);
    public void AddCurseStatus(CurseType curseType,int turnTime); 
    public void CurseHandle();
    public void CurseUiUpdate();
}
