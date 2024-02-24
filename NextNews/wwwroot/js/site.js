
///Quill Editor
var quill = new Quill('#quill-editor', {
    modules: {

        toolbar: [
            ['bold', 'italic', 'underline', 'strike', 'image'],        // toggled buttons
            ['blockquote', 'code-block'],

            [{ 'header': 1 }, { 'header': 2 }],               // custom button values
            [{ 'list': 'ordered' }, { 'list': 'bullet' }],
            [{ 'script': 'sub' }, { 'script': 'super' }],      // superscript/subscript
            [{ 'indent': '-1' }, { 'indent': '+1' }],          // outdent/indent
            [{ 'direction': 'rtl' }],                         // text direction


             // custom dropdown
            [{ 'header': [1, 2, 3, 4, 5, 6, false] }],

            [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
            [{ 'font': [] }],
            [{ 'size': ['small', false, 'large', 'huge'] }],
            [{ 'align': [] }],

            ['clean']
        ],

        imageResize: {
            displaySize: false
        }

    },


    placeholder: '',
    theme: 'snow'
});

var form = document.querySelector('#new-articles-form');
form.onsubmit = function () {

    var quillContent = quill.root.innerHTML;
   

    var input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'Content';
    input.value = quillContent;
    form.appendChild(input);
};

    $(document).ready(function () {
        $("#cloudIcon").click(function () {
            $("#weatherDetails").show();
        });

    $(document).mousemove(function () {
        $("#weatherDetails").hide();
        });
    });


  






    

