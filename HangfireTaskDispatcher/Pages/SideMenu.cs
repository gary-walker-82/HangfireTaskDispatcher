using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire.Dashboard;

namespace Hangfire.Extension.TaskDispatcher.Pages
{
    internal class SideMenu : RazorPage
    {
        public const string Title = "Tasks";
        public const string UrlRoute = "/Task";

        public Dictionary<string, List<Func<RazorPage, MenuItem>>> Items { get; }

        public SideMenu(Dictionary<string, List<Func<RazorPage, MenuItem>>> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            Items = items;
        }

        public override void Execute()
        {
            WriteLiteral("\r\n");

            if (!Items.Any()) return;

            WriteLiteral(@"<div id=""menu"">
                                <div id=""stats"" class=""panel list-group"">");
            foreach (var queue in Items)
            {
                WriteLiteral($@"<a class=""list-group-item"" data-toggle=""collapse"" data-target=""#{queue.Key}"" data-parent=""#menu""><b>{queue.Key}</b></a>
                                    <div id=""{queue.Key}"" class=""sublinks collapse"">");
                foreach (var item in queue.Value)
                {
                    var itemValue = item(this);
                    var activeString = itemValue.Active ? "active" : "list-group-item-info";
                    WriteLiteral($@"     
                        <a class=""list-group-item small {activeString}"" 
                           href=""{itemValue.Url}#{queue.Key}"">{itemValue.Text}");

                    foreach (var metric in itemValue.GetAllMetrics())
                    {
                        Write(Html.InlineMetric(metric));
                    }

                    WriteLiteral("</a>\r\n");
                }
                WriteLiteral("</div>");

            }
            WriteLiteral("</div></div>\r\n");
        }
    }
}