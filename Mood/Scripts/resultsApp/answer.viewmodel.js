function AnswerViewModel(data) {
    var self = this;

    self.id = ko.observable(data.Id);
    self.moodId = ko.observable(data.MoodId);
    self.details = ko.observable(data.Details);
    self.time = ko.observable(moment(data.Time));
    self.timeString = ko.computed(function () { return self.time().format('LLLL') });
    
    return self;
}