(function (hangfire) {

    hangfire.Management = (function () {
        function Management() {
            this._initialize();
        }
        Management.prototype._initialize = function () {
            $(document).ready(function () {
                var anchor = window.location.hash;
                $(".collapse").collapse('hide');
                $(anchor).collapse('show');
            });
            $('.js-management').each(function () {
                var container = this;

                $(this).on('click', '.js-management-input-commands',
                   function (e) {
                       var $this = $(this);
                       var confirmText = $this.data('confirm');
                       console.log($this);
                       var id = $this.attr("input-id");
                       var action = $this.attr("action");
                       if (!confirmText || confirm(confirmText)) {
                           $this.prop('disabled');
                           var loadingDelay = setTimeout(function () {
                               $this.button('loading');
                           }, 100);
                           var send = $('form#' + id).serialize();
                           send += "&action=" + action;
                           if (action === "schedule") {
                               send += "&schedule="+ $this.attr("schedule");
                           }
                           $.post($this.data('url'), send, function () {
                               clearTimeout(loadingDelay);
                               window.location.reload();
                           }).fail(function (xhr, status, error) {
                               Hangfire.Management.alert(id, "There was an error. " + error);
                           });
                       }

                       e.preventDefault();
                   });
            });
        };

        Management.alert = function (id, message) {
            $('#' + id + '_error')
                .html('<div class="alert alert-danger"><a class="close" data-dismiss="alert">×</a><strong>Error! </strong><span>' +
                    message +
                    '</span></div>');
        }

        return Management;

    })();

})(window.Hangfire = window.Hangfire || {});

function loadManagement() {
    Hangfire.management = new Hangfire.Management();

    var link = document.createElement('link');
    link.setAttribute("rel", "stylesheet");
    link.setAttribute("type", "text/css");
    link.setAttribute("href", 'https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/3.1.4/css/bootstrap-datetimepicker.min.css');
    document.getElementsByTagName("head")[0].appendChild(link);

    var url = "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/3.1.4/js/bootstrap-datetimepicker.min.js";
    $.getScript(url,
        function () {
            $(function () {
                $("div[id$='_datetimepicker']").each(function () {
                    $(this)
                        .datetimepicker({
                            format: "Do MMMM YYYY, h:mm:ss a"
                        });
                });

            });
        });
}

if (window.attachEvent) {
    window.attachEvent('onload', loadManagement);
} else {
    if (window.onload) {
        var curronload = window.onload;
        var newonload = function (evt) {
            curronload(evt);
            loadManagement(evt);
            var anchor = window.location.hash;
            $(".collapse").collapse('hide');
            $(anchor).collapse('show');
        };
        window.onload = newonload;
    } else {
        window.onload = loadManagement;
    }
}