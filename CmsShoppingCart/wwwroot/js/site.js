$(function ()
{
    if ($("a.confimDeletion").length)
    {
        $("a.confimDeletion").click(() => {
            if (!confirm("Confirm deletion")) return false;
        });
    }

    if ($("div.alert.notification").length) {
        setTimeout(() => { $("div.alert.notification").fadeOut() }, 2000);
    }
})