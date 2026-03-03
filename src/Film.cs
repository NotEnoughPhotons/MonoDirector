namespace NEP.MonoDirector.Core
{
    public sealed class Film
    {
        public Film()
        {
            m_stages = new List<Stage>();
        }

        public Film(List<Stage> stages)
        {
            m_stages = stages;
        }

        public IReadOnlyList<Stage> Stages => m_stages.AsReadOnly();

        private List<Stage> m_stages;
    }
}
