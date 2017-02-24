using System;

namespace Sitecore.Support.Shell.Framework.Commands.ContentEditor
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Shell.Framework.CommandBuilders;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Sheer;
    using Sitecore.Workflows;

    [Serializable]
    public class ShowWorkflowCommands : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            string name = context.Parameters["database"];
            string path = context.Parameters["id"];
            string str3 = context.Parameters["language"];
            string str4 = context.Parameters["version"];
            Database database = Factory.GetDatabase(name);
            if (database != null)
            {
                Item item = database.GetItem(path, Language.Parse(str3), Sitecore.Data.Version.Parse(str4));
                if (item != null && item.Locking != null && item.Locking.HasLock())
                {
                    IWorkflow workflow = item.Database.WorkflowProvider.GetWorkflow(item);
                    if (workflow != null)
                    {
                        WorkflowCommand[] commandArray = WorkflowFilterer.FilterVisibleCommands(workflow.GetCommands(item), item);
                        if ((commandArray != null) && (commandArray.Length != 0))
                        {
                            Menu contextMenu = new Menu();
                            SheerResponse.DisableOutput();
                            WorkflowCommand[] commandArray2 = commandArray;
                            for (int i = 0; i < commandArray2.Length; i = (int)(i + 1))
                            {
                                WorkflowCommand command = commandArray2[i];
                                string click = new WorkflowCommandBuilder(item, workflow, command).ToString();
                                contextMenu.Add("C" + command.CommandID, command.DisplayName, command.Icon, string.Empty, click, false, string.Empty, MenuItemType.Normal);
                            }
                            SheerResponse.EnableOutput();
                            SheerResponse.ShowContextMenu(Context.ClientPage.ClientRequest.Control, "right", contextMenu);
                        }
                    }
                }
            }
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (!Settings.Workflows.Enabled)
            {
                return CommandState.Hidden;
            }
            return base.QueryState(context);
        }
    }
}
