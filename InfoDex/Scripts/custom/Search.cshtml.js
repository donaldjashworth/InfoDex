var runningTimerId = -1;
var typingTimer;
var doneTypingInterval = 1000;
var homePath = '';
var uid = 0;

$(document).ready(function () {
    initialize();
});

function debug() {
    alert("debug()");
}

function initialize() {

    //  Wire up the Search section toggler
    $('#div_searchtoggle').click(function (e) {
        $('#div_search').slideToggle('slow');
    });

    $('#filter_search').bind("click", function (e) {
        search();
    });

    $("#filter_search").bind("mouseup", function (e) {
        var $input = $(this);
        var oldValue = $input.val();

        if (oldValue == "") return;

        setTimeout(function () {
            var newValue = $input.val();

            if (newValue == "") {
                //  TODO:  Do whatever..
            }
        }, 1);
    });

    //  //  Automatic search after non-typing delay
    //$('#filter_search').on('keyup change', function () {
    //    clearTimeout(typingTimer);
    //    typingTimer = setTimeout(search, doneTypingInterval);
    //});

    $('#filter_keywords').trigger('blur');

    $('#filter_add').bind("click", function (e) {
        addDetail();
    });

    $('#filter_recalc').bind("click", function (e) {
        recalcDatabase();
    });

    $('#detail_update').bind("click", function (e) {
        updateDetail();
    });

    //  Make this site portable by identifying the virtual root.
    var position = window.location.pathname.indexOf('/Home');
    if (position) {
        homePath = window.location.pathname.substring(0, position);
    }

    //  Attach the WYSIWYG editor to the body div
    //$('#toolbar').hide();
    $('#detail_body').wysiwyg();
    $('#detail_body').cleanHtml()

    listDatabases();
}

function tryMe(value) {
    alert("tryme(" + value + ")");
}

function addDetail() {
    var data;
    var database = $('#filter_database :selected').first().val();

    var keywords = $('#filter_keywords').val();
    if (keywords == 'Enter search text here...') {
        keywords = '';
    }

    var post = {
        DATABASE: database,
        KEYWORDS: keywords
    };

    $.ajax({
        method: 'POST',
        url: homePath + '/Home/AddDetail',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(post),
        success: function (response) {
            data = response;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        },
        complete: function () {
            fadeMessage('New details added');
            //  Submit a new search...
            search();
            $('#filter_add').button('reset');
        },
        beforeSend: function () {
            $('#filter_add').button('loading');
        }
    });
}

function updateDetail() {
    var data;
    var database = $('#filter_database :selected').first().val();
    var uid = $('#detail_uid')[0].innerText;
    var summary = $('#detail_summary')[0].innerText;
    var body = $('#detail_body').val();


    var stuff = $('#detail_body').html();
    alert(stuff);

    var post = {
        DATABASE: database,
        UID: uid,
        SUMMARY: summary,
        BODY: body
    };

    $.ajax({
        method: 'POST',
        url: '/Home/UpdateDetail',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(post),
        success: function (response) {
            data = response;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        },
        complete: function () {
            fadeMessage('Details updated');
            //  Submit a new search...
            search();
        },
        beforeSend: function () {
        }
    });
}

function recalcDatabase() {
    var data;
    var database = $('#filter_database :selected').first().val();

    var post = {
        DATABASE: database
    };

    $.ajax({
        method: 'POST',
        url: '/Home/RecalcDatabase',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(post),
        success: function (response) {
            data = response;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        },
        complete: function () {
            fadeMessage('Database recalculation complete');
            $('#filter_recalc').button('reset');
            notification.hidePleaseWait();
        },
        beforeSend: function () {
            $('#filter_recalc').button('loading');
            notification.showPleaseWait();
        }
    });
}


function getDetail(id) {
    uid = id;
    var data;
    var database = $('#filter_database :selected').first().val();
    var post = {
        DATABASE: database,
        ID: id
    };

    $.ajax({
        method: 'POST',
        url: '/Home/GetDetail',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(post),
        success: function (response) {
            $('#detail_uid')[0].innerText = response.UID;
            $('#detail_keywords')[0].innerText = response.KEYWORDS;
            $('#detail_summary')[0].innerText = response.SUMMARY;
            $('#detail_lastviewed')[0].innerText = response.LASTVIEWED;
            $('#detail_body').html(response.BODY);
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

function deleteDetail() {

    var data;
    var database = $('#filter_database :selected').first().val();

    var post = {
        DATABASE: database,
        UID: uid
    };

    $.ajax({
        method: 'POST',
        url: '/Home/DeleteDetail',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(post),
        success: function (response) {
            fadeMessage('Item deleted.');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        },
        complete: function () {
            search();
        },
        beforeSend: function () {
        }
    });
}

function search() {
    var data;
    var database = $('#filter_database :selected').first().val();

    var keywords = $('#filter_keywords').val();
    if (keywords == 'Enter search criteria here...') {
        keywords = '';
    }

    var keywordweight = $('#filter_keywordweight :selected').first().val();

    var post = {
        DATABASE: database,
        KEYWORDS: keywords,
        KEYWORDWEIGHT: keywordweight
    };

    $.ajax({
        method: 'POST',
        url: '/Home/KeywordSearch',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(post),
        success: function (response) {
            data = response;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        },
        complete: function () {
            update(data);
            fadeMessage('Search complete');
        },
        beforeSend: function () {
        }
    });

}

function update(data) {
    var deferred = $.Deferred();
    var initialized = $.fn.dataTable.isDataTable('#results');

    if (initialized) {
        var table = $('#results').dataTable();
        table.fnClearTable();
        table.fnAddData(data, true);
        table.fnDraw();
    } else {
        $('#results').DataTable({
            pageLength: 20,
            processing: true,
            deferRender: true,
            orderable: true,
            order: [[0, "desc"]],
            searching: true,
            paging: true,
            dom: 'tip',
            data: data,
            columns: [
                {
                    data: 'HITS',
                    render: function (data, type, full, meta) {
                        return '<button type="button" style="width:70px;" class="btn btn-primary" data-toggle="modal" data-target="#detailed" onclick="getDetail(' + full.ID + ');">' + full.HITS + ' <span class="glyphicon glyphicon-play"></span></button>';
                    }
                },
                { data: 'ID' },
                { data: 'KEYWORDS' },
                { data: 'SUMMARY' },
                { data: 'LASTVIEWED' },
                { data: 'CREATEDDATE' }
            ],
            initComplete: function (settings, json) {
                fadeMessage('Initialization complete');

                deferred.resolve(true);
                return deferred;
            }
        });
    }

}


function listDatabases() {
    $.ajax({
        method: 'POST',
        url: '/Home/ListDatabases',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            for (var index in response) {
                var item = response[index];
                $('#filter_database').append('<option value="' + item.FULLPATH + '">' + item.JUSTNAME + '</option>');
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

function performSearches() {
    var f1 = $.Deferred();
    var f2 = $.Deferred();

    $.when(function1(), functiion2())
        .done(function (f1, f2) {
        });
}

function function1() {
    console.log("function1()");
}

function function2() {
    console.log("function2()");
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

