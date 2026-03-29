// Category Management JavaScript

document.addEventListener('DOMContentLoaded', () => {
    initializeEventListeners();
    initializeSearch();
});

/**
 * Initialize all event listeners
 */
function initializeEventListeners() {
    const deleteButtons = document.querySelectorAll('.delete-btn');
    const cancelDeleteBtn = document.getElementById('cancelDelete');
    const deleteModal = document.getElementById('deleteModal');

    // Delete button handlers
    deleteButtons.forEach(button => {
        button.addEventListener('click', (e) => {
            e.preventDefault();
            const categoryId = button.getAttribute('data-id');
            const categoryName = button.getAttribute('data-name');
            openDeleteModal(categoryId, categoryName);
        });
    });

    // Cancel delete button
    if (cancelDeleteBtn) {
        cancelDeleteBtn.addEventListener('click', closeDeleteModal);
    }

    // Close modal when clicking outside
    if (deleteModal) {
        deleteModal.addEventListener('click', (e) => {
            if (e.target === deleteModal) {
                closeDeleteModal();
            }
        });
    }

    // Close modal with Escape key
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            closeDeleteModal();
        }
    });
}

/**
 * Initialize search functionality with real-time filtering
 */
function initializeSearch() {
    const searchInput = document.getElementById('searchInput');
    const tableRows = document.querySelectorAll('.table-row');

    if (!searchInput) return;

    searchInput.addEventListener('input', (e) => {
        const searchTerm = e.target.value.toLowerCase().trim();

        tableRows.forEach(row => {
            const categoryName = row.getAttribute('data-category-name');

            if (categoryName.includes(searchTerm)) {
                row.style.display = '';
                // Add subtle highlight animation
                row.style.animation = 'none';
                setTimeout(() => {
                    row.style.animation = 'fadeIn 0.3s ease-in-out';
                }, 10);
            } else {
                row.style.display = 'none';
            }
        });

        // Show empty state if no results
        updateEmptyState();
    });
}

/**
 * Open delete confirmation modal
 */
function openDeleteModal(categoryId, categoryName) {
    const modal = document.getElementById('deleteModal');
    const deleteMessage = document.getElementById('deleteMessage');
    const deleteId = document.getElementById('deleteId');

    if (modal && deleteMessage && deleteId) {
        deleteMessage.textContent = `Are you sure you want to delete "${categoryName}"? This action cannot be undone.`;
        deleteId.value = categoryId;
        modal.classList.add('active');

        // Focus on cancel button for accessibility
        document.getElementById('cancelDelete').focus();
    }
}

/**
 * Close delete confirmation modal
 */
function closeDeleteModal() {
    const modal = document.getElementById('deleteModal');
    if (modal) {
        modal.classList.remove('active');
    }
}

/**
 * Update empty state visibility based on visible rows
 */
function updateEmptyState() {
    const tableRows = document.querySelectorAll('.table-row');
    const visibleRows = Array.from(tableRows).filter(row => row.style.display !== 'none');
    const emptyState = document.querySelector('.empty-state');

    if (emptyState) {
        if (visibleRows.length === 0) {
            emptyState.style.display = 'flex';
        } else {
            emptyState.style.display = 'none';
        }
    }
}

/**
 * Add fade-in animation for search results
 */
const style = document.createElement('style');
style.textContent = `
    @keyframes fadeIn {
        from {
            opacity: 0;
        }
        to {
            opacity: 1;
        }
    }
`;
document.head.appendChild(style);

/**
 * Utility function to debounce search input (optional enhancement)
 */
function debounce(func, delay) {
    let timeoutId;
    return function (...args) {
        clearTimeout(timeoutId);
        timeoutId = setTimeout(() => func(...args), delay);
    };
}

/**
 * Export functions for testing purposes
 */
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        initializeEventListeners,
        initializeSearch,
        openDeleteModal,
        closeDeleteModal,
        updateEmptyState,
        debounce
    };
}
