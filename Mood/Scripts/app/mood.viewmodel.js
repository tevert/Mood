function MoodViewModel(value, img) {
    var self = this;

    self.value = value;
    self.img = img;

    self.sendMood = function () {
        $.ajax({
            method: 'put',
            url: window.location.origin + "/Survey/Answer/" + window.location.href.split('/').pop(),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ moodId: self.value }),
            success: function (data) {
                // TODO fancy animation
            }
        });
    };

    return self;
}