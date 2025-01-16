// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/****************   Check link Activation operation     ***************************/

if (window.location.pathname === '/Account/Login') {
    // Hide the button
    $('.signInBtn-contain').css('display', 'none');
}
if (window.location.pathname.toLowerCase() === '/QuoteForm/CreateQuoteForm'.toLowerCase()) {
    // Hide the button
    $('.onQouteform').css('display', 'none');
}


/***************************************************/

/*************    Header drop-down for profile **************************/
function toggleDropdown() {
    const dropdown = document.getElementById('profile-dropdown');
    dropdown.classList.toggle('hidden');
}

// Optional: Close dropdown when clicking outside
//document.addEventListener('click', function (event) {
//    const dropdown = document.getElementById('profile-dropdown');
//    const button = document.getElementById('user-menu-button');
//    if (!button.contains(event.target) && !dropdown.contains(event.target)) {
//        dropdown.classList.add('hidden');
//    }
//});

/********************************************/

//const toggleSwitch = document.getElementById("toggleSwitch");
//const switchInner = document.getElementById("switchInner");
//const yesText = document.getElementById("yesText");
//const noText = document.getElementById("noText");

//toggleSwitch.addEventListener("click", () => {
//    const isChecked = toggleSwitch.getAttribute("aria-checked") === "true";
//    toggleSwitch.setAttribute("aria-checked", !isChecked);

//    if (isChecked) {
//        switchInner.classList.remove("translate-x-5");
//        switchInner.classList.add("translate-x-0");
//        yesText.classList.remove("opacity-100");
//        yesText.classList.add("opacity-0");
//        noText.classList.remove("opacity-0");
//        noText.classList.add("opacity-100");
//        document.getElementById("IsIncurredCostNo").checked = true;

//        $('#incurredSection').hide();
//        $('#incurredBreakoutTableContain').hide();
//        $('#incurredBreakoutTableTotal').hide();
//        $(".isIncurredCost").text("Proposed Breakout");
//        $(".quoteform-table-title h3").text("Proposed Breakout");

//        // Clear all data in Incurred Breakout fields when 'No' is selected
//        $('#incurredBreakoutTable').find('input, select').val('');
//        $('#incurredBreakoutTableTotal').find('input, select').val('');
//    } else {
//        //This is for the no
//        switchInner.classList.remove("translate-x-0");
//        switchInner.classList.add("translate-x-5");
//        yesText.classList.remove("opacity-0");
//        yesText.classList.add("opacity-100");
//        noText.classList.remove("opacity-100");
//        noText.classList.add("opacity-0");
//        document.getElementById("IsIncurredCostYes").checked = true;

//        $('#incurredSection').show();
//        $('#incurredBreakoutTableContain').show();
//        $('#incurredBreakoutTableTotal').show();

//        $(".isIncurredCost").text("Incurred cost and Proposed Breakout");
//        $(".quoteform-table-title h3").text("Incurred cost and Proposed Breakout");
//    }
//});

$('#exampleModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget) // Button that triggered the modal
    var recipient = button.data('whatever') // Extract info from data-* attributes
    // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
    // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
    var modal = $(this)
    modal.find('.modal-title').text('New message to ' + recipient)
    modal.find('.modal-body input').val(recipient)
});

