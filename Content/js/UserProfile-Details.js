$(document).ready(function() {
    console.log("file loaded");
    //File Upload response from the server
    Dropzone.forElement(".dropzone").options.autoProcessQueue = false;
    Dropzone.forElement(".dropzone").options.acceptedFiles = "image/*";
    Dropzone.forElement(".dropzone").options.paramName = "file";
    Dropzone.forElement(".dropzone").options.resizeWidth ="500";
    Dropzone.forElement(".dropzone").options.resizeHeight = "500";
    Dropzone.forElement(".dropzone").options.resizeMimeType = "image/jpeg";
    //Dropzone.forElement(".dropzone").options.addRemoveLinks = true;
    Dropzone.forElement(".dropzone").options.maxFilesize = 10;
   
    $("#submit-all").on("click", function () {
        Dropzone.forElement(".dropzone").processQueue();
    });
    console.log("file loaded");
    Dropzone.forElement(".dropzone").on("addedfile", function (file) {
        var removeDiv = Dropzone.createElement("<div class='remove-div'><span>Remove image</span><img class='icon' src='../../Content/removebutton.svg' alt='Click me to remove the file.' data-dz-remove/></div>");
        var _this = this;

        // Listen to the click event
        removeDiv.addEventListener("click", function (e) {
            // Make sure the button click doesn't submit the form:
            e.preventDefault();
            e.stopPropagation();

            // Remove the file preview.
            _this.removeFile(file);
            // If you want to the delete the file on the server as well,
            // you can do the AJAX request here.
        });

        // Add the button to the file preview element.
        file.previewElement.appendChild(removeDiv);

    });
  
    console.log("file loaded");
});

$("#upload-avatar-form .file-input").on("change", function () {
    var mimeType = $(this)[0].files[0]["type"];
    if (mimeType.split("/")[0] === "image") {

        $("#upload-avatar-form").submit();
    }
});
$("#upload-avatar-form").submit(function () {
});
