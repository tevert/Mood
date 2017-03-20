function AnswerViewModel(data) {
    var self = this;

    self.id = ko.observable(data.Id);
    self.moodId = ko.observable(data.MoodId);
    self.moodName = ko.observable(data.Mood.Description);
    self.details = ko.observable(data.Details);
    self.time = ko.observable(moment.utc(data.Time).local());
    self.timeString = ko.computed(function () { return self.time().format('LLLL') });
    
    return self;
}