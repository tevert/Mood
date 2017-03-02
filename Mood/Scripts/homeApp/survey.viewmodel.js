function SurveyViewModel(data, isOwner) {
    var self = this;

    self.owner = isOwner;
    self.id = ko.observable(data.Id);
    self.description = ko.observable(data.Description);
    self.name = ko.observable(data.Name);
    self.publicResults = ko.observable(data.PublicResults);
    self.sharedUsers = ko.observable(data.SharedUsers.map(function (User) { return User.UserName; }));
    self.identifier = ko.computed(function () { return self.name() || self.id() });

    self.flash = ko.observable();
    self.error = ko.observable();

    self.copyUrl = function (baseUrl) {
        copyToClipboard(baseUrl + '/' + self.identifier());
    };

    self.save = function (baseUrl) {
        // use the id, not the identifier, since we don't know if the name is dirty or not.
        var url = baseUrl + '/' + self.id();

        // need to do a bit of extra cleaning on the sharedUsers field
        var sharedUsers = self.sharedUsers().length ?
            self.sharedUsers()
            .split(',')
            .map(function (username) { return username.trim(); })
            .filter(function (username) { return username.length > 0 })
            : null;

        $.ajax({
            method: 'post',
            url: url,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ name: self.name(), publicResults: self.publicResults(), sharedUsers: sharedUsers }),
            success: function (response) {
                if (response.error) {
                    self.error(response.error);
                } else {
                    self.error(null);
                    self.flash("flash-green");
                    setTimeout(function () {
                        self.flash('');
                    }, 1000);
                }
            },
            error: function (response) {
                self.error(response.responseText);
            }
        });
    };

    return self;
}