function HomeAppViewModel() {
    // Private state
    var self = this;
    
    self.surveys = window.surveys.map(function (surveyModel) {
        return new SurveyViewModel(surveyModel);
    });

    return self;
}

var app = new HomeAppViewModel();
