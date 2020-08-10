namespace Syn.VA.Plugins.HelloPlugin
{
    public class HelloWorldPlugin : AssistantPlugin
    {
        public HelloWorldPlugin(VirtualAssistant assistant) : base(assistant)
        {
            //Name to be displayed in Settings Panel
            DisplayName = "Hello Plugin";
        }

        public override T GetPanel<T>(params object[] parameters)
        {
            //Required if the plugin does not have any editable settings.
            return null;
        }
    }
}