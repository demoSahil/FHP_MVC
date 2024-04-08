
let serialNo = document.getElementById('SerialNo');

//-----------Input Fields
const serialNoField = document.getElementById('SerialNo');
const prefixField = document.getElementById('Prefix');
const firstNameField = document.getElementById('FirstName');
const middleNameField = document.getElementById('MiddleName');
const lastNameField = document.getElementById('LastName');
const DOBField = document.getElementById('DOB');
const currentAddressField = document.getElementById('CurrentAddress');
const educationField = document.getElementById('Education');
const currentCompanyField = document.getElementById('CurrentCompany');
const joiningDateField = document.getElementById('JoiningDate');


//---------- Buttons
const addButton = document.getElementById('add');
const editButton = document.getElementById('edit');
const clearButton = document.getElementById('clear');
const first = document.getElementById('first');
const previous = document.getElementById('previous');
const next = document.getElementById('next');
const last = document.getElementById('last');

//----- Form Element
const form = document.getElementById('employee-details-form');

const fieldsInForm = form.elements;
console.log(fieldsInForm);
let employeesData;
let currIndex;
let currentId;
let dataRetrieved = false;


function disableInputFields(fields) {
    //-------------Disabling Fields In case of View
    for (let i = 0; i < fields.length; i++) {
        console.log("in for loop");
        if (fields[i].tagName === 'INPUT' || fields[i].tagName === 'input') {
            fields[i].disabled = true;

            console.log("disabled" + fields[i]);
        }
    }
}
document.addEventListener('DOMContentLoaded', function () {
    const urlParams = new URLSearchParams(window.location.search);
    console.log(urlParams);
    const action = urlParams.get('buttonClicked');
    console.log(action);
    currentId = urlParams.get('id');

    if (action === 'New' || action === 'Update') {

        if (action === 'New') {
            serialNo.value = urlParams.get('serialNo');
        }
        serialNo.readOnly = true;

        addButton.classList.remove('hidden');
        editButton.classList.remove('hidden');
        clearButton.classList.remove('hidden');

        addButton.classList.add('visible');
        editButton.classList.add('visible');
        clearButton.classList.add('visible');

    } else if (action === 'View') {

        first.classList.remove('hidden');
        previous.classList.remove('hidden');
        next.classList.remove('hidden');
        last.classList.remove('hidden');

        first.classList.add('visible');
        previous.classList.add('visible');
        next.classList.add('visible');
        last.classList.add('visible');

        disableInputFields(fieldsInForm);

        //-------------Getting Data From XML

        const xhr = new XMLHttpRequest();
        xhr.open('GET', '/Home/EmployeesData', true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    console.log('employees List Retrieved Successfully');
                    employeesData = JSON.parse(xhr.responseText);

                    for (let i = 0; i < employeesData.length; i++) {
                        if (employeesData[i].SerialNo == currentId) {
                            currIndex = i;
                        }

                    }
                    console.log('current Index = ' + currIndex);
                    dataRetrieved = true;

                    first.addEventListener('click', () => {
                        currIndex = 0;
                        SetValuesIntoFields(employeesData[currIndex], fieldsInForm);
                        first.disabled = true;
                    });

                    previous.addEventListener('click', () => {
                        if (currIndex != 0) {
                            currIndex -= 1;
                            SetValuesIntoFields(employeesData[currIndex], fieldsInForm);
                        }
                    });

                    next.addEventListener('click', () => {
                        if (currIndex != employeesData.length - 1) {
                            currIndex += 1;
                            SetValuesIntoFields(employeesData[currIndex], fieldsInForm);
                        }
                    });

                    last.addEventListener('click', () => {

                        currIndex = employeesData.length - 1;
                        SetValuesIntoFields(employeesData[currIndex], fieldsInForm);
                    }
                    );
                    /* SetValuesIntoFields(employeesData[currIndex - 1], fieldsInForm);*/

                } else {
                    console.error('Error:', xhr.statusText);
                }
            }
        };

        xhr.send();
    }
});

function SetValuesIntoFields(object) {
    serialNoField.value = object.SerialNo;
    prefixField.value = object.Prefix;
    firstNameField.value = object.FirstName;
    middleNameField.value = object.MiddleName;
    lastNameField.value = object.LastName;
    DOBField.value = formatDate(object.DOB);
    currentAddressField.value = object.CurrentAddress;
    educationField.value = object.Education;
    currentCompanyField.value = object.CurrentCompany;
    joiningDateField.value = formatDate(object.JoiningDate);
}

function formatDate(dateString) {
    const timestamp = parseInt(dateString.match(/\d+/)[0]);
    const date = new Date(timestamp);
    const formattedDate = date.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' });
    return formattedDate.replace(/\//g, '-'); // Replace forward slashes with dashes
}
