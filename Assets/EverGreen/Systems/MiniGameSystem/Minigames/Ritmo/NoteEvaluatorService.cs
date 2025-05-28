using UnityEngine;

public class NoteEvaluatorService
{
    public enum HitResult { Perfect, Good, Bad, Miss }

    public HitResult EvaluateHit(RhythmNote note)
    {
        float distance = note.DistanceToHitArea();

        if (distance < 10f) return HitResult.Perfect;
        if (distance < 30f) return HitResult.Good;
        if (distance < 50f) return HitResult.Bad;

        return HitResult.Miss;
    }
}