function AppViewModel() {
    // Private state
    var self = this;
    
    self.moods = [new MoodViewModel(1, 'angry.png'),
        new MoodViewModel(2, 'miffed.png'),
        new MoodViewModel(3, 'neutral.png'),
        new MoodViewModel(4, 'ok.png'),
        new MoodViewModel(5, 'fantastic.png')];

    return self;
}

var app = new AppViewModel();
