function SurveyViewModel(data) {
    var self = this;

    self.id = ko.observable(data.Id);
    self.description = ko.observable(data.Description);
    self.name = ko.observable(data.Name);
    self.identifier = ko.computed(function () { return self.name() || self.id() });
    
    self.siblings = [];
    self.flash = ko.observable("");

    self.pendingRequest = ko.observable(false);

    self.copyUrl = function () {
        copyToClipboard();
    }

    self.save = function (parentVM) {
        if (self.flash()) {
            return;
        }
        self.pendingRequest(true);

        $.ajax({
            method: 'put',
            url: window.location.origin + "/Survey/Answer/" + window.location.href.split('/').pop(),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ moodId: self.value, details: parentVM.details() }),
            success: function (answer) {
                self.flash("flash-green");
                parentVM.moods
                    .filter(function (mood) { return mood.value !== self.value; })
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
            },
            complete: function () {
                self.pendingRequest(false);
            }
        });
    };

    return self;
}