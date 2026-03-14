// Data structure representing the result of an attack
public struct AttackResult
{
    public bool wasAttempted;
    public bool wasHit;
    public int attackRoll;
    public int targetAC;
    public int damageDealt;
    public string failureReason; // If wasAttempted is false, explains why

    public AttackResult(bool wasAttempted, bool wasHit, int attackRoll, int targetAC, int damageDealt, string failureReason = "")
    {
        this.wasAttempted = wasAttempted;
        this.wasHit = wasHit;
        this.attackRoll = attackRoll;
        this.targetAC = targetAC;
        this.damageDealt = damageDealt;
        this.failureReason = failureReason;
    }
}
