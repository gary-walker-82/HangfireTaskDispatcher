using System.ComponentModel;
using System.Linq;
using System.Text;
using Hangfire.Dashboard;
using Hangfire.Dashboard.Pages;
using Hangfire.Extension.TaskDispatcher.Converters;
using Hangfire.Extension.TaskDispatcher.Extensions;
using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Pages
{
    public class TaskDetailsPage : RazorPage
    {
        private readonly string _pageHeader;
        private readonly string _displayName;
        private readonly string _displayDescription;
        private readonly ITaskParameters _taskParameters;
        
        protected internal TaskDetailsPage(ITaskParameters taskParameters)
        {
            _taskParameters = taskParameters;
            var type = _taskParameters.GetType();
            _pageHeader = type.Name;
            _displayName = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??taskParameters.ToString();
            _displayDescription = type.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }

        public override void Execute()
        {
            WriteLiteral("\r\n");
            WriteLiteral("<style>.btn-default.btn-on.active{background-color: #5BB75B;color: white;}.btn-default.btn-off.active{ background-color: #DA4F49;color: white;}</style>");
            Layout = new LayoutPage(_displayName);

            WriteLiteral("<div class=\"row\">\r\n");
            WriteLiteral("<div class=\"col-md-3\">\r\n");

            Write(Html.RenderPartial(new SideMenu(TasksMenu.Items)));

            WriteLiteral("</div>\r\n");
            WriteLiteral("<div class=\"col-md-9\">\r\n");
            WriteLiteral("<div class=\"row\">\r\n");
            WriteLiteral("<h1 class=\"page-header\">\r\n");
            Write(_displayName);
            WriteLiteral("</h1>\r\n");

            Content();

            WriteLiteral("\r\n</div>\r\n");
            WriteLiteral("\r\n</div>\r\n");
            WriteLiteral("\r\n</div>\r\n");
            WriteLiteral("\r\n</div>\r\n");
        }

        protected virtual void Content()
        {
            var id = $"{_pageHeader.Replace(" ", string.Empty)}";
            var inputElementFactory = new InputElementFactory();
            var inputsHtml = _taskParameters.GetType()
                                           .GetProperties()
                                           .Where(x => x.CanRead)
                                           .Select(x => inputElementFactory.GetInputElementWriter(x))
                                           .Aggregate(new StringBuilder(), (sb, x) => sb.AppendLine(x.WriteElementAndLabel(_taskParameters)), sb => sb.ToString());

            var route = $"{TasksPage.UrlRoute}/{_taskParameters.Queue}/{_pageHeader.Replace(" ", string.Empty)}";

            Panel(id, _displayName, _displayDescription, inputsHtml, CreateButtons(route, "Enqueue", "enqueueing", id));

            WriteLiteral("\r\n<script src=\"");
            Write(Url.To($"/jsm"));
            WriteLiteral("\"></script>\r\n");
        }

        protected void Panel(string id, string heading, string description, string content, string buttons)
        {
            WriteLiteral($@"<div class=""js-management"">
                              <p>{description}</p><br/><br/>
                               <form id =""{id}"">");

            if (!string.IsNullOrEmpty(content))
            {
                WriteLiteral($@"<div> 
                                    { content}
                                </div>      
                                                     
                              ");
            }

            WriteLiteral($@"<div id=""{id}_error"" ></div>  
                            
                            <div class=""panel-footer clearfix "">
                                <div class=""pull-right"">
                                    { buttons}
                                </div>
                              </div>
                            </form>");
        }

        protected string CreateButtons(string url, string text, string loadingText, string id)
        {
            return $@"<div class=""col-sm-2 pull-right"">
                            <button class=""js-management-input-commands btn btn-sm btn-success"" 
                                    data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"" input-id=""{id}"" action=""fireandForget""> 
                                <span class=""glyphicon glyphicon-play-circle""></span>
                                &nbsp;Enqueue
                            </button>
                        </div>
                        <div class=""btn-group col-3 pull-right"">
                            <button type=""button"" i class=""btn btn-info btn-sm dropdown-toggle"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                                Schedule &nbsp;
                                <span class=""caret""></span>
                            </button>          
                            <ul class=""dropdown-menu"">
                                <li><a href=""#"" class=""js-management-input-commands"" input-id=""{id}"" action=""schedule"" schedule=""5""  
                                    data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">5 min</a></li>
                                <li><a href=""#"" class=""js-management-input-commands"" input-id=""{id}"" action=""schedule"" schedule=""10""
                                     data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">10 min</a></li>
                                <li><a href=""#"" class=""js-management-input-commands"" input-id=""{id}"" action=""schedule"" schedule=""15""
                                     data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">15 min</a></li>
                                <li><a href=""#"" class=""js-management-input-commands"" input-id=""{id}"" action=""schedule"" schedule=""30""
                                     data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">30 min</a></li>
                                <li><a href=""#"" class=""js-management-input-commands"" input-id=""{id}"" action=""schedule"" schedule=""60""
                                     data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">60 min</a></li>
                            </ul>
                        </div>
                        <div class=""col-sm-5 pull-right"">
                            <div class=""input-group input-group-sm"">
                                <input type=""text"" class=""form-control"" placeholder=""Enter a cron expression * * * * *"" id=""cron"" name=""cron"">
                                <span class=""input-group-btn "">
                                <button class=""btn btn-default btn-sm btn-warning js-management-input-commands"" type=""button""  action=""cron"" input-id=""{id}""
                                     data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">
                                    <span class=""glyphicon glyphicon-repeat""></span>
                                    &nbsp;Add Recurring</button>
                                </span>
                            </div>
                        </div>
                       ";
        }
    }
}