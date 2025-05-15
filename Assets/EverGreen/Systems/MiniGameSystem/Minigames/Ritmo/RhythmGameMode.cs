using UnityEngine;
public static class RhythmGameMode
{
    public enum RhythmGameModeType
    {
        Classic,
        Sequence,
        Piano
    }
    public static IRhythmGameMode CreateMode(RhythmGameModeType modeType, IRhythmGameController controller)
    {
        switch (modeType)
        {
            case RhythmGameModeType.Classic:
                return new ClassicMode(controller);
            case RhythmGameModeType.Sequence:
                return new SequenceMode(controller);
            case RhythmGameModeType.Piano:
                return new PianoMode(controller);
            default:
                Debug.LogError("Modo de jogo não implementado!");
                return null;
        }
    }
}