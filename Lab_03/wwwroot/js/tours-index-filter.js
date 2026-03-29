(function () {
    'use strict';

    function applyFilter(select, query) {
        var q = (query || '').trim().toLowerCase();
        var selectedValue = select.value;

        for (var i = 0; i < select.options.length; i++) {
            var opt = select.options[i];
            if (!opt.value) {
                opt.hidden = false;
                continue;
            }
            if (opt.value === selectedValue) {
                opt.hidden = false;
                continue;
            }
            var text = (opt.textContent || '').trim().toLowerCase();
            opt.hidden = q.length > 0 && text.indexOf(q) === -1;
        }
    }

    function init() {
        var filterInput = document.getElementById('destinationFilter');
        var select = document.getElementById('destinationSelect');
        if (!filterInput || !select) return;

        filterInput.addEventListener('input', function () {
            applyFilter(select, filterInput.value);
        });

        select.addEventListener('change', function () {
            applyFilter(select, filterInput.value);
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
