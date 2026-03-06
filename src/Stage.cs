using Il2CppSLZ.Marrow.Warehouse;
using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.Core
{
    public sealed class Stage
    {
        public Stage()
        {
            m_actors = new List<Actor>();
        }

        public Stage(string name)
        {
            m_actors = new List<Actor>();
            m_name = name;
        }

        public IReadOnlyList<Actor> Actors => m_actors.AsReadOnly();
        public Barcode LevelBarcode => m_levelBarcode;
        public string Name => m_name;
        public float Duration => m_duration;

        private List<Actor> m_actors;
        private Barcode m_levelBarcode;
        private string m_name;
        private float m_duration;

        public void AddActor(Actor actor)
        {
            m_actors.Add(actor);
        }

        public void AddActors(List<Actor> actors)
        {
            m_actors.AddRange(actors);
        }

        public void RemoveActor(Actor actor)
        {
            m_actors.Remove(actor);
        }

        public void RemoveActors(List<Actor> actors)
        {
            m_actors.RemoveRange(0, m_actors.Count);
        }
    }
}
