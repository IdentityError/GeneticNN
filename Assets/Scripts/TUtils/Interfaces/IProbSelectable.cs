namespace Assets.Scripts.TUtils.Interfaces
{
    public interface IProbSelectable
    {
        float ProvideSelectProbability();

        void SetSelectProbability(float prob);
    }
}