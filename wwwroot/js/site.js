/**
 * initGridSearch-live filter for card grids.
 */
function initGridSearch(inputId, cardSelector, emptyId) {
  const input = document.getElementById(inputId);
  const empty = document.getElementById(emptyId);
  if (!input) return;

  input.addEventListener("input", function () {
    const query = this.value.trim().toLowerCase();
    const cards = document.querySelectorAll(cardSelector);
    let visible = 0;

    cards.forEach((card) => {
      const haystack = (card.dataset.search || "").toLowerCase();
      const show = !query || haystack.includes(query);
      card.dataset.searchVisible = show ? "1" : "0";
      if (show) visible++;
    });

    if (empty) empty.classList.toggle("d-none", visible > 0 || !query);

    // Reset pagination to page 1 on new search
    const pager = input
      .closest(".stats-wrapper")
      ?.querySelector("[data-pager]");
    if (pager) {
      pager._goToPage(1);
    }
  });

  input.addEventListener("keydown", function (e) {
    if (e.key === "Escape") {
      this.value = "";
      this.dispatchEvent(new Event("input"));
    }
  });
}

/**
 * initGridPagination-client-side pagination for card grids.
 * @param {string} cardSelector  CSS selector for the cards
 * @param {string} pagerId       id of the pagination container element
 * @param {number} perPage       cards per page (default 9)
 */
function initGridPagination(cardSelector, pagerId, perPage = 9) {
  const container = document.getElementById(pagerId);
  if (!container) return;

  let currentPage = 1;

  function getCards() {
    return Array.from(document.querySelectorAll(cardSelector));
  }

  function getVisibleCards() {
    return getCards().filter((c) => c.dataset.searchVisible !== "0");
  }

  function goToPage(page) {
    const cards = getVisibleCards();
    const totalPages = Math.max(1, Math.ceil(cards.length / perPage));
    currentPage = Math.min(Math.max(1, page), totalPages);

    const start = (currentPage - 1) * perPage;
    const end = start + perPage;

    // Hide all first
    getCards().forEach((c) => {
      if (c.dataset.searchVisible === "0") {
        c.style.display = "none";
      } else {
        const idx = cards.indexOf(c);
        c.style.display = idx >= start && idx < end ? "" : "none";
      }
    });

    renderPager(totalPages);
  }

  function renderPager(totalPages) {
    container.innerHTML = "";
    if (totalPages <= 1) return;

    const ul = document.createElement("ul");
    ul.className = "pagination justify-content-center mt-4 mb-0";

    const mkItem = (label, page, disabled, active) => {
      const li = document.createElement("li");
      li.className =
        "page-item" + (disabled ? " disabled" : "") + (active ? " active" : "");
      const a = document.createElement("a");
      a.className = "page-link";
      a.href = "#";
      a.innerHTML = label;
      a.addEventListener("click", (e) => {
        e.preventDefault();
        if (!disabled) goToPage(page);
      });
      li.appendChild(a);
      ul.appendChild(li);
    };

    mkItem("&laquo;", currentPage - 1, currentPage === 1, false);

    for (let i = 1; i <= totalPages; i++) {
      mkItem(i, i, false, i === currentPage);
    }

    mkItem("&raquo;", currentPage + 1, currentPage === totalPages, false);

    container.appendChild(ul);
    // Expose goToPage for search integration
    container._goToPage = goToPage;
  }

  // Init
  getCards().forEach((c) => {
    if (c.dataset.searchVisible === undefined) c.dataset.searchVisible = "1";
  });
  goToPage(1);
  container._goToPage = goToPage;
}
