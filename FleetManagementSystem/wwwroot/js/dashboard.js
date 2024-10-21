let selectedVehicleMonth = new Date().getMonth() + 1;
let selectedVehicleYear = new Date().getFullYear();
let selectedDriverMonth = new Date().getMonth() + 1;
let selectedDriverYear = new Date().getFullYear();

// Load Driver Dashboard with pagination and filtering
function loadDriverDashboard(params = {}) {
    const driverFilterType = document.getElementById('driver-filter-type').value;
    //selectedDriverYear = params.year || new Date().getFullYear(); // Default to current year if not provided
    const page = params.page || 1; // Default to page 1 if not provided
    console.log("Loading driver dashboard for page: ", page);
    $.ajax({
        url: '/Dashboard/LoadDriverDashboard',
        type: 'GET',
        data: {
            filterType: driverFilterType,
            year: selectedDriverYear,
            month: selectedDriverMonth,
            page: page
        },
        success: function (result) {
            console.log("Success: ", page);
            $('#driver-dashboard-container').html(result);
            updateDateNavigation('driver');
        }
    });
}

// Load Vehicle Dashboard with pagination and filtering
function loadVehicleDashboard(params = {}) {
    const vehicleFilterType = document.getElementById('vehicle-filter-type').value;
    selectedVehicleMonth = params.month || selectedVehicleMonth;
    selectedVehicleYear = params.year || selectedVehicleYear;
    let page = params.page || 1;

    $.ajax({
        url: '/Dashboard/LoadVehicleDashboard',
        type: 'GET',
        data: {
            filterType: vehicleFilterType,
            year: selectedVehicleYear,
            month: selectedVehicleMonth,
            page: page
        },
        success: function (result) {
            $('#vehicle-dashboard-container').html(result);
            updateDateNavigation('vehicle');
        }
    });
}


function navigate(section, direction) {
    const filterType = document.getElementById(`${section}-filter-type`).value;
    let month, year;

    // Handle navigation based on the filter type (month/year) and direction (prev/next)
    if (filterType === 'month') {
        if (section === 'vehicle') {
            month = selectedVehicleMonth;
            year = selectedVehicleYear;
        } else {
            month = selectedDriverMonth;
            year = selectedDriverYear;
        }

        if (direction === 'prev') {
            if (month === 1) {
                month = 12;
                year--; // Decrement year when going back from January
            } else {
                month--;
            }
        } else {
            if (month === 12) {
                month = 1;
                year++; // Increment year when going forward from December
            } else {
                month++;
            }
        }

    } else if (filterType === 'year') {
        if (section === 'vehicle') {
            year = selectedVehicleYear;
        } else {
            year = selectedDriverYear;
        }

        if (direction === 'prev') {
            year--; // Decrement year for "prev"
        } else {
            year++; // Increment year for "next"
        }
        month = null; // Month is irrelevant for yearly navigation
    }

    // Ensure that we don't allow navigation into the future (beyond the current year and month)
    const currentYear = new Date().getFullYear();
    const currentMonth = new Date().getMonth() + 1;

    if (year > currentYear || (year === currentYear && month > currentMonth)) {
        year = currentYear;
        month = currentMonth;
    }

    if (section === 'vehicle') {
        loadVehicleDashboard({ month, year });
        selectedVehicleMonth = month;
        selectedVehicleYear = year;
    } else if (section === 'driver') {
        loadDriverDashboard({ month, year });
        selectedDriverMonth = month;
        selectedDriverYear = year;
    }
}

function applyFilter(section, filterType) {
    if (section === 'vehicle') {
        document.getElementById('vehicle-filter-type').value = filterType;
        selectedVehicleMonth = filterType === 'year' ? null : new Date().getMonth() + 1;
        loadVehicleDashboard();
        updateFilterSelection('vehicle', filterType);
        toggleNavigationDisplay('vehicle', filterType);
    } else {
        document.getElementById('driver-filter-type').value = filterType;
        selectedDriverMonth = filterType === 'year' ? null : new Date().getMonth() + 1;
        loadDriverDashboard();
        updateFilterSelection('driver', filterType);
        toggleNavigationDisplay('driver', filterType);
    }
}

function updateFilterSelection(section, filterType) {
    document.getElementById(`${section}-monthly`).style.fontWeight = filterType === "month" ? "bold" : "normal";
    document.getElementById(`${section}-yearly`).style.fontWeight = filterType === "year" ? "bold" : "normal";
}

function updateDateNavigation(section) {
    const filterType = document.getElementById(`${section}-filter-type`).value;
    if (filterType === "year") {
        document.getElementById(`current-${section}-date`).innerText = section === 'vehicle' ? selectedVehicleYear : selectedDriverYear;
    } else {
        const currentDate = new Date(section === 'vehicle' ? selectedVehicleYear : selectedDriverYear, (section === 'vehicle' ? selectedVehicleMonth : selectedDriverMonth) - 1);
        const currentMonth = currentDate.toLocaleString('default', { month: 'short' });
        const currentYear = currentDate.getFullYear();
        document.getElementById(`current-${section}-date`).innerText = `${currentMonth}, ${currentYear}`;
    }
}

function toggleNavigationDisplay(section, filterType) {
    const prevNav = document.getElementById(`prev-${section}-navigation`);
    const nextNav = document.getElementById(`next-${section}-navigation`);
    if (filterType === "year") {
        prevNav.innerText = "Prev Year";
        nextNav.innerText = "Next Year";
    } else {
        prevNav.innerText = "Prev Month";
        nextNav.innerText = "Next Month";
    }
}

document.addEventListener("DOMContentLoaded", function () {
    // Apply the monthly filter by default on page load if no filter is set
    const vehicleFilter = document.getElementById('vehicle-filter-type').value || 'month';
    const driverFilter = document.getElementById('driver-filter-type').value || 'month';

    // Set the default filter type and update the UI accordingly
    document.getElementById('vehicle-filter-type').value = vehicleFilter;
    document.getElementById('driver-filter-type').value = driverFilter;

    // Load the dashboards with the default filters
    loadVehicleDashboard();
    loadDriverDashboard();

    // Update filter selection visuals
    updateFilterSelection('vehicle', vehicleFilter);
    updateFilterSelection('driver', driverFilter);

    // Ensure date navigation is updated for both dashboards
    updateDateNavigation('vehicle');
    updateDateNavigation('driver');
});
