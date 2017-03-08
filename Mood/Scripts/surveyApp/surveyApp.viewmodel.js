function SurveyAppViewModel() {
    // Private state
    var self = this;

    self.details = ko.observable("");
    self.lastAnswer = ko.observable(null);
    self.showInstructions = ko.observable(false);
    self.pendingRequest = ko.observable(false);
    
    self.moods = window.moods.map(function (moodModel) {
        return new MoodViewModel(moodModel.Id, window.location.origin + '/Content/Images/' + moodModel.IconName + '.png');
	});

	self.showDetails = function () {
		$("#detailsButton").addClass("hidden");
		$("#details").removeClass("hidden");
	};

    self.pendingEdit = ko.computed(function () {
        return self.details ||
            self.pendingRequest() ||
            self.moods.some(function (mood) { return mood.pendingRequest(); });
    });

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
