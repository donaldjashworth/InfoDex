var runningTimerId = -1;
var typingTimer;
var doneTypingInterval = 1000;
var homePath = '';
var uid = 0;


$(document).ready(function () {
    initialize();
});


function initialize() {
    //  Make this site portable by identifying the virtual root.
    var position = window.location.pathname.indexOf('/Home');
    if (position) {
        homePath = window.location.pathname.substring(0, position);
    }


    $('#configure_database').blur();
    $('#configure_database').focus();
    $('#configure_database').bind('keyup change', filterDatabases);

    listDatabases();
}

function createDatabase() {
    var data;
    var database = $('#configure_database').val();

    var post = {
        DATABASE: database,
    };

    $.ajax({
        method: 'POST',
        url: homePath + '/Home/CreateDatabase',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(post),
        success: function (response) {
            data = response;
            fadeMessage('New Database created');
            listDatabases();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        },
        complete: function () {
        },
        beforeSend: function () {
        }
    });
}


function deleteDatabase() {
    var database = null;

    $('#configure_existing li').each(function () {
        var item = $(this);
        if (item.hasClass('active')) {
            database = item[0].innerText;
            return false;
        }
    });


    if (database == null) {
        fadeMessage('Please select a database to delete.');
        return;
    }
   
    var post = {
        DATABASE: database
    };

    $.ajax({
        method: 'POST',
        url: '/Home/DeleteDatabase',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(post),
        success: function (response) {
            fadeMessage('Selected Database deleted');
            listDatabases();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        },
        complete: function () {
        },
        beforeSend: function () {
        }
    });
}


function listDatabases() {
    $('#configure_existing').empty();

    $.ajax({
        method: 'POST',
        url: '/Home/ListDatabases',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            for (var index in response) {
                var item = response[index];
                $('#configure_existing').append('<li class="list-group-item" onclick="selectListItem(this);">' + item.JUSTNAME + '</li>');
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        },
        complete: function () {
        },
        beforeSend: function () {
        }
    });
}

function filterDatabases() {
    var filter = $('#configure_database').val().toLowerCase();
    var empty = true;
    

    $('li.list-group-item').each(function (p) {
        var item = $(this);
    
        if (item[0].innerText.toLowerCase().indexOf(filter) > -1) {
            item.css('display', 'block');
            empty = false;
        } else {
            item.css('display', 'none');
        }
    })

    if (empty) {
        $('#configure_create').removeClass('disabled');
        //$('#configure_delete').addClass('disabled');
    } else {
        $('#configure_create').addClass('disabled');
        //$('#configure_delete').removeClass('disabled');
    }
}

function selectListItem(source) {
    $('#configure_existing li').each(function (source) {
        var item = $(this);
        item.removeClass('active');
    });

    var selected = $(source);
    selected.addClass('active');
}


function fadeMessage(message) {
    $('#notification').animate({ opacity: 1 }, 1000, function () {
        $(this).text(message)
            .animate({ opacity: 0 });
    });
}

function waterMark(text, event, value) {
    if (text.value.length == 0 && event.type == "blur") {
        text.style.color = "gray";
        text.value = value;
    }

    if (text.value == value && event.type == "focus") {
        text.style.color = "black";
        text.value = "";
    }
}


var notification;
notification = notification || (function () {
    var pleaseWaitDiv = $('<div class="modal fade" id="pleaseWaitDialog" data-backdrop="static" data-keyboard="false" role="dialog" aria-labelledby="basicModal" aria-hidden="true" tabindex="-1"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><h1>Processing...</h1></div><div class="modal-body"><div class="progress progress-striped active"><div class="progress-bar" style="width: 100%;"><span class="sr-only">60% Complete</span></div></div></div></div></div></div></div></div>');
    return {
        showPleaseWait: function () {
            pleaseWaitDiv.modal();
        },
        hidePleaseWait: function () {
            pleaseWaitDiv.modal('hide');
        },

    };
})();

