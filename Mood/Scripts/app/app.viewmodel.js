function AppViewModel() {
    // Private state
    var self = this;

    self.details = ko.observable("");
    self.lastAnswer = ko.observable(null);
    self.showInstructions = ko.observable(false);
    
    self.moods = window.moods.map(function (moodModel) {
        return new MoodViewModel(moodModel.Id, window.location.origin + '/Content/Images/' + moodModel.Description + '.png');
    });

    self.submitDetails = function () {
        if (!self.lastAnswer()) {
            self.showInstructions(true);
            return;
        }
        self.showInstructions(false);

        $.ajax({
            method: 'put',
            url: window.location.origin + "/Answer/Edit/" + self.lastAnswer().Id,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ details: self.details() }),
            success: function () {
                self.details("");
                self.lastAnswer(null);
            }
        });
    }

    return self;
}

var app = new AppViewModel();
