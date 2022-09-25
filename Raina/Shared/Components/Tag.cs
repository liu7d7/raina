namespace Raina.Shared.Components
{
    public class Tag : RainaObj.Component
    {
        public int id;
        public string name;

        public Tag(int id, string name = "")
        {
            this.id = id;
            this.name = name;
        }
    }
}