function UserViewModel() {

	var self = this;

	// API client
	var client = {
		get: function(success) {
			$.ajax({
				url: '/api/users',
				success: success
			});
		}
	};

	// VM declaration
	$.extend(self, {
		name: ko.observable(),
		isAdmin: ko.observable(),
		noAdmins: ko.observable()
	});

	// init
	client.get(function(user) {
		self.name(user.name);
		self.isAdmin(user.isAdmin);
		self.noAdmins(user.noAdmins);
	});

}