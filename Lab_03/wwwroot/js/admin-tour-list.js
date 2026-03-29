/**
 * Admin Ticket List Page JavaScript
 * Handles search filtering and delete confirmations
 */

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', init);

    function init() {
        initSearch();
        initDeleteConfirm();
    }

    // ===== Search Filtering =====
    function initSearch() {
        var searchInput = document.getElementById('searchInput');
        var productTable = document.getElementById('productTable');

        if (!searchInput || !productTable) return;

        var rows = productTable.querySelectorAll('.ticket-row');
        var debounceTimer;

        searchInput.addEventListener('input', function () {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(function () {
                var query = searchInput.value.toLowerCase().trim();
                filterRows(rows, query);
            }, 200);
        });

        // Clear on Escape
        searchInput.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                searchInput.value = '';
                filterRows(rows, '');
                searchInput.blur();
            }
        });
    }

    function filterRows(rows, query) {
        var visibleCount = 0;

        rows.forEach(function (row) {
            var productName = row.dataset.productName || '';
            var isMatch = query === '' || productName.includes(query);

            if (isMatch) {
                row.classList.remove('hidden');
                visibleCount++;
            } else {
                row.classList.add('hidden');
            }
        });

        // Show/hide empty message
        var tbody = rows[0] && rows[0].parentElement;
        var emptyRow = tbody && tbody.querySelector('.empty-search-row');

        if (visibleCount === 0 && query !== '' && rows.length > 0) {
            if (!emptyRow) {
                emptyRow = document.createElement('tr');
                emptyRow.className = 'empty-search-row';
                emptyRow.innerHTML = '<td colspan="6" style="text-align:center;padding:3rem;color:#6e6e73;">' +
                    '<i class="fa-solid fa-search" style="font-size:2rem;opacity:0.4;display:block;margin-bottom:1rem;"></i>' +
                    'Khong tim thay san pham nao phu hop</td>';
                tbody.appendChild(emptyRow);
            }
        } else if (emptyRow) {
            emptyRow.remove();
        }
    }

    // ===== Delete Confirmation =====
    function initDeleteConfirm() {
        var deleteButtons = document.querySelectorAll('.action-btn--delete');

        deleteButtons.forEach(function (btn) {
            btn.addEventListener('click', function (e) {
                var productName = btn.dataset.productName || 'san pham nay';
                var confirmed = confirm('Ban co chac chan muon xoa "' + productName + '"?\n\nThao tac nay khong the hoan tac.');

                if (!confirmed) {
                    e.preventDefault();
                }
            });
        });
    }

})();
