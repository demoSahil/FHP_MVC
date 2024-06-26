﻿//--------- Handling Checkboxes

/**
 * Node List of all checkboxes other than master checkbox
 */
let checkboxes = document.querySelectorAll('.selectedRows');
AddClickEventOnCheckBoxes(checkboxes);

/**
 * Master checkbox element which selects/deselects all the other checkboxes
 */
const masterCheckbox = document.getElementById('master-CheckBox');

/**
 * An array which holds the serial No of rows selected By user
 */
let idSelectedByUser = [];

/**
 *  Represents Update Button element at Home Page
 */
let updateButton = document.getElementById('update-btn');

/**
 * Represents the view link element at home page
 */
let viewLink = document.getElementById('view-link');


/**
 *  Adds an Event Listener to checkboxes which Add/remove Id in {idselectedByUser} array 
 */
function AddClickEventOnCheckBoxes(checkboxes) {

    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', () => {

            if (checkbox.checked) {
                idSelectedByUser.push((checkbox.getAttribute('id')));
                checkbox.parentElement.parentElement.classList.add('colorRow');
                console.log('id added', idSelectedByUser);
            }
            else {
                let idToPop = checkbox.getAttribute('id');
                idSelectedByUser = idSelectedByUser.filter(item => item !== idToPop);
                checkbox.parentElement.parentElement.classList.remove('colorRow');
                console.log('id removed', idSelectedByUser);
            }

            //------------Handling the update Button part
            if (idSelectedByUser.length == 1) {

                updateButton.style.pointerEvents = '';
                viewLink.style.pointerEvents = '';


            }  // then only update/View

            else {
                updateButton.style.pointerEvents = 'none';
                viewLink.style.pointerEvents = 'none';
            } //disable updateView


        })
    });
}

/**
 * Selects/Deselects All checkboxes
 */
function SelectAllCheckbox() {

    if (masterCheckbox.checked) {
        checkboxes.forEach(checkBox => {
            checkBox.checked = true;
            idSelectedByUser.push((checkBox.getAttribute('id')));
            checkBox.parentElement.parentElement.classList.add('colorRow');
            console.log("cannot delete mroe than 1 records");
        });
    }
    else {
        checkboxes.forEach(checkBox => {
            checkBox.checked = false;
            checkBox.parentElement.parentElement.classList.remove('colorRow');
        });
        idSelectedByUser = [];
    }
}

//------------ Handling Delete Functionality

/**
 * Delete button element which deletes the user Data
 */
const deleteButton = document.getElementById('delete-btn');

/**
 * Model Dialog Yes Button For Delete
 */
const deleteProceedButton = document.getElementById('btn-Proceed-Delete');

/**
 * Model Dialog No Button For Delete
 */
const deleteAbortButton = document.getElementById('btn-Abort-Delete');

/**
 * Event Listener which contains ajax call which Deletes the data present inside {idSelectedByUser} array
 */

deleteProceedButton.addEventListener('click', () => {
    event.preventDefault();

    const xhr = new XMLHttpRequest();
    const idsJSON = JSON.stringify(idSelectedByUser);

    xhr.open('POST', '/Home/Delete', true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onload = () => {
        let response = JSON.parse(xhr.responseText);
        console.log(xhr.responseText);
        if (xhr.status == 200 && response.success) {

            idSelectedByUser.forEach(serialNo => {
                DeleteData(serialNo);
                toastr.success('Employee Deleted Successfully', 'Success');
                deleteAbortButton.click();
            });
        }

        else {
            if (response.errorMessage) {
                toastr.error(response.errorMessage, 'Failed');
                deleteAbortButton.click();
            }

            else {
                console.error('Error deleting item:', xhr.statusText);
            }
        }


    }
    xhr.send(idsJSON);
});

/**
 * Deletes data from {idSelectedByUser} array which gets deleted successfully from Ui and array both
 *  * @param {deleting the } id
 */
function DeleteData(id) {
    idSelectedByUser = idSelectedByUser.filter(item => item !== id);
    const checkBox = document.getElementById(id);

    const parent = checkBox.parentElement;
    const rowToDelete = parent.parentElement;
    rowToDelete.textContent = "";
    checkBox.remove();
    rowToDelete.remove();
    rowToDelete.remove();
    checkboxes = document.querySelectorAll('.selectedRows');
    AddClickEventOnCheckBoxes(checkboxes);
    console.log(idSelectedByUser);
}

//-------- Handling Sorting functionality

/**
 * An Array which holds the Name of table headers in sequence
 */
let tableHeaders = ['SerialNo', 'Prefix', 'FirstName', 'MiddleName', 'LastName', 'CurrentAddress', 'DOB', 'Education', 'CurrentCompany', 'JoiningDate'];

/**
 * Arrow Up Icon which when clicked sort Data in Asecnding order
 */
let arrowUpIcons = document.querySelectorAll('.sort-icon.arrow-up');

/**
 * Arrow Down Icon which when user clicked sort Data in Descending order
 */
let arrowDownIcons = document.querySelectorAll('.sort-icon.arrow-down');

/**
 * Adding Event listener(click) which sorts the Data in Ascending order
 */
arrowUpIcons.forEach((arrowUpIcon, index) => {
    arrowUpIcon.addEventListener('click', () => SortAscending(tableHeaders[index]));

});

/**
 * Adding Event listener(click) which sorts the Data in Descending order
 */
arrowDownIcons.forEach((arrowDownIcon, index) => {
    arrowDownIcon.addEventListener('click', () => SortDescending(tableHeaders[index]));
});

/**
 * Make an Ajax request to /Home/SortAsc?columnName=@{ColumnName} which return the sorted data as resonse and this function will render it to UI
 * @param {Represents Column name on which sorting is to be done} ColumnName
 */
function SortAscending(ColumnName) {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', `/Home/SortAsc?columnName=` + ColumnName, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                const sortedData = JSON.parse(xhr.responseText);
                RenderData(sortedData);
            } else {
                console.error('Error:', xhr.statusText);
            }
        }
    };
    xhr.send();
}

