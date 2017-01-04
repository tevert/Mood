window.copyToClipboard = (function () {
    var _dataString = null;
    document.addEventListener("copy", function (e) {
        if (_dataString !== null) {
            try {
                e.clipboardData.setData("text/plain", _dataString);
                e.preventDefault();
            } finally {
                _dataString = null;
            }
        }
    });
    return function (data) {
        _dataString = data;
        document.execCommand("copy");
    };
})();