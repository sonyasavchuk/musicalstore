$('select.search-select').select2({
    width: '100%'
});

$('select[multiple=true].search-select').select2({
    width: '100%',
    closeOnSelect: false
});

$('select.types-select').select2({
    width: '100%',
    tags: true,
    closeOnSelect: false,
    tokenSeparators: [','],
    language: {
        noResults: function() {
            return "Add type";
        }
    }
});

$('select.materials-select').select2({
    width: '100%',
    tags: true,
    closeOnSelect: false,
    tokenSeparators: [','],
    language: {
        noResults: function() {
            return "Add type";
        }
    }
});
