function HomeAppViewModel() {
    // Private state
    var self = this;
    
    self.mySurveys = window.model.MySurveys.map(function (surveyModel) {
        return new SurveyViewModel(surveyModel, true);
    });

    self.sharedSurveys = window.model.SharedSurveys.map(function (surveyModel) {
        return new SurveyViewModel(surveyModel, false);
    });

    return self;
}

var app = new HomeAppViewModel();
