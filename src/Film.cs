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
        public string Name => m_name;

        private List<Stage> m_stages;
        private string m_name;
    }
}
