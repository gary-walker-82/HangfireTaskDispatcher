using System.Linq;
using BLS.EmailSubscriptions.HangfireExtension.Data;
using Hangfire.Dashboard;
using Hangfire.Dashboard.Pages;

namespace BLS.EmailSubscriptions.HangfireExtension.Pages
{
    internal class EmailSubscriptionDetailsPage : RazorPage
    {
        public const string Title = "Email Subscription Details";
        public const string UrlRoute = "/EmailSubscriptions";

        public override void Execute()
        {
            WriteLiteral(@"<link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css"" integrity=""sha384-WskhaSGFgHYWDcbwN70/dfYBj47jz9qbsMId/iRN3ewGhXQFZCSftd1LZCfmhktB"" crossorigin=""anonymous"">");
            Layout = new LayoutPage(Title);

            WriteLiteral("<div class=\"col-md-12\">\r\n");
            WriteLiteral("<h1 class=\"page-header\">\r\n");
            Write(Title);
            WriteLiteral("</h1>\r\n");
            WriteLiteral(@"<!-- Card -->
<div class=""card card-image"" style=""background-image: url(https://mdbootstrap.com/img/Photos/Horizontal/Work/4-col/img%20%2814%29.jpg);"">

    <!-- Content -->
    <div >
        <div class=""row d-flex justify-content-center "">
            <div class=""col-md-10 col-xl-8"">

                <!--Accordion wrapper-->
                <div class=""accordion"" id=""accordionE"" role=""tablist"" aria-multiselectable=""true"">");

                for (var i = 0; i < ActiveSubscriptions.ListSubscriptions().Count(); i++)
                {
                    var currentSubscription = ActiveSubscriptions.ListSubscriptions()[i];
                WriteLiteral($@"<!--Accordion card -->
                    <div class =""card mb-4"" >

                    <!--Card header -->
                    <div class=""card-header p-0 z-depth-1"" role=""tab"" id=""header_{i}"" >
                    <a data-toggle=""collapse"" data-parent=""#accordion"" href=""#collapse_{i}"" aria-expanded=""true"" aria-controls=""collapse_{i}"">
                        <i class=""fa fa-cloud fa-2x p-3 mr-4 float-left black-text"" aria-hidden=""true"" ></i >
                    <h4 class =""text-uppercase white-text mb-0 py-3 mt-1"" >
                    {currentSubscription.MailBox} - {currentSubscription.SubscriptionName} last action {currentSubscription.LastActivity:g}
                    </h4 >
                    </a >
                    </div >

                    <!--Card body -->
                    <div id=""collapse_{i}"" class=""collapse"" role=""tabpanel"" aria-labelledby=""header_{i}"" data-parent=""#accordion"">
                        <div class =""card-body rgba-black-light white-text z-depth-1"" >
                    <p class=""p-md-4 mb-0"" >{string.Join(", ",currentSubscription.SubscriptionChangeCollectionData.Select(x=>x.FolderPath))}</p >
                    </div >
                    </div >
                    </div >
                    <!--Accordion card -->");

            }
            WriteLiteral(@" </div>
                <!--/.Accordion wrapper -->

            </div>
        </div>
    </div>
    <!-- Content -->
</div>
<!-- Card -->");
            /*    for (var i = 0; i < ActiveSubscriptions.Subscriptions.Count(); i++)
                {
                    WriteLiteral($@"<div class=""card"">
                        <div class=""card-header bg-danger"" id=""Subscription_{i}""><h5 class=""mb-0"">
                            <button class=""btn btn-link"" data-toggle=""collapse"" data-target=""#collapse_{i}"" aria-expanded=""true"" aria-controls=""collapseOne"">");
                    Write(ActiveSubscriptions.Subscriptions[i].SubscriptionName);
                    WriteLiteral(" - ");
                    Write(ActiveSubscriptions.Subscriptions[i].MailBox);
                    WriteLiteral(@"</button><button class=""btn btn-success"">Reset</button>");
                    WriteLiteral("                        </h5>\r\n");
                    WriteLiteral("                    </div>\r\n");
                    WriteLiteral($@"<div id=""collapse_{i}"" class=""collapse show"" aria-labelledby=""headingOne"" data-parent=""#accordion"">
                            <div class=""card-body"">
                               <div class=""row"">
                                    <div class="""">
                                        <label>Last Refresh Time:</label>
                                        <input type=""text"" class=""form-control form-control-readonly"" value=""{ActiveSubscriptions.Subscriptions[i].LastMisFileDate.Value:dd-mmm-yy}"" />
                                    </div>
                               </div>
                                <div class=""row"">
                                    <div class="""">
                                        <label>Number of Folders:</label>
                                        <input type=""text"" class=""form-control form-control-readonly"" value=""{ActiveSubscriptions.Subscriptions[i].SubscriptionData.Count}"" />
                                    </div>
                               </div>
                                <div class=""row"">
                                    <div class="""">
                                        <label>Folders:</label>
                                        <input type=""text"" class=""form-control form-control-readonly"" value=""{string.Join(",",ActiveSubscriptions.Subscriptions[i].SubscriptionData)}"" />
                                    </div>
                               </div>
                            </div>
                        </div>
                    </div>");
                    */
            WriteLiteral("\r\n</div>\r\n");
            WriteLiteral("\r\n</div>\r\n");

        }
    }
}