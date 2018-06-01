/// <reference path="jquery.d.ts" />

$(() => {
    $(document).on('click', '.clickable-default-action', function (event)
    {
        var target = $(this).find($(this).data('target'));

        if(target[0] === event.target) return true; // let it be

        target.click();

        event.preventDefault();

        return false;
    });
});

function createLink(key, url, tags)
{
    var req = {
        key: key,
        url: url,
        tags: tags
    };
}