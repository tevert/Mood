function ResultsAppViewModel() {
    var resultsModel = window.resultsModel;

    // Private state
    var self = this;
    self.surveyDescription = resultsModel.Survey.Description;

    self.answers = resultsModel.Answers.map(function (answerModel) {
        return new AnswerViewModel(answerModel);
    });

    self.copyAnswers = function () {
        var dataString = self.answers.map(function (answer) {
            return answer.time().format('l LTS') + '\t' + answer.moodId() + '\t' + answer.details();
        }).reduce(function (a1, a2) {
            return a1 + '\n' + a2;
        });
        copyToClipboard(dataString);
    }

    return self;
}

var app = new ResultsAppViewModel();
