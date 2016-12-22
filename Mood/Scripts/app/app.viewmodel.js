function AppViewModel() {
    // Private state
    var self = this;
    
    self.moods = [new MoodViewModel(1, window.location.origin + '/Content/Images/Angry.png'),
        new MoodViewModel(2, window.location.origin + '/Content/Images/Miffed.png'),
        new MoodViewModel(3, window.location.origin + '/Content/Images/Neutral.png'),
        new MoodViewModel(4, window.location.origin + '/Content/Images/Ok.png'),
        new MoodViewModel(5, window.location.origin + '/Content/Images/Fantastic.png')];

    return self;
}

var app = new AppViewModel();
