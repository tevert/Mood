function ResultsAppViewModel() {
    var resultsModel = window.resultsModel;

    // Private state
    var self = this;
    self.surveyDescription = resultsModel.Survey.Description;

    self.moods = resultsModel.Moods;

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

    /* All chart config spaghetti below here -----------------------------------------*/
    var dataForChart = self.answers
        .filter(function (answer) {
            return answer.time().isAfter(moment().subtract(1, 'weeks'));
        })
        .map(function (answer) {
            return {
                x: answer.time(), y: answer.moodId(), answer: answer
            };
        })

    self.moodChart = new Chart('moodChart', {
        type: 'line',
        data: {
            datasets: [{
                data: dataForChart,
                fill: false,
                showLine: false,
                pointBackgroundColor: dataForChart.map(function (point) { return point.answer.details() ? '#f88' : '#888'; })
            }]
        },
        options: {
            legend: {
                display: false
            },
            elements: {
                point: {
                    radius: 6
                }
            },
            tooltips: {
                displayColors: false,
                callbacks: {
                    title: function (tooltips, data) {
                        var tt = tooltips[0];
                        var answer = data.datasets[tt.datasetIndex].data[tt.index].answer;
                        return answer.moodName() + " - " + answer.time().format('LT');
                    },
                    label: function (tooltips, data) {
                        var tt = tooltips;
                        var answer = data.datasets[tt.datasetIndex].data[tt.index].answer;
                        return answer.details();
                    }
                }
            },
            scales: {
                xAxes: [{
                    type: 'time',
                    time: {
                        unit: 'day',
                        unitStepSize: 1
                    }
                }],
                yAxes: [{
                    display: true,
                    type: 'linear',
                    position: 'left',
                    ticks: {
                        min: self.moods.sort(function (mood1, mood2) { return mood1.Id - mood2.Id; })[0].Id,
                        max: self.moods.sort(function (mood1, mood2) { return mood2.Id - mood1.Id; })[0].Id,
                        stepSize: 1,
                        callback: function (value, index, values) {
                            return self.moods.filter(function (mood) { return mood.Id == value; })[0].Description;
                        }
                    }
                }]
            }
        }
    });

    return self;
}

var app = new ResultsAppViewModel();
