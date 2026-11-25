// HotelBookingSystem client-side helpers: image fallback, date range guard, and live booking estimate.
document.addEventListener("DOMContentLoaded", () => {
    // 1. Replace any missing room photography with the cohesive SVG fallback.
    document.querySelectorAll("img[data-fallback]").forEach((img) => {
        img.addEventListener("error", () => {
            const fallback = img.getAttribute("data-fallback");
            if (fallback && img.getAttribute("src") !== fallback) {
                img.setAttribute("src", fallback);
            }
        });
    });

    // 2. Keep check-out date strictly after check-in date on search and booking forms.
    const checkInInput = document.querySelector("[data-date-checkin]");
    const checkOutInput = document.querySelector("[data-date-checkout]");

    if (checkInInput && checkOutInput) {
        const syncDates = () => {
            if (!checkInInput.value) return;
            const checkIn = new Date(checkInInput.value + "T00:00:00");
            const minOut = new Date(checkIn);
            minOut.setDate(minOut.getDate() + 1);
            const minOutStr = minOut.toISOString().split("T")[0];
            checkOutInput.min = minOutStr;

            if (!checkOutInput.value || checkOutInput.value <= checkInInput.value) {
                checkOutInput.value = minOutStr;
            }
            updateBookingSummary();
        };

        checkInInput.addEventListener("change", syncDates);
        checkOutInput.addEventListener("change", updateBookingSummary);
    }

    // 3. Live client-side estimate on the booking form (server always recalculates authoritatively).
    function updateBookingSummary() {
        const summaryRoot = document.querySelector("[data-booking-summary]");
        if (!summaryRoot || !checkInInput || !checkOutInput) return;

        const rate = parseFloat(summaryRoot.getAttribute("data-nightly-rate") || "0");
        const nightsEl = summaryRoot.querySelector("[data-summary-nights]");
        const totalEl = summaryRoot.querySelector("[data-summary-total]");

        if (!checkInInput.value || !checkOutInput.value || Number.isNaN(rate)) return;

        const start = new Date(checkInInput.value + "T00:00:00");
        const end = new Date(checkOutInput.value + "T00:00:00");
        const diffDays = Math.round((end - start) / (1000 * 60 * 60 * 24));

        if (diffDays > 0) {
            if (nightsEl) nightsEl.textContent = `${diffDays} night${diffDays === 1 ? "" : "s"}`;
            if (totalEl) {
                const formatted = new Intl.NumberFormat("en-IN", {
                    style: "currency",
                    currency: "INR",
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                }).format(diffDays * rate);
                totalEl.textContent = formatted;
            }
        } else {
            if (nightsEl) nightsEl.textContent = "—";
            if (totalEl) totalEl.textContent = "—";
        }
    }
});
