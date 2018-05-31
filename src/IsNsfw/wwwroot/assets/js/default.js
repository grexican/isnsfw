// App's JavaScript
'use strict';

$(function ()
{
    $(document).on('click', '.clickable-default-action', function (event)
    {
        var target = $(this).find($(this).data('target'));

        if(target[0] == event.target) return true; // let it be

        target.click();

        event.preventDefault();
    });
});