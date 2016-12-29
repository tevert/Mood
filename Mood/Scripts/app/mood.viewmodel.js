function MoodViewModel(value, img) {
    var self = this;

    self.value = value;
    self.img = img;
    self.siblings = [];
    self.flash = ko.observable("");

    self.sendMood = function (parentVM) {
        if (self.flash()) {
            return;
        }

        $.ajax({
            method: 'put',
            url: window.location.origin + "/Survey/Answer/" + window.location.href.split('/').pop(),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ moodId: self.value, details: parentVM.details() }),
            success: function (answer) {
                self.flash("flash-green");
                parentVM.moods
                    .filter(function (mood) { return mood.value != self.value; })
                    .forEach(function (sibling) { sibling.flash("flash-fade"); });
                setTimeout(function () {
                    parentVM.moods.forEach(function (mood) { mood.flash(""); });
                }, 2000);

                // Check every 10 seconds; if the details are empty, the person is gone and we can clean up
                parentVM.lastAnswer(answer);
                function check() {
                    if (!parentVM.details()) {
                        parentVM.lastAnswer(null);
                    } else {
                        setTimeout(check, 10000);
                    }
                }
                setTimeout(check, 10000);
            }
        });
    };

    return self;
}