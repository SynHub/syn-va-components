using Syn.Workspace;

namespace Syn.VA.Plugins.HelloPlugin
{
    public class HelloNodePlugin : WorkspacePlugin
    {
        public HelloNodePlugin(WorkspaceGraph workspace) : base(workspace)
        {
            //Register Node.
            workspace.Nodes.RegisterType<HelloNode>();
        }
    }
}
