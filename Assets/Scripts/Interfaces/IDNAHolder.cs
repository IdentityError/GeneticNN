namespace Assets.Scripts.Interfaces
{
    public interface IDNAHolder
    {
        DNA ProvideDNA();

        void SetDNA(DNA dna);
    }
}