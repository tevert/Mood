function SurveyAppViewModel() {
    // Private state
    var self = this;

    self.details = ko.observable("");
    self.lastAnswer = ko.observable(null);
    self.showInstructions = ko.observable(false);
    self.pendingRequest = ko.observable(false);
    
    self.moods = window.moods.map(function (moodModel) {
        return new MoodViewModel(moodModel.Id, window.location.origin + '/Content/Images/' + moodModel.Description + '.png');
    });

    self.pendingEdit = ko.computed(function () {
        return self.details ||
            self.pendingRequest() ||
            self.moods.some(function (mood) { return mood.pendingRequest(); });
    });

    self.submitDetails = function () {
        self.pendingRequest(true);

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
                self.pendingEdit = false;
            },
            complete: function () {
                self.pendingRequest(false);
            }
        });
    };

    self.reload = function () {
        setTimeout(function () {
            if (!self.pendingEdit) {
                window.location.reload();
            } else {
                self.reload();
            }
        }, 180000);
    };
    self.reload();

    return self;
}

var app = new SurveyAppViewModel();
