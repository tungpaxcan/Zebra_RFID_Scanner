(function ($) {
    $.fn.uploader = function (options) {
        var settings = $.extend(
            {
                MessageAreaText: HTMLentity(window.parent.nofilechosenVar) + ".",
                MessageAreaTextWithFiles: HTMLentity(window.parent.filelistVar)+":",
                DefaultErrorMessage: HTMLentity(window.parent.canopenthisfileVar)+".",
                BadTypeErrorMessage: HTMLentity(window.parent.wecannotacceptthisfiletypeatthistimeVar)+".",
                acceptedFileTypes: [
                    "xlsx",
                    "xls"
                ]
            },
            options
        );

        var uploadId = 1;
        //update the messaging
        $(".file-uploader__message-area p").text(
            options.MessageAreaText || settings.MessageAreaText
        );

        //create and add the file list and the hidden input list
        var fileList = $('<ul class="file-list"></ul>');
        var hiddenInputs = $('<div class="hidden-inputs hidden"></div>');
        $(".file-uploader__message-area").after(fileList);
        $(".file-list").after(hiddenInputs);

        //when choosing a file, add the name to the list and copy the file input into the hidden inputs
        $(".file-chooser__input").on("change", function () {
            var files = document.querySelector(".file-chooser__input").files;

            for (var i = 0; i < files.length; i++) {
                
                var file = files[i];

                let formdata = new FormData();
                formdata.append('file', file);
                $.ajax({
                    type: 'post', 
                    url: '/FunctionOrder/UploadImage',
                    contentType: false,
                    processData: false,
                    data: formdata,
                    success: function (urlImage) {
                        var fileName = file.name.match(/([^\\\/]+)$/)[0];
                        var fileNameCSV = urlImage;
                        //clear any error condition
                        $(".file-chooser").removeClass("error");
                        $(".error-message").remove();

                        //validate the file
                        var check = checkFile(urlImage);
                        if (check === "valid") {
                            // move the 'real' one to hidden list
                            $(".hidden-inputs").append($(".file-chooser__input"));

                            //insert a clone after the hiddens (copy the event handlers too)
                            $(".file-chooser").append(
                                $(".file-chooser__input").clone({ withDataAndEvents: true })
                            );

                            //add the name and a remove button to the file-list
                            $(".file-list").append(
                                '<li style="display: none;"><span class="file-list__name">' +
                                fileNameCSV +
                                '</span><button class="removal-button" data-uploadid="' +
                                uploadId +
                                '"></button></li>'
                            );
                            $(".file-list").find("li:last").show(800);

                            //removal button handler
                            $(".removal-button").on("click", function (e) {
                                e.preventDefault();

                                //remove the corresponding hidden input
                                $(
                                    '.hidden-inputs input[data-uploadid="' +
                                    $(this).data("uploadid") +
                                    '"]'
                                ).remove();

                                //remove the name from file-list that corresponds to the button clicked
                                $(this)
                                    .parent()
                                    .hide("puff")
                                    .delay(10)
                                    .queue(function () {
                                        $(this).remove();
                                    });

                                //if the list is now empty, change the text back
                                if ($(".file-list li").length === 0) {
                                    $(".file-uploader__message-area").text(
                                        options.MessageAreaText || settings.MessageAreaText
                                    );
                                }
                            });

                            //so the event handler works on the new "real" one
                            $(".hidden-inputs .file-chooser__input")
                                .removeClass("file-chooser__input")
                                .attr("data-uploadId", uploadId);

                            //update the message area
                            $(".file-uploader__message-area").text(
                                options.MessageAreaTextWithFiles ||
                                settings.MessageAreaTextWithFiles
                            );

                            uploadId++;
                        } else {
                            //indicate that the file is not ok
                            $(".file-chooser").addClass("error");
                            var errorText =
                                options.DefaultErrorMessage || settings.DefaultErrorMessage;

                            if (check === "badFileName") {
                                errorText =
                                    options.BadTypeErrorMessage || settings.BadTypeErrorMessage;
                            }

                            $(".file-chooser__input").after(
                                '<p class="error-message">' + errorText + "</p>"
                            );
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error(xhr, status, error);
                    }
                })

            }
        });

        var checkFile = function (fileName) {
            var accepted = "invalid",
                acceptedFileTypes =
                    this.acceptedFileTypes || settings.acceptedFileTypes,
                regex;

            for (var i = 0; i < acceptedFileTypes.length; i++) {
                regex = new RegExp("\\." + acceptedFileTypes[i] + "$", "i");

                if (regex.test(fileName)) {
                    accepted = "valid";
                    break;
                } else {
                    accepted = "badFileName";
                }
            }

            return accepted;
        };
    };
})($);

//init
$(document).ready(function () {
    $(".fileUploader").uploader({
        MessageAreaText: HTMLentity(window.parent.nofilechosenVar) + ". " + HTMLentity(window.parent.pleaseselectafileVar)+"."
    });
});

