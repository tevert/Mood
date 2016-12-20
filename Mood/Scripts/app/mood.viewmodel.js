function MoodViewModel(value, img) {
    var self = this;

    self.value = value;
    self.img = img;

    self.sendMood = function () {
        $.ajax({
            method: 'post',
            url: 'api/moods/' + self.value,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                // TODO fancy animation
            }
        });
    };

    return self;
}