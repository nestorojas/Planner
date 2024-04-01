$(function () {
    var selected = "";
    $("#ClientName").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Project/GetClientNames",
                dataType: "json",
                data: { term: request.term },
                success: function (data) {
                    response(data.map(function (item) {
                        return { label: item.clientName, value: item.clientId };
                    }));
                }
            });
        },
        minLength: 3,
        select: function (event, ui) {
            selected = ui.item.label;
            $("#ClientId").val(ui.item.value);
        },
        close: function (event, ui) {
            $("#ClientName").val(selected);
        }
    });
    $(document).on('change', '#PredecessorId, #isDurationEnabled, #Duration', function () {
        const selectedPredecessor = $("#PredecessorId").val();
        const hasPredecessor = selectedPredecessor && selectedPredecessor != "";
        const isDurationEnabled = $("#isDurationEnabled").is(":checked");
        if (hasPredecessor) {
            $("#isDurationEnabled").prop("checked", true);
            $("#Duration").prop("readonly", false);
            $("#DueDate").prop("disabled", true);
            $("#DueDate").val(null);
        } else {
            $("#isDurationEnabled").prop("disabled", false);
            if (isDurationEnabled) {
                $("#Duration").prop("readonly", false);
                $("#DueDate").prop("disabled", true);
                const today = new Date();
                var duration = getNumber($("#Duration").val())
                const duedate = today.addDays(duration);
                $("#DueDate").val(duedate);
            } else {
                $("#Duration").prop("readonly", true);
                $("#Duration").val(null);
                $("#DueDate").prop("disabled", true);
            }
        }
    });
});
Date.prototype.addDays = function (days) {
    const date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date.toLocaleDateString();
};

function getNumber(id) {
    const parsedNumber = Number(id);
    return isNaN(parsedNumber) ? 0 : parsedNumber;
}
function onTaskCommandClick(e) {
    if (!e.rowData.IsCompleted) {
        if (e.commandColumn.type == 'Edit') {
            window.location.href = '/Task/TaskUpdate/' + e.rowData.Id;
        }
    }
}
function onTaskToolBarClick(e) {
    var pjstat = getNumber($("#ProjectStatusId").val());
    if (pjstat > 1 && pjstat < 5) {
        window.location.href = '/Task/TaskCreate/' + $("#ProjectId").val();
    }
}
function onProjectCommandClick(e) {
    if (e.commandColumn.type == 'Edit') {
        window.location.href = '/Project/ProjectEdit/' + e.rowData.ProjectId;
    }
    if (e.commandColumn.type == 'Details') {
        window.location.href = '/Project/ProjectDetail/' + e.rowData.ProjectId;
    }
}
function onProjectToolBarClick(e) {
    e.originalEvent.defaultPrevented = true
    window.location.href = '/Project/ProjectCreate';
}
function getwfdata() {
    wfdata.Id = $("#Id").val() != "" ? $("#Id").val() : "0";
    wfdata.Name = $("#Name").val();
    wfdata.Description = $("#Description").val();
}
function getPredecessorsforTask() {
    $.ajax({
        type: "GET",
        url: "/Task/GetTasks",
        data: { id: $("#ProjectId").val() },
        success: function (tasks) {
            var select = $("#PredecessorId");
            if (tasks.length > 0) {
                select.empty();
                select.append("<option value='' selected> No predecessor </option>");
                $.each(tasks, function (index, task) {
                    select.append("<option value='" + task.id + "'>" + task.name + "</option>");
                });
            } else {
                select.append("<option>No Tasks available for predecessor</option>");
            }
            select.attr("multiple", "multiple");
        },
        error: function () {
            alert("Failed to retrieve tasks.");
        }
    });
    $.ajax({
        type: "GET",
        url: "/Task/GetMemberTeam",
        success: function (members) {
            var select = $("#AssignedUserId");
            if (members.length > 1) {
                select.empty();
                select.append("<option value='' selected>Select Team Member</option>");
                $.each(members, function (index, member) {
                    select.append("<option value='" + member.id + "'>" + member.name + "</option>");
                });
            }
        },
        error: function () {
            alert("Failed to retrieve members.");
        }
    });
}
function statusTaskDetail(e) {
    var SysDate = new Date('2023-01-01');
    var div = document.createElement('div');
    var span = document.createElement('span');
    if (e.CreatedDate < SysDate) {
        span.className = 'e-inactivecolor';
        span.textContent = 'Not Started'
    } else {
        span.className = 'e-activecolor';
        span.textContent = e.CreatedDate.toLocaleDateString();
    }
    div.appendChild(span);
    return div.outerHTML;
}
function dueTaskDetail(e) {
    var SysDate = new Date();
    var div = document.createElement('div');
    var span = document.createElement('span');
    if (e.DueDate == null) {
        span.textContent = 'Not Started'
    }
    else if (e.DueDate < SysDate) {
        span.className = 'e-inactivecolor';
        span.textContent = e.DueDate.toLocaleDateString();
    } else {
        span.className = 'e-activecolor';
        span.textContent = e.DueDate.toLocaleDateString();
    }
    div.appendChild(span);
    return div.outerHTML;
}
function statusDetail(e) {
    var div = document.createElement('div');
    var span = document.createElement('span');
    if (e.IsCompleted) {
        span.className = 'e-inactivecolor';
        span.textContent = 'Completed'
    } else {
        span.className = 'e-activecolor';
        span.textContent = 'Pending'
    }
    div.appendChild(span);
    return div.outerHTML;
}