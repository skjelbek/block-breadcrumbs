(function () {

    var firstHighlighted;

    function findFirstHighlighted() {
        var highlighted = document.querySelectorAll('.bbcrumbs-highlighted');
        if (highlighted.length > 0) {
            firstHighlighted = highlighted[0];
        }
    };

    function getTop(el) {
        return el.getBoundingClientRect().top + window.pageYOffset;
    }

    function scrollToHighlighted() {
        if (firstHighlighted && firstHighlighted.getBoundingClientRect) {
            var scrollTop = getTop(firstHighlighted);
            if (scrollTop > 100) {
                if (window.scrollTo) window.scrollTo(0, scrollTop - 100);
            }
        }
    };

    document.addEventListener('DOMContentLoaded', function (e) {
        findFirstHighlighted();
        scrollToHighlighted();
    });

    window.addEventListener("load", function(e) {
        scrollToHighlighted();
    });

})();