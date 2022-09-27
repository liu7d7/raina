namespace Raina.Shared
{
    public class RainaObj
    {
        private readonly HashSet<Component> _components;
        public bool markedForRemoval;

        public RainaObj()
        {
            _components = new HashSet<Component>();
        }

        public void update()
        {
            foreach (Component component in _components)
            {
                component.update(this);
            }
        }

        public void render()
        {
            foreach (Component component in _components)
            {
                component.render(this);
            }
        }

        public void collide(RainaObj other)
        {
            foreach (Component component in _components)
            {
                component.collide(this, other);
            }
        }

        public void add(Component component)
        {
            _components.Add(component);
            _cache[component.get_type()] = component;
        }
        
        private static bool comp_finder<T>(Component comp)
        {
            return typeof(T) == comp.get_type();
        }

        private readonly Dictionary<Type, Component> _cache = new();

        public T get<T>() where T : Component
        {
            if (_cache.TryGetValue(typeof(T), out Component comp))
            {
                return (T) comp;
            }
            
            T val = (T)_components.FirstOrDefault(comp_finder<T>, null);
            _cache[typeof(T)] = val;
            return val;
        }
        
        public bool has<T>() where T : Component
        {
            return _components.Any(comp_finder<T>);
        }

        public class Component
        {
            public virtual void update(RainaObj objIn)
            {
                
            }

            public virtual void render(RainaObj objIn)
            {
                
            }
            
            public virtual void collide(RainaObj objIn, RainaObj other)
            {
                
            }

            public override int GetHashCode()
            {
                return this.get_type().get_hash_code();
            }
        }
    }
}