function MoodViewModel(value, img) {
    var self = this;

    self.value = value;
    self.img = img;
    self.flash = ko.observable("");

    self.sendMood = function () {
        if (self.flash()) {
            return;
        }

        $.ajax({
            method: 'put',
            url: window.location.origin + "/Survey/Answer/" + window.location.href.split('/').pop(),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ moodId: self.value }),
            success: function (data) {
                self.flash("flash-green");
                setTimeout(function () {
                    self.flash("");
                }, 2000);
            }
        });
    };

    return self;
}