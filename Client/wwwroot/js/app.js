window.enableDragDrop = (dotnetHelper) => {
    $(".connectedSortable").sortable({
        connectWith: ".connectedSortable"
    }).disableSelection();
}