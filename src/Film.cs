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
        public float Runtime => m_runtime;

        private List<Stage> m_stages;
        private string m_name;
        private float m_runtime;

        public void AddStage(Stage stage)
        {
            m_stages.Add(stage);
            m_runtime += stage.Duration;
        }

        public void RemoveStage(Stage stage)
        {
            m_stages.Remove(stage);
            m_runtime -= stage.Duration;
        }
    }
}
