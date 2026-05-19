// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/**
 * initGridSearch — live filter for card grids.
 * @param {string} inputId      id of the <input type="search"> element
 * @param {string} cardSelector CSS selector for cards that have [data-search] attribute
 * @param {string} emptyId      id of the "no results" element
 */
function initGridSearch(inputId, cardSelector, emptyId) {
    const input = document.getElementById(inputId);
    const empty = document.getElementById(emptyId);
    if (!input) return;

    input.addEventListener('input', function () {
        const query = this.value.trim().toLowerCase();
        const cards = document.querySelectorAll(cardSelector);
        let visible = 0;

        cards.forEach(card => {
            const haystack = (card.dataset.search || '').toLowerCase();
            const show = !query || haystack.includes(query);
            card.style.display = show ? '' : 'none';
            if (show) visible++;
        });

        if (empty) empty.classList.toggle('d-none', visible > 0 || !query);
    });

    // Clear on Escape
    input.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') { this.value = ''; this.dispatchEvent(new Event('input')); }
    });
}
