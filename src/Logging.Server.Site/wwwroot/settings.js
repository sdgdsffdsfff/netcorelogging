﻿$(function () {
    function getOnOffs() {
        var query_url = "api/GetLogOnOffs";
        var log_on_off_temp = $("#log_on_off_temp").html();
        $.get(query_url, function (result) {
            var onOffs = eval("(" + result + ")");
            for (var i = 0; i < onOffs.length; i++) {
                var onOffHtml = log_on_off_temp
                .replace(/{appId}/g, onOffs[i].AppId)
                .replace(/{appName}/g, onOffs[i].AppName)
                .replace("{checked_debug}", onOffs[i].Debug == 1 ? "checked" : "")
                .replace("{checked_info}", onOffs[i].Info == 1 ? "checked" : "")
                .replace("{checked_warm}", onOffs[i].Warm == 1 ? "checked" : "")
                .replace("{checked_error}", onOffs[i].Error == 1 ? "checked" : "");
                $("#logonoffwarp").append(onOffHtml);
            }
        });
    }

    function setOnOff(event, appid, appName) {
        var levels = $(event).parent().prev().find(":checkbox");
        var debug = $(levels[0]).prop("checked") ? 1 : 0;
        var info = $(levels[1]).prop("checked") ? 1 : 0;
        var warm = $(levels[2]).prop("checked") ? 1 : 0;
        var error = $(levels[3]).prop("checked") ? 1 : 0;

        $.post("api/SetLogOnOff", {
            appid: appid,
            appName: appName,
            debug: debug,
            info: info,
            warm: warm,
            error: error
        }, function () {
            alert("OK");
            history.go(0);
        });
    }





    getOnOffs();

    $("#addnoff").click(function () {
        var log_on_off_temp = $("#log_on_off_temp").html();
        var onOffHtml = log_on_off_temp
              .replace("{appId}", "<input type=\"text\" placeholder=\"AppId\" class=\"form-control\" style=\"width: 100px\"/>")
              .replace("{appName}", "<input type=\"text\" placeholder=\"AppName\" class=\"form-control\" style=\"width: 160px\"/>")
              .replace("{checked_debug}", "checked")
              .replace("{checked_info}", "checked")
              .replace("{checked_warm}", "checked")
              .replace("{checked_error}", "checked");
        var $onOffHtml = $(onOffHtml);
        //$onOffHtml.find("button").click(function () {

        //    var txt_appId = $($(this).parent().prev().prev().prev().children()[0]);
        //    var txt_appName = $($(this).parent().prev().prev().children()[0]);
        //    var appId = txt_appId.val();
        //    var appName = txt_appName.val();
        //    if (appId <= 0) {
        //        alert("AppId必须大于0");
        //        txt_appId.focus();
        //        return;
        //    }
        //    setOnOff(this, appId, appName);

        //});
        $("#logonoffwarp").append($onOffHtml);

    });

    $("#logonoffwarp").delegate("button", "click", function () {
        //var appid = $(this).data("appid");

        var txt_appId = $($(this).parent().prev().prev().prev().children()[0]);
        var txt_appName = $($(this).parent().prev().prev().children()[0]);
        var appId = txt_appId.val();
        var appName = txt_appName.val();
        if (!appId || appId <= 0) {
            appId = $(this).data("appid");
        }
        if (!appId || appId <= 0) {
            alert("AppId必须大于0");
            txt_appId.focus();
            return;
        }

        if (!appName || appName == "") {
            appName = $(this).data("appname");
        }
        setOnOff(this, appId, appName);
    });
});