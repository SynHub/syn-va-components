namespace Syn.VA.Plugins.Skype.Model
{
    class SkypeUser
    {
        public SkypeUser(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; set; }
        public string Id { get; set; }
    }
}
