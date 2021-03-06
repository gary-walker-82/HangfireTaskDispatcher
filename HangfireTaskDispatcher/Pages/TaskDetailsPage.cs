﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Hangfire.Dashboard;
using Hangfire.Dashboard.Pages;
using Hangfire.Extension.TaskDispatcher.Attributes;
using Hangfire.Extension.TaskDispatcher.Converters;
using Hangfire.Extension.TaskDispatcher.Extensions;
using Hangfire.Extension.TaskDispatcher.GlobalConfiguration;
using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Pages
{

    public class TaskDetailsPage : RazorPage
    {
        private readonly string _pageHeader;
        private readonly string _displayName;
        private readonly string _displayDescription;
        private readonly string _displayErrorDetails;
        private readonly ITaskParameters _taskParameters;
        private readonly List<Type> _genericTypeOptions;
        private readonly TaskDispatcherPagesOptions _options;

        protected internal TaskDetailsPage(ITaskParameters taskParameters, TaskDispatcherPagesOptions options, List<Type> genericTypeOptions = null)
        {
            _taskParameters = taskParameters;
            _genericTypeOptions = genericTypeOptions;
            _options = options;
            var type = _taskParameters.GetType();
            _pageHeader = type.Name.Replace("`1", "");
            _displayName = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? taskParameters.ToString();
            _displayDescription = type.GetCustomAttribute<DescriptionAttribute>()?.Description;
	        _displayErrorDetails = type.GetCustomAttribute<ErrorImplicationDetailsAttribute>()?.Details;
        }

        public override void Execute()
        {
            WriteLiteral("\r\n");
            WriteLiteral("<style>.btn-default.btn-on.active{background-color: #5BB75B;color: white;}.btn-default.btn-off.active{ background-color: #DA4F49;color: white;}</style>");
            Layout = new LayoutPage(_displayName);

            WriteLiteral("<div class=\"row\">\r\n");
            WriteLiteral("<div class=\"col-md-3\">\r\n");

            Write(Html.RenderPartial(new SideMenu()));

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
            var inputsHtml = AddGenericTypeOption();
            inputsHtml += _taskParameters.GetType()
                                           .GetProperties()
                                           .Where(ShouldDisplayProperty)
                                           .Select(x => inputElementFactory.GetInputElementWriter(x))
                                           .Aggregate(new StringBuilder(), (sb, x) => sb.AppendLine(x.WriteElementAndLabel(_taskParameters)), sb => sb.ToString());

            var route = $"{TasksPage.UrlRoute}/{_taskParameters.Queue}/{_pageHeader.Replace(" ", string.Empty)}";

            Panel(id, _displayName, _displayDescription, _displayErrorDetails,inputsHtml, CreateButtons(route, "Enqueue", "enqueueing", id));

            WriteLiteral("\r\n<script src=\"");
            Write(Url.To($"/jsm"));
            WriteLiteral("\"></script>\r\n");
        }

        private bool ShouldDisplayProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetCustomAttribute<TaskFormIgnoreAttribute>() != null) return false;
            if (propertyInfo.Name == "Queue") return _options.ShowQueueName;
            if (propertyInfo.CanWrite && propertyInfo.CanRead) return true;   
            if (_options.ShowReadOnlyProperties && propertyInfo.CanRead) return true;

            return false;
        }

        private string AddGenericTypeOption()
        {
            if (_genericTypeOptions == null || _genericTypeOptions.Any() == false) return "";
            var values = _genericTypeOptions.Select(x => x.Name);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($@"<select class=""form-control"" id=""objecttype"" name=""objecttype"">");
            foreach (var value in values)
            {
                stringBuilder.AppendLine($@"<option value=""{value}"">{value}</option>");
            }
            stringBuilder.AppendLine("</select>");

            var inputsHtml = $@"<div class=""form-group row"">
                        <label for=""objecttype"" class=""col-xs-3 col-form-label"">Type</label>
                        <div class=""col-xs-9"">{  stringBuilder}</div></div>";
            return inputsHtml;
        }

        protected void Panel(string id, string heading, string description,string errorDetails, string content, string buttons)
        {
            WriteLiteral($@"<div class=""js-management"">                             ");
	        if (!string.IsNullOrWhiteSpace(description))
		        WriteLiteral($@"<div class=""alert alert-info""><p>{description}</p></div>");

			if (!string.IsNullOrWhiteSpace(errorDetails))
				WriteLiteral($@"<div class=""alert alert-danger""><h4>Error Implications:</h4><p>{errorDetails}</p></div>");

			WriteLiteral($@"<form id =""{id}"">");

            if (!string.IsNullOrEmpty(content))
            {
                WriteLiteral($@"<div> 
                                    { content}
                                </div>      
                                                     
                              ");
            }

            WriteLiteral($@"<div id=""{id}_success"" ></div><div id=""{id}_error"" ></div>  
                            
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