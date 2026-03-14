using Il2CppSLZ.Marrow.Warehouse;
using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.Core
{
    public sealed class Stage
    {
        public Stage()
        {
            m_actors = new List<Actor>();
            m_props = new List<Prop>();
            m_name = "Stage";
        }

        public Stage(string name)
        {
            m_actors = new List<Actor>();
            m_props = new List<Prop>();
            m_name = name;
        }

        public IReadOnlyList<Actor> Actors => m_actors.AsReadOnly();
        public IReadOnlyList<Prop> Props => m_props.AsReadOnly();
        public Barcode LevelBarcode => m_levelBarcode;
        public string Name => m_name;
        public float Duration => m_duration;

        private List<Actor> m_actors;
        private List<Prop> m_props;
        private Barcode m_levelBarcode;
        private string m_name;
        private float m_duration;

        public static void Swap(ref Stage left, ref Stage right)
        {
            Stage temp = left;
            left = right;
            right = temp;
        }

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

        }

        public void AddProp(Prop prop)
        {
            m_props.Add(prop);
        }

        public void AddProps(List<Prop> props)
        {
            m_props.AddRange(props);
        }

        public void RemoveProp(Prop prop)
        {
            m_props.Remove(prop);
        }

        public void RemoveProps(List<Prop> props)
        {

        }

        public void SetDuration(float duration)
        {
            m_duration = duration;
        }
    }
}
