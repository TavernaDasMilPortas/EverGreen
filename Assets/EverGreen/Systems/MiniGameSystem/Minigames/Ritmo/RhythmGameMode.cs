using UnityEngine;
public static class RhythmGameMode
{

    public static IRhythmGameMode CreateMode(GameModes.Modes modeType, IRhythmGameController controller)
    {
        switch (modeType)
        {
            case GameModes.Modes.Classic:
                return new ClassicMode(controller);
            case GameModes.Modes.Sequence:
                return new SequenceMode(controller);
            case GameModes.Modes.Piano:
                return new PianoMode(controller);
            default:
                Debug.LogError("Modo de jogo não implementado!");
                return null;
        }
    }
}