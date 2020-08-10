using System.Windows;
using Syn.Workspace;
using Syn.Workspace.Events;
using Syn.Workspace.Nodes;

namespace Syn.VA.Plugins.HelloPlugin
{
    public class HelloNode : FunctionNode
    {
        public HelloNode(WorkspaceGraph workspace) : base(workspace)
        {

        }

        public override void OnTriggered(object sender, TriggerEventArgs eventArgs)
        {
            MessageBox.Show("Hello World!");
        }
    }
}
