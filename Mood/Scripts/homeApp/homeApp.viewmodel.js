function HomeAppViewModel() {
    // Private state
    var self = this;

    self.details = ko.observable("");
    self.lastAnswer = ko.observable(null);
    self.showInstructions = ko.observable(false);
    self.pendingRequest = ko.observable(false);
    
    self.surveys = window.surveys.map(function (surveyModel) {
        return new SurveyViewModel(surveyModel);
    });

    self.pendingEdit = ko.computed(function () {
        return self.details ||
            self.pendingRequest() ||
            self.moods.some(function (mood) { return mood.pendingRequest(); });
    });

    return self;
}

var app = new HomeAppViewModel();
