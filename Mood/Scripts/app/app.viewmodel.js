function AppViewModel() {
    // Private state
    var self = this;
    
    self.moods = window.moods.map(function (moodModel) {
        return new MoodViewModel(moodModel.Id, window.location.origin + '/Content/Images/' + moodModel.Description + '.png');
    });

    return self;
}

var app = new AppViewModel();
