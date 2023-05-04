//"use strict";
//// Creating a connection to SignalR Hub
//var connection = new signalR.HubConnectionBuilder().withUrl("/signalr-hub").build();

//// Starting the connection with server
//connection.start().then(function () { }).catch(function (err) {
//    return console.error(err.toString());
//});

//// Sending the message from Client
//document.getElementById("btnSend").addEventListener("click", function (event) {
//    var message = document.getElementById("message").value;
//    connection.invoke("SendMessage", message)
//    event.preventDefault();
//});

//// Subscribing to the messages broadcasted by Hub every time when a new message is pushed to it
//connection.on("BroadcastMessage", function (user, message) {
//    var finalMessage = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
//    var displayMsg = user + " : " + finalMessage;
//    var li = document.createElement("li");
//    li.textContent = displayMsg;
//    document.getElementById("listMessage").appendChild(li);
//});
$(function GetMessages () {
    $.ajax({
        type: "GET",
        url: "/Messenger/GetMessages",
        data: { "id": 2 },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response) {
            traverseData(response)
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        },
        complete: function () {
            // Schedule the next request when the current one's complete
            setTimeout(GetMessages, 500);
        }
    });

});

function traverseData(data) {
    var item = document.getElementById("listMessage");
    item.innerHTML = '';
    for (var i = 0; i < data.length; i++) {
        var li = document.createElement("li");
        li.textContent = data[i].content;
        document.getElementById("listMessage").appendChild(li);
    }
}

$(function () {
    $("#btnSave").click(function () {
        $.ajax({
            type: "POST",
            url: "/Messenger/SaveMessage",
            data: { message: $('#txtName').val() },
            success: function (response) {
                document.getElementById("txtName").value = "";
                //getNewMessage()
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    })
});

//function getNewMessage() {
//    $.ajax({
//        type: "GET",
//        url: "/Messenger/GetMessages",
//        data: { "id": 2 },
//        contentType: "application/json;charset=utf-8",
//        dataType: "json",
//        success: function (response) {
//            if (response.length > 0) {
//                //var li = document.createElement("li");
//                //li.textContent = response[response.length - 1].content;
//                document.getElementById('listMessage').appendChild(document.createElement('li').textContent = response[response.length - 1].content);
//            }
//        },
//        failure: function (response) {
//            alert(response.responseText);
//        },
//        error: function (response) {
//            alert(response.responseText);
//        }
//    });
//}

//$(function worker() {
//    $.ajax({
//        type: "GET",
//        url: "/Messenger/GetNewMessage",
//        data: { "id": 2 },
//        contentType: "application/json;charset=utf-8",
//        dataType: "json",
//        success: function (response) {
//            if (response.length > 0) {
//                traverseData(response)
//                EmptyDictionary()
//            }
//        },
//        failure: function (response) {
//            alert(response.responseText);
//        },
//        error: function (response) {
//            alert(response.responseText);
//        },
//        complete: function () {
//            // Schedule the next request when the current one's complete
//            setTimeout(worker, 2000);
//        }
//    });

//});

//function EmptyDictionary()
//{
//    $.ajax({
//        type: "GET",
//        url: "/Messenger/EmptyDictionary",
//        data: { "id": 2 },
//        contentType: "application/json;charset=utf-8",
//        dataType: "json",
//        success: function (response) {
//        },
//    });
//}