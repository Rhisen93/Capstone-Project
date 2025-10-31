namespace ForestSlice.Narrative
{
    public enum NarrativeType
    {
        Cutscene,
        Inscription,
        BossDialogue,
        HeroThought,
        Interlude,
        FinalChoice
    }

    public enum NarrativeTriggerType
    {
        OnStart,
        OnEnter,
        OnBossEnter,
        OnBossDefeat,
        OnInteract,
        Manual
    }

    public enum Biome
    {
        ForestExterior,
        DeepRoots,
        GreenFlames,
        SilentShadows,
        BrokenSanctuary,
        ForestHeart
    }

    public enum FinalVariant
    {
        Purify,
        Destroy
    }
}