/**
 * Make an Ajax request to /Home/SortDesc?columnName=@{ColumnName} which return the sorted data as response and this function will render it to UI
 * @param {Represents Column name on which sorting is to be done} ColumnName
 */
function SortDescending(ColumnName) {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', `/Home/SortDesc?columnName=` + ColumnName, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                const sortedData = JSON.parse(xhr.responseText);
                RenderData(sortedData);
            } else {
                console.error('Error:', xhr.statusText);
            }
        }
    };
    xhr.send();
}

/**
 * Render the sorted Data in UI
 * @param {An Array that holds sorted Data} 
 */
function RenderData(data) {
    let tableData = document.getElementById('tableData');
    let str = "";
    tableData.innerHTML = "";

    data.forEach(function (item) {

        str += '<tr>' +
            `<td><input type="checkbox" class="selectedRows" id=\"${item.SerialNo}\"` + '/></td>' +
            '<td>' + item.SerialNo + '</td>' +
            '<td>' + item.Prefix + '</td>' +
            '<td>' + item.FirstName + '</td>' +
            '<td>' + item.MiddleName + '</td>' +
            '<td>' + item.LastName + '</td>' +
            '<td>' + item.CurrentAddress + '</td>' +
            '<td>' + formatDate(item.DOB) + '</td>' +
            '<td>' + item.Education + '</td>' +
            '<td>' + item.CurrentCompany + '</td>' +
            '<td>' + formatDate(item.JoiningDate) + '</td>' +
            '</tr>';
    });

    tableData.innerHTML = str;
    checkboxes = document.querySelectorAll('.selectedRows');
    AddClickEventOnCheckBoxes(checkboxes);
}
/**
 * Formats the Date according to desired format[dd-MM-YYYY]
 * @param {The string which is to be formatted} dateString
 * @returns formatted Date
 */
function formatDate(dateString) {
    const timestamp = parseInt(dateString.match(/\d+/)[0]);
    const date = new Date(timestamp);
    const formattedDate = date.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' });
    return formattedDate.replace(/\//g, '-');
}
//------------- Handling Filter Functionality

/**
 * Holds the Filter Row element which is <tr>
 */
const filterRow = document.getElementById('filterRow');

/**
 * Holds the ListNode of searchBoxes Present in filter Row element
 */
let searchBoxes = filterRow.querySelectorAll('input');

/**
 * Adding Event Listener(input) to detect any changes in searchboxes
 */
searchBoxes.forEach((searchBox, index) => {
    searchBox.addEventListener('input', () => ApplyFilter());
});

/**
 * Make An Ajax Request to /Home/ApplyFilter which will return the filteredData and it wil render it to UI
 */
function ApplyFilter() {
    const searchBoxesValues = [];
    searchBoxes.forEach((searchBox, index) => {
        searchBoxesValues[index] = searchBox.value;
    });

    console.log(searchBoxesValues);

    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Home/ApplyFilter', true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                console.log('Searching completed successfully');
                const filteredData = JSON.parse(xhr.responseText);
                console.log(filteredData);
                RenderData(filteredData);
            } else {
                console.error('Error:', xhr.statusText);
            }
        }
    };
    xhr.send(JSON.stringify(searchBoxesValues));
}

/**
 * Function which removes all filter and search Values
 */
function ClearSearchAndFilter() {
    searchBoxes.forEach((searchBox) => {
        searchBox.value = "";
    });

    searchBar.value = "";
    ApplyFilter();
}

// Handling Update/View Links

/**
 * Adding Event Listener(click) which will handle the update button clicked
 */
document.getElementById('update-btn').onclick = () => {
    event.preventDefault();
    var url = "/Home/Update?buttonClicked=Update&id=" + idSelectedByUser[0]
    console.log('url' + url);
    window.location.href = url;
};

/**
 * Adding Event Listener(click) which will handle the view button clicked
 */
document.getElementById('view-link').onclick = () => {
    event.preventDefault();
    var url = "/Home/Pagination?buttonClicked=View&id=" + idSelectedByUser[0];
    window.location.href = url;
}
/**
 * Search Bar element place at top of table
 */
const searchBar = document.getElementById('search-bar');

/**
 * Array Object represents employees Data
 */
let employeesData = [];
GetEmployeesData();
searchBar.addEventListener('input', () => {
    let valuesToSearch = searchBar.value;
    let foundEmployees = [];
    for (let i = 0; i < employeesData.length; i++) {
        let employee = employeesData[i];
        let found = false;

        for (let property in employee) {

            if ((property == 'DOB' || property == 'JoiningDate')) {
                if (formatDate(employee[property]).toString().includes(valuesToSearch.toLowerCase())) {
                    found = true;
                    break;
                }
            }

            else if (employee[property].toString().toLowerCase().includes(valuesToSearch.toLowerCase())) {
                found = true;
                break;
            }
        }

        if (found) {
            foundEmployees.push(employee);
        }
    }
    RenderData(foundEmployees);
});

/**
 * Gets Employees Data And store it into employeesData array Object by making an AJAX Request[GET] To /HOME/EmployeesData
 */
function GetEmployeesData() {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', '/Home/EmployeesData', true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                console.log('employees List Retrieved Successfully');
                employeesData = JSON.parse(xhr.responseText);
                console.log(employeesData);
            }

            else {
                console.error('Error', xhr.statusText);
            }
        }
    };

    xhr.send();
}