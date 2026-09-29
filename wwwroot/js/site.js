document.addEventListener("DOMContentLoaded", function () {

    const searchInput = document.getElementById("propertySearch");
    const typeFilter = document.getElementById("propertyType");
    const bedroomFilter = document.getElementById("bedroomFilter");

    const propertyCards = document.querySelectorAll(".property-card");
    const propertyCount = document.getElementById("propertyCount");
    const noProperties = document.getElementById("noProperties");

    function filterProperties() {

        const searchText = searchInput
            ? searchInput.value.toLowerCase().trim()
            : "";

        const selectedType = typeFilter
            ? typeFilter.value.toLowerCase()
            : "";

        const selectedBedrooms = bedroomFilter
            ? bedroomFilter.value
            : "";

        let visibleCount = 0;

        propertyCards.forEach(function (card) {

            const title =
                (card.dataset.title || "").toLowerCase();

            const location =
                (card.dataset.location || "").toLowerCase();

            const type =
                (card.dataset.type || "").toLowerCase();

            const bedrooms =
                card.dataset.bedrooms || "";

            const matchesSearch =
                title.includes(searchText) ||
                location.includes(searchText) ||
                type.includes(searchText);

            const matchesType =
                selectedType === "" ||
                type === selectedType;

            const matchesBedrooms =
                selectedBedrooms === "" ||
                bedrooms === selectedBedrooms;

            if (
                matchesSearch &&
                matchesType &&
                matchesBedrooms
            ) {
                card.style.display = "";
                visibleCount++;
            } else {
                card.style.display = "none";
            }

        });

        if (propertyCount) {
            propertyCount.textContent =
                visibleCount +
                (visibleCount === 1
                    ? " property"
                    : " properties");
        }

        if (noProperties) {
            noProperties.style.display =
                visibleCount === 0
                    ? "block"
                    : "none";
        }
    }

    if (searchInput) {
        searchInput.addEventListener(
            "input",
            filterProperties
        );
    }

    if (typeFilter) {
        typeFilter.addEventListener(
            "change",
            filterProperties
        );
    }

    if (bedroomFilter) {
        bedroomFilter.addEventListener(
            "change",
            filterProperties
        );
    }

    // Run once when the page loads
    filterProperties();

});